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
    public class UnionHashBenchmarks
    {
        private static readonly TestA1 Value = new TestA25
        {
            Property11 = true,
            Property31 = 7.5,
            Property32 = new DateTime(2010, 5, 1, 5, 8, 7),
            Property34 = "new IntPtr(8)",
            Property35 = new TestB3
            {
                Property11 = true,
                Property31 = 10.5,
                Property32 = new DateTime(2000, 5, 1, 5, 8, 7),
            },
        };

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void CleanupLazy(TestA1 value)
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var r1 = value.Property11;
            var r2 = (value as TestA25)?.Property31;
            var r3 = (value as TestA25)?.Property32;
            var r4 = (value as TestA25)?.Property33;
            var r5 = (value as TestA25)?.Property34;
            var r6 = (value as TestA25)?.Property35;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Serialize")]
        public void IcyRain_Ser()
            => Serialization.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Deep clone")]
        public void IcyRain_DeepClone()
        {
            var result = new TestBufferWriter().DeepClone(Value, v => Serialization.Serialize(v), b => Serialization.Deserialize<TestA1>(b));
            CleanupLazy(result);
        }


        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Serialize")]
        public void ZeroFormatter_Ser()
            => new TestBufferWriter().Write(ZeroFormatterSerializer.Serialize(Value));

        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Deep clone")]
        public void ZeroFormatter_DeepClone()
        {
            var result = new TestBufferWriter().DeepClone(Value, v => ZeroFormatterSerializer.Serialize(v), b => ZeroFormatterSerializer.Deserialize<TestA1>(b));
            CleanupLazy(result);
        }


        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Serialize")]
        public void MessagePack_Ser()
            => MessagePackSerializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Deep clone")]
        public void MessagePack_DeepClone()
        {
            var result = new TestBufferWriter().DeepClone(Value, (b, v) => MessagePackSerializer.Serialize(b, v), s => MessagePackSerializer.Deserialize<TestA1>(s));
            CleanupLazy(result);
        }


        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Serialize")]
        public void ProtoBufNet_Ser()
            => ProtoBuf.Serializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Deep clone")]
        public void ProtoBufNet_DeepClone()
        {
            var result = new TestBufferWriter().DeepClone(Value, (b, v) => ProtoBuf.Serializer.Serialize(b, v), s => ProtoBuf.Serializer.Deserialize<TestA1>(s));
            CleanupLazy(result);
        }

    }
}