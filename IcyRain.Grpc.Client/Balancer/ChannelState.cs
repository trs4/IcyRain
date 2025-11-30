using System.Collections.Generic;
using System.Diagnostics;
using Grpc.Core;
using IcyRain.Grpc.Client.Configuration;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>Represents the state for a channel. This is created from results returned by a <see cref="Resolver"/>
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class ChannelState
{
    [DebuggerStepThrough]
    internal ChannelState(Status status, IReadOnlyList<BalancerAddress>? addresses, LoadBalancingConfig? loadBalancingConfig, BalancerAttributes attributes)
    {
        Addresses = addresses;
        LoadBalancingConfig = loadBalancingConfig;
        Status = status;
        Attributes = attributes;
    }

    /// <summary>Gets a collection of addresses. Will be <c>null</c> if <see cref="Status"/> has a non-OK value</summary>
    public IReadOnlyList<BalancerAddress>? Addresses { get; }

    /// <summary>Gets an optional load balancing config</summary>
    public LoadBalancingConfig? LoadBalancingConfig { get; }

    /// <summary>
    /// Gets the status. Successful results has an <see cref="StatusCode.OK"/> status
    /// A resolver error creates results with non-OK status. The status has details about the resolver error
    /// </summary>
    public Status Status { get; }

    /// <summary>Gets a collection of metadata attributes</summary>
    public BalancerAttributes Attributes { get; }
}
