using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class ReadOnlyMemorySegmentSwitcher : SegmentSwitcher<ReadOnlyMemory<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Serialize(ReadOnlyMemory<byte> value) => new ArraySegment<byte>(value.ToArray());

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlyMemory<byte> Deserialize(ArraySegment<byte> segment, DeserializeOptions options) => segment;
    }
}
