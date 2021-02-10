using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IcyRain.Internal;

#pragma warning disable CA1062 // Validate arguments of public methods
namespace IcyRain.Streams
{
    public static class StreamTransmitter
    {
        public static async Task SendAsync(TransferStreamWriter writer, Stream stream,
            int bufferSize = Buffers.StreamPartSize, CancellationToken cancellationToken = default)
        {
            if (stream is null)
                return;

            if (stream.CanSeek && stream.Position != 0)
                stream.Seek(0L, SeekOrigin.Begin);

            using var part = new StreamPart(stream, bufferSize);

            while (part.CanRead())
            {
                cancellationToken.ThrowIfCancellationRequested();
                await writer.WriteAsync(part).ConfigureAwait(false);
            }
        }

        public static async Task SendAsync<T>(TransferStreamDataWriter<T> writer, T data, Stream stream,
            int bufferSize = Buffers.StreamPartSize, CancellationToken cancellationToken = default)
        {
            if (stream is not null && stream.CanSeek && stream.Position != 0)
                stream.Seek(0L, SeekOrigin.Begin);

            using var part = new StreamDataPart<T>(data, stream, bufferSize);

            while (part.CanRead())
            {
                cancellationToken.ThrowIfCancellationRequested();
                await writer.WriteAsync(part).ConfigureAwait(false);
            }
        }

    }
}
#pragma warning restore CA1062 // Validate arguments of public methods
