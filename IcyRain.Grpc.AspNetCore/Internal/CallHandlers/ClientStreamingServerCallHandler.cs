using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class ClientStreamingServerCallHandler<[DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)] TService, TRequest, TResponse>
    : ServerCallHandlerBase<TService, TRequest, TResponse>
    where TRequest : class
    where TResponse : class
    where TService : class
{
    private readonly ClientStreamingServerMethodInvoker<TService, TRequest, TResponse> _invoker;

    public ClientStreamingServerCallHandler(ClientStreamingServerMethodInvoker<TService, TRequest, TResponse> invoker)
        : base(invoker)
        => _invoker = invoker;

    protected override async Task HandleCallAsyncCore(HttpContext httpContext, HttpContextServerCallContext serverCallContext)
    {
        DisableMinRequestBodyDataRateAndMaxRequestBodySize(httpContext);
        DisableRequestTimeout(httpContext);

        TResponse? response;

        var streamReader = new HttpContextStreamReader<TRequest>(serverCallContext, MethodInvoker.Method.RequestMarshaller.ContextualDeserializer);

        try
        {
            response = await _invoker.Invoke(httpContext, serverCallContext, streamReader);
        }
        finally
        {
            streamReader.Complete();
        }

        if (response is null)
        {
            // This is consistent with Grpc.Core when a null value is returned
            throw new RpcException(new Status(StatusCode.Cancelled, "No message returned from method."));
        }

        // Check if deadline exceeded while method was invoked. If it has then skip trying to write
        // the response message because it will always fail.
        // Note that the call is still going so the deadline could still be exceeded after this point.
        if (serverCallContext.DeadlineManager?.IsDeadlineExceededStarted ?? false)
            return;

        var responseBodyWriter = httpContext.Response.BodyWriter;
        await responseBodyWriter.WriteSingleMessageAsync(response, serverCallContext, MethodInvoker.Method.ResponseMarshaller.ContextualSerializer);
    }

}
