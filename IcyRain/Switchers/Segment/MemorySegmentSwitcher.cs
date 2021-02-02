using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class MemorySegmentSwitcher : SegmentSwitcher<Memory<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Serialize(Memory<byte> value) => new ArraySegment<byte>(value.ToArray());

        [MethodImpl(Flags.HotPath)]
        public sealed override Memory<byte> Deserialize(ArraySegment<byte> segment, DeserializeOptions options) => segment;
    }
}
