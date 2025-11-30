using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IcyRain.Grpc.Client.Internal.Http;

/// <summary>
/// WinHttp doesn't support streaming request data so a length needs to be specified.
/// This HttpContent pre-serializes the payload so it has a length available.
/// The payload is then written directly to the request using specialized context
/// and serializer method
/// </summary>
internal sealed class WinHttpUnaryContent<TRequest, TResponse> : HttpContent
    where TRequest : class
    where TResponse : class
{
    private readonly TRequest _request;
    private readonly Func<TRequest, Stream, Task> _startCallback;
    private readonly GrpcCall<TRequest, TResponse> _call;

    public WinHttpUnaryContent(TRequest request, Func<TRequest, Stream, Task> startCallback, GrpcCall<TRequest, TResponse> call)
    {
        _request = request;
        _startCallback = startCallback;
        _call = call;
        Headers.ContentType = GrpcProtocolConstants.GrpcContentTypeHeaderValue;
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        var writeMessageTask = _startCallback(_request, stream);
        return writeMessageTask.IsCompletedSuccessfully ? Task.CompletedTask : writeMessageTask;
    }

    protected override bool TryComputeLength(out long length)
    {
        // This will serialize the request message again.
        // Consider caching serialized content if it is a problem.
        length = GetPayloadLength();
        return true;
    }

    private int GetPayloadLength()
    {
        var serializationContext = _call.SerializationContext;
        serializationContext.CallOptions = _call.Options;
        serializationContext.Initialize();

        try
        {
            _call.Method.RequestMarshaller.ContextualSerializer(_request, serializationContext);

            return serializationContext.GetWrittenPayload().Length;
        }
        finally
        {
            serializationContext.Reset();
        }
    }

}
