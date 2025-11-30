using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Grpc.Client.Internal.Retry;

namespace IcyRain.Grpc.Client.Internal;

/// <summary>A client-side RPC invocation using HttpClient</summary>
#pragma warning disable CA2000 // Dispose objects before losing scope
[DebuggerDisplay("{Channel}")]
internal sealed class HttpClientCallInvoker : CallInvoker
{
    internal GrpcChannel Channel { get; }

    public HttpClientCallInvoker(GrpcChannel channel)
        => Channel = channel;

    /// <summary>
    /// Invokes a client streaming call asynchronously.
    /// In client streaming scenario, client sends a stream of requests and server responds with a single response
    /// </summary>
    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method, string? host, CallOptions options)
    {
        AssertMethodType(method, MethodType.ClientStreaming);

        var call = CreateRootGrpcCall(Channel, method, options);
        call.StartClientStreaming();

        var callWrapper = new AsyncClientStreamingCall<TRequest, TResponse>(
            requestStream: call.ClientStreamWriter!,
            responseAsync: call.GetResponseAsync(),
            responseHeadersAsync: Callbacks<TRequest, TResponse>.GetResponseHeadersAsync,
            getStatusFunc: Callbacks<TRequest, TResponse>.GetStatus,
            getTrailersFunc: Callbacks<TRequest, TResponse>.GetTrailers,
            disposeAction: Callbacks<TRequest, TResponse>.Dispose,
            call);

        call.CallWrapper = callWrapper;
        return callWrapper;
    }

    /// <summary>
    /// Invokes a duplex streaming call asynchronously.
    /// In duplex streaming scenario, client sends a stream of requests and server responds with a stream of responses.
    /// The response stream is completely independent and both side can be sending messages at the same time
    /// </summary>
    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method, string? host, CallOptions options)
    {
        AssertMethodType(method, MethodType.DuplexStreaming);

        var call = CreateRootGrpcCall(Channel, method, options);
        call.StartDuplexStreaming();

        var callWrapper = new AsyncDuplexStreamingCall<TRequest, TResponse>(
            requestStream: call.ClientStreamWriter!,
            responseStream: call.ClientStreamReader!,
            responseHeadersAsync: Callbacks<TRequest, TResponse>.GetResponseHeadersAsync,
            getStatusFunc: Callbacks<TRequest, TResponse>.GetStatus,
            getTrailersFunc: Callbacks<TRequest, TResponse>.GetTrailers,
            disposeAction: Callbacks<TRequest, TResponse>.Dispose,
            call);

        call.CallWrapper = callWrapper;
        return callWrapper;
    }

    /// <summary>
    /// Invokes a server streaming call asynchronously.
    /// In server streaming scenario, client sends on request and server responds with a stream of responses
    /// </summary>
    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method, string? host, CallOptions options, TRequest request)
    {
        AssertMethodType(method, MethodType.ServerStreaming);

        var call = CreateRootGrpcCall(Channel, method, options);
        call.StartServerStreaming(request);

        var callWrapper = new AsyncServerStreamingCall<TResponse>(
            responseStream: call.ClientStreamReader!,
            responseHeadersAsync: Callbacks<TRequest, TResponse>.GetResponseHeadersAsync,
            getStatusFunc: Callbacks<TRequest, TResponse>.GetStatus,
            getTrailersFunc: Callbacks<TRequest, TResponse>.GetTrailers,
            disposeAction: Callbacks<TRequest, TResponse>.Dispose,
            call);

        call.CallWrapper = callWrapper;
        return callWrapper;
    }

    /// <summary>Invokes a simple remote call asynchronously</summary>
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        Method<TRequest, TResponse> method, string? host, CallOptions options, TRequest request)
    {
        AssertMethodType(method, MethodType.Unary);

        var call = CreateRootGrpcCall(Channel, method, options);
        call.StartUnary(request);

        var callWrapper = new AsyncUnaryCall<TResponse>(
            responseAsync: call.GetResponseAsync(),
            responseHeadersAsync: Callbacks<TRequest, TResponse>.GetResponseHeadersAsync,
            getStatusFunc: Callbacks<TRequest, TResponse>.GetStatus,
            getTrailersFunc: Callbacks<TRequest, TResponse>.GetTrailers,
            disposeAction: Callbacks<TRequest, TResponse>.Dispose,
            call);

        call.CallWrapper = callWrapper;
        return callWrapper;
    }

    [Conditional("ASSERT_METHOD_TYPE")]
    private static void AssertMethodType(IMethod method, MethodType methodType)
    {
        // This can be used to assert tests are passing the right method type.
        if (method.Type != methodType)
            throw new InvalidOperationException("Expected method type: " + methodType);
    }

    /// <summary>Invokes a simple remote call in a blocking fashion</summary>
    public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string? host, CallOptions options, TRequest request)
    {
        var call = AsyncUnaryCall(method, host, options, request);
        return call.ResponseAsync.GetAwaiter().GetResult();
    }

    private static IGrpcCall<TRequest, TResponse> CreateRootGrpcCall<TRequest, TResponse>(
        GrpcChannel channel,
        Method<TRequest, TResponse> method,
        CallOptions options)
        where TRequest : class
        where TResponse : class
    {
        var methodInfo = channel.GetCachedGrpcMethodInfo(method);
        var retryPolicy = methodInfo.MethodConfig?.RetryPolicy;
        var hedgingPolicy = methodInfo.MethodConfig?.HedgingPolicy;

        if (retryPolicy is not null)
            return new RetryCall<TRequest, TResponse>(retryPolicy, channel, method, options);
        else if (hedgingPolicy != null)
            return new HedgingCall<TRequest, TResponse>(hedgingPolicy, channel, method, options);
        else // No retry/hedge policy configured. Fast path! Note that callWrapper is null here and will be set later
            return CreateGrpcCall(channel, method, options, attempt: 1, forceAsyncHttpResponse: false, callWrapper: null);
    }

    public static GrpcCall<TRequest, TResponse> CreateGrpcCall<TRequest, TResponse>(
        GrpcChannel channel,
        Method<TRequest, TResponse> method,
        CallOptions options,
        int attempt,
        bool forceAsyncHttpResponse,
        object? callWrapper)
        where TRequest : class
        where TResponse : class
    {
        ObjectDisposedException.ThrowIf(channel.Disposed, typeof(GrpcChannel));
        var methodInfo = channel.GetCachedGrpcMethodInfo(method);

        return new GrpcCall<TRequest, TResponse>(method, methodInfo, options, channel, attempt, forceAsyncHttpResponse)
        {
            CallWrapper = callWrapper
        };
    }

    // Store static callbacks so delegates are allocated once
    private static class Callbacks<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        internal static readonly Func<object, Task<Metadata>> GetResponseHeadersAsync = state => ((IGrpcCall<TRequest, TResponse>)state).GetResponseHeadersAsync();
        internal static readonly Func<object, Status> GetStatus = state => ((IGrpcCall<TRequest, TResponse>)state).GetStatus();
        internal static readonly Func<object, Metadata> GetTrailers = state => ((IGrpcCall<TRequest, TResponse>)state).GetTrailers();
        internal static readonly Action<object> Dispose = state => ((IGrpcCall<TRequest, TResponse>)state).Dispose();
    }
}
#pragma warning restore CA2000 // Dispose objects before losing scope
