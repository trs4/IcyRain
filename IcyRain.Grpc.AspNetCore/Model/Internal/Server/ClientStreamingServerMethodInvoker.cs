using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class ClientStreamingServerMethodInvoker<[DynamicallyAccessedMembers(ServerDynamicAccessConstants.ServiceAccessibility)] TService, TRequest, TResponse>
    : ServerMethodInvokerBase<TService, TRequest, TResponse>
    where TRequest : class
    where TResponse : class
    where TService : class
{
    private readonly ClientStreamingServerMethod<TService, TRequest, TResponse> _invoker;

    public ClientStreamingServerMethodInvoker(
        ClientStreamingServerMethod<TService, TRequest, TResponse> invoker,
        Method<TRequest, TResponse> method,
        MethodOptions options,
        IGrpcServiceActivator<TService> serviceActivator)
        : base(method, options, serviceActivator)
        => _invoker = invoker;

    public async Task<TResponse> Invoke(HttpContext httpContext, ServerCallContext serverCallContext, IAsyncStreamReader<TRequest> requestStream)
    {
        GrpcActivatorHandle<TService> serviceHandle = default;

        try
        {
            serviceHandle = CreateServiceHandle(httpContext);
            return await _invoker(serviceHandle.Instance, requestStream, serverCallContext);
        }
        finally
        {
            if (serviceHandle.Instance is not null)
                await ServiceActivator.ReleaseAsync(serviceHandle);
        }
    }

}
