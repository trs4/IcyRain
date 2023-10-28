using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Switchers;

internal sealed class DefaultSegmentSwitcher<T> : SegmentSwitcher<T>
{
    private readonly int? _size;

    public DefaultSegmentSwitcher()
        => _size = Serializer<Resolver, T>.Instance.GetSize();

    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> Serialize(T value)
    {
        var serializer = Serializer<Resolver, T>.Instance;
        int capacity = _size ?? serializer.GetCapacity(value);

        var buffer = Buffers.Rent(capacity);
        var writer = new Writer(buffer);
        serializer.Serialize(ref writer, value);
        return new ArraySegment<byte>(buffer, 0, writer.Size);
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override ArraySegment<byte> SerializeWithLZ4(T value, out int serializedLength)
    {
        var serializer = Serializer<Resolver, T>.Instance;
        int capacity = (_size ?? serializer.GetCapacity(value)) + 1;

        var buffer = Buffers.Rent(capacity);
        var writer = new Writer(buffer, true);
        serializer.Serialize(ref writer, value);
        serializedLength = writer.Size;

        if (serializedLength > Buffers.MinCompressSize)
            writer.CompressLZ4();

        return new ArraySegment<byte>(buffer, 0, writer.Size);
    }


    [MethodImpl(Flags.HotPath)]
    public sealed override T Deserialize(ArraySegment<byte> segment)
    {
        var reader = new Reader(segment);
        return Serializer<Resolver, T>.Instance.Deserialize(ref reader);
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override T DeserializeInUTC(ArraySegment<byte> segment)
    {
        var reader = new Reader(segment);
        return Serializer<Resolver, T>.Instance.DeserializeInUTC(ref reader);
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override T DeserializeWithLZ4(ArraySegment<byte> segment, out int decodedLength)
    {
        Reader reader;
        decodedLength = segment.Count;

        if (decodedLength == 0)
            return default;

        var inputMemory = new ReadOnlyMemory<byte>(segment.Array, segment.Offset, decodedLength);

        if (inputMemory.Span[0] == 0) // No compress
        {
            reader = new Reader(inputMemory, true);
            return Serializer<Resolver, T>.Instance.Deserialize(ref reader);
        }

        byte[] buffer = Buffers.Rent(decodedLength);
        buffer.WriteTo(segment.Array, segment.Offset, decodedLength);

        var (memory, targetBuffer) = LZ4Codec.Decode(buffer, ref decodedLength);
        reader = new Reader(memory);

        try
        {
            return Serializer<Resolver, T>.Instance.Deserialize(ref reader);
        }
        finally
        {
            Buffers.Return(targetBuffer);
            Buffers.Return(buffer);
        }
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override T DeserializeInUTCWithLZ4(ArraySegment<byte> segment, out int decodedLength)
    {
        Reader reader;
        decodedLength = segment.Count;

        if (decodedLength == 0)
            return default;

        var inputMemory = new ReadOnlyMemory<byte>(segment.Array, segment.Offset, decodedLength);

        if (inputMemory.Span[0] == 0) // No compress
        {
            reader = new Reader(inputMemory, true);
            return Serializer<Resolver, T>.Instance.DeserializeInUTC(ref reader);
        }

        byte[] buffer = Buffers.Rent(decodedLength);
        buffer.WriteTo(segment.Array, segment.Offset, decodedLength);

        var (memory, targetBuffer) = LZ4Codec.Decode(buffer, ref decodedLength);
        reader = new Reader(memory);

        try
        {
            return Serializer<Resolver, T>.Instance.DeserializeInUTC(ref reader);
        }
        finally
        {
            Buffers.Return(targetBuffer);
            Buffers.Return(buffer);
        }
    }

}
