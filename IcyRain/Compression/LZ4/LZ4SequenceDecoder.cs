using System;
using System.Buffers;
using IcyRain.Compression.LZ4.Engine;
using IcyRain.Internal;

namespace IcyRain.Compression.LZ4
{
    internal static class LZ4SequenceDecoder
    {
        public static unsafe ReadOnlySequence<byte> Decode(byte[] source, ref int decodedLength)
        {
            if ((source[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (source[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = decodedLength - dataOffset;

            fixed (byte* sourcePtr = source)
            {
                int resultDiff = sizeOfDiff == 0 ? 0 : LZ4Codec.PeekN(sourcePtr + 1, sizeOfDiff);

                if (resultDiff == 0)
                    return new ReadOnlySequence<byte>(source, dataOffset, dataLength);

                int resultLength = dataLength + resultDiff;
                byte[] target = Buffers.Rent(resultLength);

                fixed (byte* targetPtr = target)
                {
                    decodedLength = LLxx.LZ4_decompress_safe(sourcePtr + dataOffset, targetPtr, dataLength, target.Length);

                    if (decodedLength != resultLength)
                        LZ4Codec.ThrowExpectedException(decodedLength, resultLength);

                    Buffers.Return(source);
                    return new ReadOnlySequence<byte>(target, 0, decodedLength);
                }
            }
        }

    }
}
