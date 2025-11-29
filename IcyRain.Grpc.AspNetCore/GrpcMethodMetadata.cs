using System;
using System.Diagnostics.CodeAnalysis;
using Grpc.Core;
using IcyRain.Grpc.AspNetCore.Internal;

namespace IcyRain.Grpc.AspNetCore;

/// <summary>
/// Metadata for a gRPC method endpoint.
/// </summary>
public sealed class GrpcMethodMetadata
{
    /// <summary>
    /// Creates a new instance of <see cref="GrpcMethodMetadata"/> with the provided service type and method.
    /// </summary>
    /// <param name="serviceType">The implementing service type.</param>
    /// <param name="method">The method representation.</param>
    public GrpcMethodMetadata([DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)] Type serviceType, IMethod method)
    {
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        Method = method ?? throw new ArgumentNullException(nameof(method));
    }

    /// <summary>
    /// Gets the implementing service type.
    /// </summary>
    [DynamicallyAccessedMembers(GrpcProtocolConstants.ServiceAccessibility)]
    public Type ServiceType { get; }

    /// <summary>
    /// Gets the method representation.
    /// </summary>
    public IMethod Method { get; }
}
