using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using IcyRain.Grpc.AspNetCore.Internal;

namespace IcyRain.Grpc.AspNetCore;

/// <summary>A <typeparamref name="TGrpcService"/> activator abstraction</summary>
/// <typeparam name="TGrpcService">The service type</typeparam>
public interface IGrpcServiceActivator<[DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)] TGrpcService>
    where TGrpcService : class
{
    /// <summary>Creates a service</summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <returns>The created service</returns>
    GrpcActivatorHandle<TGrpcService> Create(IServiceProvider serviceProvider);

    /// <summary>Releases the specified service</summary>
    /// <param name="service">The service to release</param>
    ValueTask ReleaseAsync(GrpcActivatorHandle<TGrpcService> service);
}
