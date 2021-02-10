using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class BytesBytesSwitcher : BytesSwitcher<byte[]>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Serialize(byte[] value) => value;

        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Deserialize(byte[] bytes, DeserializeOptions options)
            => bytes ?? throw new ArgumentNullException(nameof(bytes));

        [MethodImpl(Flags.HotPath)]
        public override byte[] Deserialize(byte[] bytes, int offset, int count, DeserializeOptions options)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            if (offset == 0 && bytes.Length == count)
                return bytes;

            var result = new byte[count - offset];
            result.WriteTo(bytes, offset, count);
            return result;
        }

    }
}
