using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IcyRain.Comparers;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers
{
    internal sealed class DictionarySerializer<TResolver, TKey, TValue> : Serializer<TResolver, Dictionary<TKey, TValue>>
        where TResolver : Resolver
    {
        private readonly int? _keySize;
        private readonly int? _size;
        private readonly Serializer<TResolver, TKey> _keySerializer = Serializer<TResolver, TKey>.Instance;
        private readonly Serializer<TResolver, TValue> _valueSerializer = Serializer<TResolver, TValue>.Instance;
        private readonly IEqualityComparer<TKey> _comparer = SerializerComparer<TKey>.Instance;

        public DictionarySerializer()
        {
            _keySize = _keySerializer.GetSize();
            int? valueSize = _valueSerializer.GetSize();

            if (_keySize.HasValue && valueSize.HasValue)
                _size = _keySize.Value + valueSize.Value;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(Dictionary<TKey, TValue> value)
        {
            if (value is null || value.Count == 0)
                return 4;

            return _size.HasValue ? _size.Value * value.Count + 4 : CalculateCapacity(value);
        }

        private int CalculateCapacity(Dictionary<TKey, TValue> value)
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

        public override sealed void Serialize(ref Writer writer, Dictionary<TKey, TValue> value)
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

        public override sealed void SerializeSpot(ref Writer writer, Dictionary<TKey, TValue> value)
        {
            writer.WriteInt(value.Count);

            foreach (var pair in value)
            {
                _keySerializer.Serialize(ref writer, pair.Key);
                _valueSerializer.Serialize(ref writer, pair.Value);
            }
        }

        public override sealed Dictionary<TKey, TValue> Deserialize(ref Reader reader, DeserializeOptions options)
        {
            int length = reader.ReadInt();

            if (length > 0)
            {
                var value = new Dictionary<TKey, TValue>(length, _comparer);

                for (int i = 0; i < length; i++)
                    value.Add(_keySerializer.Deserialize(ref reader, options), _valueSerializer.Deserialize(ref reader, options));

                return value;
            }

            return length == 0 ? new Dictionary<TKey, TValue>(_comparer) : null;
        }

        public override sealed Dictionary<TKey, TValue> DeserializeSpot(ref Reader reader, DeserializeOptions options)
        {
            int length = reader.ReadInt();
            var value = new Dictionary<TKey, TValue>(length, _comparer);

            for (int i = 0; i < length; i++)
                value.Add(_keySerializer.Deserialize(ref reader, options), _valueSerializer.Deserialize(ref reader, options));

            return value;
        }

    }
}
