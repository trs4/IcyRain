using System;
using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using IcyRain.Internal;
using ZeroFormatter;

namespace IcyRain.Benchmarks
{
    [MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class SequenceBenchmarks
    {
        private readonly ReadOnlySequence<byte> Value;

        public SequenceBenchmarks()
        {
            var bytes = new byte[8192];
            new Random().NextBytes(bytes);
            Value = new ReadOnlySequence<byte>(bytes);
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
        public void ZeroFormatter_Ser()
            => new TestBufferWriter().Write(ZeroFormatterSerializer.Serialize(Value.ToArray()));

        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Deep clone")]
        public void ZeroFormatter_DeepClone()
            => new TestBufferWriter().DeepCloneSequence(Value, v => ZeroFormatterSerializer.Serialize(v), b => ZeroFormatterSerializer.Deserialize<byte[]>(b));

        #endregion
        #region MessagePack

        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Serialize")]
        public void MessagePack_Ser() => Benchmark.MessagePack.Serialize(Value);

        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Deep clone")]
        public void MessagePack_DeepClone() => Benchmark.MessagePack.DeepClone(Value);

        #endregion
        #region protobuf-net

        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Serialize")]
        public void ProtoBufNet_Ser()
            => ProtoBuf.Serializer.Serialize(new TestBufferWriter(), Value.ToArray());

        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Deep clone")]
        public void ProtoBufNet_DeepClone()
            => new TestBufferWriter().DeepCloneSequence(Value, (b, v) => ProtoBuf.Serializer.Serialize(b, v), s => ProtoBuf.Serializer.Deserialize<byte[]>(s));

        #endregion
    }
}