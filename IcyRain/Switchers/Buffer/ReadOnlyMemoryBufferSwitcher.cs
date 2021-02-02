using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class ReadOnlyMemoryBufferSwitcher : BufferSwitcher<ReadOnlyMemory<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public unsafe sealed override void Serialize(IBufferWriter<byte> buffer, ReadOnlyMemory<byte> value)
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
        public sealed override ReadOnlyMemory<byte> Deserialize(in ReadOnlySequence<byte> sequence, DeserializeOptions options)
            => sequence.IsSingleSegment ? sequence.First : new Memory<byte>(sequence.TransferToArray());
    }
}
