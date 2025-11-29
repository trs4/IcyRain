using System.Collections.Generic;

namespace IcyRain.Grpc.AspNetCore.Internal;

/// <summary>A registry of all the service methods in the application</summary>
internal sealed class ServiceMethodsRegistry
{
    public List<MethodModel> Methods { get; } = [];
}
