using Microsoft.Extensions.DependencyInjection;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class GrpcServerBuilder : IGrpcServerBuilder
{
    public GrpcServerBuilder(IServiceCollection services)
        => Services = services;

    public IServiceCollection Services { get; }
}
