using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers;

internal sealed class ObjectDictionarySerializer<TResolver, TKey, TValue, TDictionary> : Serializer<TResolver, TDictionary>
    where TResolver : Resolver
    where TDictionary : IDictionary<TKey, TValue>, new()
{
    private readonly int? _keySize;
    private readonly int? _size;
    private readonly Serializer<TResolver, TKey> _keySerializer = Serializer<TResolver, TKey>.Instance;
    private readonly Serializer<TResolver, TValue> _valueSerializer = Serializer<TResolver, TValue>.Instance;
    private readonly ConstructorInfo _capacityConstructor;

    public ObjectDictionarySerializer()
    {
        _keySize = _keySerializer.GetSize();
        int? valueSize = _valueSerializer.GetSize();

        if (_keySize.HasValue && valueSize.HasValue)
            _size = _keySize.Value + valueSize.Value;

        _capacityConstructor = typeof(TDictionary).GetConstructors().FirstOrDefault(c =>
        {
            var parameters = c.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == Types.Int && parameters[0].Name == "capacity";
        });
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TDictionary value)
    {
        if (value is null || value.Count == 0)
            return 4;

        return _size.HasValue ? _size.Value * value.Count + 4 : CalculateCapacity(value);
    }

    private int CalculateCapacity(TDictionary value)
    {
        int capacity = 4;

        if (_keySize.HasValue)
        {
            capacity += _keySize.Value * value.Count;
        }
        else
        {
            foreach (var key in value.Keys)
                capacity += _keySerializer.GetCapacity(key);
        }

        foreach (var item in value.Values)
            capacity += _valueSerializer.GetCapacity(item);

        return capacity;
    }

    public override sealed void Serialize(ref Writer writer, TDictionary value)
    {
        int length = value is null ? -1 : value.Count;
        writer.WriteInt(length);

        if (length > 0)
        {
            foreach (var pair in value)
            {
                _keySerializer.Serialize(ref writer, pair.Key);
                _valueSerializer.Serialize(ref writer, pair.Value);
            }
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, TDictionary value)
    {
        writer.WriteInt(value.Count);

        foreach (var pair in value)
        {
            _keySerializer.Serialize(ref writer, pair.Key);
            _valueSerializer.Serialize(ref writer, pair.Value);
        }
    }

    public override sealed TDictionary Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length < 0)
            return default;

        var value = _capacityConstructor is null
            ? new TDictionary()
            : (TDictionary)_capacityConstructor.Invoke(new object[] { length });

        for (int i = 0; i < length; i++)
            value.Add(_keySerializer.Deserialize(ref reader), _valueSerializer.Deserialize(ref reader));

        return value;
    }

    public override sealed TDictionary DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length < 0)
            return default;

        var value = _capacityConstructor is null
            ? new TDictionary()
            : (TDictionary)_capacityConstructor.Invoke(new object[] { length });

        for (int i = 0; i < length; i++)
            value.Add(_keySerializer.DeserializeInUTC(ref reader), _valueSerializer.DeserializeInUTC(ref reader));

        return value;
    }

    public override sealed TDictionary DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        var value = _capacityConstructor is null
            ? new TDictionary()
            : (TDictionary)_capacityConstructor.Invoke(new object[] { length });

        for (int i = 0; i < length; i++)
            value.Add(_keySerializer.Deserialize(ref reader), _valueSerializer.Deserialize(ref reader));

        return value;
    }

    public override sealed TDictionary DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        var value = _capacityConstructor is null
            ? new TDictionary()
            : (TDictionary)_capacityConstructor.Invoke(new object[] { length });

        for (int i = 0; i < length; i++)
            value.Add(_keySerializer.DeserializeInUTC(ref reader), _valueSerializer.DeserializeInUTC(ref reader));

        return value;
    }

}
