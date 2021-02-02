using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace IcyRain.Benchmarks
{
    [MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class SpanArrayBenchmarks
    {
        private static readonly bool[] Array = new bool[] { true, false, false, true, false, };
        private static readonly byte[] ByteArray = new byte[] { 1, 0, 0, 1, 0, };

        [Benchmark]
        public byte[] BlockCopy()
        {
            byte[] result = new byte[Array.Length];
            Buffer.BlockCopy(Array, 0, result, 0, Array.Length);
            return result;
        }

        [Benchmark]
        public Span<byte> MemoryMarshalCast()
            => MemoryMarshal.Cast<bool, byte>(Array.AsSpan());

        [Benchmark]
        public Span<byte> MemoryMarshalCastCopy()
        {
            byte[] result = new byte[Array.Length];
            var cast = MemoryMarshal.Cast<bool, byte>(Array.AsSpan());
            cast.TryCopyTo(result);
            return result;
        }

        [Benchmark]
        public Span<byte> MemoryMarshalAsBytes()
            => MemoryMarshal.AsBytes(Array.AsSpan());

        [Benchmark]
        public Span<byte> MemoryMarshalAsBytesCopy()
        {
            byte[] result = new byte[Array.Length];
            var cast = MemoryMarshal.AsBytes(Array.AsSpan());
            cast.TryCopyTo(result);
            return result;
        }

        [Benchmark]
        public bool[] MemoryMarshalCastDes()
        {
            bool[] result = new bool[ByteArray.Length];
            MemoryMarshal.Cast<byte, bool>(ByteArray.AsSpan()).TryCopyTo(result);
            return result;
        }

        [Benchmark]
        public unsafe byte[] CopyBlock()
        {
            byte[] result = new byte[Array.Length];

            fixed (byte* ptr = result)
            fixed (bool* ptr2 = Array)
                Unsafe.CopyBlock(ptr, ptr2, (uint)Array.Length);

            return result;
        }

        [Benchmark]
        public unsafe bool[] CopyBlockDes()
        {
            bool[] result = new bool[Array.Length];

            fixed (bool* ptr = result)
            fixed (byte* ptr2 = ByteArray)
                Unsafe.CopyBlock(ptr, ptr2, (uint)ByteArray.Length);

            return result;
        }

    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
