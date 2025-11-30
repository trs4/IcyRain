using System.Diagnostics;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>Represents subchannel state
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class SubchannelState
{
    /// <summary>Initializes a new instance of the <see cref="SubchannelState"/> class</summary>
    /// <param name="state">The connectivity state</param>
    /// <param name="status">The status</param>
    [DebuggerStepThrough]
    internal SubchannelState(ConnectivityState state, Status status)
    {
        State = state;
        Status = status;
    }

    /// <summary>Gets the connectivity state</summary>
    public ConnectivityState State { get; }

    /// <summary>Gets the status</summary>
    public Status Status { get; }
}
