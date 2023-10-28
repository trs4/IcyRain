using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;

namespace IcyRain.Switchers;

internal sealed class SegmentBufferSwitcher : BufferSwitcher<ArraySegment<byte>>
{
    [MethodImpl(Flags.HotPath)]
    public unsafe sealed override int Serialize(IBufferWriter<byte> buffer, ArraySegment<byte> value)
    {
        if (buffer is null)
            throw new ArgumentNullException(nameof(buffer));

        int length = value.Count;

        if (length > 0)
        {
            fixed (byte* ptr = buffer.GetSpan(length))
            fixed (byte* ptrValue = value.Array)
                Unsafe.CopyBlock(ptr, ptrValue + value.Offset, (uint)length);
        }

        buffer.Advance(length);
        return length;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe sealed override int SerializeWithLZ4(IBufferWriter<byte> buffer, ArraySegment<byte> value, out int serializedLength)
    {
        if (buffer is null)
            throw new ArgumentNullException(nameof(buffer));

        int encodedLength = serializedLength = value.Count;

        if (serializedLength > 0)
        {
            fixed (byte* ptr = buffer.GetSpan(serializedLength + 1))
            fixed (byte* ptrValue = value.Array)
            {
                if (serializedLength > Buffers.MinCompressSize)
                    LZ4Codec.Encode(ptrValue, ptr, ref encodedLength);
                else
                    BlockBuilder.NoCompression(ptr, ptrValue, ref encodedLength);
            }
        }

        buffer.Advance(encodedLength);
        return encodedLength;
    }


    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> Deserialize(in ReadOnlySequence<byte> sequence)
        => new ArraySegment<byte>(sequence.TransferToArray());

    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> DeserializeInUTC(in ReadOnlySequence<byte> sequence)
        => new ArraySegment<byte>(sequence.TransferToArray());

    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> DeserializeWithLZ4(in ReadOnlySequence<byte> sequence, int sequenceLength, out int decodedLength)
        => sequence.TransferToSegmentWithLZ4Decompress(sequenceLength, out decodedLength);

    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> DeserializeInUTCWithLZ4(in ReadOnlySequence<byte> sequence, int sequenceLength, out int decodedLength)
        => sequence.TransferToSegmentWithLZ4Decompress(sequenceLength, out decodedLength);
}
