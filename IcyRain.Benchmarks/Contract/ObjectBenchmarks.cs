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
    public class ObjectBenchmarks
    {
        private static readonly TestData Value = new TestData
        {
            Property1 = true,
            Property2 = 25,
            Property3 = 4.5,
            Property4 = new DateTime(2021, 5, 1, 5, 8, 7),
            Property5 = "test",
        };

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CleanupLazy(TestData value)
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var r1 = value.Property1;
            var r2 = value.Property2;
            var r3 = value.Property3;
            var r4 = value.Property4;
            var r5 = value.Property5;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Serialize")]
        public void IcyRain_Ser()
            => Serialization.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Deep clone")]
        public void IcyRain_DeepClone()
        {
            var result = new TestBufferWriter().DeepClone(Value, v => Serialization.Serialize(v), b => Serialization.Deserialize<TestData>(b));
            CleanupLazy(result);
        }


        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Serialize")]
        public void ZeroFormatter_Ser()
            => new TestBufferWriter().Write(ZeroFormatterSerializer.Serialize(Value));

        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Deep clone")]
        public void ZeroFormatter_DeepClone()
        {
            var result = new TestBufferWriter().DeepClone(Value, v => ZeroFormatterSerializer.Serialize(v), b => ZeroFormatterSerializer.Deserialize<TestData>(b));
            CleanupLazy(result);
        }


        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Serialize")]
        public void MessagePack_Ser()
            => MessagePackSerializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Deep clone")]
        public void MessagePack_DeepClone()
        {
            var result = new TestBufferWriter().DeepClone(Value, (b, v) => MessagePackSerializer.Serialize(b, v), s => MessagePackSerializer.Deserialize<TestData>(s));
            CleanupLazy(result);
        }


        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Serialize")]
        public void ProtoBufNet_Ser()
            => ProtoBuf.Serializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Deep clone")]
        public void ProtoBufNet_DeepClone()
        {
            var result = new TestBufferWriter().DeepClone(Value, (b, v) => ProtoBuf.Serializer.Serialize(b, v), s => ProtoBuf.Serializer.Deserialize<TestData>(s));
            CleanupLazy(result);
        }

    }
}