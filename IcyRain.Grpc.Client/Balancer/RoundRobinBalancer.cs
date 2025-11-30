using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IcyRain.Grpc.Client.Configuration;
using IcyRain.Grpc.Client.Internal;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>
/// A <see cref="LoadBalancer"/> that attempts to connect to all addresses. gRPC calls are distributed
/// across all successful connections using round-robin logic
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
internal sealed class RoundRobinBalancer : SubchannelsLoadBalancer
{
    private readonly IRandomGenerator _randomGenerator;

    /// <summary>Initializes a new instance of the <see cref="RoundRobinBalancer"/> class</summary>
    /// <param name="controller">The controller</param>
    /// <param name="loggerFactory">The logger factory</param>
    public RoundRobinBalancer(IChannelControlHelper controller) : this(controller, new RandomGenerator()) { }

    internal RoundRobinBalancer(IChannelControlHelper controller, IRandomGenerator randomGenerator)
        : base(controller) => _randomGenerator = randomGenerator;

    protected override SubchannelPicker CreatePicker(IReadOnlyList<Subchannel> readySubchannels)
    {
        var pickCount = _randomGenerator.Next(0, readySubchannels.Count);
        return new RoundRobinPicker(readySubchannels, pickCount);
    }

}

internal sealed class RoundRobinPicker : SubchannelPicker
{
    // Internal for testing
    internal readonly List<Subchannel> _subchannels;
    private long _pickCount;

    public RoundRobinPicker(IReadOnlyList<Subchannel> subchannels, long pickCount)
    {
        _subchannels = [.. subchannels];
        _pickCount = pickCount;
    }

    public override PickResult Pick(PickContext context)
    {
        var c = Interlocked.Increment(ref _pickCount);
        var index = c % _subchannels.Count;
        var item = _subchannels[(int)index];

        return PickResult.ForSubchannel(item);
    }

    public override string ToString() => string.Join(", ", _subchannels.Select(s => s.ToString()));
}

/// <summary>
/// A <see cref="LoadBalancerFactory"/> that matches the name <c>round_robin</c>
/// and creates <see cref="RoundRobinBalancer"/> instances
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class RoundRobinBalancerFactory : LoadBalancerFactory
{
    public override string Name { get; } = LoadBalancingConfig.RoundRobinPolicyName;

    public override LoadBalancer Create(LoadBalancerOptions options) => new RoundRobinBalancer(options.Controller);
}
