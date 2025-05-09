﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers;

internal sealed class IEnumerableSerializer<TResolver, T> : Serializer<TResolver, IEnumerable<T>>
    where TResolver : Resolver
{
    private readonly int? _size;
    private readonly Serializer<TResolver, T> _serializer = Serializer<TResolver, T>.Instance;

    public IEnumerableSerializer()
        => _size = _serializer.GetSize();

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(IEnumerable<T> value)
    {
        if (value is null)
            return 4;

        int capacity = 4;

        if (value.TryGetArray(out var array, out int count))
        {
            if (_size.HasValue)
                return count * _size.Value + 4;

            for (int i = 0; i < count; i++)
                capacity += _serializer.GetCapacity(array[i]);
        }
        else
        {
            foreach (var item in value)
                capacity += _serializer.GetCapacity(item);
        }

        return capacity;
    }

    public override sealed void Serialize(ref Writer writer, IEnumerable<T> value)
    {
        if (value is null)
        {
            writer.WriteInt(-1);
        }
        else if (value.TryGetArray(out var array, out int count))
        {
            writer.WriteInt(count);

            for (int i = 0; i < count; i++)
                _serializer.Serialize(ref writer, array[i]);
        }
        else
        {
            int length = value.CalculateLength();
            writer.WriteInt(length);

            foreach (var item in value)
                _serializer.Serialize(ref writer, item);
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, IEnumerable<T> value)
    {
        if (value.TryGetArray(out var array, out int count))
        {
            writer.WriteInt(count);

            for (int i = 0; i < count; i++)
                _serializer.Serialize(ref writer, array[i]);
        }
        else
        {
            int length = value.CalculateLength();
            writer.WriteInt(length);

            foreach (var item in value)
                _serializer.Serialize(ref writer, item);
        }
    }

    public override sealed IEnumerable<T> Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _serializer.Deserialize(ref reader);

            return value;
        }

        return length == 0 ? [] : null;
    }

    public override sealed IEnumerable<T> DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _serializer.DeserializeInUTC(ref reader);

            return value;
        }

        return length == 0 ? [] : null;
    }

    public override sealed IEnumerable<T> DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return [];

        var value = new T[length];

        for (int i = 0; i < length; i++)
            value[i] = _serializer.Deserialize(ref reader);

        return value;
    }

    public override sealed IEnumerable<T> DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return [];

        var value = new T[length];

        for (int i = 0; i < length; i++)
            value[i] = _serializer.DeserializeInUTC(ref reader);

        return value;
    }

}
