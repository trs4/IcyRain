using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IcyRain.Streams
{
    internal sealed class TransferReaderStream : TransferStream
    {
        private readonly TransferStreamReader _reader;

        public TransferReaderStream(TransferStreamReader reader, Action onDispose)
            : base(onDispose)
            => _reader = reader ?? throw new ArgumentNullException(nameof(reader));

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
}
