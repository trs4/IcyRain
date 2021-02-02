using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class BytesSegmentSwitcher : SegmentSwitcher<byte[]>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Serialize(byte[] value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            return new ArraySegment<byte>(value);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Deserialize(ArraySegment<byte> segment, DeserializeOptions options)
            => segment.Count == 0 ? Array.Empty<byte>() : (segment.Array.Length == segment.Count ? segment.Array : segment.TransferToArray());
    }
}
