using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers;

internal sealed class ArraySerializer<TResolver, T> : Serializer<TResolver, T[]>
    where TResolver : Resolver
{
    private readonly int? _size;
    private readonly Serializer<TResolver, T> _serializer = Serializer<TResolver, T>.Instance;

    public ArraySerializer()
        => _size = _serializer.GetSize();

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(T[] value)
    {
        if (value is null || value.Length == 0)
            return 4;

        return _size.HasValue ? value.Length * _size.Value + 4 : CalculateCapacity(value);
    }

    private int CalculateCapacity(T[] value)
    {
        int capacity = 4;

        for (int i = 0; i < value.Length; i++)
            capacity += _serializer.GetCapacity(value[i]);

        return capacity;
    }

    public override sealed void Serialize(ref Writer writer, T[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
        {
            for (int i = 0; i < length; i++)
                _serializer.Serialize(ref writer, value[i]);
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, T[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
        {
            for (int i = 0; i < value.Length; i++)
                _serializer.Serialize(ref writer, value[i]);
        }
    }

    public override sealed T[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _serializer.Deserialize(ref reader);

            return value;
        }

        return length == 0 ? Array.Empty<T>() : null;
    }

    public override sealed T[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _serializer.DeserializeInUTC(ref reader);

            return value;
        }

        return length == 0 ? Array.Empty<T>() : null;
    }

    public override sealed T[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return Array.Empty<T>();

        var value = new T[length];

        for (int i = 0; i < length; i++)
            value[i] = _serializer.Deserialize(ref reader);

        return value;
    }

    public override sealed T[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return Array.Empty<T>();

        var value = new T[length];

        for (int i = 0; i < length; i++)
            value[i] = _serializer.DeserializeInUTC(ref reader);

        return value;
    }

}
