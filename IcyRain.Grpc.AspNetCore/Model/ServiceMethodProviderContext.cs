using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Grpc.Core;
using IcyRain.Grpc.AspNetCore.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Patterns;

namespace IcyRain.Grpc.AspNetCore;

/// <summary>
/// A context for <see cref="IServiceMethodProvider{TService}"/>.
/// </summary>
/// <typeparam name="TService">Service type for the context.</typeparam>
public class ServiceMethodProviderContext<[DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)] TService> where TService : class
{
    private readonly ServerCallHandlerFactory<TService> _serverCallHandlerFactory;

    internal ServiceMethodProviderContext(ServerCallHandlerFactory<TService> serverCallHandlerFactory, object? argument)
    {
        _serverCallHandlerFactory = serverCallHandlerFactory;
        Argument = argument;
    }

    internal List<MethodModel> Methods { get; } = [];

    /// <summary>
    /// Gets the argument passed to the <see cref="IServiceMethodProvider{TService}"/> constructor.
    /// </summary>
    public object? Argument { get; }

    /// <summary>
    /// Adds a unary method to a service.
    /// </summary>
    /// <typeparam name="TRequest">Request message type for this method.</typeparam>
    /// <typeparam name="TResponse">Response message type for this method.</typeparam>
    /// <param name="method">The method description.</param>
    /// <param name="metadata">The method metadata. This metadata can be used by routing and middleware when invoking a gRPC method.</param>
    /// <param name="invoker">The method invoker that is executed when the method is called.</param>
    public void AddUnaryMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, IList<object> metadata,
        UnaryServerMethod<TService, TRequest, TResponse> invoker)
        where TRequest : class
        where TResponse : class
    {
        var callHandler = _serverCallHandlerFactory.CreateUnary<TRequest, TResponse>(method, invoker);
        AddMethod(method, RoutePatternFactory.Parse(method.FullName), metadata, callHandler.HandleCallAsync);
    }

    /// <summary>
    /// Adds a server streaming method to a service.
    /// </summary>
    /// <typeparam name="TRequest">Request message type for this method.</typeparam>
    /// <typeparam name="TResponse">Response message type for this method.</typeparam>
    /// <param name="method">The method description.</param>
    /// <param name="metadata">The method metadata. This metadata can be used by routing and middleware when invoking a gRPC method.</param>
    /// <param name="invoker">The method invoker that is executed when the method is called.</param>
    public void AddServerStreamingMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, IList<object> metadata,
        ServerStreamingServerMethod<TService, TRequest, TResponse> invoker)
        where TRequest : class
        where TResponse : class
    {
        var callHandler = _serverCallHandlerFactory.CreateServerStreaming<TRequest, TResponse>(method, invoker);
        AddMethod(method, RoutePatternFactory.Parse(method.FullName), metadata, callHandler.HandleCallAsync);
    }

    /// <summary>
    /// Adds a client streaming method to a service.
    /// </summary>
    /// <typeparam name="TRequest">Request message type for this method.</typeparam>
    /// <typeparam name="TResponse">Response message type for this method.</typeparam>
    /// <param name="method">The method description.</param>
    /// <param name="metadata">The method metadata. This metadata can be used by routing and middleware when invoking a gRPC method.</param>
    /// <param name="invoker">The method invoker that is executed when the method is called.</param>
    public void AddClientStreamingMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, IList<object> metadata,
        ClientStreamingServerMethod<TService, TRequest, TResponse> invoker)
        where TRequest : class
        where TResponse : class
    {
        var callHandler = _serverCallHandlerFactory.CreateClientStreaming<TRequest, TResponse>(method, invoker);
        AddMethod(method, RoutePatternFactory.Parse(method.FullName), metadata, callHandler.HandleCallAsync);
    }

    /// <summary>
    /// Adds a duplex streaming method to a service.
    /// </summary>
    /// <typeparam name="TRequest">Request message type for this method.</typeparam>
    /// <typeparam name="TResponse">Response message type for this method.</typeparam>
    /// <param name="method">The method description.</param>
    /// <param name="metadata">The method metadata. This metadata can be used by routing and middleware when invoking a gRPC method.</param>
    /// <param name="invoker">The method invoker that is executed when the method is called.</param>
    public void AddDuplexStreamingMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, IList<object> metadata,
        DuplexStreamingServerMethod<TService, TRequest, TResponse> invoker)
        where TRequest : class
        where TResponse : class
    {
        var callHandler = _serverCallHandlerFactory.CreateDuplexStreaming<TRequest, TResponse>(method, invoker);
        AddMethod(method, RoutePatternFactory.Parse(method.FullName), metadata, callHandler.HandleCallAsync);
    }

    /// <summary>
    /// Adds a method to a service. This method is handled by the specified <see cref="RequestDelegate"/>.
    /// This overload of <c>AddMethod</c> is intended for advanced scenarios where raw processing of HTTP requests
    /// is desired.
    /// Note: experimental API that can change or be removed without any prior notice.
    /// </summary>
    /// <typeparam name="TRequest">Request message type for this method.</typeparam>
    /// <typeparam name="TResponse">Response message type for this method.</typeparam>
    /// <param name="method">The method description.</param>
    /// <param name="pattern">The method pattern. This pattern is used by routing to match the method to an HTTP request.</param>
    /// <param name="metadata">The method metadata. This metadata can be used by routing and middleware when invoking a gRPC method.</param>
    /// <param name="invoker">The <see cref="RequestDelegate"/> that is executed when the method is called.</param>
    public void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, RoutePattern pattern, IList<object> metadata, RequestDelegate invoker)
        where TRequest : class
        where TResponse : class
    {
        var methodModel = new MethodModel(method, pattern, metadata, invoker);
        Methods.Add(methodModel);
    }

}
