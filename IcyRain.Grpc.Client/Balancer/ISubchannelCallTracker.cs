namespace IcyRain.Grpc.Client.Balancer;

/// <summary>An interface for tracking subchannel calls
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public interface ISubchannelCallTracker
{
    /// <summary>Called when a subchannel call is started after a load balance pick</summary>
    void Start();

    /// <summary>Called when a subchannel call is completed</summary>
    /// <param name="context">The complete context</param>
    void Complete(CompletionContext context);
}
