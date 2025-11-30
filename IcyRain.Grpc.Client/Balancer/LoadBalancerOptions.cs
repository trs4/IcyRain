using System.Collections.Generic;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>Options for creating a <see cref="LoadBalancer"/></summary>
public sealed class LoadBalancerOptions
{
    /// <summary>Initializes a new instance of the <see cref="LoadBalancerOptions"/> class</summary>
    /// <param name="controller">The controller</param>
    /// <param name="loggerFactory">The logger factory</param>
    /// <param name="configuration">The load balancer configuration</param>
    public LoadBalancerOptions(IChannelControlHelper controller, IDictionary<string, object> configuration)
    {
        Controller = controller;
        Configuration = configuration;
    }

    /// <summary>Gets the <see cref="IChannelControlHelper"/></summary>
    public IChannelControlHelper Controller { get; }

    /// <summary>Gets the load balancer configuration</summary>
    public IDictionary<string, object> Configuration { get; }
}
