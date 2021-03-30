using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class MemorySegmentSwitcher : SegmentSwitcher<Memory<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Serialize(Memory<byte> value)
            => new ArraySegment<byte>(value.ToArray());

        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> SerializeWithLZ4(Memory<byte> value, out int serializedLength)
        {
            serializedLength = value.Length;
            return LZ4ArrayCodec.EncodeToSegment(value.Span);
        }


        [MethodImpl(Flags.HotPath)]
        public sealed override Memory<byte> Deserialize(ArraySegment<byte> segment) => segment;

        [MethodImpl(Flags.HotPath)]
        public sealed override Memory<byte> DeserializeInUTC(ArraySegment<byte> segment) => segment;

        [MethodImpl(Flags.HotPath)]
        public sealed override Memory<byte> DeserializeWithLZ4(ArraySegment<byte> segment, out int decodedLength)
        {
            byte[] buffer = segment.TransferToRentArrayWithLZ4Decompress(out decodedLength);
            return new Memory<byte>(buffer, 0, decodedLength);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override Memory<byte> DeserializeInUTCWithLZ4(ArraySegment<byte> segment, out int decodedLength)
        {
            byte[] buffer = segment.TransferToRentArrayWithLZ4Decompress(out decodedLength);
            return new Memory<byte>(buffer, 0, decodedLength);
        }

    }
}
