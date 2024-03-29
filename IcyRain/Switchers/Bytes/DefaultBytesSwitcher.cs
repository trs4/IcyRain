﻿using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Switchers;

internal sealed class DefaultBytesSwitcher<T> : BytesSwitcher<T>
{
    private readonly int? _size;

    public DefaultBytesSwitcher()
        => _size = Serializer<Resolver, T>.Instance.GetSize();

    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] Serialize(T value)
    {
        var serializer = Serializer<Resolver, T>.Instance;
        int capacity = _size ?? serializer.GetCapacity(value);

        var buffer = Buffers.Rent(capacity);
        var writer = new Writer(buffer);
        serializer.Serialize(ref writer, value);
        return Buffers.ToArray(buffer, writer.Size);
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override byte[] SerializeWithLZ4(T value, out int serializedLength)
    {
        var serializer = Serializer<Resolver, T>.Instance;
        int capacity = (_size ?? serializer.GetCapacity(value)) + 1;

        var buffer = Buffers.Rent(capacity);
        var writer = new Writer(buffer, true);
        serializer.Serialize(ref writer, value);
        serializedLength = writer.Size;

        if (serializedLength > Buffers.MinCompressSize)
            writer.CompressLZ4();

        return Buffers.ToArray(buffer, writer.Size);
    }


    [MethodImpl(Flags.HotPath)]
    public sealed override T Deserialize(byte[] bytes, int offset, int count)
    {
        var reader = new Reader(new ReadOnlyMemory<byte>(bytes, offset, count));
        return Serializer<Resolver, T>.Instance.Deserialize(ref reader);
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override T DeserializeInUTC(byte[] bytes, int offset, int count)
    {
        var reader = new Reader(new ReadOnlyMemory<byte>(bytes, offset, count));
        return Serializer<Resolver, T>.Instance.DeserializeInUTC(ref reader);
    }

    [MethodImpl(Flags.HotPath)]
    public sealed override T DeserializeWithLZ4(byte[] bytes, int offset, int count, out int decodedLength)
    {
        Reader reader;
        decodedLength = count;

        if (count == 0)
            return default;

        var inputMemory = new ReadOnlyMemory<byte>(bytes, offset, count);

        if (inputMemory.Span[0] == 0) // No compress
        {
            reader = new Reader(inputMemory, true);
            return Serializer<Resolver, T>.Instance.Deserialize(ref reader);
        }

        byte[] buffer = Buffers.Rent(count);
        buffer.WriteTo(bytes, offset, count);

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
    public sealed override T DeserializeInUTCWithLZ4(byte[] bytes, int offset, int count, out int decodedLength)
    {
        Reader reader;
        decodedLength = count;

        if (count == 0)
            return default;

        var inputMemory = new ReadOnlyMemory<byte>(bytes, offset, count);

        if (inputMemory.Span[0] == 0) // No compress
        {
            reader = new Reader(inputMemory, true);
            return Serializer<Resolver, T>.Instance.DeserializeInUTC(ref reader);
        }

        byte[] buffer = Buffers.Rent(count);
        buffer.WriteTo(bytes, offset, count);

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
