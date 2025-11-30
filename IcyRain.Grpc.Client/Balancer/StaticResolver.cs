using System;
using System.Collections.Generic;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>A <see cref="Resolver"/> that returns a static collection of addresses
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
internal sealed class StaticResolver : Resolver
{
    private readonly List<BalancerAddress> _addresses;

    /// <summary>Initializes a new instance of the <see cref="StaticResolver"/> class with the specified addresses</summary>
    /// <param name="addresses">The resolved addresses</param>
    public StaticResolver(IEnumerable<BalancerAddress> addresses)
        => _addresses = [.. addresses];

    public override void Start(Action<ResolverResult> listener)
        => listener(ResolverResult.ForResult(_addresses, serviceConfig: null, serviceConfigStatus: null));
}

/// <summary>A <see cref="ResolverFactory"/> that matches the URI scheme <c>static</c> and creates <see cref="StaticResolver"/> instances
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class StaticResolverFactory : ResolverFactory
{
    private readonly Func<Uri, IEnumerable<BalancerAddress>> _addressesCallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticResolverFactory"/> class with a callback
    /// that returns a collection of addresses for a target <see cref="Uri"/>
    /// </summary>
    /// <param name="addressesCallback">A callback that returns a collection of addresses for a target <see cref="Uri"/></param>
    public StaticResolverFactory(Func<Uri, IEnumerable<BalancerAddress>> addressesCallback)
        => _addressesCallback = addressesCallback;

    public override string Name => "static";

    public override Resolver Create(ResolverOptions options)
        => new StaticResolver(_addressesCallback(options.Address));
}
