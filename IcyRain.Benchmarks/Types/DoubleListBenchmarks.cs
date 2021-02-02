using System;
using System.Collections.Generic;
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

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Serialize")]
        public void IcyRain_Ser()
            => Serialization.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Deep clone")]
        public void IcyRain_DeepClone()
            => new TestBufferWriter().DeepClone(Value, v => Serialization.Serialize(v), b => Serialization.Deserialize<List<double>>(b));


        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Serialize")]
        public void ZeroFormatter_Ser()
            => new TestBufferWriter().Write(ZeroFormatterSerializer.Serialize(Value));

        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Deep clone")]
        public void ZeroFormatter_DeepClone()
            => new TestBufferWriter().DeepClone(Value, v => ZeroFormatterSerializer.Serialize(v), b => ZeroFormatterSerializer.Deserialize<List<double>>(b));


        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Serialize")]
        public void MessagePack_Ser()
            => MessagePackSerializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Deep clone")]
        public void MessagePack_DeepClone()
            => new TestBufferWriter().DeepClone(Value, (b, v) => MessagePackSerializer.Serialize(b, v), s => MessagePackSerializer.Deserialize<List<double>>(s));


        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Serialize")]
        public void ProtoBufNet_Ser()
            => ProtoBuf.Serializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Deep clone")]
        public void ProtoBufNet_DeepClone()
            => new TestBufferWriter().DeepClone(Value, (b, v) => ProtoBuf.Serializer.Serialize(b, v), s => ProtoBuf.Serializer.Deserialize<List<double>>(s));
    }
}