using System.Diagnostics.CodeAnalysis;

namespace IcyRain.Grpc.AspNetCore.Internal;

/// <summary>Contains constants for specifying dynamically accessed member types for gRPC server services</summary>
internal static class ServerDynamicAccessConstants
{
    /// <summary>
    /// Specifies the member types that should be dynamically accessed for gRPC services.
    /// Includes public constructors, public methods, and non-public methods.
    /// </summary>
    internal const DynamicallyAccessedMemberTypes ServiceAccessibility =
        DynamicallyAccessedMemberTypes.PublicConstructors |
        DynamicallyAccessedMemberTypes.PublicMethods |
        DynamicallyAccessedMemberTypes.NonPublicMethods;
}
