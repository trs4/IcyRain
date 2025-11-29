using System;
using System.Diagnostics.CodeAnalysis;
using IcyRain.Grpc.AspNetCore.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace IcyRain.Grpc.AspNetCore;

/// <summary>
/// Extension methods for the gRPC services.
/// </summary>
public static class GrpcServicesExtensions
{
    /// <summary>
    /// Adds service specific options to an <see cref="IGrpcServerBuilder"/>.
    /// </summary>
    /// <typeparam name="TService">The service type to configure.</typeparam>
    /// <param name="grpcBuilder">The <see cref="IGrpcServerBuilder"/>.</param>
    /// <param name="configure">A callback to configure the service options.</param>
    /// <returns>The same instance of the <see cref="IGrpcServerBuilder"/> for chaining.</returns>
    public static IGrpcServerBuilder AddServiceOptions<TService>(this IGrpcServerBuilder grpcBuilder, Action<GrpcServiceOptions<TService>> configure)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(grpcBuilder);

        grpcBuilder.Services.AddSingleton<IConfigureOptions<GrpcServiceOptions<TService>>, GrpcServiceOptionsSetup<TService>>();
        grpcBuilder.Services.Configure(configure);
        return grpcBuilder;
    }

    /// <summary>
    /// Adds gRPC services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
    /// <returns>An <see cref="IGrpcServerBuilder"/> that can be used to further configure the gRPC services.</returns>
    public static IGrpcServerBuilder AddGrpc(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddRoutingCore();
        services.Configure<RouteOptions>(ConfigureRouting);
        services.AddOptions();
        services.TryAddSingleton<GrpcMarkerService>();
        services.TryAddSingleton(typeof(ServerCallHandlerFactory<>));
        services.TryAddSingleton(typeof(IGrpcServiceActivator<>), typeof(DefaultGrpcServiceActivator<>));
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<GrpcServiceOptions>, GrpcServiceOptionsSetup>());

        // Model
        services.TryAddSingleton<ServiceMethodsRegistry>();
        services.TryAddSingleton(typeof(ServiceRouteBuilder<>));
        services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IServiceMethodProvider<>), typeof(BinderServiceMethodProvider<>)));

        var builder = new GrpcServerBuilder(services);
        builder.AddServiceOptions<ServerServiceDefinitionMarker>(options => options.SuppressCreatingService = true);

        return builder;

        static void ConfigureRouting(RouteOptions options)
        {
            // Unimplemented constraint is added to the route as an inline constraint to avoid RoutePatternFactory.Parse overload that includes parameter policies.
            // That overload infers strings as regex constraints, which brings in
            // the regex engine when publishing trimmed or AOT apps. This change reduces Native AOT gRPC server app size by about 1 MB.
            AddParameterPolicy<GrpcUnimplementedConstraint>(options, GrpcServerConstants.GrpcUnimplementedConstraintPrefix);
        }

        // This ensures the policy's constructors are preserved in .NET 6 with trimming. Remove when .NET 6 is no longer supported.
        static void AddParameterPolicy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(RouteOptions options, string name)
            where T : IParameterPolicy
            => options.SetParameterPolicy<T>(name);
    }

    /// <summary>
    /// Adds gRPC services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
    /// <param name="configureOptions">An <see cref="Action{GrpcServiceOptions}"/> to configure the provided <see cref="GrpcServiceOptions"/>.</param>
    /// <returns>An <see cref="IGrpcServerBuilder"/> that can be used to further configure the gRPC services.</returns>
    public static IGrpcServerBuilder AddGrpc(this IServiceCollection services, Action<GrpcServiceOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services.Configure(configureOptions).AddGrpc();
    }

}
