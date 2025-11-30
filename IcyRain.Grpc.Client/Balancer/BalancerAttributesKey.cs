using System.Diagnostics;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>
/// Represents the key used to get and set <see cref="BalancerAttributes"/> values
/// <para>
/// Note: Experimental API that can change or be removed without any prior notice
/// </para>
/// </summary>
/// <typeparam name="TValue">The value type</typeparam>
#pragma warning disable CA1815 // Override equals and operator equals on value types
public readonly struct BalancerAttributesKey<TValue>
{
    /// <summary>Initializes a new instance of the <see cref="BalancerAttributesKey{TValue}"/> struct with the specified key</summary>
    /// <param name="key">The key</param>
    [DebuggerStepThrough]
    public BalancerAttributesKey(string key)
        => Key = key;

    /// <summary>Gets the key</summary>
    public string Key { get; }
}
#pragma warning restore CA1815 // Override equals and operator equals on value types
