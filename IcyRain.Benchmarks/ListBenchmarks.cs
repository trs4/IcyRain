using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using IcyRain.Internal;

#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1002 // Do not expose generic lists
namespace IcyRain.Benchmarks;

[MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class ListBenchmarks
{
    private static readonly int[] Array = new int[] { 1, 0, 0, 1, 0, 2345, 34573, 34567, 3456, 76, 2345, 2645, 3457, 234 };

    [Benchmark]
    public List<int> Create()
        => new List<int>(Array);

    [Benchmark]
    public List<int> Reference()
        => Array.CreateList();
}
#pragma warning restore CA1002 // Do not expose generic lists
#pragma warning restore CA1707 // Identifiers should not contain underscores
