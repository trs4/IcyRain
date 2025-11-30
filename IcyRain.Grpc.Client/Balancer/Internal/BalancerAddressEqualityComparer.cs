using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace IcyRain.Grpc.Client.Balancer.Internal;

internal sealed class BalancerAddressEqualityComparer : IEqualityComparer<BalancerAddress>
{
    internal static readonly BalancerAddressEqualityComparer Instance = new BalancerAddressEqualityComparer();

    public bool Equals(BalancerAddress? x, BalancerAddress? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x == null || y == null)
            return false;

        if (!x.EndPoint.Equals(y.EndPoint))
            return false;

        return BalancerAttributes.DeepEquals(x._attributes, y._attributes);
    }

    public int GetHashCode([DisallowNull] BalancerAddress obj) => throw new NotSupportedException();
}
