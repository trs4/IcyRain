using Microsoft.Extensions.DependencyInjection;

namespace IcyRain.Grpc.AspNetCore.Internal;

/// <summary>A marker class used to determine if all the required gRPC services were added to the <see cref="IServiceCollection"/></summary>
internal sealed class GrpcMarkerService
{
}
