using System.Diagnostics;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>Represents the balancer state
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class BalancerState
{
    /// <summary>Initializes a new instance of the <see cref="BalancerState"/> class with the specified state</summary>
    /// <param name="connectivityState">The connectivity state</param>
    /// <param name="picker">The subchannel picker</param>
    [DebuggerStepThrough]
    public BalancerState(ConnectivityState connectivityState, SubchannelPicker picker)
    {
        ConnectivityState = connectivityState;
        Picker = picker;
    }

    /// <summary>Gets the connectivity state</summary>
    public ConnectivityState ConnectivityState { get; }

    /// <summary>Gets the subchannel picker</summary>
    public SubchannelPicker Picker { get; }
}
