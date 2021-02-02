using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace IcyRain.Internal
{
    internal static class ReadOnlySequenceExtensions
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static unsafe byte[] TransferToArray(this in ArraySegment<byte> segment)
        {
            byte[] bytes = new byte[segment.Count];

            fixed (byte* ptr = bytes)
            fixed (byte* ptrSegment = segment.Array)
                Unsafe.CopyBlock(ptr, ptrSegment + segment.Offset, (uint)segment.Count);

            return bytes;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static unsafe byte[] TransferToArray(this in ReadOnlySequence<byte> value)
        {
            if (value.IsEmpty)
                return Array.Empty<byte>();

            byte[] result = new byte[value.Length];
            int offset = 0;
            var position = value.Start;

            fixed (byte* ptr = result)
            {
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

            return result;
        }

    }
}
