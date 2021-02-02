using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers
{
    internal sealed class NullableSerializer<TResolver, T> : Serializer<TResolver, T?>
        where TResolver : Resolver
        where T : struct
    {
        private readonly int? _size;
        private readonly Serializer<TResolver, T> _serializer = Serializer<TResolver, T>.Instance;

        public NullableSerializer()
        {
            int? size = _serializer.GetSize();

            if (size.HasValue)
                _size = size.Value + 1;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => _size;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(T? value)
            => value.HasValue ? (_size ?? _serializer.GetCapacity(value.Value) + 1) : 1;

        public override sealed void Serialize(ref Writer writer, T? value)
        {
            if (value.HasValue)
            {
                writer.WriteBoolTrue();
                _serializer.Serialize(ref writer, value.Value);
            }
            else
            {
                writer.WriteBoolFalse();
            }
        }

        public override sealed void SerializeSpot(ref Writer writer, T? value)
            => throw new NotSupportedException();

        public override sealed T? Deserialize(ref Reader reader, DeserializeOptions options)
        {
            bool hasValue = reader.ReadBool();
            return hasValue ? _serializer.Deserialize(ref reader, options) : null;
        }

        public override sealed T? DeserializeSpot(ref Reader reader, DeserializeOptions options)
            => throw new NotSupportedException();
    }
}
