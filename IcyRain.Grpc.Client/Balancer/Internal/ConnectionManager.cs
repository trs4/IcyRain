using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Grpc.Client.Configuration;
using IcyRain.Grpc.Client.Internal;

namespace IcyRain.Grpc.Client.Balancer.Internal;

internal sealed class ConnectionManager : IDisposable, IChannelControlHelper
{
    public static readonly BalancerAttributesKey<string> HostOverrideKey = new BalancerAttributesKey<string>("HostOverride");
    private static readonly ChannelIdProvider _channelIdProvider = new ChannelIdProvider();

    private readonly object _lock;
    private readonly Resolver _resolver;
    private readonly ISubchannelTransportFactory _subchannelTransportFactory;
    private readonly List<Subchannel> _subchannels;
    private readonly List<StateWatcher> _stateWatchers;
    private readonly TaskCompletionSource<object?> _resolverStartedTcs;
    private readonly long _channelId;
    private LoadBalancer? _balancer;
    private SubchannelPicker? _picker;
    private Task<SubchannelPicker>? _pickerTask; // Cache picker wrapped in task once and reuse
    private bool _resolverStarted;
    private TaskCompletionSource<SubchannelPicker> _nextPickerTcs;
    private int _currentSubchannelId;
    private ServiceConfig? _previousServiceConfig;

    internal ConnectionManager(
        Resolver resolver,
        bool disableResolverServiceConfig,
        IBackoffPolicyFactory backoffPolicyFactory,
        ISubchannelTransportFactory subchannelTransportFactory,
        LoadBalancerFactory[] loadBalancerFactories)
    {
        _lock = new object();
        _nextPickerTcs = new TaskCompletionSource<SubchannelPicker>(TaskCreationOptions.RunContinuationsAsynchronously);
        _resolverStartedTcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        _channelId = _channelIdProvider.GetNextChannelId();

        BackoffPolicyFactory = backoffPolicyFactory;
        _subchannels = [];
        _stateWatchers = [];
        _resolver = resolver;
        DisableResolverServiceConfig = disableResolverServiceConfig;
        _subchannelTransportFactory = subchannelTransportFactory;
        LoadBalancerFactories = loadBalancerFactories;
    }

    public ConnectivityState State { get; private set; }

    public IBackoffPolicyFactory BackoffPolicyFactory { get; }

    public bool DisableResolverServiceConfig { get; }

    public LoadBalancerFactory[] LoadBalancerFactories { get; }

    // For unit tests
    internal IReadOnlyList<Subchannel> GetSubchannels()
    {
        lock (_subchannels)
            return [.. _subchannels];
    }

    internal string GetNextId()
    {
        var nextSubchannelId = Interlocked.Increment(ref _currentSubchannelId);
        return $"{_channelId}-{nextSubchannelId}";
    }

    public void ConfigureBalancer(Func<IChannelControlHelper, LoadBalancer> configure)
    {
        _balancer = configure(this);
    }

    Subchannel IChannelControlHelper.CreateSubchannel(SubchannelOptions options)
    {
        var subchannel = new Subchannel(this, options.Addresses);
        subchannel.SetTransport(_subchannelTransportFactory.Create(subchannel));

        lock (_subchannels)
            _subchannels.Add(subchannel);

        return subchannel;
    }

    void IChannelControlHelper.RefreshResolver() => _resolver.Refresh();

    private void OnResolverResult(ResolverResult result)
    {
        if (_balancer is null)
            throw new InvalidOperationException($"Load balancer not configured");

        var channelStatus = result.Status;

        // Additionally, only use resolved service config if not disabled
        LoadBalancingConfig? loadBalancingConfig = null;

        if (!DisableResolverServiceConfig)
        {
            ServiceConfig? workingServiceConfig = null;

            if (result.ServiceConfig == null)
            {
                // Step 4 and 5
                if (result.ServiceConfigStatus is null)
                {
                    // Step 5: Use default service config if none is provided
                    workingServiceConfig = new ServiceConfig();
                    _previousServiceConfig = workingServiceConfig;
                }
                else if (_previousServiceConfig is null) // Step 4
                    channelStatus = result.ServiceConfigStatus.Value; // Step 4.ii: If no config was provided or set previously, then treat resolution as a failure
                else
                    workingServiceConfig = _previousServiceConfig; // Step 4.i: Continue using previous service config if it was set and a new one is not provided
            }
            else
            {
                // Step 3: Use provided service config if it is set.
                workingServiceConfig = result.ServiceConfig;
                _previousServiceConfig = result.ServiceConfig;
            }

            if (workingServiceConfig?.LoadBalancingConfigs.Count > 0)
            {
                ChildHandlerLoadBalancer.TryGetValidServiceConfigFactory(
                    workingServiceConfig.LoadBalancingConfigs, LoadBalancerFactories, out loadBalancingConfig, out var _);
            }
        }

        var state = new ChannelState(
            channelStatus,
            result.Addresses,
            loadBalancingConfig,
            BalancerAttributes.Empty);

        lock (_lock)
        {
            _balancer.UpdateChannelState(state);
            _resolverStartedTcs.TrySetResult(null);
        }
    }

    internal void OnSubchannelStateChange(Subchannel subchannel, ConnectivityState state, Status status)
    {
        if (state == ConnectivityState.Shutdown)
        {
            lock (_subchannels)
            {
                var removed = _subchannels.Remove(subchannel);
                Debug.Assert(removed);
            }
        }

        lock (_lock)
        {
            subchannel.RaiseStateChanged(state, status);
        }
    }

    public async Task ConnectAsync(bool waitForReady, CancellationToken token)
    {
        await EnsureResolverStartedAsync().ConfigureAwait(false);

        if (!waitForReady || State == ConnectivityState.Ready)
            return;
        else
        {
            Task waitForReadyTask;

            lock (_lock)
            {
                var state = State;

                if (state == ConnectivityState.Ready)
                    return;

                waitForReadyTask = WaitForStateChangedAsync(state, waitForState: ConnectivityState.Ready, token);
                _balancer?.RequestConnection();
            }

            await waitForReadyTask.ConfigureAwait(false);
        }
    }

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private Task EnsureResolverStartedAsync()
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
    {
        // Ensure that the resolver has started and has resolved at least once.
        // This ensures an inner load balancer has been created and is running.
        if (!_resolverStarted)
        {
            lock (_lock)
            {
                if (!_resolverStarted)
                {
                    _resolver.Start(OnResolverResult);
                    _resolver.Refresh();

                    _resolverStarted = true;
                }
            }
        }

        return _resolverStartedTcs.Task;
    }

    public void UpdateState(BalancerState state)
    {
        lock (_lock)
        {
            if (State != state.ConnectivityState)
            {
                State = state.ConnectivityState;

                // Iterate in reverse to reduce shifting items in the list as watchers are removed.
                for (var i = _stateWatchers.Count - 1; i >= 0; i--)
                {
                    var stateWatcher = _stateWatchers[i];

                    // Trigger watcher if either:
                    // 1. Watcher is waiting for any state change.
                    // 2. The state change matches the watcher's.
                    if (stateWatcher.WaitForState == null || stateWatcher.WaitForState == State)
                    {
                        _stateWatchers.RemoveAt(i);
                        stateWatcher.Tcs.SetResult(null);
                    }
                }
            }

            if (!Equals(_picker, state.Picker))
            {
                _picker = state.Picker;
                _pickerTask = Task.FromResult(state.Picker);
                _nextPickerTcs.SetResult(state.Picker);
                _nextPickerTcs = new TaskCompletionSource<SubchannelPicker>(TaskCreationOptions.RunContinuationsAsynchronously);
            }
        }
    }

    public async ValueTask<(Subchannel Subchannel, BalancerAddress Address, ISubchannelCallTracker? SubchannelCallTracker)> PickAsync(
        PickContext context, bool waitForReady, CancellationToken token)
    {
        SubchannelPicker? previousPicker = null;

        // Wait for a valid picker. When the client state changes a new picker will be returned.
        // Cancellation will break out of the loop. Typically cancellation will come from a
        // deadline specified for a call being exceeded.
        while (true)
        {
            var currentPicker = await GetPickerAsync(previousPicker, token).ConfigureAwait(false);
            var result = currentPicker.Pick(context);

            switch (result.Type)
            {
                case PickResultType.Complete:
                    var subchannel = result.Subchannel!;
                    var (address, state) = subchannel.GetAddressAndState();

                    if (address != null)
                    {
                        if (state == ConnectivityState.Ready)
                            return (subchannel, address, result.SubchannelCallTracker);
                        else
                            previousPicker = currentPicker;
                    }
                    else
                        previousPicker = currentPicker;

                    break;
                case PickResultType.Queue:
                    previousPicker = currentPicker;
                    break;
                case PickResultType.Fail:
                    if (waitForReady)
                        previousPicker = currentPicker;
                    else
                        throw new RpcException(result.Status);

                    break;
                case PickResultType.Drop:
                    // Use metadata on the exception to signal the request was dropped.
                    // Metadata is checked by retry. If request was dropped then it isn't retried.
                    var metadata = new Metadata { new Metadata.Entry(GrpcProtocolConstants.DropRequestTrailer, bool.TrueString) };
                    throw new RpcException(result.Status, metadata);
                default:
                    throw new InvalidOperationException($"Unexpected pick result type: {result.Type}");
            }
        }
    }

    private Task<SubchannelPicker> GetPickerAsync(SubchannelPicker? currentPicker, CancellationToken token)
    {
        lock (_lock)
        {
            if (_picker != null && _picker != currentPicker)
            {
                Debug.Assert(_pickerTask != null);
                return _pickerTask;
            }
            else
                return _nextPickerTcs.Task.WaitAsync(token);
        }
    }

    internal Task WaitForStateChangedAsync(ConnectivityState lastObservedState, ConnectivityState? waitForState, CancellationToken token)
    {
        StateWatcher? watcher;

        lock (_lock)
        {
            if (State != lastObservedState)
                return Task.CompletedTask;
            else
            {
                // Minor optimization to check if we're already waiting for state change
                // using the specified cancellation token.
                foreach (var stateWatcher in _stateWatchers)
                {
                    if (stateWatcher.CancellationToken == token && stateWatcher.WaitForState == waitForState)
                        return stateWatcher.Tcs.Task;
                }

                watcher = new StateWatcher(waitForState, new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously), token);
                _stateWatchers.Add(watcher);
            }
        }

        return WaitForStateChangedAsyncCore(watcher);
    }

    private async Task WaitForStateChangedAsyncCore(StateWatcher watcher)
    {
        using (watcher.CancellationToken.Register(OnCancellation, watcher))
            await watcher.Tcs.Task.ConfigureAwait(false);
    }

    private void OnCancellation(object? s)
    {
        lock (_lock)
        {
            StateWatcher watcher = (StateWatcher)s!;

            if (_stateWatchers.Remove(watcher))
                watcher.Tcs.SetCanceled(watcher.CancellationToken);
        }
    }

    // Use a standard class for the watcher because:
    // 1. On cancellation, a watcher is removed from collection. Should use default Equals implementation. Record overrides Equals.
    // 2. This type is cast to object. A struct will box.
    private sealed class StateWatcher
    {
        public StateWatcher(ConnectivityState? waitForState, TaskCompletionSource<object?> tcs, CancellationToken token)
        {
            WaitForState = waitForState;
            Tcs = tcs;
            CancellationToken = token;
        }

        public ConnectivityState? WaitForState { get; }

        public TaskCompletionSource<object?> Tcs { get; }

        public CancellationToken CancellationToken { get; }
    }

    public void Dispose()
    {
        _resolver.Dispose();

        lock (_lock)
        {
            _balancer?.Dispose();

            // Cancel pending state watchers.
            // Iterate in reverse to reduce shifting items in the list as watchers are removed.
            for (var i = _stateWatchers.Count - 1; i >= 0; i--)
            {
                var stateWatcher = _stateWatchers[i];

                stateWatcher.Tcs.SetCanceled();
                _stateWatchers.RemoveAt(i);
            }
        }
    }

}
