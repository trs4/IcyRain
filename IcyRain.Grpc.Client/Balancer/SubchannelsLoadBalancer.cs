using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Grpc.Core;
using IcyRain.Grpc.Client.Balancer.Internal;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>
/// An abstract <see cref="LoadBalancer"/> that manages creating <see cref="Subchannel"/> instances
/// from addresses. It is designed to make it easy to implement a custom picking policy by overriding
/// <see cref="CreatePicker(IReadOnlyList{Subchannel})"/> and returning a custom <see cref="SubchannelPicker"/>.
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public abstract class SubchannelsLoadBalancer : LoadBalancer
{
    /// <summary>Gets the controller</summary>
    protected IChannelControlHelper Controller { get; }

    /// <summary>Gets the connectivity state</summary>
    protected ConnectivityState State { get; private set; }

    private readonly List<AddressSubchannel> _addressSubchannels;

    /// <summary>Initializes a new instance of the <see cref="SubchannelsLoadBalancer"/> class</summary>
    /// <param name="controller">The controller</param>
    protected SubchannelsLoadBalancer(IChannelControlHelper controller)
    {
        _addressSubchannels = [];
        Controller = controller;
    }

    private void ResolverError(Status status)
    {
        // If balancer doesn't have a ready subchannel then remove any current subchannels
        // and update channel state with resolver error.
        switch (State)
        {
            case ConnectivityState.Idle:
            case ConnectivityState.Connecting:
            case ConnectivityState.TransientFailure:
                foreach (var addressSubchannel in _addressSubchannels)
                    RemoveSubchannel(addressSubchannel.Subchannel);
                
                _addressSubchannels.Clear();
                Controller.UpdateState(new BalancerState(ConnectivityState.TransientFailure, new ErrorPicker(status)));
                break;
        }
    }

    private static int? FindSubchannelByAddress(List<AddressSubchannel> addressSubchannels, BalancerAddress address)
    {
        for (var i = 0; i < addressSubchannels.Count; i++)
        {
            var s = addressSubchannels[i];

            if (Equals(s.Address.EndPoint, address.EndPoint))
                return i;
        }

        return null;
    }

    private static AddressSubchannel? FindSubchannel(List<AddressSubchannel> addressSubchannels, Subchannel subchannel)
    {
        for (var i = 0; i < addressSubchannels.Count; i++)
        {
            var s = addressSubchannels[i];

            if (Equals(s.Subchannel, subchannel))
                return s;
        }

        return null;
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
            ResolverError(new Status(StatusCode.Unavailable, "Resolver returned no addresses"));
            return;
        }

        var allUpdatedSubchannels = new List<AddressSubchannel>();
        var newSubchannels = new List<Subchannel>();
        var hasModifiedSubchannels = false;
        var currentSubchannels = _addressSubchannels.ToList();

        // The state's addresses is the new authoritative list of addresses.
        // However, we want to keep existing subchannels when possible.
        foreach (var address in state.Addresses)
        {
            // Check existing subchannels for a match.
            var i = FindSubchannelByAddress(currentSubchannels, address);
            AddressSubchannel newOrCurrentSubchannel;

            if (i != null)
            {
                // There is a match so take current subchannel.
                newOrCurrentSubchannel = currentSubchannels[i.Value];

                // Remove from current collection because any subchannels
                // remaining in this collection at the end will be disposed.
                currentSubchannels.RemoveAt(i.Value);

                // Check if address attributes have changed. If they have then update the subchannel address.
                // The new subchannel address has the same endpoint so the connection isn't impacted.
                if (!BalancerAddressEqualityComparer.Instance.Equals(address, newOrCurrentSubchannel.Address))
                {
                    newOrCurrentSubchannel = new AddressSubchannel(
                        newOrCurrentSubchannel.Subchannel,
                        address,
                        newOrCurrentSubchannel.LastKnownState);

                    newOrCurrentSubchannel.Subchannel.UpdateAddresses([address]);
                    hasModifiedSubchannels = true;
                }
            }
            else
            {
                // No match so create a new subchannel.
                var c = Controller.CreateSubchannel(new SubchannelOptions([address]));
                c.OnStateChanged(s => UpdateSubchannelState(c, s));

                newSubchannels.Add(c);
                newOrCurrentSubchannel = new AddressSubchannel(c, address);
            }

            allUpdatedSubchannels.Add(newOrCurrentSubchannel);
        }

        // Any sub-connections still in this collection are no longer returned by the resolver.
        // This can all be removed.
        var removedSubConnections = currentSubchannels;

        if (removedSubConnections.Count == 0 && newSubchannels.Count == 0 && !hasModifiedSubchannels)
            return;

        foreach (var removedSubConnection in removedSubConnections)
            RemoveSubchannel(removedSubConnection.Subchannel);

        _addressSubchannels.Clear();
        _addressSubchannels.AddRange(allUpdatedSubchannels);

        // Start new connections after collection on balancer has been updated.
        foreach (var c in newSubchannels)
            c.RequestConnection();

        UpdateBalancingState(state.Status);
    }

    private void UpdateBalancingState(Status status)
    {
        var readySubchannels = new List<Subchannel>();

        for (var i = 0; i < _addressSubchannels.Count; i++)
        {
            var addressSubchannel = _addressSubchannels[i];

            if (addressSubchannel.LastKnownState == ConnectivityState.Ready)
                readySubchannels.Add(addressSubchannel.Subchannel);
        }

        if (readySubchannels.Count == 0)
        {
            // No READY subchannels, determine aggregate state and error status
            var isConnecting = false;

            foreach (var subchannel in _addressSubchannels)
            {
                var state = subchannel.LastKnownState;

                if (state == ConnectivityState.Connecting || state == ConnectivityState.Idle)
                {
                    isConnecting = true;
                    break;
                }
            }

            if (isConnecting)
                UpdateChannelState(ConnectivityState.Connecting, EmptyPicker.Instance);
            else
            {
                // Only care about status if it is non-OK.
                // Pass it to the picker so that it is reported to the caller on pick.
                var errorStatus = status.StatusCode != StatusCode.OK
                    ? status
                    : new Status(StatusCode.Internal, "Unknown error");

                UpdateChannelState(ConnectivityState.TransientFailure, new ErrorPicker(errorStatus));
            }
        }
        else
            UpdateChannelState(ConnectivityState.Ready, CreatePicker(readySubchannels));
    }

    private void UpdateChannelState(ConnectivityState state, SubchannelPicker subchannelPicker)
    {
        State = state;
        Controller.UpdateState(new BalancerState(state, subchannelPicker));
    }

    private void UpdateSubchannelState(Subchannel subchannel, SubchannelState state)
    {
        var addressSubchannel = FindSubchannel(_addressSubchannels, subchannel);

        if (addressSubchannel is null)
            return;

        addressSubchannel.UpdateKnownState(state.State);

        UpdateBalancingState(state.Status);

        if (state.State == ConnectivityState.TransientFailure || state.State == ConnectivityState.Idle)
            Controller.RefreshResolver();

        if (state.State == ConnectivityState.Idle)
            subchannel.RequestConnection();
    }

    private static void RemoveSubchannel(Subchannel subchannel) => subchannel.Dispose();

    public override void RequestConnection()
    {
        foreach (var addressSubchannel in _addressSubchannels)
            addressSubchannel.Subchannel.RequestConnection();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        foreach (var addressSubchannel in _addressSubchannels)
            RemoveSubchannel(addressSubchannel.Subchannel);
        
        _addressSubchannels.Clear();
    }

    /// <summary>
    /// Creates a <see cref="SubchannelPicker"/> for the specified <see cref="Subchannel"/> instances.
    /// This method can be overriden to return new a <see cref="SubchannelPicker"/> implementation
    /// with custom load balancing logic.
    /// </summary>
    /// <param name="readySubchannels">A collection of ready subchannels.</param>
    /// <returns>A subchannel picker.</returns>
    protected abstract SubchannelPicker CreatePicker(IReadOnlyList<Subchannel> readySubchannels);

    [DebuggerDisplay("Subchannel = {Subchannel.Id}, Address = {Address}, LastKnownState = {LastKnownState}")]
    private sealed class AddressSubchannel
    {
        private ConnectivityState _lastKnownState;

        public AddressSubchannel(Subchannel subchannel, BalancerAddress address, ConnectivityState lastKnownState = ConnectivityState.Idle)
        {
            Subchannel = subchannel;
            Address = address;
            _lastKnownState = lastKnownState;
        }

        // Track connectivity state that has been updated to load balancer.
        // This is used instead of state on subchannel because subchannel state
        // can be updated from other threads while load balancer is running.
        public ConnectivityState LastKnownState => _lastKnownState;

        public Subchannel Subchannel { get; }

        public BalancerAddress Address { get; }

        public void UpdateKnownState(ConnectivityState knownState) => _lastKnownState = knownState;
    }

}
