using System;
using System.Collections.Generic;
using System.Linq;
using IcyRain.Grpc.Client.Internal.Configuration;

namespace IcyRain.Grpc.Client.Configuration;

/// <summary>A <see cref="ServiceConfig"/> represents information about a service</summary>
public sealed class ServiceConfig : ConfigObject
{
    private const string MethodConfigPropertyName = "methodConfig";
    private const string LoadBalancingConfigPropertyName = "loadBalancingConfig";
    private const string RetryThrottlingPropertyName = "retryThrottling";

    private readonly ConfigProperty<Values<MethodConfig, object>, IList<object>> _methods =
        new(i => new Values<MethodConfig, object>(i ?? [],
            s => s.Inner,
            s => new MethodConfig((IDictionary<string, object>)s)), MethodConfigPropertyName);

    private readonly ConfigProperty<Values<LoadBalancingConfig, object>, IList<object>> _loadBalancingConfigs =
        new(i => new Values<LoadBalancingConfig, object>(i ?? [],
            s => s.Inner,
            s => CreateLoadBalancingConfig((IDictionary<string, object>)s)), LoadBalancingConfigPropertyName);

    private static LoadBalancingConfig CreateLoadBalancingConfig(IDictionary<string, object> s)
    {
        if (s.Count == 1)
        {
            var item = s.Single();

            return item.Key switch
            {
                LoadBalancingConfig.RoundRobinPolicyName => new RoundRobinConfig(s),
                LoadBalancingConfig.PickFirstPolicyName => new PickFirstConfig(s),
                _ => new LoadBalancingConfig(s), // Unknown/unsupported config. Use base type
            };
        }

        throw new InvalidOperationException("Unexpected error when parsing load balancing config.");
    }

    private readonly ConfigProperty<RetryThrottlingPolicy, IDictionary<string, object>> _retryThrottling =
        new(i => i is not null ? new RetryThrottlingPolicy(i) : null, RetryThrottlingPropertyName);

    /// <summary>Initializes a new instance of the <see cref="ServiceConfig"/> class</summary>
    public ServiceConfig() { }

    internal ServiceConfig(IDictionary<string, object> inner) : base(inner) { }

    // Multiple LB policies can be specified; clients will iterate through
    // the list in order and stop at the first policy that they support. If none
    // are supported, the service config is considered invalid.

    /// <summary>
    /// Gets a collection of <see cref="LoadBalancingConfig"/> instances. The client will iterate
    /// through the configured policies in order and use the first policy that is supported.
    /// If none are supported by the client then a configuration error is thrown
    /// </summary>
    public IList<LoadBalancingConfig> LoadBalancingConfigs => _loadBalancingConfigs.GetValue(this)!;

    /// <summary>
    /// Gets a collection of <see cref="MethodConfig"/> instances. This collection is used to specify
    /// configuration on a per-method basis. <see cref="MethodConfig.Names"/> determines which calls
    /// a method config applies to
    /// </summary>
    public IList<MethodConfig> MethodConfigs => _methods.GetValue(this)!;

    /// <summary>
    /// Gets or sets the retry throttling policy.
    /// If a <see cref="RetryThrottlingPolicy"/> is provided, gRPC will automatically throttle
    /// retry attempts and hedged RPCs when the client's ratio of failures to
    /// successes exceeds a threshold
    /// </summary>
    public RetryThrottlingPolicy? RetryThrottling
    {
        get => _retryThrottling.GetValue(this);
        set => _retryThrottling.SetValue(this, value);
    }

}
