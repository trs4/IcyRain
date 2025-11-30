using System;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>Context used to signal a call is complete
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class CompletionContext
{
    /// <summary>Gets or sets the <see cref="BalancerAddress"/> a call was made with. Required</summary>
    public BalancerAddress? Address { get; set; }

    /// <summary>Gets or sets the <see cref="Exception"/> thrown when making the call</summary>
    public Exception? Error { get; set; }
}
