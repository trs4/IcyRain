using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Compression.LZ4
{
    internal static class LZ4SegmentEncoder
    {
        public static unsafe ArraySegment<byte> Encode(byte[] source)
        {
            fixed (byte* sourcePtr = source)
                return Encode(sourcePtr, source.Length);
        }

        public static unsafe ArraySegment<byte> Encode(Span<byte> source)
        {
            fixed (byte* sourcePtr = source)
                return Encode(sourcePtr, source.Length);
        }

        public static unsafe ArraySegment<byte> Encode(ReadOnlySpan<byte> source)
        {
            fixed (byte* sourcePtr = source)
                return Encode(sourcePtr, source.Length);
        }

        [MethodImpl(Flags.HotPath)]
        private static unsafe ArraySegment<byte> Encode(byte* sourcePtr, int size)
        {
            byte[] buffer = Buffers.Rent(size + 1);

            fixed (byte* ptr = buffer)
            {
                if (size > Buffers.MinCompressSize)
                    LZ4Codec.Encode(sourcePtr, ptr, ref size);
                else
                    BlockBuilder.NoCompression(ptr, sourcePtr, ref size);
            }

            return new ArraySegment<byte>(buffer, 0, size);
        }

    }
}
