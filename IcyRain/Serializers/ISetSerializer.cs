﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IcyRain.Comparers;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers
{
    internal sealed class ISetSerializer<TResolver, T> : Serializer<TResolver, ISet<T>>
        where TResolver : Resolver
    {
        private readonly int? _size;
        private readonly Serializer<TResolver, T> _serializer = Serializer<TResolver, T>.Instance;
        private readonly IEqualityComparer<T> _comparer = SerializerComparer<T>.Instance;

        public ISetSerializer()
            => _size = _serializer.GetSize();

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(ISet<T> value)
        {
            if (value is null || value.Count == 0)
                return 4;

            return _size.HasValue ? _size.Value * value.Count + 4 : CalculateCapacity(value);
        }

        private int CalculateCapacity(ISet<T> value)
        {
            int capacity = 4;

            foreach (var item in value)
                capacity += _serializer.GetCapacity(item);

            return capacity;
        }

        public override sealed void Serialize(ref Writer writer, ISet<T> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
            {
                foreach (var item in value)
                    _serializer.Serialize(ref writer, item);
            }
        }

        public override sealed void SerializeSpot(ref Writer writer, ISet<T> value)
        {
            writer.WriteInt(value.Count);

            foreach (var item in value)
                _serializer.Serialize(ref writer, item);
        }

        public override sealed ISet<T> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length > 0)
            {
                var value = new HashSet<T>(length, _comparer);

                for (int i = 0; i < length; i++)
                    value.Add(_serializer.Deserialize(ref reader));

                return value;
            }

            return length == 0 ? new HashSet<T>(_comparer) : null;
        }

        public override sealed ISet<T> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length > 0)
            {
                var value = new HashSet<T>(length, _comparer);

                for (int i = 0; i < length; i++)
                    value.Add(_serializer.DeserializeInUTC(ref reader));

                return value;
            }

            return length == 0 ? new HashSet<T>(_comparer) : null;
        }

        public override sealed ISet<T> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            var value = new HashSet<T>(length, _comparer);

            for (int i = 0; i < length; i++)
                value.Add(_serializer.Deserialize(ref reader));

            return value;
        }

        public override sealed ISet<T> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            var value = new HashSet<T>(length, _comparer);

            for (int i = 0; i < length; i++)
                value.Add(_serializer.DeserializeInUTC(ref reader));

            return value;
        }

    }
}
