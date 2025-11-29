using System;
using System.Diagnostics.CodeAnalysis;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class BinderServiceMethodProvider<[DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)] TService>
    : IServiceMethodProvider<TService> where TService : class
{
    public BinderServiceMethodProvider() { }

    public void OnServiceMethodDiscovery(ServiceMethodProviderContext<TService> context)
    {
        var bindMethodInfo = BindMethodFinder.GetBindMethod(typeof(TService));

        // Invoke BindService(ServiceBinderBase, BaseType)
        if (bindMethodInfo is not null)
        {
            // The second parameter is always the service base type
            var serviceParameter = bindMethodInfo.GetParameters()[1];
            var binder = new ProviderServiceBinder<TService>(context, serviceParameter.ParameterType);

            try
            {
                bindMethodInfo.Invoke(null, [binder, null]);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error binding gRPC service '{typeof(TService).Name}'.", ex);
            }
        }
    }

}
