using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using IcyRain.Data.Objects;

namespace IcyRain.Benchmarks
{
    [MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class UnionHashBenchmarks : IIcyRainBenchmark //, IZeroFormatterBenchmark, IMessagePackBenchmark, IProtoBufNetBenchmark
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


        #region IcyRain

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Serialize")]
        public void IcyRain_Ser() => Benchmark.IcyRain.Serialize(Value);

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Deep clone")]
        public void IcyRain_DeepClone() => CleanupLazy(Benchmark.IcyRain.DeepClone(Value));


        [Benchmark(Description = "IcyRain+LZ4"), BenchmarkCategory("Serialize")]
        public void IcyRainLZ4_Ser() => Benchmark.IcyRain.SerializeLZ4(Value);

        [Benchmark(Description = "IcyRain+LZ4"), BenchmarkCategory("Deep clone")]
        public void IcyRainLZ4_DeepClone() => CleanupLazy(Benchmark.IcyRain.DeepCloneLZ4(Value));


        [Benchmark(Description = "IcyRain+LZ4+UTC"), BenchmarkCategory("Deep clone")]
        public void IcyRainLZ4UTC_DeepClone() => CleanupLazy(Benchmark.IcyRain.DeepCloneLZ4UTC(Value));

        #endregion
        //#region ZeroFormatter

        //[Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Serialize")]
        //public void ZeroFormatter_Ser() => Benchmark.ZeroFormatter.Serialize(Value);

        //[Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Deep clone")]
        //public void ZeroFormatter_DeepClone() => CleanupLazy(Benchmark.ZeroFormatter.DeepClone(Value));


        //[Benchmark(Description = "ZeroFormatter+LZ4"), BenchmarkCategory("Serialize")]
        //public void ZeroFormatterLZ4_Ser() => Benchmark.ZeroFormatter.SerializeLZ4(Value);

        //[Benchmark(Description = "ZeroFormatter+LZ4"), BenchmarkCategory("Deep clone")]
        //public void ZeroFormatterLZ4_DeepClone() => CleanupLazy(Benchmark.ZeroFormatter.DeepCloneLZ4(Value));

        //#endregion
        //#region MessagePack

        //[Benchmark(Description = "MessagePack"), BenchmarkCategory("Serialize")]
        //public void MessagePack_Ser() => Benchmark.MessagePack.Serialize(Value);

        //[Benchmark(Description = "MessagePack"), BenchmarkCategory("Deep clone")]
        //public void MessagePack_DeepClone() => CleanupLazy(Benchmark.MessagePack.DeepClone(Value));

        //#endregion
        //#region protobuf-net

        //[Benchmark(Description = "protobuf-net"), BenchmarkCategory("Serialize")]
        //public void ProtoBufNet_Ser() => Benchmark.ProtobufNet.Serialize(Value);

        //[Benchmark(Description = "protobuf-net"), BenchmarkCategory("Deep clone")]
        //public void ProtoBufNet_DeepClone() => CleanupLazy(Benchmark.ProtobufNet.DeepClone(Value));

        //#endregion
    }
}