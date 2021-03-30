using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
                int encodedLength = Encode(sourcePtr + 1, size, targetPtr, size);

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
                    decodedLength = Decode(sourcePtr + dataOffset, span.Length - dataOffset, targetPtr, target.Length);

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
            int encodedLength = Encode(sourcePtr, size, destinationPtr + headerSize, size - headerSize);

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

        /// <summary>Version of LZ4 implementation.</summary>
        public const int Version = 192;

        /// <summary>
        /// Enforces 32-bit compression/decompression algorithm even on 64-bit systems.
        /// Please note, this property should not be used on regular basis, it just allows
        /// to workaround some problems on platforms which do not support 64-bit the same was
        /// as Intel (for example: unaligned read/writes).
        /// </summary>
        public static bool Enforce32
        {
            get => LL.Enforce32;
            set => LL.Enforce32 = value;
        }

        /// <summary>Maximum size after compression.</summary>
        /// <param name="length">Length of input buffer.</param>
        /// <returns>Maximum length after compression.</returns>
        public static int MaximumOutputSize(int length) =>
            LL.LZ4_compressBound(length);

        /// <summary>Compresses data from one buffer into another.</summary>
        /// <param name="source">Input buffer.</param>
        /// <param name="sourceLength">Length of input buffer.</param>
        /// <param name="target">Output buffer.</param>
        /// <param name="targetLength">Output buffer length.</param>
        /// <param name="level">Compression level.</param>
        /// <returns>Number of bytes written, or negative value if output buffer is too small.</returns>
        public static unsafe int Encode(
            byte* source, int sourceLength,
            byte* target, int targetLength,
            LZ4Level level = LZ4Level.L00_FAST)
        {
            if (sourceLength <= 0)
                return 0;

            var encoded =
                level < LZ4Level.L03_HC
                    ? LLxx.LZ4_compress_fast(source, target, sourceLength, targetLength, 1)
                    : LLxx.LZ4_compress_HC(source, target, sourceLength, targetLength, (int)level);
            return encoded <= 0 ? -1 : encoded;
        }

        /// <summary>Compresses data from one buffer into another.</summary>
        /// <param name="source">Input buffer.</param>
        /// <param name="target">Output buffer.</param>
        /// <param name="level">Compression level.</param>
        /// <returns>Number of bytes written, or negative value if output buffer is too small.</returns>
        public static unsafe int Encode(
            ReadOnlySpan<byte> source, Span<byte> target,
            LZ4Level level = LZ4Level.L00_FAST)
        {
            var sourceLength = source.Length;
            if (sourceLength <= 0)
                return 0;

            var targetLength = target.Length;
            fixed (byte* sourceP = &MemoryMarshal.GetReference(source))
            fixed (byte* targetP = &MemoryMarshal.GetReference(target))
                return Encode(sourceP, sourceLength, targetP, targetLength, level);
        }

        /// <summary>Compresses data from one buffer into another.</summary>
        /// <param name="source">Input buffer.</param>
        /// <param name="sourceOffset">Input buffer offset.</param>
        /// <param name="sourceLength">Input buffer length.</param>
        /// <param name="target">Output buffer.</param>
        /// <param name="targetOffset">Output buffer offset.</param>
        /// <param name="targetLength">Output buffer length.</param>
        /// <param name="level">Compression level.</param>
        /// <returns>Number of bytes written, or negative value if output buffer is too small.</returns>
        public static unsafe int Encode(
            byte[] source, int sourceOffset, int sourceLength,
            byte[] target, int targetOffset, int targetLength,
            LZ4Level level = LZ4Level.L00_FAST)
        {
            fixed (byte* sourceP = source)
            fixed (byte* targetP = target)
                return Encode(
                    sourceP + sourceOffset, sourceLength,
                    targetP + targetOffset, targetLength,
                    level);
        }

        /// <summary>Decompresses data from given buffer.</summary>
        /// <param name="source">Input buffer.</param>
        /// <param name="sourceLength">Input buffer length.</param>
        /// <param name="target">Output buffer.</param>
        /// <param name="targetLength">Output buffer length.</param>
        /// <returns>Number of bytes written, or negative value if output buffer is too small.</returns>
        public static unsafe int Decode(
            byte* source, int sourceLength,
            byte* target, int targetLength)
        {
            if (sourceLength <= 0)
                return 0;

            var decoded = LLxx.LZ4_decompress_safe(source, target, sourceLength, targetLength);
            return decoded <= 0 ? -1 : decoded;
        }

        /// <summary>Decompresses data from given buffer.</summary>
        /// <param name="source">Input buffer.</param>
        /// <param name="target">Output buffer.</param>
        /// <returns>Number of bytes written, or negative value if output buffer is too small.</returns>
        public static unsafe int Decode(
            ReadOnlySpan<byte> source, Span<byte> target)
        {
            var sourceLength = source.Length;
            if (sourceLength <= 0)
                return 0;

            var targetLength = target.Length;
            fixed (byte* sourceP = &MemoryMarshal.GetReference(source))
            fixed (byte* targetP = &MemoryMarshal.GetReference(target))
                return Decode(sourceP, sourceLength, targetP, targetLength);
        }

        /// <summary>Decompresses data from given buffer.</summary>
        /// <param name="source">Input buffer.</param>
        /// <param name="sourceOffset">Input buffer offset.</param>
        /// <param name="sourceLength">Input buffer length.</param>
        /// <param name="target">Output buffer.</param>
        /// <param name="targetOffset">Output buffer offset.</param>
        /// <param name="targetLength">Output buffer length.</param>
        /// <returns>Number of bytes written, or negative value if output buffer is too small.</returns>
        public static unsafe int Decode(
            byte[] source, int sourceOffset, int sourceLength,
            byte[] target, int targetOffset, int targetLength)
        {
            fixed (byte* sourceP = source)
            fixed (byte* targetP = target)
                return Decode(
                    sourceP + sourceOffset, sourceLength,
                    targetP + targetOffset, targetLength);
        }
    }
}
