using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Grpc.Client.Internal.Http;
using IcyRain.Grpc.Service.Internal;

namespace IcyRain.Grpc.Client.Internal;

internal abstract class GrpcCall
{
    private GrpcCallSerializationContext? _serializationContext;
    private DefaultDeserializationContext? _deserializationContext;

    protected Metadata? Trailers { get; set; }

    public bool ResponseFinished { get; protected set; }

    public HttpResponseMessage? HttpResponse { get; protected set; }

    public GrpcCallSerializationContext SerializationContext => _serializationContext ??= new GrpcCallSerializationContext(this);

    public DefaultDeserializationContext DeserializationContext => _deserializationContext ??= new DefaultDeserializationContext();

    public CallOptions Options { get; }

    public GrpcChannel Channel { get; }

    public string? RequestGrpcEncoding { get; internal set; }

    public abstract Task<Status> CallTask { get; }

    public abstract CancellationToken CancellationToken { get; }

    public abstract Type RequestType { get; }

    public abstract Type ResponseType { get; }

    protected GrpcCall(CallOptions options, GrpcChannel channel)
    {
        Options = options;
        Channel = channel;
    }

    public Exception CreateCanceledStatusException(Exception? ex = null)
    {
        var status = CallTask.IsCompletedSuccessfully ? CallTask.Result : new Status(StatusCode.Cancelled, string.Empty, ex);
        return CreateRpcException(status);
    }

    public CancellationToken GetCanceledToken(CancellationToken methodCancellationToken)
    {
        if (methodCancellationToken.IsCancellationRequested)
            return methodCancellationToken;
        else if (Options.CancellationToken.IsCancellationRequested)
            return Options.CancellationToken;
        else if (CancellationToken.IsCancellationRequested)
            return CancellationToken;

        return CancellationToken.None;
    }

    internal RpcException CreateRpcException(Status status)
    {
        // This code can be called from a background task.
        // If an error is thrown when parsing the trailers into a Metadata
        // collection then the background task will never complete and
        // the gRPC call will hang. If the trailers are invalid then log
        // an error message and return an empty trailers collection
        // on the RpcException that we want to return to the app.
        Metadata? trailers = null;

        try
        {
            TryGetTrailers(out trailers);
        }
        catch { }

        return new RpcException(status, trailers ?? Metadata.Empty);
    }

    public Exception CreateFailureStatusException(Status status)
    {
        if (Channel.ThrowOperationCanceledOnCancellation
            && (status.StatusCode == StatusCode.DeadlineExceeded || status.StatusCode == StatusCode.Cancelled))
        {
            // Convert status response of DeadlineExceeded to OperationCanceledException when
            // ThrowOperationCanceledOnCancellation is true.
            // This avoids a race between the client-side timer and the server status throwing different
            // errors on deadline exceeded.
            return new OperationCanceledException();
        }

        return CreateRpcException(status);
    }

    protected bool TryGetTrailers([NotNullWhen(true)] out Metadata? trailers)
    {
        if (Trailers is null)
        {
            // Trailers are read from the end of the request.
            // If the request isn't finished then we can't get the trailers.
            if (!ResponseFinished)
            {
                trailers = null;
                return false;
            }

            Debug.Assert(HttpResponse is not null);
            Trailers = GrpcProtocolHelpers.BuildMetadata(HttpResponse.TrailingHeaders);
        }

        trailers = Trailers;
        return true;
    }

    internal static Status? ValidateHeaders(HttpResponseMessage httpResponse, out Metadata? trailers)
    {
        // gRPC status can be returned in the header when there is no message (e.g. unimplemented status)
        // An explicitly specified status header has priority over other failing statuses
        if (GrpcProtocolHelpers.TryGetStatusCore(httpResponse.Headers, out var status))
        {
            // Trailers are in the header because there is no message.
            // Note that some default headers will end up in the trailers (e.g. Date, Server).
            trailers = GrpcProtocolHelpers.BuildMetadata(httpResponse.Headers);
            return status;
        }

        trailers = null;

        // ALPN negotiation is sending HTTP/1.1 and HTTP/2
        // Check that the response wasn't downgraded to HTTP/1.1
        if (httpResponse.Version < GrpcProtocolConstants.Http2Version)
            return new Status(StatusCode.Internal, $"Bad gRPC response. Response protocol downgraded to HTTP/{httpResponse.Version.ToString(2)}");

        if (httpResponse.StatusCode != HttpStatusCode.OK)
        {
            var statusCode = MapHttpStatusToGrpcCode(httpResponse.StatusCode);
            return new Status(statusCode, "Bad gRPC response. HTTP status code: " + (int)httpResponse.StatusCode);
        }

        // Don't access Headers.ContentType property because it is not threadsafe
        var contentType = HttpRequestHelpers.GetHeaderValue(httpResponse.Content?.Headers, "Content-Type");

        if (contentType is null)
            return new Status(StatusCode.Cancelled, "Bad gRPC response. Response did not have a content-type header");

        if (!CommonGrpcProtocolHelpers.IsContentType(GrpcProtocolConstants.GrpcContentType, contentType))
            return new Status(StatusCode.Cancelled, "Bad gRPC response. Invalid content-type value: " + contentType);

        return null; // Call is still in progress
    }

    private static StatusCode MapHttpStatusToGrpcCode(HttpStatusCode httpStatusCode) => httpStatusCode switch
    {
        // 400
        HttpStatusCode.BadRequest or HttpStatusCode.RequestHeaderFieldsTooLarge => StatusCode.Internal,
        // 401
        HttpStatusCode.Unauthorized => StatusCode.Unauthenticated,
        // 403
        HttpStatusCode.Forbidden => StatusCode.PermissionDenied,
        // 404
        HttpStatusCode.NotFound => StatusCode.Unimplemented,
        // 429
        HttpStatusCode.TooManyRequests or HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout => StatusCode.Unavailable,
        _ => ((int)httpStatusCode >= 100 && (int)httpStatusCode < 200) ? StatusCode.Internal : StatusCode.Unknown, // 100
    };

}
