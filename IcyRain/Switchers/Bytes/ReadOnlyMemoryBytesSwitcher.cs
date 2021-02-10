using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class ReadOnlyMemoryBytesSwitcher : BytesSwitcher<ReadOnlyMemory<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Serialize(ReadOnlyMemory<byte> value)
            => value.ToArray();

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlyMemory<byte> Deserialize(byte[] bytes, DeserializeOptions options)
            => bytes ?? throw new ArgumentNullException(nameof(bytes));

        [MethodImpl(Flags.HotPath)]
        public override ReadOnlyMemory<byte> Deserialize(byte[] bytes, int offset, int count, DeserializeOptions options)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            return new ReadOnlyMemory<byte>(bytes, offset, count);
        }

    }
}
