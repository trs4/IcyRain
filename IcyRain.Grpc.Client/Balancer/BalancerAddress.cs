using System.Diagnostics;
using System.Net;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>Represents a balancer address
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class BalancerAddress
{
    // Internal so address attributes can be compared without using the Attributes property
    // The property allocates an empty collection if one isn't already present
    internal BalancerAttributes? _attributes;

    /// <summary>Initializes a new instance of the <see cref="BalancerAddress"/> class with the specified <see cref="DnsEndPoint"/></summary>
    /// <param name="endPoint">The end point</param>
    [DebuggerStepThrough]
    public BalancerAddress(DnsEndPoint endPoint)
        => EndPoint = endPoint;

    /// <summary>Initializes a new instance of the <see cref="BalancerAddress"/> class with the specified host and port</summary>
    /// <param name="host">The host</param>
    /// <param name="port">The port</param>
    [DebuggerStepThrough]
    public BalancerAddress(string host, int port) : this(new BalancerEndPoint(host, port)) { }

    /// <summary>Gets the address <see cref="DnsEndPoint"/></summary>
    public DnsEndPoint EndPoint { get; }

    /// <summary>Gets the address attributes</summary>
    public BalancerAttributes Attributes => _attributes ??= new();

    /// <summary>Returns a string that reprsents the address</summary>
    public override string ToString() => $"{EndPoint.Host}:{EndPoint.Port}";

    private sealed class BalancerEndPoint : DnsEndPoint
    {
        private string? _cachedToString;

        public BalancerEndPoint(string host, int port) : base(host, port) { }

        public override string ToString()
            => _cachedToString ??= $"{Host}:{Port}"; // Improve ToString performance when logging by caching ToString. Don't include DnsEndPoint address family
    }

}
