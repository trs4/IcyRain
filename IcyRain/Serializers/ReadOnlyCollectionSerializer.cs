﻿using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers;

internal sealed class ReadOnlyCollectionSerializer<TResolver, T> : Serializer<TResolver, ReadOnlyCollection<T>>
    where TResolver : Resolver
{
    private readonly int? _size;
    private readonly Serializer<TResolver, T> _serializer = Serializer<TResolver, T>.Instance;
    private static readonly ReadOnlyCollection<T> _empty = new ReadOnlyCollection<T>(Array.Empty<T>());

    public ReadOnlyCollectionSerializer()
        => _size = _serializer.GetSize();

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(ReadOnlyCollection<T> value)
    {
        if (value is null || value.Count == 0)
            return 4;

        return _size.HasValue ? value.Count * _size.Value + 4 : CalculateCapacity(value);
    }

    private int CalculateCapacity(ReadOnlyCollection<T> value)
    {
        int capacity = 4;

        if (value.TryGetArray(out var array, out var list, out int count))
        {
            for (int i = 0; i < count; i++)
                capacity += _serializer.GetCapacity(array[i]);
        }
        else
        {
            foreach (var item in list)
                capacity += _serializer.GetCapacity(item);
        }

        return capacity;
    }

    public override sealed void Serialize(ref Writer writer, ReadOnlyCollection<T> value)
    {
        int length = value is null ? -1 : value.Count;
        writer.WriteInt(length);

        if (length > 0)
        {
            if (value.TryGetArray(out var array, out var list, out int count))
            {
                for (int i = 0; i < count; i++)
                    _serializer.Serialize(ref writer, array[i]);
            }
            else
            {
                foreach (var item in list)
                    _serializer.Serialize(ref writer, item);
            }
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, ReadOnlyCollection<T> value)
    {
        writer.WriteInt(value.Count);

        if (value.TryGetArray(out var array, out var list, out int count))
        {
            for (int i = 0; i < count; i++)
                _serializer.Serialize(ref writer, array[i]);
        }
        else
        {
            foreach (var item in list)
                _serializer.Serialize(ref writer, item);
        }
    }

    public override sealed ReadOnlyCollection<T> Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _serializer.Deserialize(ref reader);

            return new ReadOnlyCollection<T>(value);
        }

        return length == 0 ? _empty : null;
    }

    public override sealed ReadOnlyCollection<T> DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _serializer.DeserializeInUTC(ref reader);

            return new ReadOnlyCollection<T>(value);
        }

        return length == 0 ? _empty : null;
    }

    public override sealed ReadOnlyCollection<T> DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return _empty;

        var value = new T[length];

        for (int i = 0; i < length; i++)
            value[i] = _serializer.Deserialize(ref reader);

        return new ReadOnlyCollection<T>(value);
    }

    public override sealed ReadOnlyCollection<T> DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return _empty;

        var value = new T[length];

        for (int i = 0; i < length; i++)
            value[i] = _serializer.DeserializeInUTC(ref reader);

        return new ReadOnlyCollection<T>(value);
    }

}
