using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using IcyRain.Data.Objects;
using IcyRain.Internal;
using MessagePack;
using ZeroFormatter;

namespace IcyRain.Benchmarks
{
    [MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class UnionBenchmarks
    {
        private static readonly TestB1 Value = new TestB3
        {
            Property11 = true,
            Property31 = 7.5,
            Property32 = new DateTime(2010, 5, 1, 5, 8, 7),
        };

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CleanupLazy(TestB1 value)
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var r1 = value.Property11;
            var r2 = (value as TestB3)?.Property31;
            var r3 = (value as TestB3)?.Property32;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Serialize")]
        public void IcyRain_Ser()
            => Serialization.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Deep clone")]
        public void IcyRain_DeepClone()
        {
            var result = new TestBufferWriter().DeepClone(Value, v => Serialization.Serialize(v), b => Serialization.Deserialize<TestB1>(b));
            CleanupLazy(result);
        }


        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Serialize")]
        public void ZeroFormatter_Ser()
            => new TestBufferWriter().Write(ZeroFormatterSerializer.Serialize(Value));

        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Deep clone")]
        public void ZeroFormatter_DeepClone()
        {
            var result = new TestBufferWriter().DeepClone(Value, v => ZeroFormatterSerializer.Serialize(v), b => ZeroFormatterSerializer.Deserialize<TestB1>(b));
            CleanupLazy(result);
        }


        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Serialize")]
        public void MessagePack_Ser()
            => MessagePackSerializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Deep clone")]
        public void MessagePack_DeepClone()
        {
            var result = new TestBufferWriter().DeepClone(Value, (b, v) => MessagePackSerializer.Serialize(b, v), s => MessagePackSerializer.Deserialize<TestB1>(s));
            CleanupLazy(result);
        }


        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Serialize")]
        public void ProtoBufNet_Ser()
            => ProtoBuf.Serializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Deep clone")]
        public void ProtoBufNet_DeepClone()
        {
            var result = new TestBufferWriter().DeepClone(Value, (b, v) => ProtoBuf.Serializer.Serialize(b, v), s => ProtoBuf.Serializer.Deserialize<TestB1>(s));
            CleanupLazy(result);
        }

    }
}