using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class DefaultGrpcServiceActivator<[DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)] TGrpcService>
    : IGrpcServiceActivator<TGrpcService> where TGrpcService : class
{
    private static readonly Lazy<ObjectFactory> _objectFactory = new Lazy<ObjectFactory>(
        static () => ActivatorUtilities.CreateFactory(typeof(TGrpcService), Type.EmptyTypes));

    public GrpcActivatorHandle<TGrpcService> Create(IServiceProvider serviceProvider)
    {
        var service = serviceProvider.GetService<TGrpcService>();

        if (service is null)
        {
            service = (TGrpcService)_objectFactory.Value(serviceProvider, []);
            return new GrpcActivatorHandle<TGrpcService>(service, created: true, state: null);
        }

        return new GrpcActivatorHandle<TGrpcService>(service, created: false, state: null);
    }

    public ValueTask ReleaseAsync(GrpcActivatorHandle<TGrpcService> service)
    {
        if (service.Instance is null)
            throw new ArgumentException("Service instance is null.", nameof(service));

        if (service.Created)
        {
            if (service.Instance is IAsyncDisposable asyncDisposableService)
                return asyncDisposableService.DisposeAsync();

            if (service.Instance is IDisposable disposableService)
            {
                disposableService.Dispose();
                return default;
            }
        }

        return default;
    }

}
