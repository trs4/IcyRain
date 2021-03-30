using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class SequenceSegmentSwitcher : SegmentSwitcher<ReadOnlySequence<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Serialize(ReadOnlySequence<byte> value)
            => new ArraySegment<byte>(value.TransferToArray());

        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> SerializeWithLZ4(ReadOnlySequence<byte> value, out int serializedLength)
        {
            byte[] buffer = value.TransferToRentArray();
            serializedLength = buffer.Length;
            var result = LZ4ArrayCodec.EncodeToSegment(buffer);

            Buffers.Return(buffer);
            return result;
        }


        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlySequence<byte> Deserialize(ArraySegment<byte> segment)
            => segment.Count == 0 ? default : new ReadOnlySequence<byte>(segment);

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlySequence<byte> DeserializeInUTC(ArraySegment<byte> segment)
            => segment.Count == 0 ? default : new ReadOnlySequence<byte>(segment);

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlySequence<byte> DeserializeWithLZ4(ArraySegment<byte> segment, out int decodedLength)
        {
            byte[] buffer = segment.TransferToRentArrayWithLZ4Decompress(out decodedLength);
            return new ReadOnlySequence<byte>(buffer, 0, decodedLength);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlySequence<byte> DeserializeInUTCWithLZ4(ArraySegment<byte> segment, out int decodedLength)
        {
            byte[] buffer = segment.TransferToRentArrayWithLZ4Decompress(out decodedLength);
            return new ReadOnlySequence<byte>(buffer, 0, decodedLength);
        }

    }
}
