using System.Collections.Generic;
using System.Diagnostics;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>Represents options used to create <see cref="Subchannel"/>
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class SubchannelOptions
{
    /// <summary>Initializes a new instance of the <see cref="SubchannelOptions"/> class</summary>
    /// <param name="addresses">A collection of addresses</param>
    [DebuggerStepThrough]
    public SubchannelOptions(IReadOnlyList<BalancerAddress> addresses)
        => Addresses = addresses;

    /// <summary>Gets a collection of addresses</summary>
    public IReadOnlyList<BalancerAddress> Addresses { get; }
}
