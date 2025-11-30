using System.Net.Http;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>Context used to pick a <see cref="Subchannel"/>
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class PickContext
{
    /// <summary>Gets or sets the <see cref="HttpRequestMessage"/></summary>
    public HttpRequestMessage? Request { get; set; }
}
