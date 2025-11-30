namespace IcyRain.Grpc.Client.Balancer;

/// <summary>Provides essentials for <see cref="LoadBalancer"/> implementations
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public interface IChannelControlHelper
{
    /// <summary>
    /// Creates a <see cref="Subchannel"/>, which is a logical connection to the specified addresses.
    /// The <see cref="LoadBalancer"/> is responsible for closing unused subchannels, and closing
    /// all subchannels on shutdown
    /// </summary>
    /// <param name="options">The options for the new <see cref="Subchannel"/></param>
    /// <returns>A new <see cref="Subchannel"/></returns>
    Subchannel CreateSubchannel(SubchannelOptions options);

    /// <summary>
    /// Update the balancing state. This includes a new <see cref="ConnectivityState"/> and
    /// <see cref="SubchannelPicker"/>. The state is used by currently queued and future calls.
    /// </summary>
    /// <param name="state">The balancer state</param>
    void UpdateState(BalancerState state);

    /// <summary>Request the configured <see cref="Resolver"/> to refresh</summary>
    void RefreshResolver();
}
