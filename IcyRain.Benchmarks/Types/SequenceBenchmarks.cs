using System;
using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using IcyRain.Internal;
using MessagePack;
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

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Serialize")]
        public void IcyRain_Ser()
            => Serialization.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Deep clone")]
        public void IcyRain_DeepClone()
            => new TestBufferWriter().DeepClone(Value, v => Serialization.Serialize(v), b => Serialization.Deserialize<ReadOnlySequence<byte>>(b));


        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Serialize")]
        public void ZeroFormatter_Ser()
            => new TestBufferWriter().Write(ZeroFormatterSerializer.Serialize(Value.ToArray()));

        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Deep clone")]
        public void ZeroFormatter_DeepClone()
            => new TestBufferWriter().DeepCloneSequence(Value, v => ZeroFormatterSerializer.Serialize(v), b => ZeroFormatterSerializer.Deserialize<byte[]>(b));


        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Serialize")]
        public void MessagePack_Ser()
            => MessagePackSerializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Deep clone")]
        public void MessagePack_DeepClone()
            => new TestBufferWriter().DeepClone(Value, (b, v) => MessagePackSerializer.Serialize(b, v), s => MessagePackSerializer.Deserialize<ReadOnlySequence<byte>>(s));


        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Serialize")]
        public void ProtoBufNet_Ser()
            => ProtoBuf.Serializer.Serialize(new TestBufferWriter(), Value.ToArray());

        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Deep clone")]
        public void ProtoBufNet_DeepClone()
            => new TestBufferWriter().DeepCloneSequence(Value, (b, v) => ProtoBuf.Serializer.Serialize(b, v), s => ProtoBuf.Serializer.Deserialize<byte[]>(s));
    }
}