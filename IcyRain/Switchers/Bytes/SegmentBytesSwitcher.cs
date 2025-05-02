using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;

namespace IcyRain.Switchers;

internal sealed class SegmentBytesSwitcher : BytesSwitcher<ArraySegment<byte>>
{
    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] Serialize(ArraySegment<byte> value)
        => value.Count == 0 ? [] : (value.Array.Length == value.Count ? value.Array : value.TransferToArray());

    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] SerializeWithLZ4(ArraySegment<byte> value, out int serializedLength)
    {
        serializedLength = value.Count;
        return LZ4ArrayEncoder.Encode(value);
    }


    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> Deserialize(byte[] bytes, int offset, int count)
        => bytes is null ? throw new ArgumentNullException(nameof(bytes)) : new ArraySegment<byte>(bytes, offset, count);

    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> DeserializeInUTC(byte[] bytes, int offset, int count)
        => bytes is null ? throw new ArgumentNullException(nameof(bytes)) : new ArraySegment<byte>(bytes, offset, count);

    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> DeserializeWithLZ4(byte[] bytes, int offset, int count, out int decodedLength)
    {
        decodedLength = count;
        byte[] result = LZ4ArrayDecoder.RentDecode(new Span<byte>(bytes, offset, count), ref decodedLength);
        return new ArraySegment<byte>(result, 0, decodedLength);
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> DeserializeInUTCWithLZ4(byte[] bytes, int offset, int count, out int decodedLength)
    {
        decodedLength = count;
        byte[] result = LZ4ArrayDecoder.RentDecode(new Span<byte>(bytes, offset, count), ref decodedLength);
        return new ArraySegment<byte>(result, 0, decodedLength);
    }

}
