using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace IcyRain.Grpc.AspNetCore;

public sealed class GrpcServiceEndpointConventionBuilder : IEndpointConventionBuilder
{
    private readonly List<IEndpointConventionBuilder> _endpointConventionBuilders;

    internal GrpcServiceEndpointConventionBuilder(IEnumerable<IEndpointConventionBuilder> endpointConventionBuilders)
        => _endpointConventionBuilders = [.. endpointConventionBuilders];

    public void Add(Action<EndpointBuilder> convention)
    {
        foreach (var endpointConventionBuilder in _endpointConventionBuilders)
            endpointConventionBuilder.Add(convention);
    }

}
