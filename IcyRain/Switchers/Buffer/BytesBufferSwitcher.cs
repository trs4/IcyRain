﻿using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;

namespace IcyRain.Switchers;

internal sealed class BytesBufferSwitcher : BufferSwitcher<byte[]>
{
    [MethodImpl(Flags.HotPath)]
    public unsafe sealed override int Serialize(IBufferWriter<byte> buffer, byte[] value)
    {
        if (buffer is null)
            throw new ArgumentNullException(nameof(buffer));

        if (value is null)
            throw new ArgumentNullException(nameof(value));

        int length = value.Length;

        if (length > 0)
        {
            fixed (byte* ptr = buffer.GetSpan(length))
            fixed (byte* ptrValue = value)
                Unsafe.CopyBlock(ptr, ptrValue, (uint)length);
        }

        buffer.Advance(length);
        return length;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe sealed override int SerializeWithLZ4(IBufferWriter<byte> buffer, byte[] value, out int serializedLength)
    {
        if (buffer is null)
            throw new ArgumentNullException(nameof(buffer));

        if (value is null)
            throw new ArgumentNullException(nameof(value));

        int encodedLength = serializedLength = value.Length;

        if (serializedLength > 0)
        {
            fixed (byte* ptr = buffer.GetSpan(serializedLength + 1))
            fixed (byte* ptrValue = value)
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
    public sealed override byte[] Deserialize(in ReadOnlySequence<byte> sequence)
        => sequence.TransferToArray();

    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] DeserializeInUTC(in ReadOnlySequence<byte> sequence)
        => sequence.TransferToArray();

    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] DeserializeWithLZ4(in ReadOnlySequence<byte> sequence, int sequenceLength, out int decodedLength)
        => sequence.TransferToArrayWithLZ4Decompress(sequenceLength, out decodedLength);

    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] DeserializeInUTCWithLZ4(in ReadOnlySequence<byte> sequence, int sequenceLength, out int decodedLength)
        => sequence.TransferToArrayWithLZ4Decompress(sequenceLength, out decodedLength);
}
