using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Switchers
{
    internal sealed class DefaultBytesSwitcher<T> : BytesSwitcher<T>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Serialize(T value)
        {
            var serializer = Serializer<Resolver, T>.Instance;
            var buffer = Buffers.Rent(serializer.GetCapacity(value));
            var writer = new Writer(buffer);
            serializer.Serialize(ref writer, value);
            return Buffers.ToArray(buffer, writer.Size);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override T Deserialize(byte[] bytes, DeserializeOptions options)
        {
            try
            {
                var reader = new Reader(bytes);
                return Serializer<Resolver, T>.Instance.Deserialize(ref reader, options ?? DeserializeOptions.Default);
            }
            finally
            {
                Buffers.Return(bytes);
            }
        }

        [MethodImpl(Flags.HotPath)]
        public override T Deserialize(byte[] bytes, int offset, int count, DeserializeOptions options)
        {
            try
            {
                var reader = new Reader(new ReadOnlyMemory<byte>(bytes, offset, count));
                return Serializer<Resolver, T>.Instance.Deserialize(ref reader, options ?? DeserializeOptions.Default);
            }
            finally
            {
                Buffers.Return(bytes);
            }
        }

    }
}
