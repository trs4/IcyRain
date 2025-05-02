using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers;

internal sealed class ObjectCollectionSerializer<TResolver, T, TCollection> : Serializer<TResolver, TCollection>
    where TResolver : Resolver
    where TCollection : ICollection<T>, new()
{
    private readonly int? _size;
    private readonly Serializer<TResolver, T> _serializer = Serializer<TResolver, T>.Instance;
    private readonly ConstructorInfo _capacityConstructor;

    public ObjectCollectionSerializer()
    {
        _size = _serializer.GetSize();

        _capacityConstructor = typeof(TCollection).GetConstructors().FirstOrDefault(c =>
        {
            var parameters = c.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == Types.Int && parameters[0].Name == "capacity";
        });
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TCollection value)
    {
        if (value is null || value.Count == 0)
            return 4;

        return _size.HasValue ? value.Count * _size.Value + 4 : CalculateCapacity(value);
    }

    private int CalculateCapacity(TCollection value)
    {
        int capacity = 4;

        foreach (var item in value)
            capacity += _serializer.GetCapacity(item);

        return capacity;
    }

    public override sealed void Serialize(ref Writer writer, TCollection value)
    {
        int length = value is null ? -1 : value.Count;
        writer.WriteInt(length);

        if (length > 0)
        {
            foreach (var item in value)
                _serializer.Serialize(ref writer, item);
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, TCollection value)
    {
        writer.WriteInt(value.Count);

        foreach (var item in value)
            _serializer.Serialize(ref writer, item);
    }

    public override sealed TCollection Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length < 0)
            return default;

        var value = _capacityConstructor is null
            ? new TCollection()
            : (TCollection)_capacityConstructor.Invoke([length]);

        for (int i = 0; i < length; i++)
            value.Add(_serializer.Deserialize(ref reader));

        return value;
    }

    public override sealed TCollection DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length < 0)
            return default;

        var value = _capacityConstructor is null
            ? new TCollection()
            : (TCollection)_capacityConstructor.Invoke([length]);

        for (int i = 0; i < length; i++)
            value.Add(_serializer.DeserializeInUTC(ref reader));

        return value;
    }

    public override sealed TCollection DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        var value = _capacityConstructor is null
            ? new TCollection()
            : (TCollection)_capacityConstructor.Invoke([length]);

        for (int i = 0; i < length; i++)
            value.Add(_serializer.Deserialize(ref reader));

        return value;
    }

    public override sealed TCollection DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        var value = _capacityConstructor is null
            ? new TCollection()
            : (TCollection)_capacityConstructor.Invoke([length]);

        for (int i = 0; i < length; i++)
            value.Add(_serializer.DeserializeInUTC(ref reader));

        return value;
    }

}
