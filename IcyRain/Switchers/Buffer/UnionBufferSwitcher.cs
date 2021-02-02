using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Switchers
{
    internal sealed class UnionBufferSwitcher<T> : BufferSwitcher<T>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override void Serialize(IBufferWriter<byte> buffer, T value)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            var serializer = Serializer<UnionResolver, T>.Instance;
            var writer = new Writer(buffer.GetSpan(serializer.GetCapacity(value)));
            serializer.Serialize(ref writer, value);
            buffer.Advance(writer.Size);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override T Deserialize(in ReadOnlySequence<byte> sequence, DeserializeOptions options)
        {
            var reader = new Reader(sequence);
            return Serializer<UnionResolver, T>.Instance.Deserialize(ref reader, options ?? DeserializeOptions.Default);
        }

    }
}
