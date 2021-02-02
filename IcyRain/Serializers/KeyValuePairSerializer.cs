using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers
{
    internal sealed class KeyValuePairSerializer<TResolver, TKey, TValue> : Serializer<TResolver, KeyValuePair<TKey, TValue>>
        where TResolver : Resolver
    {
        private readonly int? _keySize;
        private readonly int? _size;
        private readonly Serializer<TResolver, TKey> _keySerializer = Serializer<TResolver, TKey>.Instance;
        private readonly Serializer<TResolver, TValue> _valueSerializer = Serializer<TResolver, TValue>.Instance;

        public KeyValuePairSerializer()
        {
            _keySize = _keySerializer.GetSize();
            int? valueSize = _valueSerializer.GetSize();

            if (_keySize.HasValue && valueSize.HasValue)
                _size = _keySize.Value + valueSize.Value;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => _size;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(KeyValuePair<TKey, TValue> value)
            => _size ?? ((_keySize ?? _keySerializer.GetCapacity(value.Key)) + _valueSerializer.GetCapacity(value.Value));

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, KeyValuePair<TKey, TValue> value)
        {
            _keySerializer.Serialize(ref writer, value.Key);
            _valueSerializer.Serialize(ref writer, value.Value);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, KeyValuePair<TKey, TValue> value)
        {
            _keySerializer.Serialize(ref writer, value.Key);
            _valueSerializer.Serialize(ref writer, value.Value);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed KeyValuePair<TKey, TValue> Deserialize(ref Reader reader, DeserializeOptions options)
            => new KeyValuePair<TKey, TValue>(_keySerializer.Deserialize(ref reader, options), _valueSerializer.Deserialize(ref reader, options));

        [MethodImpl(Flags.HotPath)]
        public override sealed KeyValuePair<TKey, TValue> DeserializeSpot(ref Reader reader, DeserializeOptions options)
            => new KeyValuePair<TKey, TValue>(_keySerializer.Deserialize(ref reader, options), _valueSerializer.Deserialize(ref reader, options));
    }
}
