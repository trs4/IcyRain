using System;
using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class SegmentSegmentSwitcher : SegmentSwitcher<ArraySegment<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Serialize(ArraySegment<byte> value) => value;

        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> SerializeWithLZ4(ArraySegment<byte> value, out int serializedLength)
        {
            serializedLength = value.Count;
            return LZ4ArrayCodec.EncodeToSegment(value);
        }


        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Deserialize(ArraySegment<byte> segment) => segment;

        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> DeserializeInUTC(ArraySegment<byte> segment) => segment;

        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> DeserializeWithLZ4(ArraySegment<byte> segment, out int decodedLength)
        {
            byte[] buffer = segment.TransferToRentArrayWithLZ4Decompress(out decodedLength);
            return new ArraySegment<byte>(buffer, 0, decodedLength);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> DeserializeInUTCWithLZ4(ArraySegment<byte> segment, out int decodedLength)
        {
            byte[] buffer = segment.TransferToRentArrayWithLZ4Decompress(out decodedLength);
            return new ArraySegment<byte>(buffer, 0, decodedLength);
        }

    }
}
