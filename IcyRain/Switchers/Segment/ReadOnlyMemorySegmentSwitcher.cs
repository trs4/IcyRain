using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;

namespace IcyRain.Switchers;

internal sealed class ReadOnlyMemorySegmentSwitcher : SegmentSwitcher<ReadOnlyMemory<byte>>
{
    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> Serialize(ReadOnlyMemory<byte> value)
        => new ArraySegment<byte>(value.ToArray());

    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> SerializeWithLZ4(ReadOnlyMemory<byte> value, out int serializedLength)
    {
        serializedLength = value.Length;
        return LZ4SegmentEncoder.Encode(value.Span);
    }


    [MethodImpl(Flags.HotPath)]
    public sealed override ReadOnlyMemory<byte> Deserialize(ArraySegment<byte> segment) => segment;

    [MethodImpl(Flags.HotPath)]
    public sealed override ReadOnlyMemory<byte> DeserializeInUTC(ArraySegment<byte> segment) => segment;

    [MethodImpl(Flags.HotPath)]
    public sealed override ReadOnlyMemory<byte> DeserializeWithLZ4(ArraySegment<byte> segment, out int decodedLength)
    {
        byte[] buffer = segment.TransferToRentArrayWithLZ4Decompress(out decodedLength);
        return new ReadOnlyMemory<byte>(buffer, 0, decodedLength);
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override ReadOnlyMemory<byte> DeserializeInUTCWithLZ4(ArraySegment<byte> segment, out int decodedLength)
    {
        byte[] buffer = segment.TransferToRentArrayWithLZ4Decompress(out decodedLength);
        return new ReadOnlyMemory<byte>(buffer, 0, decodedLength);
    }

}
