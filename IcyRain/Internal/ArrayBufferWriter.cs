using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace IcyRain.Internal
{
    internal sealed class ArrayBufferWriter : IBufferWriter<byte>, IDisposable
    {
        private byte[] _buffer;

        public int Count { get; private set; }

        [MethodImpl(Flags.HotPath)]
        public void Advance(int count) => Count = count;

        public Memory<byte> GetMemory(int sizeHint = 0) => throw new NotSupportedException();

        [MethodImpl(Flags.HotPath)]
        public Span<byte> GetSpan(int sizeHint = 0)
        {
            _buffer = Buffers.Rent(sizeHint);
            return _buffer.AsSpan();
        }

        [MethodImpl(Flags.HotPath)]
        public byte[] ToArray()
        {
            if (_buffer is null)
                return Array.Empty<byte>();
            else if (_buffer.Length == Count)
                return _buffer;

            var result = new byte[Count];
            Buffer.BlockCopy(_buffer, 0, result, 0, Count);
            return result;
        }

        [MethodImpl(Flags.HotPath)]
        public ArraySegment<byte> ToSegment()
            => _buffer is null ? default : new ArraySegment<byte>(_buffer, 0, Count);

        [MethodImpl(Flags.HotPath)]
        public ReadOnlySequence<byte> ToSequence()
            => _buffer is null ? default : new ReadOnlySequence<byte>(_buffer, 0, Count);

        public void Dispose() => Buffers.Return(_buffer);
    }
}
