using System;
using Grpc.Core;
using IcyRain.Grpc.Client.Balancer.Internal;
using IcyRain.Grpc.Client.Configuration;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>
/// A <see cref="LoadBalancer"/> that attempts to connect to addresses until a connection
/// is successfully made. gRPC calls are all made to the first successful connection.
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
internal sealed class PickFirstBalancer : LoadBalancer
{
    private readonly IChannelControlHelper _controller;

    internal Subchannel? _subchannel;
    private ConnectivityState _state;

    /// <summary>Initializes a new instance of the <see cref="PickFirstBalancer"/> class</summary>
    /// <param name="controller">The controller</param>
    public PickFirstBalancer(IChannelControlHelper controller)
        => _controller = controller;

    private void ResolverError(Status status)
    {
        // If balancer doesn't have a ready subchannel then remove any current subchannel
        // and update channel state with resolver error
        switch (_state)
        {
            case ConnectivityState.Idle:
            case ConnectivityState.Connecting:
            case ConnectivityState.TransientFailure:
                if (_subchannel is not null)
                    RemoveSubchannel();
                
                _controller.UpdateState(new BalancerState(ConnectivityState.TransientFailure, new ErrorPicker(status)));
                break;
        }
    }

    private void RemoveSubchannel()
    {
        _subchannel?.Dispose();
        _subchannel = null;
    }

    public override void UpdateChannelState(ChannelState state)
    {
        if (state.Status.StatusCode != StatusCode.OK)
        {
            ResolverError(state.Status);
            return;
        }

        if (state.Addresses is null || state.Addresses.Count == 0)
        {
            ResolverError(new Status(StatusCode.Unavailable, "Resolver returned no addresses."));
            return;
        }

        if (_subchannel is null)
        {
            try
            {
                _subchannel = _controller.CreateSubchannel(new SubchannelOptions(state.Addresses));
                _subchannel.OnStateChanged(s => UpdateSubchannelState(_subchannel, s));
            }
            catch (Exception ex)
            {
                var picker = new ErrorPicker(new Status(StatusCode.Unavailable, "Error creating subchannel.", ex));
                _controller.UpdateState(new BalancerState(ConnectivityState.TransientFailure, picker));
                throw;
            }

            _controller.UpdateState(new BalancerState(ConnectivityState.Idle, EmptyPicker.Instance));
            _subchannel.RequestConnection();
        }
        else
            _subchannel.UpdateAddresses(state.Addresses);
    }

    private void UpdateSubchannelState(Subchannel subchannel, SubchannelState state)
    {
        if (_subchannel != subchannel)
            return;

        switch (state.State)
        {
            case ConnectivityState.Ready:
                UpdateChannelState(state.State, new PickFirstPicker(_subchannel));
                break;
            case ConnectivityState.Idle:
                _controller.RefreshResolver();

                // Pick first load balancer waits until a request is made before establishing a connection.
                // Return picker that will request a connection on pick.
                UpdateChannelState(state.State, new RequestConnectionPicker(_subchannel));
                break;
            case ConnectivityState.Connecting:
                UpdateChannelState(state.State, EmptyPicker.Instance);
                break;
            case ConnectivityState.TransientFailure:
                UpdateChannelState(state.State, new ErrorPicker(state.Status));
                break;
            case ConnectivityState.Shutdown:
                UpdateChannelState(state.State, EmptyPicker.Instance);
                _subchannel = null;
                break;
        }
    }

    private void UpdateChannelState(ConnectivityState state, SubchannelPicker subchannelPicker)
    {
        _state = state;
        _controller.UpdateState(new BalancerState(state, subchannelPicker));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        RemoveSubchannel();
    }

    public override void RequestConnection() => _subchannel?.RequestConnection();
}

internal class PickFirstPicker : SubchannelPicker
{
    internal Subchannel Subchannel { get; }

    public PickFirstPicker(Subchannel subchannel)
        => Subchannel = subchannel;

    public override PickResult Pick(PickContext context) => PickResult.ForSubchannel(Subchannel);
}

internal sealed class RequestConnectionPicker : PickFirstPicker
{
    public RequestConnectionPicker(Subchannel subchannel) : base(subchannel) { }

    public override PickResult Pick(PickContext context)
    {
        Subchannel.RequestConnection();
        return base.Pick(context);
    }
}

/// <summary>A <see cref="LoadBalancerFactory"/> that matches the name <c>pick_first</c> and creates <see cref="PickFirstBalancer"/> instances
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class PickFirstBalancerFactory : LoadBalancerFactory
{
    public override string Name { get; } = LoadBalancingConfig.PickFirstPolicyName;

    public override LoadBalancer Create(LoadBalancerOptions options) => new PickFirstBalancer(options.Controller);
}
