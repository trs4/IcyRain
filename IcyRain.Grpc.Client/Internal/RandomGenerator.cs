using System;

namespace IcyRain.Grpc.Client.Internal;

internal interface IRandomGenerator
{
    int Next(int minValue, int maxValue);

    double NextDouble();
}

internal sealed class RandomGenerator : IRandomGenerator
{
    private readonly Random _random;

    public RandomGenerator()
    {
        // Can't use a singleton instance of RandomGenerator.
        // Random isn't threadsafe and Random.Shared requires .NET 6+.
        _random = new Random();
    }

    public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);

    public double NextDouble() => _random.NextDouble();
}
