using System;
using System.Threading.Tasks;
using IcyRain.Grpc.Service.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IcyRain.Grpc.AspNetCore.Web.Internal;

internal sealed partial class GrpcWebMiddleware
{
    private readonly GrpcWebOptions _options;
    private readonly ILogger<GrpcWebMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GrpcWebMiddleware(IOptions<GrpcWebOptions> options, ILogger<GrpcWebMiddleware> logger, RequestDelegate next)
    {
        _options = options.Value;
        _logger = logger;
        _next = next;
    }

    public Task Invoke(HttpContext httpContext)
    {
        var grcpWebContext = GetGrpcWebContext(httpContext);

        if (grcpWebContext.Request != ServerGrpcWebMode.None && _options.DefaultEnabled)
            return HandleGrpcWebRequest(httpContext, grcpWebContext);

        return _next(httpContext);
    }

    private async Task HandleGrpcWebRequest(HttpContext httpContext, ServerGrpcWebContext grcpWebContext)
    {
        var feature = new GrpcWebFeature(grcpWebContext, httpContext);

        var initialProtocol = httpContext.Request.Protocol;

        // Modifying the request is required to stop Grpc.AspNetCore.Server from rejecting it
        httpContext.Request.Protocol = HttpProtocol.Http2;
        httpContext.Request.ContentType = ResolveContentType(GrpcWebProtocolConstants.GrpcContentType, httpContext.Request.ContentType!);

        // Update response content type back to gRPC-Web
        httpContext.Response.OnStarting(() =>
        {
            // Reset request protocol back to its original value. Not doing this causes a 2 second
            // delay when making HTTP/1.1 calls.
            httpContext.Request.Protocol = initialProtocol;

            if (CommonGrpcProtocolHelpers.IsContentType(GrpcWebProtocolConstants.GrpcContentType, httpContext.Response.ContentType!))
            {
                var contentType = grcpWebContext.Response == ServerGrpcWebMode.GrpcWeb
                    ? GrpcWebProtocolConstants.GrpcWebContentType
                    : GrpcWebProtocolConstants.GrpcWebTextContentType;

                var responseContentType = ResolveContentType(contentType, httpContext.Response.ContentType!);
                httpContext.Response.ContentType = responseContentType;
            }

            return Task.CompletedTask;
        });

        try
        {
            await _next(httpContext);

            // If trailers have already been written in CompleteAsync then this will no-op
            await feature.WriteTrailersAsync();
        }
        finally
        {
            feature.DetachFromContext(httpContext);
        }
    }

    private static string ResolveContentType(string newContentType, string originalContentType)
    {
        var contentSuffixIndex = originalContentType.IndexOf('+', StringComparison.Ordinal);

        if (contentSuffixIndex != -1)
            newContentType += originalContentType.Substring(contentSuffixIndex);

        return newContentType;
    }

    internal static ServerGrpcWebContext GetGrpcWebContext(HttpContext httpContext)
    {
        // gRPC requests are always POST
        if (!HttpMethods.IsPost(httpContext.Request.Method))
            return default;

        // Only run middleware for 'application/grpc-web' or 'application/grpc-web-text'
        if (!TryGetWebMode(httpContext.Request.ContentType, out var requestMode))
            return default;

        if (TryGetWebMode(httpContext.Request.Headers.Accept, out var responseMode))
        {
            // gRPC-Web request and response types are typically the same
            // That means 'application/grpc-web-text' requests also have an 'accept' header value of 'application/grpc-web-text'
            return new ServerGrpcWebContext(requestMode, responseMode);
        }
        else
        {
            // If there isn't a request 'accept' header then default to mode to 'application/grpc`
            return new ServerGrpcWebContext(requestMode, ServerGrpcWebMode.GrpcWeb);
        }
    }

    private static bool TryGetWebMode(string? contentType, out ServerGrpcWebMode mode)
    {
        if (!string.IsNullOrEmpty(contentType))
        {
            if (CommonGrpcProtocolHelpers.IsContentType(GrpcWebProtocolConstants.GrpcWebContentType, contentType))
            {
                mode = ServerGrpcWebMode.GrpcWeb;
                return true;
            }
            else if (CommonGrpcProtocolHelpers.IsContentType(GrpcWebProtocolConstants.GrpcWebTextContentType, contentType))
            {
                mode = ServerGrpcWebMode.GrpcWebText;
                return true;
            }
        }

        mode = ServerGrpcWebMode.None;
        return false;
    }

}
