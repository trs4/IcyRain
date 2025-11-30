using System;
using System.Threading;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>
/// Factory for creating new <see cref="Resolver"/> instances. A factory is used when the
/// target address <see cref="Uri"/> scheme matches the factory name.
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public abstract class ResolverFactory
{
    internal static readonly ResolverFactory[] KnownLoadResolverFactories =
    [
        new DnsResolverFactory(Timeout.InfiniteTimeSpan)
    ];

    /// <summary>Gets the resolver factory name. A factory is used when the target <see cref="Uri"/> scheme matches the factory name</summary>
    public abstract string Name { get; }

    /// <summary>Creates a new <see cref="Resolver"/> with the specified options</summary>
    /// <param name="options">Options for creating a <see cref="Resolver"/></param>
    /// <returns>A new <see cref="Resolver"/></returns>
    public abstract Resolver Create(ResolverOptions options);
}
