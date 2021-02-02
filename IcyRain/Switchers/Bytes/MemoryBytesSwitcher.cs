using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class MemoryBytesSwitcher : BytesSwitcher<Memory<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Serialize(Memory<byte> value)
            => value.ToArray();

        [MethodImpl(Flags.HotPath)]
        public sealed override Memory<byte> Deserialize(byte[] bytes, DeserializeOptions options)
            => bytes ?? throw new ArgumentNullException(nameof(bytes));
    }
}
