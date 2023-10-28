using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IcyRain.Streams;

internal sealed class TransferDataReaderStream<T> : TransferDataStream<T>
{
    private readonly TransferStreamDataReader<T> _reader;

    public TransferDataReaderStream(TransferStreamDataReader<T> reader, Action onDispose)
        : base(reader.Data, onDispose)
        => _reader = reader;

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        => CopyToCoreAsync(_reader, destination, cancellationToken);

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => ReadCoreAsync(_reader, buffer, offset, count, cancellationToken);

#if !NETFRAMEWORK
    public sealed override int Read(Span<byte> buffer)
        => ReadCore(_reader, buffer);

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        => ReadCoreAsync(_reader, buffer, cancellationToken);
#endif
}
