using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers;

internal sealed class IReadOnlyListSerializer<TResolver, T> : Serializer<TResolver, IReadOnlyList<T>>
    where TResolver : Resolver
{
    private readonly int? _size;
    private readonly Serializer<TResolver, T> _serializer = Serializer<TResolver, T>.Instance;
    private readonly Serializer<TResolver, T[]> _arraySerializer = Serializer<TResolver, T[]>.Instance;

    public IReadOnlyListSerializer()
        => _size = _serializer.GetSize();

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(IReadOnlyList<T> value)
    {
        if (value is null || value.Count == 0)
            return 4;

        return _size.HasValue ? value.Count * _size.Value + 4 : CalculateCapacity(value);
    }

    private int CalculateCapacity(IReadOnlyList<T> value)
    {
        if (value is T[] arrayValue)
            return _arraySerializer.GetCapacity(arrayValue);
        else if (value is List<T> listValue)
            return Serializer<TResolver, List<T>>.Instance.GetCapacity(listValue);

        int capacity = 4;

        for (int i = 0; i < value.Count; i++)
            capacity += _serializer.GetCapacity(value[i]);

        return capacity;
    }

    public override sealed void Serialize(ref Writer writer, IReadOnlyList<T> value)
    {
        if (value is T[] arrayValue)
            _arraySerializer.Serialize(ref writer, arrayValue);
        else if (value is List<T> listValue)
            Serializer<TResolver, List<T>>.Instance.Serialize(ref writer, listValue);
        else
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
            {
                for (int i = 0; i < value.Count; i++)
                    _serializer.Serialize(ref writer, value[i]);
            }
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, IReadOnlyList<T> value)
    {
        if (value is T[] arrayValue)
            _arraySerializer.SerializeSpot(ref writer, arrayValue);
        else if (value is List<T> listValue)
            Serializer<TResolver, List<T>>.Instance.SerializeSpot(ref writer, listValue);
        else
        {
            writer.WriteInt(value.Count);

            for (int i = 0; i < value.Count; i++)
                _serializer.Serialize(ref writer, value[i]);
        }
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed IReadOnlyList<T> Deserialize(ref Reader reader)
        => _arraySerializer.Deserialize(ref reader);

    [MethodImpl(Flags.HotPath)]
    public override sealed IReadOnlyList<T> DeserializeInUTC(ref Reader reader)
        => _arraySerializer.DeserializeInUTC(ref reader);

    [MethodImpl(Flags.HotPath)]
    public override sealed IReadOnlyList<T> DeserializeSpot(ref Reader reader)
        => _arraySerializer.DeserializeSpot(ref reader);

    [MethodImpl(Flags.HotPath)]
    public override sealed IReadOnlyList<T> DeserializeInUTCSpot(ref Reader reader)
        => _arraySerializer.DeserializeInUTCSpot(ref reader);
}
