using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class ServerStreamingServerCallHandler<[DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)] TService, TRequest, TResponse>
    : ServerCallHandlerBase<TService, TRequest, TResponse>
    where TRequest : class
    where TResponse : class
    where TService : class
{
    private readonly ServerStreamingServerMethodInvoker<TService, TRequest, TResponse> _invoker;

    public ServerStreamingServerCallHandler(ServerStreamingServerMethodInvoker<TService, TRequest, TResponse> invoker)
        : base(invoker)
        => _invoker = invoker;

    protected override async Task HandleCallAsyncCore(HttpContext httpContext, HttpContextServerCallContext serverCallContext)
    {
        DisableRequestTimeout(httpContext);

        var request = await httpContext.Request.BodyReader.ReadSingleMessageAsync(serverCallContext, MethodInvoker.Method.RequestMarshaller.ContextualDeserializer);
        var streamWriter = new HttpContextStreamWriter<TResponse>(serverCallContext, MethodInvoker.Method.ResponseMarshaller.ContextualSerializer);

        try
        {
            await _invoker.Invoke(httpContext, serverCallContext, request, streamWriter);
        }
        finally
        {
            streamWriter.Complete();
        }
    }

}
