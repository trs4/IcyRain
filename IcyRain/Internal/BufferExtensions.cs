using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace IcyRain.Internal
{
    internal static class BufferExtensions
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

        [MethodImpl(Flags.HotPath)]
        public static unsafe void WriteTo(this byte[] destination, byte[] buffer, int offset, int count)
        {
            fixed (byte* ptr = destination)
            fixed (byte* ptrValue = buffer)
                Unsafe.CopyBlock(ptr, ptrValue + offset, (uint)count);
        }

        [MethodImpl(Flags.HotPath)]
        public static unsafe void WriteToBuffer(this byte[] source, ref int sourceOffset, byte[] buffer, ref int offset, int count)
        {
            fixed (byte* ptr = buffer)
            fixed (byte* ptrValue = source)
                Unsafe.CopyBlock(ptr + offset, ptrValue + sourceOffset, (uint)count);

            sourceOffset += count;
            offset += count;
        }

        [MethodImpl(Flags.HotPath)]
        public static unsafe void WriteToBuffer(this byte[] source, ref int sourceOffset, Span<byte> buffer)
        {
            fixed (byte* ptr = buffer)
            fixed (byte* ptrValue = source)
                Unsafe.CopyBlock(ptr, ptrValue + sourceOffset, (uint)buffer.Length);

            sourceOffset += buffer.Length;
        }

        [MethodImpl(Flags.HotPath)]
        public static unsafe void WriteToBuffer(this byte[] source, ref int sourceOffset, Memory<byte> buffer)
        {
            fixed (byte* ptr = buffer.Span)
            fixed (byte* ptrValue = source)
                Unsafe.CopyBlock(ptr, ptrValue + sourceOffset, (uint)buffer.Length);

            sourceOffset += buffer.Length;
        }

        public static unsafe void WriteToBuffer(this in ReadOnlySequence<byte> sequence, byte[] buffer, ref int offset)
        {
            if (sequence.IsSingleSegment)
            {
                var span = sequence.First.Span;

                fixed (byte* ptr = buffer)
                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                offset += span.Length;
            }
            else
            {
                WriteToBufferMultiple(sequence, buffer, ref offset);
            }
        }

        private static unsafe void WriteToBufferMultiple(in ReadOnlySequence<byte> sequence, byte[] buffer, ref int offset)
        {
            var position = sequence.Start;

            fixed (byte* ptr = buffer)
            {
                while (sequence.TryGet(ref position, out var memory))
                {
                    var span = memory.Span;

                    fixed (byte* ptrValue = span)
                        Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                    offset += span.Length;

                    if (position.GetObject() is null)
                        break;
                }
            }
        }

        public static unsafe void WriteToBuffer(this in ReadOnlySequence<byte> sequence, Span<byte> buffer, ref int offset)
        {
            if (sequence.IsSingleSegment)
            {
                var span = sequence.First.Span;

                fixed (byte* ptr = buffer)
                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                offset += span.Length;
            }
            else
            {
                WriteToBufferMultiple(sequence, buffer, ref offset);
            }
        }

        private static unsafe void WriteToBufferMultiple(in ReadOnlySequence<byte> sequence, Span<byte> buffer, ref int offset)
        {
            var position = sequence.Start;

            fixed (byte* ptr = buffer)
            {
                while (sequence.TryGet(ref position, out var memory))
                {
                    var span = memory.Span;

                    fixed (byte* ptrValue = span)
                        Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                    offset += span.Length;

                    if (position.GetObject() is null)
                        break;
                }
            }
        }

        public static unsafe void WriteToBuffer(this in ReadOnlySequence<byte> sequence, byte[] buffer)
        {
            if (sequence.IsSingleSegment)
            {
                var span = sequence.First.Span;

                fixed (byte* ptr = buffer)
                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr, ptrValue, (uint)span.Length);
            }
            else
            {
                WriteToBufferMultiple(sequence, buffer);
            }
        }

        private static unsafe void WriteToBufferMultiple(in ReadOnlySequence<byte> sequence, byte[] buffer)
        {
            var position = sequence.Start;
            int offset = 0;

            fixed (byte* ptr = buffer)
            {
                while (sequence.TryGet(ref position, out var memory))
                {
                    var span = memory.Span;

                    fixed (byte* ptrValue = span)
                        Unsafe.CopyBlock(ptr + offset, ptrValue, (uint)span.Length);

                    offset += span.Length;

                    if (position.GetObject() is null)
                        break;
                }
            }
        }

    }
}
