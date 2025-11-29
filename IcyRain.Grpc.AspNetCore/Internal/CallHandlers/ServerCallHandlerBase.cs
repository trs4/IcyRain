using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.Net.Http.Headers;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal abstract class ServerCallHandlerBase<[DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)] TService, TRequest, TResponse>
    where TService : class
    where TRequest : class
    where TResponse : class
{
    protected ServerCallHandlerBase(ServerMethodInvokerBase<TService, TRequest, TResponse> methodInvoker)
        => MethodInvoker = methodInvoker;

    protected ServerMethodInvokerBase<TService, TRequest, TResponse> MethodInvoker { get; }

    public Task HandleCallAsync(HttpContext httpContext)
    {
        if (GrpcProtocolHelpers.IsInvalidContentType(httpContext, out var error))
            return ServerCallHandlerBase<TService, TRequest, TResponse>.ProcessInvalidContentTypeRequest(httpContext, error);

        if (!GrpcProtocolConstants.IsHttp2(httpContext.Request.Protocol) && !GrpcProtocolConstants.IsHttp3(httpContext.Request.Protocol))
            return ServerCallHandlerBase<TService, TRequest, TResponse>.ProcessNonHttp2Request(httpContext);

        var serverCallContext = new HttpContextServerCallContext(httpContext, MethodInvoker.Options, typeof(TRequest), typeof(TResponse));
        GrpcProtocolHelpers.AddProtocolHeaders(httpContext.Response);

        try
        {
            serverCallContext.Initialize();

            var handleCallTask = HandleCallAsyncCore(httpContext, serverCallContext);

            if (handleCallTask.IsCompletedSuccessfully)
                return serverCallContext.EndCallAsync();
            else
                return AwaitHandleCall(serverCallContext, MethodInvoker.Method, handleCallTask);
        }
        catch (Exception ex)
        {
            return serverCallContext.ProcessHandlerErrorAsync(ex, MethodInvoker.Method.Name);
        }

        static async Task AwaitHandleCall(HttpContextServerCallContext serverCallContext, Method<TRequest, TResponse> method, Task handleCall)
        {
            try
            {
                await handleCall;
                await serverCallContext.EndCallAsync();
            }
            catch (Exception ex)
            {
                await serverCallContext.ProcessHandlerErrorAsync(ex, method.Name);
            }
        }
    }

    protected abstract Task HandleCallAsyncCore(HttpContext httpContext, HttpContextServerCallContext serverCallContext);

    protected static void DisableMinRequestBodyDataRateAndMaxRequestBodySize(HttpContext httpContext)
    {
        var minRequestBodyDataRateFeature = httpContext.Features.Get<IHttpMinRequestBodyDataRateFeature>();
        minRequestBodyDataRateFeature?.MinDataRate = null;

        var maxRequestBodySizeFeature = httpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();

        if (maxRequestBodySizeFeature is not null && !maxRequestBodySizeFeature.IsReadOnly)
            maxRequestBodySizeFeature.MaxRequestBodySize = null;
    }

    protected static void DisableRequestTimeout(HttpContext httpContext)
    {
        // Disable global request timeout on streaming methods.
        var requestTimeoutFeature = httpContext.Features.Get<IHttpRequestTimeoutFeature>();

        if (requestTimeoutFeature is not null)
        {
            // Don't disable if the endpoint has explicit timeout metadata.
            var endpoint = httpContext.GetEndpoint();

            if (endpoint is not null
                && (endpoint.Metadata.GetMetadata<RequestTimeoutAttribute>() is not null || endpoint.Metadata.GetMetadata<RequestTimeoutPolicy>() is not null))
            {
                return;
            }

            requestTimeoutFeature.DisableTimeout();
        }
    }

    private static Task ProcessNonHttp2Request(HttpContext httpContext)
    {
        var protocolError = $"Request protocol '{httpContext.Request.Protocol}' is not supported.";
        GrpcProtocolHelpers.BuildHttpErrorResponse(httpContext.Response, StatusCodes.Status426UpgradeRequired, StatusCode.Internal, protocolError);
        httpContext.Response.Headers[HeaderNames.Upgrade] = GrpcProtocolConstants.Http2Protocol;
        return Task.CompletedTask;
    }

    private static Task ProcessInvalidContentTypeRequest(HttpContext httpContext, string error)
    {
        // This might be a CORS preflight request and CORS middleware hasn't been configured
        if (GrpcProtocolHelpers.IsCorsPreflightRequest(httpContext))
        {
            GrpcProtocolHelpers.BuildHttpErrorResponse(httpContext.Response, StatusCodes.Status405MethodNotAllowed, StatusCode.Internal,
                "Unhandled CORS preflight request received. CORS may not be configured correctly in the application.");

            httpContext.Response.Headers[HeaderNames.Allow] = HttpMethods.Post;
            return Task.CompletedTask;
        }
        else
        {
            GrpcProtocolHelpers.BuildHttpErrorResponse(httpContext.Response, StatusCodes.Status415UnsupportedMediaType, StatusCode.Internal, error);
            return Task.CompletedTask;
        }
    }

}
