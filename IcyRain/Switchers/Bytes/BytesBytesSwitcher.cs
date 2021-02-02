using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class BytesBytesSwitcher : BytesSwitcher<byte[]>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Serialize(byte[] value) => value;

        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Deserialize(byte[] bytes, DeserializeOptions options) => bytes;
    }
}
