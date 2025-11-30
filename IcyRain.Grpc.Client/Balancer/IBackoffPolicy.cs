using System;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>An interface for specifying backoff duration
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public interface IBackoffPolicy
{
    /// <summary>Returns the next backoff duration</summary>
    TimeSpan NextBackoff();
}

/// <summary>
/// A factory for creating <see cref="IBackoffPolicy"/> instances
/// <para>
/// Note: Experimental API that can change or be removed without any prior notice
/// </para>
/// </summary>
public interface IBackoffPolicyFactory
{
    /// <summary>Creates a backoff policy</summary>
    IBackoffPolicy Create();
}
