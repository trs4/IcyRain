using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Grpc.Client.Balancer.Internal;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>
/// Represents a logical connection. A subchannel is created with one or more addresses to equivalent servers.
/// <para>
/// A subchannel maintains at most one physical connection (aka transport) for sending new gRPC calls.
/// If there isn't an active transport, and a call is assigned to the subchannel, it will create
/// a new transport. A transport won't be created otherwise unless <see cref="RequestConnection"/>
/// is called to create a transport if there isn't any.
/// </para>
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class Subchannel : IDisposable
{
    internal readonly List<BalancerAddress> _addresses;
    internal readonly object Lock;

    internal readonly ConnectionManager _manager;
    private readonly SemaphoreSlim _connectSemaphore;

    private ISubchannelTransport _transport = default!;
    private ConnectContext? _connectContext;
    private ConnectivityState _state;
    private TaskCompletionSource<object?>? _delayInterruptTcs;
    private int _currentRegistrationId;

    internal ISubchannelTransport Transport => _transport;

    internal string Id { get; }

    /// <summary>
    /// Connectivity state is internal rather than public because it can be updated by multiple threads while
    /// a load balancer is building the picker.
    /// Load balancers that care about multiple subchannels should track state by subscribing to
    /// Subchannel.OnStateChanged and storing results
    /// </summary>
    internal ConnectivityState State => _state;

    /// <summary>Gets the current connected address</summary>
    public BalancerAddress? CurrentAddress
    {
        get
        {
            if (_transport.CurrentEndPoint is { } ep)
            {
                lock (Lock)
                    return GetAddressByEndpoint(_addresses, ep);
            }

            return null;
        }
    }

    /// <summary>Gets the metadata attributes</summary>
    public BalancerAttributes Attributes { get; }

    internal (BalancerAddress? Address, ConnectivityState State) GetAddressAndState()
    {
        lock (Lock)
            return (CurrentAddress, State);
    }

    internal Subchannel(ConnectionManager manager, IReadOnlyList<BalancerAddress> addresses)
    {
        Lock = new object();
        _connectSemaphore = new SemaphoreSlim(1);

        Id = manager.GetNextId();
        _addresses = [.. addresses];
        _manager = manager;
        Attributes = new BalancerAttributes();
    }

    internal void SetTransport(ISubchannelTransport transport)
        => _transport = transport;

    private readonly List<StateChangedRegistration> _stateChangedRegistrations = [];

    /// <summary>Registers a callback that will be invoked this subchannel's state changes</summary>
    /// <param name="callback">The callback that will be invoked when the subchannel's state changes</param>
    /// <returns>A subscription that can be disposed to unsubscribe from state changes</returns>
    public IDisposable OnStateChanged(Action<SubchannelState> callback)
    {
        var registration = new StateChangedRegistration(this, callback);
        _stateChangedRegistrations.Add(registration);
        return registration;
    }

    private string GetNextRegistrationId()
    {
        var registrationId = Interlocked.Increment(ref _currentRegistrationId);
        return Id + "-" + registrationId;
    }

    private sealed class StateChangedRegistration : IDisposable
    {
        private readonly Subchannel _subchannel;
        private readonly Action<SubchannelState> _callback;

        public string RegistrationId { get; }

        public StateChangedRegistration(Subchannel subchannel, Action<SubchannelState> callback)
        {
            _subchannel = subchannel;
            _callback = callback;
            RegistrationId = subchannel.GetNextRegistrationId();
        }

        public void Invoke(SubchannelState state) => _callback(state);

        public void Dispose() => _subchannel._stateChangedRegistrations.Remove(this);
    }

    /// <summary>Replaces the existing addresses used with this <see cref="Subchannel"/>
    /// <para>
    /// If the subchannel has an active connection and the new addresses contain the connected address
    /// then the connection is reused. Otherwise the subchannel will reconnect.
    /// </para>
    /// </summary>
    public void UpdateAddresses(IReadOnlyList<BalancerAddress> addresses)
    {
        var requireReconnect = false;

        lock (Lock)
        {
            if (_addresses.SequenceEqual(addresses, BalancerAddressEqualityComparer.Instance))
                return; // Don't do anything if new addresses match existing addresses

            // Get a copy of the current address before updating addresses.
            // Updating addresses to not contain this value changes the property to return null.
            var currentAddress = CurrentAddress;

            _addresses.Clear();
            _addresses.AddRange(addresses);

            switch (_state)
            {
                case ConnectivityState.Idle:
                    break;
                case ConnectivityState.Connecting:
                case ConnectivityState.TransientFailure:
                    requireReconnect = true;
                    break;
                case ConnectivityState.Ready:
                    // Check if the subchannel is connected to an address that's not longer present.
                    // In this situation require the subchannel to reconnect to a new address.
                    if (currentAddress is not null)
                    {
                        if (GetAddressByEndpoint(_addresses, currentAddress.EndPoint) is null)
                            requireReconnect = true;
                    }
                    break;
                case ConnectivityState.Shutdown:
                    throw new InvalidOperationException($"Subchannel id '{Id}' has been shutdown.");
                default:
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                    throw new ArgumentOutOfRangeException("state", _state, "Unexpected state.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
            }
        }

        if (requireReconnect)
        {
            lock (Lock)
                CancelInProgressConnectUnsynchronized();
            
            _transport.Disconnect();
            RequestConnection();
        }
    }

    /// <summary>Creates a connection (aka transport), if there isn't an active one</summary>
    public void RequestConnection()
    {
        var connectionRequested = false;

        lock (Lock)
        {
            switch (_state)
            {
                case ConnectivityState.Idle:
                    // Only start connecting underlying transport if in an idle state.
                    // Update connectivity state outside of subchannel lock to avoid deadlock.
                    connectionRequested = true;
                    break;
                case ConnectivityState.Connecting:
                case ConnectivityState.Ready:
                case ConnectivityState.TransientFailure:
                    // We're already attempting to connect to the transport.
                    // If the connection is waiting in a delayed backoff then interrupt
                    // the delay and immediately retry connection.
                    _delayInterruptTcs?.TrySetResult(null);
                    return;
                case ConnectivityState.Shutdown:
                    throw new InvalidOperationException($"Subchannel id '{Id}' has been shutdown.");
                default:
                    throw new ArgumentOutOfRangeException("state", _state, "Unexpected state.");
            }
        }

        Debug.Assert(connectionRequested, "Ensure that only expected state made it to this point.");

        UpdateConnectivityState(ConnectivityState.Connecting, "Connection requested.");

        // Don't capture the current ExecutionContext and its AsyncLocals onto the connect
        var restoreFlow = false;

        if (!ExecutionContext.IsFlowSuppressed())
        {
            ExecutionContext.SuppressFlow();
            restoreFlow = true;
        }

        _ = Task.Run(ConnectTransportAsync);

        // Restore the current ExecutionContext
        if (restoreFlow)
            ExecutionContext.RestoreFlow();
    }

    private void CancelInProgressConnectUnsynchronized()
    {
        Debug.Assert(Monitor.IsEntered(Lock));

        if (_connectContext != null && !_connectContext.Disposed)
        {
            // Cancel connect cancellation token
            _connectContext.CancelConnect();
            _connectContext.Dispose();
        }

        _delayInterruptTcs?.TrySetResult(null);
    }

    private ConnectContext GetConnectContextUnsynchronized()
    {
        Debug.Assert(Monitor.IsEntered(Lock));

        // There shouldn't be a previous connect in progress, but cancel the CTS to ensure they're no longer running
        CancelInProgressConnectUnsynchronized();

        var connectContext = _connectContext = new ConnectContext(_transport.ConnectTimeout ?? Timeout.InfiniteTimeSpan);
        return connectContext;
    }

    private async Task ConnectTransportAsync()
    {
        ConnectContext connectContext;
        Task? waitSemaporeTask = null;

        lock (Lock)
        {
            // Don't start connecting if the subchannel has been shutdown. Transport/semaphore will be disposed if shutdown
            if (_state == ConnectivityState.Shutdown)
                return;

            // There is already a connect in-progress on this transport.
            // Don't cancel and start again as that causes queued requests waiting on the connect to fail
            if (_connectContext != null && !_connectContext.Disposed)
            {
                _delayInterruptTcs?.TrySetResult(null);
                return;
            }

            connectContext = GetConnectContextUnsynchronized();

            // Use a semaphore to limit one connection attempt at a time. This is done to prevent a race conditional where a canceled connect
            // overwrites the status of a successful connect.
            //
            // Try to get semaphore without waiting. If semaphore is already taken then start a task to wait for it to be released.
            // Start this inside a lock to make sure subchannel isn't shutdown before waiting for semaphore
            if (!_connectSemaphore.Wait(0))
                waitSemaporeTask = _connectSemaphore.WaitAsync(connectContext.CancellationToken);
        }

        if (waitSemaporeTask is not null)
        {
            try
            {
                await waitSemaporeTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return; // Canceled while waiting for semaphore
            }
        }

        try
        {
            var backoffPolicy = _manager.BackoffPolicyFactory.Create();

            for (var attempt = 0; ; attempt++)
            {
                lock (Lock)
                {
                    if (_state == ConnectivityState.Shutdown)
                        return;
                }

                switch (await _transport.TryConnectAsync(connectContext, attempt).ConfigureAwait(false))
                {
                    case ConnectResult.Success:
                        return;
                    case ConnectResult.Timeout:
                        // Reset connectivity state back to idle so that new calls try to reconnect.
                        UpdateConnectivityState(ConnectivityState.Idle, new Status(StatusCode.Unavailable, "Timeout connecting to subchannel"));
                        return;
                    case ConnectResult.Failure:
                    default:
                        break;
                }

                connectContext.CancellationToken.ThrowIfCancellationRequested();

                _delayInterruptTcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
#pragma warning disable CA2000 // Dispose objects before losing scope
                var delayCts = new CancellationTokenSource();
#pragma warning restore CA2000 // Dispose objects before losing scope

                var backoffTicks = backoffPolicy.NextBackoff().Ticks;
                // Task.Delay supports up to Int32.MaxValue milliseconds.
                // Note that even if the maximum backoff is configured to this maximum, the jitter could push it over the limit.
                // Force an upper bound here to ensure an unsupported backoff is never used.
                backoffTicks = Math.Min(backoffTicks, TimeSpan.TicksPerMillisecond * int.MaxValue);
                
                var backkoff = TimeSpan.FromTicks(backoffTicks);
                var completedTask = await Task.WhenAny(Task.Delay(backkoff, delayCts.Token), _delayInterruptTcs.Task).ConfigureAwait(false);

                if (completedTask != _delayInterruptTcs.Task)
                {
                    // Task.Delay won. Check CTS to see if it won because of cancellation
                    delayCts.Token.ThrowIfCancellationRequested();
                }
                else
                {
                    // Cancel the Task.Delay that's no longer needed
                    delayCts.Cancel();

                    // Check to connect context token to see if the delay was interrupted because of a connect cancellation
                    connectContext.CancellationToken.ThrowIfCancellationRequested();

                    // Delay interrupt was triggered. Reset back-off
                    backoffPolicy = _manager.BackoffPolicyFactory.Create();
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            UpdateConnectivityState(ConnectivityState.TransientFailure, new Status(StatusCode.Unavailable, "Error connecting to subchannel", ex));
        }
        finally
        {
            lock (Lock)
            {
                // Dispose context because it might have been created with a connect timeout.
                // Want to clean up the connect timeout timer.
                connectContext.Dispose();

                // Subchannel could have been disposed while connect is running.
                // If subchannel is shutting down then don't release semaphore to avoid ObjectDisposedException.
                if (_state != ConnectivityState.Shutdown)
                    _connectSemaphore.Release();
            }
        }
    }

    internal bool UpdateConnectivityState(ConnectivityState state, string successDetail)
        => UpdateConnectivityState(state, new Status(StatusCode.OK, successDetail));

    internal bool UpdateConnectivityState(ConnectivityState state, Status status)
    {
        Debug.Assert(!Monitor.IsEntered(Lock), "Ensure the subchannel lock isn't held here. Updating channel state with the subchannel lock can cause a deadlock.");

        lock (Lock)
        {
            // Don't update subchannel state if the state is the same or the subchannel has been shutdown.
            //
            // This could happen when:
            // 1. Start trying to connect with a subchannel.
            // 2. Address resolver updates and subchannel address is no longer there and subchannel is shutdown.
            // 3. Connection attempt fails and tries to update subchannel state.
            if (_state == state || _state == ConnectivityState.Shutdown)
                return false;
            
            _state = state;
        }

        // Notify channel outside of lock to avoid deadlocks.
        _manager.OnSubchannelStateChange(this, state, status);
        return true;
    }

    internal void RaiseStateChanged(ConnectivityState state, Status status)
    {
        if (_stateChangedRegistrations.Count > 0)
        {
            var subchannelState = new SubchannelState(state, status);

            foreach (var registration in _stateChangedRegistrations)
                registration.Invoke(subchannelState);
        }
    }

    private static BalancerAddress? GetAddressByEndpoint(List<BalancerAddress> addresses, DnsEndPoint endPoint)
    {
        foreach (var a in addresses)
        {
            if (a.EndPoint.Equals(endPoint))
                return a;
        }

        return null;
    }

    public override string ToString()
    {
        lock (Lock)
            return $"Id: {Id}, Addresses: {string.Join(", ", _addresses)}, State: {_state}, Current address: {CurrentAddress}";
    }

    /// <summary>Returns the addresses that this subchannel is bound to</summary>
    /// <returns>The addresses that this subchannel is bound to</returns>
    public IReadOnlyList<BalancerAddress> GetAddresses()
    {
        lock (Lock)
            return [.. _addresses];
    }

    /// <summary>
    /// Disposes the <see cref="Subchannel"/>.
    /// The subchannel state is updated to <see cref="ConnectivityState.Shutdown"/>.
    /// After dispose the subchannel should no longer be returned by the latest <see cref="SubchannelPicker"/>.
    /// </summary>
    public void Dispose()
    {
        UpdateConnectivityState(ConnectivityState.Shutdown, "Subchannel disposed.");
        _stateChangedRegistrations.Clear();

        lock (Lock)
        {
            CancelInProgressConnectUnsynchronized();
            _transport.Dispose();
            _connectSemaphore.Dispose();
        }
    }

}
