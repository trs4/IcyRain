using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;

namespace IcyRain.Switchers;

internal sealed class MemoryBytesSwitcher : BytesSwitcher<Memory<byte>>
{
    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] Serialize(Memory<byte> value)
        => value.ToArray();

    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] SerializeWithLZ4(Memory<byte> value, out int serializedLength)
    {
        serializedLength = value.Length;
        return LZ4ArrayEncoder.Encode(value.Span);
    }


    [MethodImpl(Flags.HotPath)]
    public sealed override Memory<byte> Deserialize(byte[] bytes, int offset, int count)
    {
        if (bytes is null)
            throw new ArgumentNullException(nameof(bytes));

        return new Memory<byte>(bytes, offset, count);
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override Memory<byte> DeserializeInUTC(byte[] bytes, int offset, int count)
    {
        if (bytes is null)
            throw new ArgumentNullException(nameof(bytes));

        return new Memory<byte>(bytes, offset, count);
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override Memory<byte> DeserializeWithLZ4(byte[] bytes, int offset, int count, out int decodedLength)
    {
        decodedLength = count;
        byte[] result = LZ4ArrayDecoder.RentDecode(new Span<byte>(bytes, offset, count), ref decodedLength);
        return new Memory<byte>(result, 0, decodedLength);
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override Memory<byte> DeserializeInUTCWithLZ4(byte[] bytes, int offset, int count, out int decodedLength)
    {
        decodedLength = count;
        byte[] result = LZ4ArrayDecoder.RentDecode(new Span<byte>(bytes, offset, count), ref decodedLength);
        return new Memory<byte>(result, 0, decodedLength);
    }

}
