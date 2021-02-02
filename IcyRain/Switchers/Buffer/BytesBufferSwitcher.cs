using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class BytesBufferSwitcher : BufferSwitcher<byte[]>
    {
        [MethodImpl(Flags.HotPath)]
        public unsafe sealed override void Serialize(IBufferWriter<byte> buffer, byte[] value)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length > 0)
            {
                fixed (byte* ptr = buffer.GetSpan(value.Length))
                fixed (byte* ptrValue = value)
                    Unsafe.CopyBlock(ptr, ptrValue, (uint)value.Length);
            }

            buffer.Advance(value.Length);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override byte[] Deserialize(in ReadOnlySequence<byte> sequence, DeserializeOptions options)
            => sequence.TransferToArray();
    }
}
