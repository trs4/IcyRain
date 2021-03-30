using System;
using System.Buffers;

namespace IcyRain.Internal
{
    public sealed class TestBufferWriter : IBufferWriter<byte>
    {
        #region IBufferWriter

        private int _count;
        private byte[] _buffer;

        public void Advance(int count) => _count = count;

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            if (sizeHint <= 0)
                sizeHint = 8192;

            _buffer = new byte[sizeHint];
            return _buffer.AsMemory();
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            if (sizeHint <= 0)
                sizeHint = 8092;

            _buffer = new byte[sizeHint];
            return _buffer.AsSpan();
        }

        #endregion
        #region Get

        public byte[] ToArray()
        {
            if (_buffer.Length == _count)
                return _buffer;

            var result = new byte[_count];
            Buffer.BlockCopy(_buffer, 0, result, 0, _count);
            return result;
        }

        public ArraySegment<byte> ToSegment()
            => new ArraySegment<byte>(_buffer, 0, _count);

        public ReadOnlySequence<byte> ToSequence()
            => new ReadOnlySequence<byte>(_buffer, 0, _count);

        #endregion

        public void Write(byte[] bytes)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            var span = GetSpan(bytes.Length);
            bytes.AsSpan().TryCopyTo(span);
            Advance(bytes.Length);
        }

        public T DeepCloneBuffer<T>(T value, Action<IBufferWriter<byte>, T> serialize, Func<ReadOnlySequence<byte>, T> deserialize)
        {
            serialize(this, value);
            return deserialize(ToSequence());
        }

        public T DeepCloneBuffer<T>(T value, Func<IBufferWriter<byte>, T, int> serialize, Func<ReadOnlySequence<byte>, T> deserialize)
        {
            serialize(this, value);
            return deserialize(ToSequence());
        }

        public T DeepCloneBuffer<T>(T value, Func<IBufferWriter<byte>, T, int> serialize, Func<ReadOnlySequence<byte>, int, T> deserialize)
        {
            serialize(this, value);
            return deserialize(ToSequence(), _count);
        }

        public T DeepCloneBytes<T>(T value, Func<T, byte[]> serialize, Func<byte[], T> deserialize)
        {
            Write(serialize(value));
            return deserialize(ToArray());
        }

        public ReadOnlySequence<byte> DeepCloneSequence(ReadOnlySequence<byte> value, Func<byte[], byte[]> serialize, Func<byte[], byte[]> deserialize)
        {
            Write(serialize(value.First.Span.ToArray()));
            return new ReadOnlySequence<byte>(deserialize(ToArray()));
        }

        public ReadOnlySequence<byte> DeepCloneSequence(ReadOnlySequence<byte> value, Action<IBufferWriter<byte>, byte[]> serialize, Func<ReadOnlySequence<byte>, byte[]> deserialize)
        {
            serialize(this, value.ToArray());
            return new ReadOnlySequence<byte>(deserialize(ToSequence()));
        }

    }
}
