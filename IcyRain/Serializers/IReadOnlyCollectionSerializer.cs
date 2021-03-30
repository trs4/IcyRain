using System;
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
            int capacity = 4;

            if (value.TryGetArray(out var array))
            {
                for (int i = 0; i < value.Count; i++)
                    capacity += _serializer.GetCapacity(array[i]);
            }
            else
            {
                foreach (var item in value)
                    capacity += _serializer.GetCapacity(item);
            }

            return capacity;
        }

        public override sealed void Serialize(ref Writer writer, IReadOnlyCollection<T> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
            {
                if (value.TryGetArray(out var array))
                {
                    for (int i = 0; i < length; i++)
                        _serializer.Serialize(ref writer, array[i]);
                }
                else
                {
                    foreach (var item in value)
                        _serializer.Serialize(ref writer, item);
                }
            }
        }

        public override sealed void SerializeSpot(ref Writer writer, IReadOnlyCollection<T> value)
        {
            writer.WriteInt(value.Count);

            if (value.TryGetArray(out var array))
            {
                for (int i = 0; i < value.Count; i++)
                    _serializer.Serialize(ref writer, array[i]);
            }
            else
            {
                foreach (var item in value)
                    _serializer.Serialize(ref writer, item);
            }
        }

        public override sealed IReadOnlyCollection<T> Deserialize(ref Reader reader)
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

        public override sealed IReadOnlyCollection<T> DeserializeInUTC(ref Reader reader)
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

        public override sealed IReadOnlyCollection<T> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length == 0)
                return Array.Empty<T>();

            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _serializer.Deserialize(ref reader);

            return value;
        }

        public override sealed IReadOnlyCollection<T> DeserializeInUTCSpot(ref Reader reader)
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
}
