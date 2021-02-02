using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class MemoryBufferSwitcher : BufferSwitcher<Memory<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public unsafe sealed override void Serialize(IBufferWriter<byte> buffer, Memory<byte> value)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            int length = value.Length;

            if (length > 0)
            {
                fixed (byte* ptr = buffer.GetSpan(length))
                fixed (byte* ptrValue = value.Span)
                    Unsafe.CopyBlock(ptr, ptrValue, (uint)length);
            }

            buffer.Advance(length);
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override Memory<byte> Deserialize(in ReadOnlySequence<byte> sequence, DeserializeOptions options)
            => sequence.IsEmpty ? default : new Memory<byte>(sequence.IsSingleSegment ? sequence.First.ToArray() : sequence.TransferToArray());
    }
}
