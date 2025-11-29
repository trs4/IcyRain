using Microsoft.Extensions.DependencyInjection;

namespace IcyRain.Grpc.AspNetCore;

/// <summary>A builder abstraction for configuring gRPC servers</summary>
public interface IGrpcServerBuilder
{
    /// <summary>Gets the builder service collection</summary>
    IServiceCollection Services { get; }
}
