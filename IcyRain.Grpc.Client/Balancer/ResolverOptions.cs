using System;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>Options for creating a resolver
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class ResolverOptions
{
    /// <summary>Initializes a new instance of the <see cref="ResolverOptions"/> class</summary>
    internal ResolverOptions(Uri address, int defaultPort, GrpcChannelOptions channelOptions)
    {
        Address = address;
        DefaultPort = defaultPort;
        DisableServiceConfig = channelOptions.DisableResolverServiceConfig;
        ChannelOptions = channelOptions;
    }

    /// <summary>Gets the address</summary>
    public Uri Address { get; }

    /// <summary>Gets the default port. This port is used when the resolver address doesn't specify a port</summary>
    public int DefaultPort { get; }

    /// <summary>Gets a flag indicating whether the resolver should disable resolving a service config</summary>
    public bool DisableServiceConfig { get; }

    /// <summary>Gets the channel options</summary>
    public GrpcChannelOptions ChannelOptions { get; }
}
