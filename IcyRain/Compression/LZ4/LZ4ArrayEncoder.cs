using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4.Engine;
using IcyRain.Internal;

namespace IcyRain.Compression.LZ4
{
    internal static class LZ4ArrayEncoder
    {
        [MethodImpl(Flags.HotPath)]
        public static unsafe byte[] Encode(byte[] source)
        {
            fixed (byte* sourcePtr = source)
                return Encode(sourcePtr, source.Length);
        }

        [MethodImpl(Flags.HotPath)]
        public static unsafe byte[] Encode(Span<byte> source)
        {
            fixed (byte* sourcePtr = source)
                return Encode(sourcePtr, source.Length);
        }

        [MethodImpl(Flags.HotPath)]
        public static unsafe byte[] Encode(ReadOnlySpan<byte> source)
        {
            fixed (byte* sourcePtr = source)
                return Encode(sourcePtr, source.Length);
        }

        private static unsafe byte[] Encode(byte* sourcePtr, int size)
        {
            byte[] result;

            if (size > Buffers.MinCompressSize)
            {
                int headerSize = sizeof(byte) + (size switch { > 0xffff or < 0 => 4, > 0xff => 2, _ => 1 });
                byte[] target = Buffers.Rent(size + 1);

                fixed (byte* targetPtr = target)
                {
                    int encodedLength = LLxx.LZ4_compress_fast(sourcePtr, targetPtr, size, size, 1);

                    if (encodedLength > 0 && encodedLength < size)
                    {
                        result = new byte[headerSize + encodedLength];

                        fixed (byte* ptr = result)
                        {
                            Unsafe.CopyBlock(ptr + headerSize, targetPtr, (uint)encodedLength);

                            int diffLength = size - encodedLength;
                            int sizeOfDiff = headerSize - 1;
                            result[0] = (byte)((0 & 0x07) | (((sizeOfDiff == 4 ? 3 : sizeOfDiff) & 0x3) << 6));
                            Unsafe.CopyBlockUnaligned(ref result[1], ref *(byte*)&diffLength, (uint)sizeOfDiff);
                        }

                        Buffers.Return(target);
                        return result;
                    }
                }

                Buffers.Return(target);
            }

            result = new byte[size + 1];

            fixed (byte* ptr = result)
                Unsafe.CopyBlock(ptr + 1, sourcePtr, (uint)size);

            return result;
        }

    }
}
