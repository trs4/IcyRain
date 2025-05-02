using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;

namespace IcyRain.Internal;

internal static class BufferExtensions
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static unsafe byte[] TransferToArray(this in ArraySegment<byte> segment)
    {
        byte[] bytes = new byte[segment.Count];

        fixed (byte* ptr = bytes)
        fixed (byte* ptrSegment = segment.Array)
            Unsafe.CopyBlock(ptr, ptrSegment + segment.Offset, (uint)segment.Count);

        return bytes;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static unsafe byte[] TransferToArray(this in ReadOnlySequence<byte> value)
    {
        long length = value.Length;

        if (length == 0)
            return [];

        byte[] result = new byte[length];

        if (value.IsSingleSegment)
        {
            value.First.Span.TryCopyTo(result);
            return result;
        }

        int offset = 0;
        var position = value.Start;

        fixed (byte* ptr = result)
        {
            while (value.TryGet(ref position, out var memory))
            {
                var span = memory.Span;

                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                offset += span.Length;

                if (position.GetObject() is null)
                    break;
            }
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static unsafe byte[] TransferToRentArray(this in ReadOnlySequence<byte> value)
    {
        int length = (int)value.Length;

        if (length == 0)
            return [];

        byte[] result = Buffers.Rent(length);

        if (value.IsSingleSegment)
        {
            value.First.Span.TryCopyTo(result);
            return result;
        }

        int offset = 0;
        var position = value.Start;

        fixed (byte* ptr = result)
        {
            while (value.TryGet(ref position, out var memory))
            {
                var span = memory.Span;

                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                offset += span.Length;

                if (position.GetObject() is null)
                    break;
            }
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static unsafe byte[] TransferToArrayWithLZ4Decompress(this in ReadOnlySequence<byte> value,
        int length, out int decodedLength)
    {
        decodedLength = length;

        if (length <= 1)
            return [];

        ReadOnlySpan<byte> span;
        int ptrValueOffset;
        byte[] result;

        if (value.IsSingleSegment)
        {
            span = value.First.Span;
            ptrValueOffset = span[0] == 0 ? 1 : 0;
            result = ptrValueOffset == 0 ? Buffers.Rent(length) : new byte[length - 1];
            value.First.Span.Slice(ptrValueOffset).TryCopyTo(result);
        }
        else
        {
            int offset = 0;
            var position = value.Start;

            value.TryGet(ref position, out var memory);
            span = memory.Span;
            ptrValueOffset = span[0] == 0 ? 1 : 0;
            result = ptrValueOffset == 0 ? Buffers.Rent(length) : new byte[length - 1];

            fixed (byte* ptr = result)
            {
                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr + offset, ptrValue + ptrValueOffset, (uint)(span.Length - ptrValueOffset));

                offset += span.Length - ptrValueOffset;

                if (position.GetObject() is not null)
                {
                    while (value.TryGet(ref position, out memory))
                    {
                        span = memory.Span;

                        fixed (byte* ptrValue = span)
                            Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                        offset += span.Length;

                        if (position.GetObject() is null)
                            break;
                    }
                }
            }
        }

        return ptrValueOffset == 1
            ? result
            : LZ4ArrayDecoder.Decode(result, ref decodedLength);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static unsafe ArraySegment<byte> TransferToSegmentWithLZ4Decompress(this in ReadOnlySequence<byte> value,
        int length, out int decodedLength)
    {
        decodedLength = length;

        if (length <= 1)
            return default;

        ReadOnlySpan<byte> span;
        bool uncompressed;
        byte[] buffer = Buffers.Rent(length);

        if (value.IsSingleSegment)
        {
            span = value.First.Span;
            uncompressed = span[0] == 0;
            value.First.Span.TryCopyTo(buffer);
        }
        else
        {
            int offset = 0;
            var position = value.Start;

            value.TryGet(ref position, out var memory);
            span = memory.Span;
            uncompressed = span[0] == 0;

            fixed (byte* ptr = buffer)
            {
                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                offset += span.Length;

                if (position.GetObject() is not null)
                {
                    while (value.TryGet(ref position, out memory))
                    {
                        span = memory.Span;

                        fixed (byte* ptrValue = span)
                            Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                        offset += span.Length;

                        if (position.GetObject() is null)
                            break;
                    }
                }
            }
        }

        return uncompressed
            ? new ArraySegment<byte>(buffer, 1, length - 1)
            : LZ4SegmentDecoder.Decode(buffer, ref decodedLength);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static unsafe ReadOnlySequence<byte> TransferToSequenceWithLZ4Decompress(this in ReadOnlySequence<byte> value,
        int length, out int decodedLength)
    {
        decodedLength = length;

        if (length <= 1)
            return default;

        ReadOnlySpan<byte> span;
        bool uncompressed;
        byte[] buffer = Buffers.Rent(length);

        if (value.IsSingleSegment)
        {
            span = value.First.Span;
            uncompressed = span[0] == 0;
            value.First.Span.TryCopyTo(buffer);
        }
        else
        {
            int offset = 0;
            var position = value.Start;

            value.TryGet(ref position, out var memory);
            span = memory.Span;
            uncompressed = span[0] == 0;

            fixed (byte* ptr = buffer)
            {
                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                offset += span.Length;

                if (position.GetObject() is not null)
                {
                    while (value.TryGet(ref position, out memory))
                    {
                        span = memory.Span;

                        fixed (byte* ptrValue = span)
                            Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                        offset += span.Length;

                        if (position.GetObject() is null)
                            break;
                    }
                }
            }
        }

        return uncompressed
            ? new ReadOnlySequence<byte>(buffer, 1, length - 1)
            : LZ4SequenceDecoder.Decode(buffer, ref decodedLength);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static unsafe byte[] TransferToArrayWithLZ4Decompress(this ArraySegment<byte> value, out int decodedLength)
    {
        decodedLength = value.Count;

        if (decodedLength <= 1)
        {
            decodedLength = 0;
            return [];
        }

        byte[] result;

        if (value.Array[value.Offset] == 0)
        {
            decodedLength--;
            result = new byte[decodedLength];
            result.WriteTo(value.Array, value.Offset + 1, decodedLength);
            return result;
        }

        result = LZ4ArrayDecoder.Decode(value);
        decodedLength = result.Length;
        return result;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static unsafe byte[] TransferToRentArrayWithLZ4Decompress(this ArraySegment<byte> value, out int decodedLength)
    {
        decodedLength = value.Count;

        if (decodedLength <= 1)
        {
            decodedLength = 0;
            return [];
        }

        if (value.Array[value.Offset] == 0)
        {
            decodedLength--;
            byte[] result = Buffers.Rent(decodedLength);
            result.WriteTo(value.Array, value.Offset + 1, decodedLength);
            return result;
        }

        return LZ4ArrayDecoder.RentDecode(value, ref decodedLength);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static unsafe void WriteWithLZ4Compress(this in ReadOnlySequence<byte> value, byte* destination,
        ref int encodedLength)
    {
        int length = encodedLength;

        if (value.IsSingleSegment)
        {
            fixed (byte* ptrValue = value.First.Span)
            {
                if (length > Buffers.MinCompressSize)
                    LZ4Codec.Encode(ptrValue, destination, ref encodedLength);
                else
                    BlockBuilder.NoCompression(destination, ptrValue, ref encodedLength);
            }
        }
        else
        {
            byte[] buffer = Buffers.Rent(length);
            WriteToBufferMultiple(value, buffer);

            fixed (byte* ptrValue = buffer)
            {
                if (length > Buffers.MinCompressSize)
                    LZ4Codec.Encode(ptrValue, destination, ref encodedLength);
                else
                    BlockBuilder.NoCompression(destination, ptrValue, ref encodedLength);
            }

            Buffers.Return(buffer);
        }
    }

    [MethodImpl(Flags.HotPath)]
    public static unsafe void WriteTo(this byte[] destination, byte[] buffer, int offset, int count)
    {
        fixed (byte* ptr = destination)
        fixed (byte* ptrValue = buffer)
            Unsafe.CopyBlock(ptr, ptrValue + offset, (uint)count);
    }

    [MethodImpl(Flags.HotPath)]
    public static unsafe void WriteToBuffer(this byte[] source, ref int sourceOffset, byte[] buffer, ref int offset, int count)
    {
        fixed (byte* ptr = buffer)
        fixed (byte* ptrValue = source)
            Unsafe.CopyBlock(ptr + offset, ptrValue + sourceOffset, (uint)count);

        sourceOffset += count;
        offset += count;
    }

    [MethodImpl(Flags.HotPath)]
    public static unsafe void WriteToBuffer(this byte[] source, ref int sourceOffset, Span<byte> buffer)
    {
        fixed (byte* ptr = buffer)
        fixed (byte* ptrValue = source)
            Unsafe.CopyBlock(ptr, ptrValue + sourceOffset, (uint)buffer.Length);

        sourceOffset += buffer.Length;
    }

    [MethodImpl(Flags.HotPath)]
    public static unsafe void WriteToBuffer(this byte[] source, ref int sourceOffset, Memory<byte> buffer)
    {
        fixed (byte* ptr = buffer.Span)
        fixed (byte* ptrValue = source)
            Unsafe.CopyBlock(ptr, ptrValue + sourceOffset, (uint)buffer.Length);

        sourceOffset += buffer.Length;
    }

    public static unsafe void WriteToBuffer(this in ReadOnlySequence<byte> sequence, byte[] buffer, ref int offset)
    {
        if (sequence.IsSingleSegment)
        {
            var span = sequence.First.Span;

            fixed (byte* ptr = buffer)
            fixed (byte* ptrValue = span)
                Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

            offset += span.Length;
        }
        else
        {
            WriteToBufferMultiple(sequence, buffer, ref offset);
        }
    }

    private static unsafe void WriteToBufferMultiple(in ReadOnlySequence<byte> sequence, byte[] buffer, ref int offset)
    {
        var position = sequence.Start;

        fixed (byte* ptr = buffer)
        {
            while (sequence.TryGet(ref position, out var memory))
            {
                var span = memory.Span;

                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                offset += span.Length;

                if (position.GetObject() is null)
                    break;
            }
        }
    }

    public static unsafe void WriteToBuffer(this in ReadOnlySequence<byte> sequence, Span<byte> buffer, ref int offset)
    {
        if (sequence.IsSingleSegment)
        {
            var span = sequence.First.Span;

            fixed (byte* ptr = buffer)
            fixed (byte* ptrValue = span)
                Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

            offset += span.Length;
        }
        else
        {
            WriteToBufferMultiple(sequence, buffer, ref offset);
        }
    }

    private static unsafe void WriteToBufferMultiple(in ReadOnlySequence<byte> sequence, Span<byte> buffer, ref int offset)
    {
        var position = sequence.Start;

        fixed (byte* ptr = buffer)
        {
            while (sequence.TryGet(ref position, out var memory))
            {
                var span = memory.Span;

                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                offset += span.Length;

                if (position.GetObject() is null)
                    break;
            }
        }
    }

    public static unsafe void WriteToBuffer(this in ReadOnlySequence<byte> sequence, byte[] buffer)
    {
        if (sequence.IsSingleSegment)
        {
            var span = sequence.First.Span;

            fixed (byte* ptr = buffer)
            fixed (byte* ptrValue = span)
                Unsafe.CopyBlock(ptr, ptrValue, (uint)span.Length);
        }
        else
        {
            WriteToBufferMultiple(sequence, buffer);
        }
    }

    private static unsafe void WriteToBufferMultiple(in ReadOnlySequence<byte> sequence, byte[] buffer)
    {
        var position = sequence.Start;
        int offset = 0;

        fixed (byte* ptr = buffer)
        {
            while (sequence.TryGet(ref position, out var memory))
            {
                var span = memory.Span;

                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                offset += span.Length;

                if (position.GetObject() is null)
                    break;
            }
        }
    }

}
