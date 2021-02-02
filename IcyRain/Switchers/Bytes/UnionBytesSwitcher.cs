using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Switchers
{
    internal sealed class UnionBytesSwitcher<T> : BytesSwitcher<T>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Serialize(T value)
        {
            var serializer = Serializer<UnionResolver, T>.Instance;
            var buffer = Buffers.Rent(serializer.GetCapacity(value));
            var writer = new Writer(buffer);
            serializer.Serialize(ref writer, value);
            return Buffers.ToArray(buffer, writer.Size);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override T Deserialize(byte[] bytes, DeserializeOptions options)
        {
            var reader = new Reader(bytes);
            return Serializer<UnionResolver, T>.Instance.Deserialize(ref reader, options ?? DeserializeOptions.Default);
        }

    }
}
