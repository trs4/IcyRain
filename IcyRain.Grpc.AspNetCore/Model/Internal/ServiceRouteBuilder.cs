using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class ServiceRouteBuilder<[DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)] TService> where TService : class
{
    private readonly IEnumerable<IServiceMethodProvider<TService>> _serviceMethodProviders;
    private readonly ServerCallHandlerFactory<TService> _serverCallHandlerFactory;
    private readonly ServiceMethodsRegistry _serviceMethodsRegistry;

    public ServiceRouteBuilder(
        IEnumerable<IServiceMethodProvider<TService>> serviceMethodProviders,
        ServerCallHandlerFactory<TService> serverCallHandlerFactory,
        ServiceMethodsRegistry serviceMethodsRegistry)
    {
        _serviceMethodProviders = [.. serviceMethodProviders];
        _serverCallHandlerFactory = serverCallHandlerFactory;
        _serviceMethodsRegistry = serviceMethodsRegistry;
    }

    internal List<IEndpointConventionBuilder> Build(IEndpointRouteBuilder endpointRouteBuilder, object? argument)
    {
        var serviceMethodProviderContext = new ServiceMethodProviderContext<TService>(_serverCallHandlerFactory, argument);

        foreach (var serviceMethodProvider in _serviceMethodProviders)
            serviceMethodProvider.OnServiceMethodDiscovery(serviceMethodProviderContext);

        var endpointConventionBuilders = new List<IEndpointConventionBuilder>();

        if (serviceMethodProviderContext.Methods.Count > 0)
        {
            foreach (var method in serviceMethodProviderContext.Methods)
            {
                var endpointBuilder = endpointRouteBuilder.Map(method.Pattern, method.RequestDelegate);

                endpointBuilder.Add(ep =>
                {
                    ep.DisplayName = $"gRPC - {method.Pattern.RawText}";
                    ep.Metadata.Add(new GrpcMethodMetadata(typeof(TService), method.Method));

                    foreach (var item in method.Metadata)
                        ep.Metadata.Add(item);
                });

                endpointConventionBuilders.Add(endpointBuilder);
            }
        }

        CreateUnimplementedEndpoints(
            endpointRouteBuilder,
            _serviceMethodsRegistry,
            _serverCallHandlerFactory,
            serviceMethodProviderContext.Methods,
            endpointConventionBuilders);

        _serviceMethodsRegistry.Methods.AddRange(serviceMethodProviderContext.Methods);
        return endpointConventionBuilders;
    }

    internal static void CreateUnimplementedEndpoints(
        IEndpointRouteBuilder endpointRouteBuilder,
        ServiceMethodsRegistry serviceMethodsRegistry,
        ServerCallHandlerFactory<TService> serverCallHandlerFactory,
        List<MethodModel> serviceMethods,
        List<IEndpointConventionBuilder> endpointConventionBuilders)
    {
        // Return UNIMPLEMENTED status for missing service:
        // - /{service}/{method} + content-type header = grpc/application
        if (!serverCallHandlerFactory.IgnoreUnknownServices && serviceMethodsRegistry.Methods.Count == 0)
        {
            // Only one unimplemented service endpoint is needed for the application
            endpointConventionBuilders.Add(CreateUnimplementedEndpoint(endpointRouteBuilder,
                $"{{unimplementedService}}/{{unimplementedMethod:{GrpcServerConstants.GrpcUnimplementedConstraintPrefix}}}",
                "Unimplemented service", ServerCallHandlerFactory<TService>.CreateUnimplementedService()));
        }

        // Return UNIMPLEMENTED status for missing method:
        // - /Package.Service/{method} + content-type header = grpc/application
        if (!serverCallHandlerFactory.IgnoreUnknownMethods)
        {
            var serviceNames = serviceMethods.Select(m => m.Method.ServiceName).Distinct();

            // Typically there should be one service name for a type
            // In case the bind method sets up multiple services in one call we'll loop over them
            foreach (var serviceName in serviceNames)
            {
                if (serviceMethodsRegistry.Methods.Any(m => string.Equals(m.Method.ServiceName, serviceName, StringComparison.Ordinal)))
                {
                    // Only one unimplemented method endpoint is need for the service
                    continue;
                }

                endpointConventionBuilders.Add(CreateUnimplementedEndpoint(endpointRouteBuilder,
                    $"{serviceName}/{{unimplementedMethod:{GrpcServerConstants.GrpcUnimplementedConstraintPrefix}}}",
                    $"Unimplemented method for {serviceName}", ServerCallHandlerFactory<TService>.CreateUnimplementedMethod()));
            }
        }
    }

    private static IEndpointConventionBuilder CreateUnimplementedEndpoint(
        IEndpointRouteBuilder endpointRouteBuilder, string pattern, string displayName, RequestDelegate requestDelegate)
    {
        var endpointBuilder = endpointRouteBuilder.Map(pattern, requestDelegate);

        endpointBuilder.Add(ep =>
        {
            ep.DisplayName = $"gRPC - {displayName}";
            // Don't add POST metadata here. It will return 405 status for other HTTP methods which isn't
            // what we want. That check is made in a constraint instead.
        });

        return endpointBuilder;
    }

}
