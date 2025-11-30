using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IcyRain.Grpc.Client.Internal.Http;

internal sealed class PushUnaryContent<TRequest, TResponse> : HttpContent
    where TRequest : class
    where TResponse : class
{
    private readonly TRequest _request;
    private readonly Func<TRequest, Stream, Task> _startCallback;

    public PushUnaryContent(TRequest request, Func<TRequest, Stream, Task> startCallback)
    {
        _request = request;
        _startCallback = startCallback;
        Headers.ContentType = GrpcProtocolConstants.GrpcContentTypeHeaderValue;
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        var writeMessageTask = _startCallback(_request, stream);
        return writeMessageTask.IsCompletedSuccessfully ? Task.CompletedTask : writeMessageTask;
    }

    protected override bool TryComputeLength(out long length)
    {
        // We can't know the length of the content being pushed to the output stream.
        length = -1;
        return false;
    }

}
