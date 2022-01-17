using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4.Engine;
using IcyRain.Internal;

namespace IcyRain.Compression.LZ4
{
    internal static class LZ4ArrayDecoder
    {
        public static unsafe byte[] Decode(byte[] source, ref int decodedLength)
        {
            if ((source[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (source[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = decodedLength - dataOffset;
            byte[] result;

            fixed (byte* sourcePtr = source)
            {
                int resultDiff = sizeOfDiff == 0 ? 0 : LZ4Codec.PeekN(sourcePtr + 1, sizeOfDiff);

                if (resultDiff == 0)
                {
                    result = new byte[dataLength];

                    fixed (byte* ptrResult = result)
                        Unsafe.CopyBlock(ptrResult, sourcePtr + dataOffset, (uint)dataLength);

                    decodedLength = dataLength;
                    Buffers.Return(source);
                    return result;
                }

                int resultLength = dataLength + resultDiff;
                byte[] target = Buffers.Rent(resultLength);

                fixed (byte* targetPtr = target)
                {
                    decodedLength = LLxx.LZ4_decompress_safe(sourcePtr + dataOffset, targetPtr, dataLength, target.Length);

                    if (decodedLength != resultLength)
                        LZ4Codec.ThrowExpectedException(decodedLength, resultLength);

                    result = new byte[decodedLength];

                    fixed (byte* ptrResult = result)
                        Unsafe.CopyBlock(ptrResult, targetPtr, (uint)decodedLength);

                    Buffers.Return(source);
                    return result;
                }
            }
        }

        public static unsafe byte[] Decode(ArraySegment<byte> source)
        {
            byte firstByte = source.Array[source.Offset];

            if ((firstByte & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (firstByte >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = source.Count - dataOffset;
            byte[] result;

            fixed (byte* sourcePtr = source.Array)
            {
                int resultDiff = sizeOfDiff == 0 ? 0 : LZ4Codec.PeekN(sourcePtr + 1 + source.Offset, sizeOfDiff);

                if (resultDiff == 0)
                {
                    result = new byte[dataLength];

                    fixed (byte* ptrResult = result)
                        Unsafe.CopyBlock(ptrResult, sourcePtr + dataOffset + source.Offset, (uint)dataLength);

                    Buffers.Return(source.Array);
                    return result;
                }

                int resultLength = dataLength + resultDiff;
                byte[] target = Buffers.Rent(resultLength);

                fixed (byte* targetPtr = target)
                {
                    int decodedLength = LLxx.LZ4_decompress_safe(sourcePtr + dataOffset + source.Offset, targetPtr, dataLength, target.Length);

                    if (decodedLength != resultLength)
                        LZ4Codec.ThrowExpectedException(decodedLength, resultLength);

                    result = new byte[decodedLength];

                    fixed (byte* ptrResult = result)
                        Unsafe.CopyBlock(ptrResult, targetPtr, (uint)decodedLength);

                    Buffers.Return(source.Array);
                    return result;
                }
            }
        }

        public static unsafe byte[] RentDecode(ArraySegment<byte> source, ref int decodedLength)
        {
            byte firstByte = source.Array[source.Offset];

            if ((firstByte & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (firstByte >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = decodedLength - dataOffset;
            byte[] result;

            fixed (byte* sourcePtr = source.Array)
            {
                int resultDiff = sizeOfDiff == 0 ? 0 : LZ4Codec.PeekN(sourcePtr + 1 + source.Offset, sizeOfDiff);

                if (resultDiff == 0)
                {
                    result = Buffers.Rent(dataLength);

                    fixed (byte* ptrResult = result)
                        Unsafe.CopyBlock(ptrResult, sourcePtr + dataOffset + source.Offset, (uint)dataLength);

                    decodedLength = dataLength;
                    Buffers.Return(source.Array);
                    return result;
                }

                int resultLength = dataLength + resultDiff;
                byte[] target = Buffers.Rent(resultLength);

                fixed (byte* targetPtr = target)
                {
                    decodedLength = LLxx.LZ4_decompress_safe(sourcePtr + dataOffset + source.Offset, targetPtr, dataLength, target.Length);

                    if (decodedLength != resultLength)
                        LZ4Codec.ThrowExpectedException(decodedLength, resultLength);

                    result = Buffers.Rent(decodedLength);

                    fixed (byte* ptrResult = result)
                        Unsafe.CopyBlock(ptrResult, targetPtr, (uint)decodedLength);

                    Buffers.Return(source.Array);
                    return result;
                }
            }
        }

        public static unsafe byte[] RentDecode(Span<byte> source, ref int decodedLength)
        {
            if ((source[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (source[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = decodedLength - dataOffset;
            byte[] result;

            fixed (byte* sourcePtr = source)
            {
                int resultDiff = sizeOfDiff == 0 ? 0 : LZ4Codec.PeekN(sourcePtr + 1, sizeOfDiff);

                if (resultDiff == 0)
                {
                    result = Buffers.Rent(dataLength);

                    fixed (byte* ptrResult = result)
                        Unsafe.CopyBlock(ptrResult, sourcePtr + dataOffset, (uint)dataLength);

                    decodedLength = dataLength;
                    return result;
                }

                int resultLength = dataLength + resultDiff;
                byte[] target = Buffers.Rent(resultLength);

                fixed (byte* targetPtr = target)
                {
                    decodedLength = LLxx.LZ4_decompress_safe(sourcePtr + dataOffset, targetPtr, dataLength, target.Length);

                    if (decodedLength != resultLength)
                        LZ4Codec.ThrowExpectedException(decodedLength, resultLength);

                    // %%TODO Optimize
                    result = Buffers.Rent(decodedLength);

                    fixed (byte* ptrResult = result)
                        Unsafe.CopyBlock(ptrResult, targetPtr, (uint)decodedLength);

                    return result;
                }
            }
        }

        public static unsafe byte[] RentDecode(ReadOnlySpan<byte> source, ref int decodedLength)
        {
            if ((source[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (source[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = decodedLength - dataOffset;
            byte[] result;

            fixed (byte* sourcePtr = source)
            {
                int resultDiff = sizeOfDiff == 0 ? 0 : LZ4Codec.PeekN(sourcePtr + 1, sizeOfDiff);

                if (resultDiff == 0)
                {
                    result = Buffers.Rent(dataLength);

                    fixed (byte* ptrResult = result)
                        Unsafe.CopyBlock(ptrResult, sourcePtr + dataOffset, (uint)dataLength);

                    decodedLength = dataLength;
                    return result;
                }

                int resultLength = dataLength + resultDiff;
                byte[] target = Buffers.Rent(resultLength);

                fixed (byte* targetPtr = target)
                {
                    decodedLength = LLxx.LZ4_decompress_safe(sourcePtr + dataOffset, targetPtr, dataLength, target.Length);

                    if (decodedLength != resultLength)
                        LZ4Codec.ThrowExpectedException(decodedLength, resultLength);

                    // %%TODO Optimize
                    result = Buffers.Rent(decodedLength);

                    fixed (byte* ptrResult = result)
                        Unsafe.CopyBlock(ptrResult, targetPtr, (uint)decodedLength);

                    return result;
                }
            }
        }

    }
}
