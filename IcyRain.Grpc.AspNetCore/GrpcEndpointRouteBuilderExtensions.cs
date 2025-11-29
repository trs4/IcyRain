using System;
using System.Diagnostics.CodeAnalysis;
using Grpc.Core;
using IcyRain.Grpc.AspNetCore.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace IcyRain.Grpc.AspNetCore;

/// <summary>
/// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add gRPC service endpoints.
/// </summary>
public static class GrpcEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps incoming requests to the specified <typeparamref name="TService"/> type.
    /// </summary>
    /// <typeparam name="TService">The service type to map requests to.</typeparam>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <returns>A <see cref="GrpcServiceEndpointConventionBuilder"/> for endpoints associated with the service.</returns>
    public static GrpcServiceEndpointConventionBuilder MapGrpcService<[DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)] TService>(this IEndpointRouteBuilder builder) where TService : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        ValidateServicesRegistered(builder.ServiceProvider);

        var serviceRouteBuilder = builder.ServiceProvider.GetRequiredService<ServiceRouteBuilder<TService>>();
        var endpointConventionBuilders = serviceRouteBuilder.Build(builder, argument: null);

        return new GrpcServiceEndpointConventionBuilder(endpointConventionBuilders);
    }

    /// <summary>
    /// Maps incoming requests to the <see cref="ServerServiceDefinition"/> instance from the specified factory.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="mapDefinition">The factory for <see cref="ServerServiceDefinition"/> instance.</param>
    /// <returns>A <see cref="GrpcServiceEndpointConventionBuilder"/> for endpoints associated with the service.</returns>
    public static GrpcServiceEndpointConventionBuilder MapGrpcService(this IEndpointRouteBuilder builder, Func<IServiceProvider, ServerServiceDefinition> mapDefinition)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ArgumentNullException.ThrowIfNull(mapDefinition, nameof(mapDefinition));

        var serviceDefinition = mapDefinition(builder.ServiceProvider);

        return MapGrpcService(builder, serviceDefinition);
    }

    /// <summary>
    /// Maps incoming requests to the specified <see cref="ServerServiceDefinition"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="serviceDefinition">The instance of <see cref="ServerServiceDefinition"/>.</param>
    /// <returns>A <see cref="GrpcServiceEndpointConventionBuilder"/> for endpoints associated with the service.</returns>
    public static GrpcServiceEndpointConventionBuilder MapGrpcService(this IEndpointRouteBuilder builder, ServerServiceDefinition serviceDefinition)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ArgumentNullException.ThrowIfNull(serviceDefinition, nameof(serviceDefinition));

        ValidateServicesRegistered(builder.ServiceProvider);

        var serviceRouteBuilder = builder.ServiceProvider.GetRequiredService<ServiceRouteBuilder<ServerServiceDefinitionMarker>>();
        var endpointConventionBuilders = serviceRouteBuilder.Build(builder, argument: serviceDefinition);

        return new GrpcServiceEndpointConventionBuilder(endpointConventionBuilders);
    }

#pragma warning disable IDE0270 // Use coalesce expression
    private static void ValidateServicesRegistered(IServiceProvider serviceProvider)
    {
        var marker = serviceProvider.GetService(typeof(GrpcMarkerService));

        if (marker is null)
        {
            throw new InvalidOperationException("Unable to find the required services. Please add all the required services by calling " +
                "'IServiceCollection.AddGrpc' inside the call to 'ConfigureServices(...)' in the application startup code.");
        }
    }
#pragma warning restore IDE0270 // Use coalesce expression

}
