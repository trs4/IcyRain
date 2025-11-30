using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IcyRain.Grpc.Client.Internal.Http;

internal sealed class PushStreamContent<TRequest, TResponse> : HttpContent
    where TRequest : class
    where TResponse : class
{
    private readonly HttpContentClientStreamWriter<TRequest, TResponse> _streamWriter;
    private readonly Func<Stream, Task>? _startCallback;

    public PushStreamContent(HttpContentClientStreamWriter<TRequest, TResponse> streamWriter)
    {
        Headers.ContentType = GrpcProtocolConstants.GrpcContentTypeHeaderValue;
        _streamWriter = streamWriter;
    }

    public PushStreamContent(
        HttpContentClientStreamWriter<TRequest, TResponse> streamWriter,
        Func<Stream, Task>? startCallback) : this(streamWriter)
    {
        _startCallback = startCallback;
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        // Immediately flush request stream to send headers
        // https://github.com/dotnet/corefx/issues/39586#issuecomment-516210081
        await stream.FlushAsync().ConfigureAwait(false);

        if (_startCallback is not null)
            await _startCallback(stream).ConfigureAwait(false);

        // Pass request stream to writer
        _streamWriter.WriteStreamTcs.TrySetResult(stream);

        // Wait for the writer to report it is complete
        await _streamWriter.CompleteTcs.Task.ConfigureAwait(false);
    }

    protected override bool TryComputeLength(out long length)
    {
        // We can't know the length of the content being pushed to the output stream.
        length = -1;
        return false;
    }

    // Hacky. ReadAsStreamAsync does not complete until SerializeToStreamAsync finishes.
    // WARNING: Will run SerializeToStreamAsync again on .NET Framework.
    internal Task PushComplete => ReadAsStreamAsync();

    // Internal for testing.
    internal Task SerializeToStreamAsync(Stream stream)
        => SerializeToStreamAsync(stream, context: null);
}
