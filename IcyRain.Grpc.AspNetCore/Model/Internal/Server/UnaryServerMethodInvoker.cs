using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class UnaryServerMethodInvoker<[DynamicallyAccessedMembers(ServerDynamicAccessConstants.ServiceAccessibility)] TService, TRequest, TResponse>
    : ServerMethodInvokerBase<TService, TRequest, TResponse>
    where TRequest : class
    where TResponse : class
    where TService : class
{
    private readonly UnaryServerMethod<TService, TRequest, TResponse> _invoker;

    public UnaryServerMethodInvoker(
        UnaryServerMethod<TService, TRequest, TResponse> invoker,
        Method<TRequest, TResponse> method,
        MethodOptions options,
        IGrpcServiceActivator<TService> serviceActivator)
        : base(method, options, serviceActivator)
        => _invoker = invoker;

    public Task<TResponse> Invoke(HttpContext httpContext, ServerCallContext serverCallContext, TRequest request)
    {
        GrpcActivatorHandle<TService> serviceHandle = default;
        Task<TResponse>? invokerTask = null;

        try
        {
            serviceHandle = CreateServiceHandle(httpContext);
            invokerTask = _invoker(serviceHandle.Instance, request, serverCallContext);
        }
        catch (Exception ex)
        {
            if (serviceHandle.Instance is not null)
            {
                var releaseTask = ServiceActivator.ReleaseAsync(serviceHandle);

                if (!releaseTask.IsCompletedSuccessfully)
                {
                    var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                    return AwaitServiceReleaseAndThrow(releaseTask, exceptionDispatchInfo);
                }
            }

            return Task.FromException<TResponse>(ex);
        }

        if (invokerTask.IsCompletedSuccessfully && serviceHandle.Instance is not null)
        {
            var releaseTask = ServiceActivator.ReleaseAsync(serviceHandle);

            if (!releaseTask.IsCompletedSuccessfully)
                return AwaitServiceReleaseAndReturn(invokerTask.Result, serviceHandle);

            return invokerTask;
        }

        return AwaitInvoker(invokerTask, serviceHandle);
    }

    private async Task<TResponse> AwaitInvoker(Task<TResponse> invokerTask, GrpcActivatorHandle<TService> serviceHandle)
    {
        try
        {
            return await invokerTask;
        }
        finally
        {
            if (serviceHandle.Instance is not null)
                await ServiceActivator.ReleaseAsync(serviceHandle);
        }
    }

    private static async Task<TResponse> AwaitServiceReleaseAndThrow(ValueTask releaseTask, ExceptionDispatchInfo ex)
    {
        await releaseTask;
        ex.Throw();
        return null;
    }

    private async Task<TResponse> AwaitServiceReleaseAndReturn(TResponse invokerResult, GrpcActivatorHandle<TService> serviceHandle)
    {
        await ServiceActivator.ReleaseAsync(serviceHandle);
        return invokerResult;
    }

}
