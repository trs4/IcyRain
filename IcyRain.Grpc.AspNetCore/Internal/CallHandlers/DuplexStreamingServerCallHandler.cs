using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class DuplexStreamingServerCallHandler<[DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)] TService, TRequest, TResponse>
    : ServerCallHandlerBase<TService, TRequest, TResponse>
    where TRequest : class
    where TResponse : class
    where TService : class
{
    private readonly DuplexStreamingServerMethodInvoker<TService, TRequest, TResponse> _invoker;

    public DuplexStreamingServerCallHandler(DuplexStreamingServerMethodInvoker<TService, TRequest, TResponse> invoker)
        : base(invoker)
        => _invoker = invoker;

    protected override async Task HandleCallAsyncCore(HttpContext httpContext, HttpContextServerCallContext serverCallContext)
    {
        DisableMinRequestBodyDataRateAndMaxRequestBodySize(httpContext);
        DisableRequestTimeout(httpContext);

        var streamReader = new HttpContextStreamReader<TRequest>(serverCallContext, MethodInvoker.Method.RequestMarshaller.ContextualDeserializer);
        var streamWriter = new HttpContextStreamWriter<TResponse>(serverCallContext, MethodInvoker.Method.ResponseMarshaller.ContextualSerializer);

        try
        {
            await _invoker.Invoke(httpContext, serverCallContext, streamReader, streamWriter);
        }
        finally
        {
            streamReader.Complete();
            streamWriter.Complete();
        }
    }

}
