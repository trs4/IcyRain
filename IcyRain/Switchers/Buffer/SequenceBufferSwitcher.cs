using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    internal sealed class SequenceBufferSwitcher : BufferSwitcher<ReadOnlySequence<byte>>
    {
        [MethodImpl(Flags.HotPath)]
        public unsafe sealed override void Serialize(IBufferWriter<byte> buffer, ReadOnlySequence<byte> value)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            int length = (int)value.Length;

            if (length > 0)
            {
                fixed (byte* ptr = buffer.GetSpan(length))
                {
                    if (value.IsSingleSegment)
                    {
                        fixed (byte* ptrValue = value.First.Span)
                            Unsafe.CopyBlock(ptr, ptrValue, (uint)length);
                    }
                    else
                    {
                        WriteReadOnlySequenceMultiple(ptr, value);
                    }
                }
            }

            buffer.Advance(length);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static unsafe void WriteReadOnlySequenceMultiple(byte* ptr, in ReadOnlySequence<byte> value)
        {
            int offset = 0;
            var position = value.Start;

            while (value.TryGet(ref position, out var memory))
            {
                var span = memory.Span;

                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                offset += span.Length;

                if (position.GetObject() is null)
                    break;
            }
        }

        [MethodImpl(Flags.HotPath)]
        public sealed override ReadOnlySequence<byte> Deserialize(in ReadOnlySequence<byte> sequence, DeserializeOptions options)
            => sequence;
    }
}
