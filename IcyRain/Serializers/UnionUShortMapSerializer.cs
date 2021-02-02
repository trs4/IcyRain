using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

#pragma warning disable CA1062 // Validate arguments of public methods
namespace IcyRain.Serializers
{
    public abstract class UnionUShortMapSerializer<T> : Serializer<UnionResolver, T>
        where T : class
    {
        private static readonly Dictionary<Type, UnionUShortData<T>> _map = new Dictionary<Type, UnionUShortData<T>>();
        private static readonly Dictionary<ushort, DeserializeMethod<T>> _deserializeMap = new Dictionary<ushort, DeserializeMethod<T>>();

        public void Add<TUnionType>(ushort index)
            where TUnionType : T
        {
            _map.Add(typeof(TUnionType), new UnionUShortData<T>(index,
                (T value) => Serializer<Resolver, TUnionType>.Instance.GetCapacity((TUnionType)value),
                (ref Writer writer, T value) => Serializer<Resolver, TUnionType>.Instance.SerializeSpot(ref writer, (TUnionType)value)));

            _deserializeMap.Add(index,
                (ref Reader reader, DeserializeOptions options) => Serializer<Resolver, TUnionType>.Instance.DeserializeSpot(ref reader, options));
        }

        protected UnionUShortMapSerializer() { }

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(T value)
            => value is null ? 1 : (_map.TryGetValue(value.GetType(), out var data)
                ? data.GetCapacity(value) + 1
                : throw new InvalidOperationException("Unknown type: " + value.GetType().FullName));

        public override sealed void Serialize(ref Writer writer, T value)
        {
            if (value is null)
            {
                writer.WriteUShort(0); // Empty object
            }
            else if (_map.TryGetValue(value.GetType(), out var data))
            {
                writer.WriteUShort(data.Index);
                data.SerializeSpot(ref writer, value);
            }
            else
            {
                throw new InvalidOperationException("Unknown type: " + value.GetType().FullName);
            }
        }

        public override sealed void SerializeSpot(ref Writer writer, T value)
        {
            if (_map.TryGetValue(value.GetType(), out var data))
            {
                writer.WriteUShort(data.Index);
                data.SerializeSpot(ref writer, value);
            }
            else
            {
                throw new InvalidOperationException("Unknown type: " + value.GetType().FullName);
            }
        }

        public override sealed T Deserialize(ref Reader reader, DeserializeOptions options)
        {
            ushort index = reader.ReadUShort();

            return index == 0 ? null : (_deserializeMap.TryGetValue(index, out var deserializeMethod)
                ? deserializeMethod(ref reader, options)
                : throw new InvalidOperationException("Unknown index: " + index));
        }

        public override sealed T DeserializeSpot(ref Reader reader, DeserializeOptions options)
        {
            ushort index = reader.ReadUShort();

            return _deserializeMap.TryGetValue(index, out var deserializeMethod)
                ? deserializeMethod(ref reader, options)
                : throw new InvalidOperationException("Unknown index: " + index);
        }

    }
}
#pragma warning restore CA1062 // Validate arguments of public methods
