using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class SequenceSegmentSwitcher : SegmentSwitcher<ReadOnlySequence<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Serialize(ReadOnlySequence<byte> value)
        {
            int length = (int)value.Length;

            return length == 0
                ? default
                : new ArraySegment<byte>(value.IsSingleSegment ? value.First.Span.ToArray() : value.TransferToArray());
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlySequence<byte> Deserialize(ArraySegment<byte> segment, DeserializeOptions options)
            => segment.Count == 0 ? default : new ReadOnlySequence<byte>(segment);
    }
}
