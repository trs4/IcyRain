using System.Collections.Generic;

namespace IcyRain.Grpc.Client.Configuration;

/// <summary>Configuration for pick_first load balancer policy</summary>
public sealed class PickFirstConfig : LoadBalancingConfig
{
    /// <summary>Initializes a new instance of the <see cref="PickFirstConfig"/> class</summary>
    public PickFirstConfig() : base(PickFirstPolicyName) { }

    internal PickFirstConfig(IDictionary<string, object> inner) : base(inner) { }
}
