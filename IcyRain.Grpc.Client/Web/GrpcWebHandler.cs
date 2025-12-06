using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IcyRain.Grpc.Client.Web.Internal;
using IcyRain.Grpc.Service.Internal;
using OperatingSystem = IcyRain.Grpc.Client.Internal.OperatingSystem;

namespace IcyRain.Grpc.Client.Web;

/// <summary>A <see cref="DelegatingHandler"/> implementation that executes gRPC-Web request processing</summary>
/// <remarks><para>This message handler implementation should be used with the .NET client for gRPC to make gRPC-Web calls</para></remarks>
public sealed class GrpcWebHandler : DelegatingHandler
{
    internal const string WebAssemblyEnableStreamingResponseKey = "WebAssemblyEnableStreamingResponse";

    /// <summary>
    /// Gets or sets the gRPC-Web mode to use when making gRPC-Web calls
    /// <para>
    /// When <see cref="GrpcWebMode.GrpcWeb"/>, gRPC-Web calls are made with the <c>application/grpc-web</c> content type,
    /// and binary gRPC messages are sent and received
    /// </para>
    /// <para>
    /// When <see cref="GrpcWebMode.GrpcWebText"/>, gRPC-Web calls are made with the <c>application/grpc-web-text</c> content type,
    /// and base64 encoded gRPC messages are sent and received. Base64 encoding is required for gRPC-Web server streaming calls
    /// to stream correctly in browser apps
    /// </para>
    /// </summary>
    public GrpcWebMode GrpcWebMode { get; set; }

    /// <summary>Creates a new instance of <see cref="GrpcWebHandler"/></summary>
    public GrpcWebHandler() { }

    /// <summary>Creates a new instance of <see cref="GrpcWebHandler"/></summary>
    /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages</param>
    public GrpcWebHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    /// <summary>Creates a new instance of <see cref="GrpcWebHandler"/></summary>
    /// <param name="mode">The gRPC-Web mode to use when making gRPC-Web calls</param>
    public GrpcWebHandler(GrpcWebMode mode)
        => GrpcWebMode = mode;

    /// <summary>Creates a new instance of <see cref="GrpcWebHandler"/></summary>
    /// <param name="mode">The gRPC-Web mode to use when making gRPC-Web calls</param>
    /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages</param>
    public GrpcWebHandler(GrpcWebMode mode, HttpMessageHandler innerHandler)
        : base(innerHandler)
        => GrpcWebMode = mode;

    /// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation</summary>
    /// <param name="request">The HTTP request message to send to the server</param>
    /// <param name="token">A cancellation token to cancel operation</param>
    /// <returns>The task object representing the asynchronous operation</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        if (CommonGrpcProtocolHelpers.IsContentType(GrpcWebProtocolConstants.GrpcContentType, request.Content?.Headers.ContentType?.MediaType))
            return SendAsyncCore(request, token);

        return base.SendAsync(request, token);
    }

    private async Task<HttpResponseMessage> SendAsyncCore(HttpRequestMessage request, CancellationToken token)
    {
        request.Content = new GrpcWebRequestContent(request.Content!, GrpcWebMode);

        // https://github.com/grpc/grpc/blob/f8a5022a2629e0929eb30e0583af66f0c220791b/doc/PROTOCOL-WEB.md
        // The client library should indicate to the server via the "Accept" header that the response stream
        // needs to be text encoded e.g. when XHR is used or due to security policies with XHR
        if (GrpcWebMode == GrpcWebMode.GrpcWebText)
            request.Headers.TryAddWithoutValidation("Accept", GrpcWebProtocolConstants.GrpcWebTextContentType);

        if (OperatingSystem.Instance.IsBrowser)
            FixBrowserUserAgent(request);

        // Set WebAssemblyEnableStreamingResponse to true on gRPC-Web request
        // https://github.com/mono/mono/blob/a0d69a4e876834412ba676f544d447ec331e7c01/sdks/wasm/framework/src/System.Net.Http.WebAssemblyHttpHandler/WebAssemblyHttpHandler.cs#L149
        //
        // This must be set so WASM will stream the response. Without this setting the WASM HTTP handler will only
        // return content once the entire response has been downloaded. This breaks server streaming
        //
        // https://github.com/mono/mono/issues/18718
        request.SetOption(WebAssemblyEnableStreamingResponseKey, true);

        if (request.RequestUri?.Scheme == Uri.UriSchemeHttps
            && request.VersionPolicy == HttpVersionPolicy.RequestVersionExact
            && request.Version == System.Net.HttpVersion.Version20)
        {
            // If no explicit HttpVersion is set and the request is using TLS then change the version policy
            // to allow for HTTP/1.1. HttpVersionPolicy.RequestVersionOrLower it will be compatible
            // with HTTP/1.1 and HTTP/2
            request.VersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
        }

        var response = await base.SendAsync(request, token).ConfigureAwait(false);

        if (response.Content is not null && IsMatchingResponseContentType(GrpcWebMode, response.Content.Headers.ContentType?.MediaType))
            response.Content = new GrpcWebResponseContent(response.Content, GrpcWebMode, response.TrailingHeaders);

        // The gRPC client validates HTTP version 2.0 and will error if it isn't. Always set
        // the version to 2.0, even for non-gRPC content type. The client maps HTTP status codes
        // to gRPC statuses, e.g. HTTP 404 -> gRPC unimplemented
        //
        // Note: Some handlers don't correctly set HttpResponseMessage.Version.
        // We can't rely on it being set correctly. It is safest to always set it to 2.0
        response.Version = GrpcWebProtocolConstants.Http2Version;

        return response;
    }

    private static void FixBrowserUserAgent(HttpRequestMessage request)
    {
        const string userAgentHeader = "User-Agent";

        // Remove the user-agent header and re-add it as x-user-agent
        // We don't want to override the browser's user-agent value
        // Consistent with grpc-web JS client which sends its header in x-user-agent
        // https://github.com/grpc/grpc-web/blob/2e3e8d2c501c4ddce5406ac24a637003eabae4cf/javascript/net/grpc/web/grpcwebclientbase.js#L323
        if (request.Headers.TryGetValues(userAgentHeader, out var values))
        {
            request.Headers.Remove(userAgentHeader);
            request.Headers.TryAddWithoutValidation("X-User-Agent", values);
        }
    }

    private static bool IsMatchingResponseContentType(GrpcWebMode mode, string? contentType)
    {
        if (mode == GrpcWebMode.GrpcWeb)
            return CommonGrpcProtocolHelpers.IsContentType(GrpcWebProtocolConstants.GrpcWebContentType, contentType);

        if (mode == GrpcWebMode.GrpcWebText)
            return CommonGrpcProtocolHelpers.IsContentType(GrpcWebProtocolConstants.GrpcWebTextContentType, contentType);

        return false;
    }

}

