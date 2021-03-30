using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Compression.LZ4
{
    internal static class LZ4ArrayCodec
    {
        #region Encode

        public static unsafe byte[] EncodeToArray(byte[] source)
        {
            fixed (byte* sourcePtr = source)
                return EncodeToArray(sourcePtr, source.Length);
        }

        public static unsafe byte[] EncodeToArray(Span<byte> source)
        {
            fixed (byte* sourcePtr = source)
                return EncodeToArray(sourcePtr, source.Length);
        }

        public static unsafe byte[] EncodeToArray(ReadOnlySpan<byte> source)
        {
            fixed (byte* sourcePtr = source)
                return EncodeToArray(sourcePtr, source.Length);
        }

        [MethodImpl(Flags.HotPath)]
        private static unsafe byte[] EncodeToArray(byte* sourcePtr, int size)
        {
            byte[] result;

            if (size > Buffers.MinCompressSize)
            {
                int headerSize = sizeof(byte) + (size switch { > 0xffff or < 0 => 4, > 0xff => 2, _ => 1 });
                byte[] target = Buffers.Rent(size + 1);

                fixed (byte* targetPtr = target)
                {
                    int encodedLength = LZ4Codec.Encode(sourcePtr, size, targetPtr, size);

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


        public static unsafe ArraySegment<byte> EncodeToSegment(byte[] source)
        {
            fixed (byte* sourcePtr = source)
                return EncodeToSegment(sourcePtr, source.Length);
        }

        public static unsafe ArraySegment<byte> EncodeToSegment(Span<byte> source)
        {
            fixed (byte* sourcePtr = source)
                return EncodeToSegment(sourcePtr, source.Length);
        }

        public static unsafe ArraySegment<byte> EncodeToSegment(ReadOnlySpan<byte> source)
        {
            fixed (byte* sourcePtr = source)
                return EncodeToSegment(sourcePtr, source.Length);
        }

        [MethodImpl(Flags.HotPath)]
        private static unsafe ArraySegment<byte> EncodeToSegment(byte* sourcePtr, int size)
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

        #endregion
        #region Decode

        public static unsafe byte[] DecodeToArray(byte[] source)
        {
            if ((source[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (source[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = source.Length - dataOffset;
            byte[] result;

            fixed (byte* sourcePtr = source)
            {
                int resultDiff = sizeOfDiff == 0 ? 0 : LZ4Codec.PeekN(sourcePtr + 1, sizeOfDiff);

                if (resultDiff == 0)
                {
                    result = new byte[dataLength];

                    fixed (byte* ptrResult = result)
                        Unsafe.CopyBlock(ptrResult, sourcePtr + dataOffset, (uint)dataLength);

                    Buffers.Return(source);
                    return result;
                }

                int resultLength = dataLength + resultDiff;
                byte[] target = Buffers.Rent(resultLength);

                fixed (byte* targetPtr = target)
                {
                    int decodedLength = LZ4Codec.Decode(sourcePtr + dataOffset, source.Length - dataOffset, targetPtr, target.Length);

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

        public static unsafe byte[] DecodeToArray(byte[] source, ref int decodedLength)
        {
            if ((source[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (source[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = source.Length - dataOffset;
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
                    decodedLength = LZ4Codec.Decode(sourcePtr + dataOffset, source.Length - dataOffset, targetPtr, target.Length);

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

        public static unsafe byte[] DecodeToArray(ArraySegment<byte> source)
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
                    int decodedLength = LZ4Codec.Decode(sourcePtr + dataOffset + source.Offset, source.Count - dataOffset, targetPtr, target.Length);

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

        public static unsafe byte[] DecodeToRentArray(ArraySegment<byte> source, ref int decodedLength)
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
                    decodedLength = LZ4Codec.Decode(sourcePtr + dataOffset + source.Offset, source.Count - dataOffset, targetPtr, target.Length);

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

        public static unsafe byte[] DecodeToRentArray(Span<byte> source, ref int decodedLength)
        {
            if ((source[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (source[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = source.Length - dataOffset;
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
                    decodedLength = LZ4Codec.Decode(sourcePtr + dataOffset, source.Length - dataOffset, targetPtr, target.Length);

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

        public static unsafe byte[] DecodeToRentArray(ReadOnlySpan<byte> source, ref int decodedLength)
        {
            if ((source[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (source[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = source.Length - dataOffset;
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
                    decodedLength = LZ4Codec.Decode(sourcePtr + dataOffset, source.Length - dataOffset, targetPtr, target.Length);

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


        public static unsafe ArraySegment<byte> DecodeToSegment(byte[] source)
        {
            if ((source[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (source[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = source.Length - dataOffset;

            fixed (byte* sourcePtr = source)
            {
                int resultDiff = sizeOfDiff == 0 ? 0 : LZ4Codec.PeekN(sourcePtr + 1, sizeOfDiff);

                if (resultDiff == 0)
                    return new ArraySegment<byte>(source, dataOffset, dataLength);

                int resultLength = dataLength + resultDiff;
                byte[] target = Buffers.Rent(resultLength);

                fixed (byte* targetPtr = target)
                {
                    int decodedLength = LZ4Codec.Decode(sourcePtr + dataOffset, source.Length - dataOffset, targetPtr, target.Length);

                    if (decodedLength != resultLength)
                        LZ4Codec.ThrowExpectedException(decodedLength, resultLength);

                    Buffers.Return(source);
                    return new ArraySegment<byte>(target, 0, decodedLength);
                }
            }
        }

        public static unsafe ArraySegment<byte> DecodeToSegment(byte[] source, ref int decodedLength)
        {
            if ((source[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (source[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = source.Length - dataOffset;

            fixed (byte* sourcePtr = source)
            {
                int resultDiff = sizeOfDiff == 0 ? 0 : LZ4Codec.PeekN(sourcePtr + 1, sizeOfDiff);

                if (resultDiff == 0)
                    return new ArraySegment<byte>(source, dataOffset, dataLength);

                int resultLength = dataLength + resultDiff;
                byte[] target = Buffers.Rent(resultLength);

                fixed (byte* targetPtr = target)
                {
                    decodedLength = LZ4Codec.Decode(sourcePtr + dataOffset, source.Length - dataOffset, targetPtr, target.Length);

                    if (decodedLength != resultLength)
                        LZ4Codec.ThrowExpectedException(decodedLength, resultLength);

                    Buffers.Return(source);
                    return new ArraySegment<byte>(target, 0, decodedLength);
                }
            }
        }


        public static unsafe ReadOnlySequence<byte> DecodeToSequence(byte[] source)
        {
            if ((source[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (source[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = source.Length - dataOffset;

            fixed (byte* sourcePtr = source)
            {
                int resultDiff = sizeOfDiff == 0 ? 0 : LZ4Codec.PeekN(sourcePtr + 1, sizeOfDiff);

                if (resultDiff == 0)
                    return new ReadOnlySequence<byte>(source, dataOffset, dataLength);

                int resultLength = dataLength + resultDiff;
                byte[] target = Buffers.Rent(resultLength);

                fixed (byte* targetPtr = target)
                {
                    int decodedLength = LZ4Codec.Decode(sourcePtr + dataOffset, source.Length - dataOffset, targetPtr, target.Length);

                    if (decodedLength != resultLength)
                        LZ4Codec.ThrowExpectedException(decodedLength, resultLength);

                    Buffers.Return(source);
                    return new ReadOnlySequence<byte>(target, 0, decodedLength);
                }
            }
        }

        public static unsafe ReadOnlySequence<byte> DecodeToSequence(byte[] source, ref int decodedLength)
        {
            if ((source[0] & 0x07) != 0)
                throw new InvalidOperationException("Header is corrupted");

            int sizeOfDiff = (source[0] >> 6) & 0x3;

            if (sizeOfDiff == 3)
                sizeOfDiff = 4;

            ushort dataOffset = (ushort)(1 + sizeOfDiff);
            int dataLength = source.Length - dataOffset;

            fixed (byte* sourcePtr = source)
            {
                int resultDiff = sizeOfDiff == 0 ? 0 : LZ4Codec.PeekN(sourcePtr + 1, sizeOfDiff);

                if (resultDiff == 0)
                    return new ReadOnlySequence<byte>(source, dataOffset, dataLength);

                int resultLength = dataLength + resultDiff;
                byte[] target = Buffers.Rent(resultLength);

                fixed (byte* targetPtr = target)
                {
                    decodedLength = LZ4Codec.Decode(sourcePtr + dataOffset, source.Length - dataOffset, targetPtr, target.Length);

                    if (decodedLength != resultLength)
                        LZ4Codec.ThrowExpectedException(decodedLength, resultLength);

                    Buffers.Return(source);
                    return new ReadOnlySequence<byte>(target, 0, decodedLength);
                }
            }
        }

        #endregion
    }
}
