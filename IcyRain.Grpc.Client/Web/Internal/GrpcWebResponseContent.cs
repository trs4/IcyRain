using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IcyRain.Grpc.Service.Internal;

namespace IcyRain.Grpc.Client.Web.Internal;

internal sealed class GrpcWebResponseContent : HttpContent
{
    private readonly HttpContent _inner;
    private readonly GrpcWebMode _mode;
    private readonly HttpHeaders _responseTrailers;
    private Stream? _innerStream;

    public GrpcWebResponseContent(HttpContent inner, GrpcWebMode mode, HttpHeaders responseTrailers)
    {
        _inner = inner;
        _mode = mode;
        _responseTrailers = responseTrailers;

        foreach (var header in inner.Headers)
            Headers.TryAddWithoutValidation(header.Key, header.Value);

        Headers.ContentType = GrpcWebProtocolConstants.GrpcHeader;
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        // This method will only be called by tests when response content is
        // accessed via ReadAsBytesAsync. The gRPC client will always
        // call ReadAsStreamAsync, which will call CreateContentReadStreamAsync
        _innerStream = await _inner.ReadAsStreamAsync().ConfigureAwait(false);

        if (_mode == GrpcWebMode.GrpcWebText)
            _innerStream = new Base64ResponseStream(_innerStream);

        _innerStream = new GrpcWebResponseStream(_innerStream, _responseTrailers);
        await _innerStream.CopyToAsync(stream).ConfigureAwait(false);
    }

#pragma warning disable CA2000 // Dispose objects before losing scope
    protected override async Task<Stream> CreateContentReadStreamAsync()
    {
        var stream = await _inner.ReadAsStreamAsync().ConfigureAwait(false);

        if (_mode == GrpcWebMode.GrpcWebText)
            stream = new Base64ResponseStream(stream);

        return new GrpcWebResponseStream(stream, _responseTrailers); //, _response);
    }
#pragma warning restore CA2000 // Dispose objects before losing scope

    protected override bool TryComputeLength(out long length)
    {
        length = -1;
        return false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // This is important. Disposing original response content will cancel the gRPC call
            _inner.Dispose();
            _innerStream?.Dispose();
        }

        base.Dispose(disposing);
    }

}
