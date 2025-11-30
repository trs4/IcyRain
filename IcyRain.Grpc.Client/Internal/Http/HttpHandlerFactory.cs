using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace IcyRain.Grpc.Client.Internal.Http;

internal static class HttpHandlerFactory
{
    public static bool TryCreatePrimaryHandler([NotNullWhen(true)] out HttpMessageHandler? primaryHandler)
    {
        // If we're in .NET 5 and SocketsHttpHandler is supported (it's not in Blazor WebAssembly)
        // then create SocketsHttpHandler with EnableMultipleHttp2Connections set to true. That will
        // allow a gRPC channel to create new connections if the maximum allow concurrency is exceeded.
        if (SocketsHttpHandler.IsSupported)
        {
            primaryHandler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true
            };
            return true;
        }

        primaryHandler = new HttpClientHandler();
        return true;
    }

    public static Exception CreateUnsupportedHandlerException()
    {
        var message =
            $"gRPC requires extra configuration on .NET implementations that don't support gRPC over HTTP/2. " +
            $"An HTTP provider must be specified using {nameof(GrpcChannelOptions)}.{nameof(GrpcChannelOptions.HttpHandler)}." +
            $"The configured HTTP provider must either support HTTP/2 or be configured to use gRPC-Web. " +
            $"See https://aka.ms/aspnet/grpc/netstandard for details.";

        return new PlatformNotSupportedException(message);
    }

}
