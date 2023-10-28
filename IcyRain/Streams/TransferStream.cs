using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using IcyRain.Internal;

#pragma warning disable CA1062 // Validate arguments of public methods
namespace IcyRain.Streams;

public abstract class TransferStream : Stream
{
    private long _position;
    private int _offset;
    private StreamPart _part;
    private readonly Action _onDispose;

    protected TransferStream(Action onDispose)
        => _onDispose = onDispose;

    public static TransferStream Create(TransferStreamReader reader, Action onDispose = null)
        => new TransferReaderStream(reader, onDispose);

    #region Stream

    public sealed override bool CanRead => true;

    public sealed override bool CanSeek => false;

    public sealed override bool CanWrite => false;

    public sealed override long Length => throw new NotSupportedException();

    public sealed override long Position
    {
        get => _position;
        set => throw new NotSupportedException();
    }

    public sealed override void Flush() { }

    public sealed override int Read(byte[] buffer, int offset, int count)
    {
        try
        {
            return Task.Run(async () =>
            {
                var task = ReadAsync(buffer, offset, count, CancellationToken.None);
                return task.IsCompleted ? task.Result : await task.ConfigureAwait(false);
            }).Result;
        }
        catch (AggregateException e)
        {
            ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            return default;
        }
    }

    public sealed override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public sealed override void SetLength(long value) => throw new NotSupportedException();

    public sealed override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public sealed override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => throw new NotSupportedException();

    public sealed override void WriteByte(byte value) => throw new NotSupportedException();

#if !NETFRAMEWORK
    public sealed override void Write(ReadOnlySpan<byte> buffer) => throw new NotSupportedException();

    public sealed override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
#endif

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _onDispose?.Invoke();

            _part?.Dispose();
            _part = null;
        }

        base.Dispose(disposing);
    }

    #endregion

    [MethodImpl(Flags.HotPath)]
    protected async Task CopyToCoreAsync(TransferStreamReader reader, Stream destination, CancellationToken cancellationToken)
    {
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));

        while (await reader.MoveNext(cancellationToken).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var part = reader.Current;

#if NETFRAMEWORK
            var task = destination.WriteAsync(part.Buffer, 0, part.BufferSize, cancellationToken);
#else
            var task = destination.WriteAsync(new ReadOnlyMemory<byte>(part.Buffer, 0, part.BufferSize), cancellationToken);
#endif
            if (!task.IsCompleted)
                await task.ConfigureAwait(false);

            _position += part.BufferSize;
        }
    }

    [MethodImpl(Flags.HotPath)]
    protected async Task<int> ReadCoreAsync(TransferStreamReader reader, byte[] buffer, int offset, int count,
        CancellationToken cancellationToken)
    {
        if (buffer is null)
            throw new ArgumentNullException(nameof(buffer));

        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (buffer.Length - offset < count)
            throw new ArgumentException("buffer.Length");

        int readCount = 0;

        while (count > 0)
        {
            if (_part is null)
            {
                if (reader.IsCompleted || !await reader.MoveNext(cancellationToken).ConfigureAwait(false))
                    return readCount;

                _part = reader.Current;
                _offset = 0;
            }

            if (count >= _part.BufferSize)
            {
                _part.Buffer.WriteToBuffer(ref _offset, buffer, ref offset, _part.BufferSize);
                readCount += _part.BufferSize;
                count -= _part.BufferSize;
                _part.Dispose();
                _part = null;
            }
            else
            {
                _part.Buffer.WriteToBuffer(ref _offset, buffer, ref offset, count);
                return readCount + count;
            }
        }

        return readCount;
    }

    [MethodImpl(Flags.HotPath)]
    protected int ReadCore(TransferStreamReader reader, Span<byte> buffer)
    {
        int count = buffer.Length;
        int readCount = 0;

        while (count > 0)
        {
            if (_part is null)
            {
                if (reader.IsCompleted || !Task.Run(async () => await reader.MoveNext(CancellationToken.None).ConfigureAwait(false)).Result)
                    return readCount;

                _part = reader.Current;
                _offset = 0;
            }

            if (count == _part.BufferSize)
            {
                _part.Buffer.WriteToBuffer(ref _offset, buffer);
                readCount += _part.BufferSize;
                _part.Dispose();
                _part = null;
                return readCount;
            }
            else if (count > _part.BufferSize)
            {
                _part.Buffer.WriteToBuffer(ref _offset, buffer.Slice(0, _part.BufferSize));
                buffer = buffer.Slice(_part.BufferSize);
                readCount += _part.BufferSize;
                count -= _part.BufferSize;
                _part.Dispose();
                _part = null;
            }
            else
            {
                _part.Buffer.WriteToBuffer(ref _offset, buffer);
                return readCount + count;
            }
        }

        return readCount;
    }

    [MethodImpl(Flags.HotPath)]
    protected async ValueTask<int> ReadCoreAsync(TransferStreamReader reader, Memory<byte> buffer,
        CancellationToken cancellationToken)
    {
        int count = buffer.Length;
        int readCount = 0;

        while (count > 0)
        {
            if (_part is null)
            {
                if (reader.IsCompleted || !await reader.MoveNext(cancellationToken).ConfigureAwait(false))
                    return readCount;

                _part = reader.Current;
                _offset = 0;
            }

            if (count == _part.BufferSize)
            {
                _part.Buffer.WriteToBuffer(ref _offset, buffer);
                readCount += _part.BufferSize;
                _part.Dispose();
                _part = null;
                return readCount;
            }
            else if (count > _part.BufferSize)
            {
                _part.Buffer.WriteToBuffer(ref _offset, buffer.Slice(0, _part.BufferSize));
                buffer = buffer.Slice(_part.BufferSize);
                readCount += _part.BufferSize;
                count -= _part.BufferSize;
                _part.Dispose();
                _part = null;
            }
            else
            {
                _part.Buffer.WriteToBuffer(ref _offset, buffer);
                return readCount + count;
            }
        }

        return readCount;
    }

}
#pragma warning restore CA1062 // Validate arguments of public methods
