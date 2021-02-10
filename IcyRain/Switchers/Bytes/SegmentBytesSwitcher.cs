using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class SegmentBytesSwitcher : BytesSwitcher<ArraySegment<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Serialize(ArraySegment<byte> value)
            => value.Count == 0 ? Array.Empty<byte>() : (value.Array.Length == value.Count ? value.Array : value.TransferToArray());

        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Deserialize(byte[] bytes, DeserializeOptions options)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            return new ArraySegment<byte>(bytes);
        }

        [MethodImpl(Flags.HotPath)]
        public override ArraySegment<byte> Deserialize(byte[] bytes, int offset, int count, DeserializeOptions options)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            return new ArraySegment<byte>(bytes, offset, count);
        }

    }
}
