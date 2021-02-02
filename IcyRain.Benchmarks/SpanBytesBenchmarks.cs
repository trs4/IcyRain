using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace IcyRain.Benchmarks
{
    [MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class SpanBytesBenchmarks
    {
        private static readonly byte[] ByteArray = new byte[] { 1, 0, 0, 1, 0, };

        [Benchmark]
        public unsafe byte[] CopyBlock()
        {
            byte[] result = new byte[ByteArray.Length];

            fixed (byte* ptr = result)
            fixed (byte* ptrValue = ByteArray)
                Unsafe.CopyBlock(ptr, ptrValue, (uint)result.Length);

            return result;
        }

        [Benchmark]
        public unsafe byte[] Copy()
        {
            byte[] result = new byte[ByteArray.Length];
            ByteArray.AsSpan().TryCopyTo(result);
            return result;
        }

        [Benchmark]
        public byte[] BlockCopy()
        {
            byte[] result = new byte[ByteArray.Length];
            Buffer.BlockCopy(ByteArray, 0, result, 0, ByteArray.Length);
            return result;
        }

    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
