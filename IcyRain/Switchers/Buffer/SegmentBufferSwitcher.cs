using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class SegmentBufferSwitcher : BufferSwitcher<ArraySegment<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public unsafe sealed override void Serialize(IBufferWriter<byte> buffer, ArraySegment<byte> value)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            if (value.Count > 0)
            {
                fixed (byte* ptr = buffer.GetSpan(value.Count))
                fixed (byte* ptrValue = value.Array)
                    Unsafe.CopyBlock(ptr, ptrValue + value.Offset, (uint)value.Count);
            }

            buffer.Advance(value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override ArraySegment<byte> Deserialize(in ReadOnlySequence<byte> sequence, DeserializeOptions options)
            => sequence.IsEmpty ? default : new ArraySegment<byte>(sequence.TransferToArray());
    }
}
