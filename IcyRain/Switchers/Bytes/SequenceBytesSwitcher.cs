using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class SequenceBytesSwitcher : BytesSwitcher<ReadOnlySequence<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Serialize(ReadOnlySequence<byte> value)
            => value.TransferToArray();

        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] SerializeWithLZ4(ReadOnlySequence<byte> value, out int serializedLength)
        {
            byte[] buffer = value.TransferToRentArray();
            serializedLength = buffer.Length;
            byte[] result = LZ4ArrayEncoder.Encode(buffer);

            Buffers.Return(buffer);
            return result;
        }


        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlySequence<byte> Deserialize(byte[] bytes, int offset, int count)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            return new ReadOnlySequence<byte>(bytes, offset, count);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlySequence<byte> DeserializeInUTC(byte[] bytes, int offset, int count)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            return new ReadOnlySequence<byte>(bytes, offset, count);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlySequence<byte> DeserializeWithLZ4(byte[] bytes, int offset, int count, out int decodedLength)
        {
            decodedLength = count;
            byte[] result = LZ4ArrayDecoder.RentDecode(new Span<byte>(bytes, offset, count), ref decodedLength);
            return new ReadOnlySequence<byte>(result, 0, decodedLength);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlySequence<byte> DeserializeInUTCWithLZ4(byte[] bytes, int offset, int count, out int decodedLength)
        {
            decodedLength = count;
            byte[] result = LZ4ArrayDecoder.RentDecode(new Span<byte>(bytes, offset, count), ref decodedLength);
            return new ReadOnlySequence<byte>(result, 0, decodedLength);
        }

    }
}
