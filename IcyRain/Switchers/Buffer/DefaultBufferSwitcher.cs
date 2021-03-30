using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Switchers
{
    internal sealed class DefaultBufferSwitcher<T> : BufferSwitcher<T>
    {
        private readonly int? _size;

        public DefaultBufferSwitcher()
            => _size = Serializer<Resolver, T>.Instance.GetSize();

        [MethodImpl(Flags.HotPath)]
        public sealed override int Serialize(IBufferWriter<byte> buffer, T value)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            var serializer = Serializer<Resolver, T>.Instance;
            int capacity = _size ?? serializer.GetCapacity(value);

            var writer = new Writer(buffer.GetSpan(capacity));
            serializer.Serialize(ref writer, value);
            buffer.Advance(writer.Size);
            return writer.Size;
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override int SerializeWithLZ4(IBufferWriter<byte> buffer, T value, out int serializedLength)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            var serializer = Serializer<Resolver, T>.Instance;
            int capacity = _size ?? serializer.GetCapacity(value);

            var writer = new Writer(buffer.GetSpan(capacity + 1), true);
            serializer.Serialize(ref writer, value);
            serializedLength = writer.Size;

            if (serializedLength > Buffers.MinCompressSize)
                writer.CompressLZ4();

            buffer.Advance(writer.Size);
            return writer.Size;
        }


        [MethodImpl(Flags.HotPath)]
        public sealed override T Deserialize(in ReadOnlySequence<byte> sequence)
        {
            var reader = new Reader(sequence);
            return Serializer<Resolver, T>.Instance.Deserialize(ref reader);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override T DeserializeInUTC(in ReadOnlySequence<byte> sequence)
        {
            var reader = new Reader(sequence);
            return Serializer<Resolver, T>.Instance.DeserializeInUTC(ref reader);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override T DeserializeWithLZ4(in ReadOnlySequence<byte> sequence, int sequenceLength, out int decodedLength)
        {
            Reader reader;
            decodedLength = sequenceLength;

            if (sequenceLength == 0)
                return default;

            if (sequence.First.Span[0] == 0) // No compress
            {
                reader = new Reader(sequence, true);
                return Serializer<Resolver, T>.Instance.Deserialize(ref reader);
            }

            byte[] buffer = Buffers.Rent(sequenceLength);
            sequence.WriteToBuffer(buffer);

            var (memory, targetBuffer) = LZ4Codec.Decode(buffer, ref decodedLength);
            reader = new Reader(memory);

            try
            {
                return Serializer<Resolver, T>.Instance.Deserialize(ref reader);
            }
            finally
            {
                Buffers.Return(targetBuffer);
                Buffers.Return(buffer);
            }
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override T DeserializeInUTCWithLZ4(in ReadOnlySequence<byte> sequence, int sequenceLength, out int decodedLength)
        {
            Reader reader;
            decodedLength = sequenceLength;

            if (sequenceLength == 0)
                return default;

            if (sequence.First.Span[0] == 0) // No compress
            {
                reader = new Reader(sequence, true);
                return Serializer<Resolver, T>.Instance.DeserializeInUTC(ref reader);
            }

            byte[] buffer = Buffers.Rent(sequenceLength);
            sequence.WriteToBuffer(buffer);

            var (memory, targetBuffer) = LZ4Codec.Decode(buffer, ref decodedLength);
            reader = new Reader(memory);

            try
            {
                return Serializer<Resolver, T>.Instance.DeserializeInUTC(ref reader);
            }
            finally
            {
                Buffers.Return(targetBuffer);
                Buffers.Return(buffer);
            }
        }

    }
}
