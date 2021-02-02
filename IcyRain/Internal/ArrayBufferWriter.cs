using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace IcyRain.Internal
{
    internal sealed class ArrayBufferWriter : IBufferWriter<byte>, IDisposable
    {
        private int _count;
        private byte[] _buffer;

        [MethodImpl(Flags.HotPath)]
        public void Advance(int count) => _count = count;

        [MethodImpl(Flags.HotPath)]
        public Memory<byte> GetMemory(int sizeHint = 0) => throw new NotSupportedException();

        [MethodImpl(Flags.HotPath)]
        public Span<byte> GetSpan(int sizeHint = 0)
        {
            _buffer = Buffers.MinArrayLength > sizeHint ? new byte[sizeHint] : ArrayPool<byte>.Shared.Rent(sizeHint);
            return _buffer.AsSpan();
        }

        [MethodImpl(Flags.HotPath)]
        public byte[] ToArray()
        {
            if (_buffer is null)
                return Array.Empty<byte>();
            else if (_buffer.Length == _count)
                return _buffer;

            var result = new byte[_count];
            Buffer.BlockCopy(_buffer, 0, result, 0, _count);
            return result;
        }

        [MethodImpl(Flags.HotPath)]
        public ArraySegment<byte> ToSegment()
            => _buffer is null ? default : new ArraySegment<byte>(_buffer, 0, _count);

        [MethodImpl(Flags.HotPath)]
        public ReadOnlySequence<byte> ToSequence()
            => _buffer is null ? default : new ReadOnlySequence<byte>(_buffer, 0, _count);

        public void Dispose()
        {
            if (_buffer is not null && _buffer.Length >= Buffers.MinArrayLength)
                ArrayPool<byte>.Shared.Return(_buffer);
        }

    }
}
