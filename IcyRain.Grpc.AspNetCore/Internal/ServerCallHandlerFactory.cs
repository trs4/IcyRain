using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace IcyRain.Grpc.AspNetCore.Internal;

/// <summary>
/// Creates server call handlers. Provides a place to get services that call handlers will use.
/// </summary>
internal sealed partial class ServerCallHandlerFactory<[DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)] TService> where TService : class
{
    private readonly IGrpcServiceActivator<TService> _serviceActivator;
    private readonly GrpcServiceOptions _globalOptions;
    private readonly GrpcServiceOptions<TService> _serviceOptions;

    public ServerCallHandlerFactory(
        IOptions<GrpcServiceOptions> globalOptions,
        IOptions<GrpcServiceOptions<TService>> serviceOptions,
        IGrpcServiceActivator<TService> serviceActivator)
    {
        _serviceActivator = serviceActivator;
        _serviceOptions = serviceOptions.Value;
        _globalOptions = globalOptions.Value;
    }

    // Internal for testing
    internal MethodOptions CreateMethodOptions() => MethodOptions.Create([_globalOptions, _serviceOptions]);

    public UnaryServerCallHandler<TService, TRequest, TResponse> CreateUnary<TRequest, TResponse>(
        Method<TRequest, TResponse> method, UnaryServerMethod<TService, TRequest, TResponse> invoker)
        where TRequest : class
        where TResponse : class
    {
        var options = CreateMethodOptions();
        var methodInvoker = new UnaryServerMethodInvoker<TService, TRequest, TResponse>(invoker, method, options, _serviceActivator);
        return new UnaryServerCallHandler<TService, TRequest, TResponse>(methodInvoker);
    }

    public ClientStreamingServerCallHandler<TService, TRequest, TResponse> CreateClientStreaming<TRequest, TResponse>(
        Method<TRequest, TResponse> method, ClientStreamingServerMethod<TService, TRequest, TResponse> invoker)
        where TRequest : class
        where TResponse : class
    {
        var options = CreateMethodOptions();
        var methodInvoker = new ClientStreamingServerMethodInvoker<TService, TRequest, TResponse>(invoker, method, options, _serviceActivator);
        return new ClientStreamingServerCallHandler<TService, TRequest, TResponse>(methodInvoker);
    }

    public DuplexStreamingServerCallHandler<TService, TRequest, TResponse> CreateDuplexStreaming<TRequest, TResponse>(
        Method<TRequest, TResponse> method, DuplexStreamingServerMethod<TService, TRequest, TResponse> invoker)
        where TRequest : class
        where TResponse : class
    {
        var options = CreateMethodOptions();
        var methodInvoker = new DuplexStreamingServerMethodInvoker<TService, TRequest, TResponse>(invoker, method, options, _serviceActivator);
        return new DuplexStreamingServerCallHandler<TService, TRequest, TResponse>(methodInvoker);
    }

    public ServerStreamingServerCallHandler<TService, TRequest, TResponse> CreateServerStreaming<TRequest, TResponse>(
        Method<TRequest, TResponse> method, ServerStreamingServerMethod<TService, TRequest, TResponse> invoker)
        where TRequest : class
        where TResponse : class
    {
        var options = CreateMethodOptions();
        var methodInvoker = new ServerStreamingServerMethodInvoker<TService, TRequest, TResponse>(invoker, method, options, _serviceActivator);
        return new ServerStreamingServerCallHandler<TService, TRequest, TResponse>(methodInvoker);
    }

    public static RequestDelegate CreateUnimplementedMethod()
    {
        return httpContext =>
        {
            // CORS preflight request should be handled by CORS middleware.
            // If it isn't then return 404 from endpoint request delegate.
            if (GrpcProtocolHelpers.IsCorsPreflightRequest(httpContext))
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return Task.CompletedTask;
            }

            GrpcProtocolHelpers.AddProtocolHeaders(httpContext.Response);

            var unimplementedMethod = httpContext.Request.RouteValues["unimplementedMethod"]?.ToString() ?? "<unknown>";

            GrpcProtocolHelpers.SetStatus(GrpcProtocolHelpers.GetTrailersDestination(httpContext.Response),
                new Status(StatusCode.Unimplemented, "Method is unimplemented."));

            return Task.CompletedTask;
        };
    }

    public bool IgnoreUnknownServices => _globalOptions.IgnoreUnknownServices ?? false;
    public bool IgnoreUnknownMethods => _serviceOptions.IgnoreUnknownServices ?? _globalOptions.IgnoreUnknownServices ?? false;

    public static RequestDelegate CreateUnimplementedService()
    {
        return httpContext =>
        {
            // CORS preflight request should be handled by CORS middleware.
            // If it isn't then return 404 from endpoint request delegate.
            if (GrpcProtocolHelpers.IsCorsPreflightRequest(httpContext))
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return Task.CompletedTask;
            }

            GrpcProtocolHelpers.AddProtocolHeaders(httpContext.Response);

            var unimplementedService = httpContext.Request.RouteValues["unimplementedService"]?.ToString() ?? "<unknown>";

            GrpcProtocolHelpers.SetStatus(GrpcProtocolHelpers.GetTrailersDestination(httpContext.Response),
                new Status(StatusCode.Unimplemented, "Service is unimplemented."));

            return Task.CompletedTask;
        };
    }

}
