using System.Collections.Generic;
using System.Linq;

namespace IcyRain.Grpc.Client.Configuration;

/// <summary>Base type for load balancer policy configuration</summary>
public class LoadBalancingConfig : ConfigObject
{
    /// <summary><c>pick_first</c> policy name</summary>
    public const string PickFirstPolicyName = "pick_first";

    /// <summary><c>round_robin</c> policy name</summary>
    public const string RoundRobinPolicyName = "round_robin";

    /// <summary>Initializes a new instance of the <see cref="LoadBalancingConfig"/> class</summary>
    public LoadBalancingConfig(string loadBalancingPolicyName)
        => Inner[loadBalancingPolicyName] = new Dictionary<string, object>();

    internal LoadBalancingConfig(IDictionary<string, object> inner) : base(inner) { }

    /// <summary>Gets the load balancer policy name</summary>
    public string PolicyName => Inner.Keys.Single();
}
