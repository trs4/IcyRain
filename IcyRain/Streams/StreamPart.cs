using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Streams;

/// <summary>System class for transfer stream</summary>
public class StreamPart : IDisposable
{
    private int _bytesRead;
    private bool _disposed;

    /// <summary>Serialize side</summary>
    [MethodImpl(Flags.HotPath)]
    internal StreamPart(Stream stream, int bufferSize = Buffers.StreamPartSize)
    {
        if (stream is null)
        {
            Stream = Stream.Null;
            BufferSize = bufferSize;
            _bytesRead = 0;
        }
        else
        {
            Stream = stream;
            BufferSize = _bytesRead = bufferSize;
        }
    }

    /// <summary>Deserialize size</summary>
    [MethodImpl(Flags.HotPath)]
    internal StreamPart(ReadOnlySequence<byte> sequence)
    {
        BufferSize = (int)sequence.Length;
        Buffer = Buffers.Rent(BufferSize);
        sequence.WriteToBuffer(Buffer); // Copy, because sequence changing on complete deserialization
    }

    public Stream Stream { get; }

    public int BufferSize { get; }

    [MethodImpl(Flags.HotPath)]
    public virtual bool CanRead() => BufferSize == _bytesRead;

    public byte[] Buffer { get; }

    [MethodImpl(Flags.HotPath)]
#if NETFRAMEWORK
    internal virtual unsafe void Serialize(IBufferWriter<byte> buffer)
#else
    internal virtual void Serialize(IBufferWriter<byte> buffer)
#endif
    {
#if NETFRAMEWORK
        byte[] bytes = Buffers.Rent(BufferSize);
        _bytesRead = Stream.Read(bytes, 0, BufferSize);

        if (_bytesRead > 0)
        {
            fixed (byte* ptr = buffer.GetSpan(_bytesRead))
            fixed (byte* ptrValue = bytes)
                Unsafe.CopyBlock(ptr, ptrValue, (uint)_bytesRead);

            Buffers.Return(bytes);
        }
#else
        _bytesRead = Stream.Read(buffer.GetSpan(BufferSize));
#endif
        buffer.Advance(_bytesRead);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        Buffers.Return(Buffer);
        Stream?.Dispose();
        GC.SuppressFinalize(this);
    }

}
