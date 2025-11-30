using System;
using System.Diagnostics;
using IcyRain.Grpc.Client.Internal;

namespace IcyRain.Grpc.Client.Balancer.Internal;

internal sealed class ExponentialBackoffPolicy : IBackoffPolicy
{
    internal const double Multiplier = 1.6;
    internal const double Jitter = 0.2;

    private readonly IRandomGenerator _randomGenerator;
    private readonly long _maxBackoffTicks;
    private long _nextBackoffTicks;

    public ExponentialBackoffPolicy(
        IRandomGenerator randomGenerator,
        long initialBackoffTicks,
        long maxBackoffTicks)
    {
        Debug.Assert(initialBackoffTicks > 0);
        Debug.Assert(maxBackoffTicks <= int.MaxValue);

        _randomGenerator = randomGenerator;
        _nextBackoffTicks = initialBackoffTicks;
        _maxBackoffTicks = maxBackoffTicks;
    }

    public TimeSpan NextBackoff()
    {
        var currentBackoffTicks = _nextBackoffTicks;
        _nextBackoffTicks = Math.Min((long)Math.Round(currentBackoffTicks * Multiplier), _maxBackoffTicks);

        currentBackoffTicks += UniformRandom(-Jitter * currentBackoffTicks, Jitter * currentBackoffTicks);
        return TimeSpan.FromTicks(currentBackoffTicks);
    }

    private long UniformRandom(double low, double high)
    {
        Debug.Assert(high >= low);

        var mag = high - low;
        return (long)(_randomGenerator.NextDouble() * mag + low);
    }

}

internal sealed class ExponentialBackoffPolicyFactory : IBackoffPolicyFactory
{
    private readonly IRandomGenerator _randomGenerator;
    private readonly TimeSpan _initialReconnectBackoff;
    private readonly TimeSpan? _maxReconnectBackoff;

    public ExponentialBackoffPolicyFactory(IRandomGenerator randomGenerator, TimeSpan initialReconnectBackoff, TimeSpan? maxReconnectBackoff)
    {
        _randomGenerator = randomGenerator;
        _initialReconnectBackoff = initialReconnectBackoff;
        _maxReconnectBackoff = maxReconnectBackoff;
    }

    public IBackoffPolicy Create()
    {
        // Limit ticks to Int32.MaxValue. Task.Delay can't use larger values,
        // and larger values mean we need to worry about overflows.
        return new ExponentialBackoffPolicy(
            _randomGenerator,
            Math.Min(_initialReconnectBackoff.Ticks, int.MaxValue),
            Math.Min(_maxReconnectBackoff?.Ticks ?? long.MaxValue, int.MaxValue));
    }

}
