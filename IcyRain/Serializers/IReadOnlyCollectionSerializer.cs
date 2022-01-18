using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers
{
    internal sealed class IReadOnlyCollectionSerializer<TResolver, T> : Serializer<TResolver, IReadOnlyCollection<T>>
        where TResolver : Resolver
    {
        private readonly int? _size;
        private readonly Serializer<TResolver, T> _serializer = Serializer<TResolver, T>.Instance;
        private readonly Serializer<TResolver, T[]> _arraySerializer = Serializer<TResolver, T[]>.Instance;

        public IReadOnlyCollectionSerializer()
            => _size = _serializer.GetSize();

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(IReadOnlyCollection<T> value)
        {
            if (value is null || value.Count == 0)
                return 4;

            return _size.HasValue ? value.Count * _size.Value + 4 : CalculateCapacity(value);
        }

        private int CalculateCapacity(IReadOnlyCollection<T> value)
        {
            if (value is T[] arrayValue)
                return _arraySerializer.GetCapacity(arrayValue);
            else if (value is List<T> listValue)
                return Serializer<TResolver, List<T>>.Instance.GetCapacity(listValue);

            int capacity = 4;

            foreach (var item in value)
                capacity += _serializer.GetCapacity(item);

            return capacity;
        }

        public override sealed void Serialize(ref Writer writer, IReadOnlyCollection<T> value)
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
                    foreach (var item in value)
                        _serializer.Serialize(ref writer, item);
                }
            }
        }

        public override sealed void SerializeSpot(ref Writer writer, IReadOnlyCollection<T> value)
        {
            if (value is T[] arrayValue)
                _arraySerializer.SerializeSpot(ref writer, arrayValue);
            else if (value is List<T> listValue)
                Serializer<TResolver, List<T>>.Instance.SerializeSpot(ref writer, listValue);
            else
            {
                writer.WriteInt(value.Count);

                foreach (var item in value)
                    _serializer.Serialize(ref writer, item);
            }
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed IReadOnlyCollection<T> Deserialize(ref Reader reader)
            => _arraySerializer.Deserialize(ref reader);

        [MethodImpl(Flags.HotPath)]
        public override sealed IReadOnlyCollection<T> DeserializeInUTC(ref Reader reader)
            => _arraySerializer.DeserializeInUTC(ref reader);

        [MethodImpl(Flags.HotPath)]
        public override sealed IReadOnlyCollection<T> DeserializeSpot(ref Reader reader)
            => _arraySerializer.Deserialize(ref reader);

        [MethodImpl(Flags.HotPath)]
        public override sealed IReadOnlyCollection<T> DeserializeInUTCSpot(ref Reader reader)
            => _arraySerializer.DeserializeInUTC(ref reader);
    }
}
