using System.Threading;

namespace IcyRain.Grpc.Client.Balancer.Internal;

internal sealed class ChannelIdProvider
{
    private long _currentChannelId;

    public long GetNextChannelId() => Interlocked.Increment(ref _currentChannelId);
}
