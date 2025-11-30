using System.Collections.Generic;

namespace IcyRain.Grpc.Client.Configuration;

/// <summary>Configuration for pick_first load balancer policy</summary>
public sealed class RoundRobinConfig : LoadBalancingConfig
{
    /// <summary>Initializes a new instance of the <see cref="RoundRobinConfig"/> class</summary>
    public RoundRobinConfig() : base(RoundRobinPolicyName) { }

    internal RoundRobinConfig(IDictionary<string, object> inner) : base(inner) { }
}
