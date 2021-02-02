using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class SegmentSegmentSwitcher : SegmentSwitcher<ArraySegment<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Serialize(ArraySegment<byte> value) => value;

        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Deserialize(ArraySegment<byte> segment, DeserializeOptions options) => segment;
    }
}
