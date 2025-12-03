using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Grpc.Service.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace IcyRain.Grpc.AspNetCore.Internal;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
internal sealed partial class HttpContextServerCallContext : ServerCallContext
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private static readonly AuthContext UnauthenticatedContext = new AuthContext(null, []);
    private string? _peer;
    private Metadata? _requestHeaders;
    private Metadata? _responseTrailers;
    private Status _status;
    private AuthContext? _authContext;
    internal ServerCallDeadlineManager? DeadlineManager;
    private HttpContextSerializationContext? _serializationContext;
    private DefaultDeserializationContext? _deserializationContext;

    internal HttpContextServerCallContext(HttpContext httpContext, MethodOptions options, Type requestType, Type responseType)
    {
        HttpContext = httpContext;
        Options = options;
        RequestType = requestType;
        ResponseType = responseType;
    }

    internal HttpContext HttpContext { get; }

    internal MethodOptions Options { get; }

    internal Type RequestType { get; }

    internal Type ResponseType { get; }

    internal string? ResponseGrpcEncoding { get; private set; }

    internal HttpContextSerializationContext SerializationContext
    {
        get => _serializationContext ??= new HttpContextSerializationContext(this);
    }

    internal DefaultDeserializationContext DeserializationContext
    {
        get => _deserializationContext ??= new DefaultDeserializationContext();
    }

    internal bool HasResponseTrailers => _responseTrailers != null;

    protected override string MethodCore => HttpContext.Request.Path.Value!;

    protected override string HostCore => HttpContext.Request.Host.Value!;

    protected override string PeerCore => _peer ??= BuildPeer(); // Follows the standard at https://github.com/grpc/grpc/blob/master/doc/naming.md

    private string BuildPeer()
    {
        var connection = HttpContext.Connection;

        if (connection.RemoteIpAddress is null)
            return "unknown"; // Match Grpc.Core

        return connection.RemoteIpAddress.AddressFamily switch
        {
            AddressFamily.InterNetwork => $"ipv4:{connection.RemoteIpAddress}:{connection.RemotePort}",
            AddressFamily.InterNetworkV6 => $"ipv6:[{connection.RemoteIpAddress}]:{connection.RemotePort}",
            _ => $"unknown:{connection.RemoteIpAddress}:{connection.RemotePort}",
        };
    }

    protected override DateTime DeadlineCore => DeadlineManager?.Deadline ?? DateTime.MaxValue;

    protected override Metadata RequestHeadersCore
    {
        get
        {
            if (_requestHeaders is null)
            {
                _requestHeaders = [];

                foreach (var header in HttpContext.Request.Headers)
                {
                    if (GrpcProtocolHelpers.ShouldSkipHeader(header.Key))
                        continue;

                    if (header.Key.EndsWith(Metadata.BinaryHeaderSuffix, StringComparison.OrdinalIgnoreCase))
                        _requestHeaders.Add(header.Key, GrpcProtocolHelpers.ParseBinaryHeader(header.Value!));
                    else
                        _requestHeaders.Add(header.Key, header.Value!);
                }
            }

            return _requestHeaders;
        }
    }

    internal Task ProcessHandlerErrorAsync(Exception ex, string method)
    {
        if (DeadlineManager is null)
        {
            ProcessHandlerError(ex, method);
            return Task.CompletedTask;
        }

        // Could have a fast path for no deadline being raised when an error happens,
        // but it isn't worth the complexity.
        return ProcessHandlerErrorAsyncCore(ex, method);
    }

    private async Task ProcessHandlerErrorAsyncCore(Exception ex, string method)
    {
        Debug.Assert(DeadlineManager is not null, "Deadline manager should have been created.");

        if (!DeadlineManager.TrySetCallComplete())
            await DeadlineManager.WaitDeadlineCompleteAsync();

        try
        {
            ProcessHandlerError(ex, method);
        }
        finally
        {
            await DeadlineManager.DisposeAsync();
        }
    }

    private void ProcessHandlerError(Exception ex, string method)
    {
        if (ex is RpcException rpcException)
        {
            // There are two sources of metadata entries on the server-side:
            // 1. serverCallContext.ResponseTrailers
            // 2. trailers in RpcException thrown by user code in server side handler.
            // As metadata allows duplicate keys, the logical thing to do is
            // to just merge trailers from RpcException into serverCallContext.ResponseTrailers.
            foreach (var entry in rpcException.Trailers)
                ResponseTrailers.Add(entry);

            _status = rpcException.Status;
        }
        else
        {
            var message = ErrorMessageHelper.BuildErrorMessage("Exception was thrown by handler.", ex, Options.EnableDetailedErrors);
            _status = new Status(StatusCode.Unknown, message, ex);
        }

        // Don't update trailers if request has exceeded deadline
        if (DeadlineManager is null || !DeadlineManager.IsDeadlineExceededStarted)
            HttpContext.Response.ConsolidateTrailers(this);

        DeadlineManager?.SetCallEnded();
    }

    // If there is a deadline then we need to have our own cancellation token.
    // Deadline will call CompleteAsync, then Reset/Abort. This order means RequestAborted
    // is not raised, so deadlineCts will be triggered instead.
    protected override CancellationToken CancellationTokenCore => DeadlineManager?.CancellationToken ?? HttpContext.RequestAborted;

    protected override Metadata ResponseTrailersCore => _responseTrailers ??= [];

    protected override Status StatusCore
    {
        get => _status;
        set => _status = value;
    }

    internal Task EndCallAsync()
    {
        if (DeadlineManager is null)
        {
            EndCallCore();
            return Task.CompletedTask;
        }
        else if (DeadlineManager.TrySetCallComplete())
        {
            // Fast path when deadline hasn't been raised.
            EndCallCore();
            return DeadlineManager.DisposeAsync().AsTask();
        }

        return EndCallAsyncCore(); // Deadline is exceeded
    }

    private async Task EndCallAsyncCore()
    {
        Debug.Assert(DeadlineManager is not null, "Deadline manager should have been created.");

        try
        {
            // Deadline has started
            await DeadlineManager.WaitDeadlineCompleteAsync();

            EndCallCore();
            DeadlineManager.SetCallEnded();
        }
        finally
        {
            await DeadlineManager.DisposeAsync();
        }
    }

    private void EndCallCore()
    {
        // Don't update trailers if request has exceeded deadline
        if (DeadlineManager is null || !DeadlineManager.IsDeadlineExceededStarted)
            HttpContext.Response.ConsolidateTrailers(this);
    }

    protected override WriteOptions? WriteOptionsCore { get; set; }

    protected override AuthContext AuthContextCore
    {
        get
        {
            if (_authContext is null)
            {
                var clientCertificate = HttpContext.Connection.ClientCertificate;
                _authContext = clientCertificate is null ? UnauthenticatedContext : GrpcProtocolHelpers.CreateAuthContext(clientCertificate);
            }

            return _authContext;
        }
    }

    public ServerCallContext ServerCallContext => this;

    protected override IDictionary<object, object> UserStateCore => HttpContext.Items!;

    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options)
        => throw new NotImplementedException("CreatePropagationToken will be implemented in a future version.");

    protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
    {
        ArgumentNullException.ThrowIfNull(responseHeaders);

        // Headers can only be written once. Throw on subsequent call to write response header instead of silent no-op
        if (HttpContext.Response.HasStarted)
            throw new InvalidOperationException("Response headers can only be sent once per call.");

        foreach (var header in responseHeaders)
        {
            if (header.Key == GrpcProtocolConstants.CompressionRequestAlgorithmHeader)
            {
                // grpc-internal-encoding-request is used in the server to set message compression
                // on a per-call bassis.
                // 'grpc-encoding' is sent even if WriteOptions.Flags = NoCompress. In that situation
                // individual messages will not be written with compression.
                ResponseGrpcEncoding = header.Value;
                HttpContext.Response.Headers[GrpcProtocolConstants.MessageEncodingHeader] = ResponseGrpcEncoding;
            }
            else
            {
                var encodedValue = header.IsBinary ? Convert.ToBase64String(header.ValueBytes) : header.Value;
                try
                {
                    HttpContext.Response.Headers.Append(header.Key, encodedValue);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error adding response header '{header.Key}'.", ex);
                }
            }
        }

        return HttpContext.Response.BodyWriter.FlushAsync().GetAsTask();
    }

    // Clock is for testing
    public void Initialize()
    {
        var timeout = GetTimeout();

        if (timeout != TimeSpan.Zero)
            DeadlineManager = new ServerCallDeadlineManager(this, timeout);
    }

    private TimeSpan GetTimeout()
    {
        if (HttpContext.Request.Headers.TryGetValue(GrpcProtocolConstants.TimeoutHeader, out var values)
            && GrpcProtocolHelpers.TryDecodeTimeout(values, out var timeout) && timeout > TimeSpan.Zero)
        {
            if (timeout.Ticks > GrpcProtocolConstants.MaxDeadlineTicks)
                timeout = TimeSpan.FromTicks(GrpcProtocolConstants.MaxDeadlineTicks);

            return timeout;
        }

        return TimeSpan.Zero;
    }

    internal async Task DeadlineExceededAsync()
    {
        var status = new Status(StatusCode.DeadlineExceeded, "Deadline Exceeded");

        var trailersDestination = GrpcProtocolHelpers.GetTrailersDestination(HttpContext.Response);
        GrpcProtocolHelpers.SetStatus(trailersDestination, status);

        _status = status;

        // Immediately send remaining response content and trailers
        // If feature is null then reset/abort will still end request, but response won't have trailers
        var completionFeature = HttpContext.Features.Get<IHttpResponseBodyFeature>();

        if (completionFeature is not null)
            await completionFeature.CompleteAsync();

        CancelRequest();
    }

    internal void CancelRequest()
    {
        // HttpResetFeature should always be set on context,
        // but in case it isn't, fall back to HttpContext.Abort.
        // Abort will send error code INTERNAL_ERROR.
        var resetFeature = HttpContext.Features.Get<IHttpResetFeature>();

        if (resetFeature is not null)
        {
            var errorCode = GrpcProtocolConstants.GetCancelErrorCode(HttpContext.Request.Protocol);
            resetFeature.Reset(errorCode);
        }
        else
            HttpContext.Abort();
    }

    internal string? GetRequestGrpcEncoding()
        => HttpContext.Request.Headers.TryGetValue(GrpcProtocolConstants.MessageEncodingHeader, out var values) ? values.ToString() : null;

    internal bool IsEncodingInRequestAcceptEncoding(string encoding)
    {
        if (HttpContext.Request.Headers.TryGetValue(GrpcProtocolConstants.MessageAcceptEncodingHeader, out var values))
        {
            var acceptEncoding = values.ToString().AsSpan();

            while (true)
            {
                var separatorIndex = acceptEncoding.IndexOf(',');
                ReadOnlySpan<char> segment;

                if (separatorIndex != -1)
                {
                    segment = acceptEncoding.Slice(0, separatorIndex);
                    acceptEncoding = acceptEncoding.Slice(separatorIndex + 1);
                }
                else
                    segment = acceptEncoding;

                segment = segment.Trim();
                               
                if (segment.SequenceEqual(encoding)) // Check segment
                    return true;

                if (separatorIndex == -1)
                    break;
            }

            if (acceptEncoding.SequenceEqual(encoding)) // Check remainder
                return true;
        }

        return false;
    }

}
