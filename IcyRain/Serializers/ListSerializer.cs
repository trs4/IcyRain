using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers
{
    internal sealed class ListSerializer<TResolver, T> : Serializer<TResolver, List<T>>
        where TResolver : Resolver
    {
        private readonly int? _size;
        private readonly Serializer<TResolver, T> _serializer = Serializer<TResolver, T>.Instance;

        public ListSerializer()
            => _size = _serializer.GetSize();

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<T> value)
        {
            if (value is null || value.Count == 0)
                return 4;

            return _size.HasValue ? value.Count * _size.Value + 4 : CalculateCapacity(value);
        }

        private int CalculateCapacity(List<T> value)
        {
            var array = value.GetArray();
            int capacity = 4;

            for (int i = 0; i < value.Count; i++)
                capacity += _serializer.GetCapacity(array[i]);

            return capacity;
        }

        public override sealed void Serialize(ref Writer writer, List<T> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
            {
                var array = value.GetArray();

                for (int i = 0; i < length; i++)
                    _serializer.Serialize(ref writer, array[i]);
            }
        }

        public override sealed void SerializeSpot(ref Writer writer, List<T> value)
        {
            writer.WriteInt(value.Count);
            var array = value.GetArray();

            for (int i = 0; i < value.Count; i++)
                _serializer.Serialize(ref writer, array[i]);
        }

        public override sealed List<T> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length > 0)
            {
                var value = new T[length];

                for (int i = 0; i < length; i++)
                    value[i] = _serializer.Deserialize(ref reader);

                return value.CreateList();
            }

            return length == 0 ? new List<T>() : null;
        }

        public override sealed List<T> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length > 0)
            {
                var value = new T[length];

                for (int i = 0; i < length; i++)
                    value[i] = _serializer.DeserializeInUTC(ref reader);

                return value.CreateList();
            }

            return length == 0 ? new List<T>() : null;
        }

        public override sealed List<T> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length == 0)
                return new List<T>();

            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _serializer.Deserialize(ref reader);

            return value.CreateList();
        }

        public override sealed List<T> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length == 0)
                return new List<T>();

            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _serializer.DeserializeInUTC(ref reader);

            return value.CreateList();
        }

    }
}
