using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class ServerStreamingServerMethodInvoker<[DynamicallyAccessedMembers(ServerDynamicAccessConstants.ServiceAccessibility)] TService, TRequest, TResponse>
    : ServerMethodInvokerBase<TService, TRequest, TResponse>
    where TRequest : class
    where TResponse : class
    where TService : class
{
    private readonly ServerStreamingServerMethod<TService, TRequest, TResponse> _invoker;

    public ServerStreamingServerMethodInvoker(
        ServerStreamingServerMethod<TService, TRequest, TResponse> invoker,
        Method<TRequest, TResponse> method,
        MethodOptions options,
        IGrpcServiceActivator<TService> serviceActivator)
        : base(method, options, serviceActivator)
        => _invoker = invoker;

    public async Task Invoke(HttpContext httpContext, ServerCallContext serverCallContext, TRequest request, IServerStreamWriter<TResponse> streamWriter)
    {
        GrpcActivatorHandle<TService> serviceHandle = default;

        try
        {
            serviceHandle = CreateServiceHandle(httpContext);
            await _invoker(serviceHandle.Instance, request, streamWriter, serverCallContext);
        }
        finally
        {
            if (serviceHandle.Instance is not null)
                await ServiceActivator.ReleaseAsync(serviceHandle);
        }
    }

}
