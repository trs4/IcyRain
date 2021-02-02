using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Switchers
{
    internal sealed class DefaultSegmentSwitcher<T> : SegmentSwitcher<T>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Serialize(T value)
        {
            var serializer = Serializer<Resolver, T>.Instance;
            var buffer = Buffers.Rent(serializer.GetCapacity(value));
            var writer = new Writer(buffer);
            serializer.Serialize(ref writer, value);
            return new ArraySegment<byte>(buffer, 0, writer.Size);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override T Deserialize(ArraySegment<byte> segment, DeserializeOptions options)
        {
            var reader = new Reader(segment);
            return Serializer<Resolver, T>.Instance.Deserialize(ref reader, options ?? DeserializeOptions.Default);
        }

    }
}
