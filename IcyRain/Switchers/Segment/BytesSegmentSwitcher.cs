using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;

namespace IcyRain.Switchers;

internal sealed class BytesSegmentSwitcher : SegmentSwitcher<byte[]>
{
    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> Serialize(byte[] value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return new ArraySegment<byte>(value);
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> SerializeWithLZ4(byte[] value, out int serializedLength)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        serializedLength = value.Length;
        return LZ4SegmentEncoder.Encode(value);
    }


    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] Deserialize(ArraySegment<byte> segment)
        => segment.Count == 0 ? [] : (segment.Array.Length == segment.Count ? segment.Array : segment.TransferToArray());

    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] DeserializeInUTC(ArraySegment<byte> segment)
        => segment.Count == 0 ? [] : (segment.Array.Length == segment.Count ? segment.Array : segment.TransferToArray());

    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] DeserializeWithLZ4(ArraySegment<byte> segment, out int decodedLength)
        => segment.TransferToArrayWithLZ4Decompress(out decodedLength);

    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] DeserializeInUTCWithLZ4(ArraySegment<byte> segment, out int decodedLength)
        => segment.TransferToArrayWithLZ4Decompress(out decodedLength);
}
