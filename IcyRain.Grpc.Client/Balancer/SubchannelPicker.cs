namespace IcyRain.Grpc.Client.Balancer;

/// <summary>
/// Base type for picking a subchannel. A <see cref="SubchannelPicker"/> is responsible for picking
/// a ready <see cref="Subchannel"/> that gRPC calls will use
/// <para>
/// Load balancers implement <see cref="SubchannelPicker"/> with their own balancing logic to
/// determine which subchannel is returned for a call
/// </para>
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public abstract class SubchannelPicker
{
    /// <summary>Picks a ready <see cref="Subchannel"/> for the specified context</summary>
    /// <param name="context">The pick content</param>
    /// <returns>A ready <see cref="Subchannel"/></returns>
    public abstract PickResult Pick(PickContext context);
}
