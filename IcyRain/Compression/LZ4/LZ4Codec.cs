using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4.Engine;
using IcyRain.Internal;

namespace IcyRain.Compression.LZ4
{
    /// <summary>Static class exposing LZ4 block compression methods</summary>
    internal static class LZ4Codec
    {
        #region Pikler

        public static unsafe void Encode(Span<byte> source, ref int spanOffset)
        {
            if (spanOffset == 1)
                return;

            int size = spanOffset - 1;
            int headerSize = sizeof(byte) + (size switch { > 0xffff or < 0 => 4, > 0xff => 2, _ => 1 });
            byte[] target = Buffers.Rent(size);

            fixed (byte* sourcePtr = source)
            fixed (byte* targetPtr = target)
            {
                int encodedLength = LLxx.LZ4_compress_fast(sourcePtr + 1, targetPtr, size, size, 1);

                if (encodedLength <= 0 || encodedLength >= size)
                {
                    source[0] = 0;
                    Buffers.Return(target);
                    return;
                }

                Unsafe.CopyBlock(sourcePtr + headerSize, targetPtr, (uint)encodedLength);

                int diffLength = size - encodedLength;
                int sizeOfDiff = headerSize - 1;
                source[0] = (byte)((0 & 0x07) | (((sizeOfDiff == 4 ? 3 : sizeOfDiff) & 0x3) << 6));
                Unsafe.CopyBlockUnaligned(ref source[1], ref *(byte*)&diffLength, (uint)sizeOfDiff);

                spanOffset = headerSize + encodedLength;
                Buffers.Return(target);
            }
        }

        public static unsafe (ReadOnlyMemory<byte>, byte[]) Decode(in ReadOnlyMemory<byte> source, ref int decodedLength)
        {
            var span = source.Span;

            if ((span[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (span[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = span.Length - dataOffset;

            fixed (byte* sourcePtr = span)
            {
                int resultDiff = sizeOfDiff == 0 ? 0 : PeekN(sourcePtr + 1, sizeOfDiff);

                if (resultDiff == 0)
                    return (source.Slice(dataOffset), null);

                int resultLength = dataLength + resultDiff;
                byte[] target = Buffers.Rent(resultLength);

                fixed (byte* targetPtr = target)
                    decodedLength = LLxx.LZ4_decompress_safe(sourcePtr + dataOffset, targetPtr, span.Length - dataOffset, target.Length);

                if (decodedLength != resultLength)
                    ThrowExpectedException(decodedLength, resultLength);

                return (new ReadOnlyMemory<byte>(target, 0, decodedLength), target);
            }
        }

        [MethodImpl(Flags.HotPath)]
        public static unsafe int PeekN(byte* sourcePtr, int size)
        {
            int result = default;
            Unsafe.CopyBlockUnaligned(&result, sourcePtr, (uint)size);
            return result;
        }

        public static void ThrowExpectedException(int decodedLength, int expectedLength)
            => throw new InvalidOperationException($"Expected to decode {decodedLength} bytes but {expectedLength} has been decoded");

        #endregion
        #region Write

        /// <remarks>destinationPtr.Length = size + 1</remarks>
        public static unsafe void Encode(byte* sourcePtr, byte* destinationPtr, ref int size)
        {
            int headerSize = sizeof(byte) + (size switch { > 0xffff or < 0 => 4, > 0xff => 2, _ => 1 });
            int encodedLength = LLxx.LZ4_compress_fast(sourcePtr, destinationPtr + headerSize, size, size - headerSize, 1);

            if (encodedLength <= 0 || encodedLength >= size)
            {
                *destinationPtr = 0;
                Unsafe.CopyBlock(destinationPtr + 1, sourcePtr, (uint)size);
                size++;
                return;
            }

            int diffLength = size - encodedLength;
            int sizeOfDiff = headerSize - 1;
            *destinationPtr = (byte)((0 & 0x07) | (((sizeOfDiff == 4 ? 3 : sizeOfDiff) & 0x3) << 6));
            Unsafe.CopyBlockUnaligned(destinationPtr + 1, (byte*)&diffLength, (uint)sizeOfDiff);

            size = headerSize + encodedLength;
        }

        #endregion
    }
}
