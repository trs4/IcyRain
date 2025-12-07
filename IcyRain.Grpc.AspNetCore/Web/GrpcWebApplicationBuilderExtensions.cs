using System;
using IcyRain.Grpc.AspNetCore.Web.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace IcyRain.Grpc.AspNetCore.Web;

/// <summary>Provides extension methods for <see cref="IApplicationBuilder"/> to add gRPC-Web middleware</summary>
public static class GrpcWebApplicationBuilderExtensions
{
    /// <summary>Adds gRPC-Web middleware to the specified <see cref="IApplicationBuilder"/></summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to</param>
    /// <returns>A reference to this instance after the operation has completed</returns>
    public static IApplicationBuilder UseGrpcWeb(this IApplicationBuilder builder)
        => builder.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });

    /// <summary>Adds gRPC-Web middleware to the specified <see cref="IApplicationBuilder"/></summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to</param>
    /// <param name="options">The <see cref="IApplicationBuilder"/> to add the middleware to</param>
    /// <returns>A reference to this instance after the operation has completed</returns>
    public static IApplicationBuilder UseGrpcWeb(this IApplicationBuilder builder, GrpcWebOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);
        return builder.UseMiddleware<GrpcWebMiddleware>(Options.Create(options));
    }

}
