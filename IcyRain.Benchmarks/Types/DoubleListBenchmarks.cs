using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;

namespace IcyRain.Benchmarks
{
    [MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class DoubleListBenchmarks
    {
        private static readonly List<double> Value;

        static DoubleListBenchmarks()
        {
            const int count = 500;
            Value = new List<double>(count);
            var random = new Random();

            for (int i = 0; i < count; i++)
                Value.Add(random.NextDouble());
        }

        #region IcyRain

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Serialize")]
        public void IcyRain_Ser() => Benchmark.IcyRain.Serialize(Value);

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Deep clone")]
        public void IcyRain_DeepClone() => Benchmark.IcyRain.DeepClone(Value);


        [Benchmark(Description = "IcyRain+LZ4"), BenchmarkCategory("Serialize")]
        public void IcyRainLZ4_Ser() => Benchmark.IcyRain.SerializeLZ4(Value);

        [Benchmark(Description = "IcyRain+LZ4"), BenchmarkCategory("Deep clone")]
        public void IcyRainLZ4_DeepClone() => Benchmark.IcyRain.DeepCloneLZ4(Value);


        [Benchmark(Description = "IcyRain+LZ4+UTC"), BenchmarkCategory("Deep clone")]
        public void IcyRainLZ4UTC_DeepClone() => Benchmark.IcyRain.DeepCloneLZ4UTC(Value);

        #endregion
        #region ZeroFormatter

        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Serialize")]
        public void ZeroFormatter_Ser() => Benchmark.ZeroFormatter.Serialize(Value);

        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Deep clone")]
        public void ZeroFormatter_DeepClone() => Benchmark.ZeroFormatter.DeepClone(Value);


        [Benchmark(Description = "ZeroFormatter+LZ4"), BenchmarkCategory("Serialize")]
        public void ZeroFormatterLZ4_Ser() => Benchmark.ZeroFormatter.SerializeLZ4(Value);

        [Benchmark(Description = "ZeroFormatter+LZ4"), BenchmarkCategory("Deep clone")]
        public void ZeroFormatterLZ4_DeepClone() => Benchmark.ZeroFormatter.DeepCloneLZ4(Value);

        #endregion
        #region MessagePack

        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Serialize")]
        public void MessagePack_Ser() => Benchmark.MessagePack.Serialize(Value);

        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Deep clone")]
        public void MessagePack_DeepClone() => Benchmark.MessagePack.DeepClone(Value);

        #endregion
        #region protobuf-net

        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Serialize")]
        public void ProtoBufNet_Ser() => Benchmark.ProtobufNet.Serialize(Value);

        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Deep clone")]
        public void ProtoBufNet_DeepClone() => Benchmark.ProtobufNet.DeepClone(Value);

        #endregion
    }
}