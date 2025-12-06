using System;
using System.Diagnostics;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Balancer;

/// <summary>A balancing decision made by a <see cref="SubchannelPicker"/> for a gRPC call
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public sealed class PickResult
{
    [DebuggerStepThrough]
    private PickResult(PickResultType pickResultType, Subchannel? subchannel, Status status, ISubchannelCallTracker? subchannelCallTracker)
    {
        Type = pickResultType;
        Subchannel = subchannel;
        Status = status;
        SubchannelCallTracker = subchannelCallTracker;
    }

    /// <summary>The pick result type</summary>
    public PickResultType Type { get; }

    /// <summary>The <see cref="Subchannel"/> provided by <see cref="ForSubchannel(Subchannel, ISubchannelCallTracker?)"/></summary>
    public Subchannel? Subchannel { get; }

    /// <summary>The <see cref="Status"/> provided by <see cref="ForFailure(Status)"/> or <see cref="ForDrop(Status)"/></summary>
    public Status Status { get; }

    /// <summary>The optional <see cref="SubchannelCallTracker"/> provided by <see cref="ForSubchannel(Subchannel, ISubchannelCallTracker?)"/></summary>
    public ISubchannelCallTracker? SubchannelCallTracker { get; }

    /// <summary>
    /// Create a <see cref="PickResult"/> that provides a <see cref="Balancer.Subchannel"/> to gRPC calls.
    /// <para>
    /// A result created with a <see cref="Balancer.Subchannel"/> won't necessarily be used by a gRPC call.
    /// The subchannel's state may change at the same time the picker is making a decision. That means the
    /// decision may be made with outdated information. For example, a picker may return a subchannel
    /// with a state that is <see cref="ConnectivityState.Ready"/>, but
    /// becomes <see cref="ConnectivityState.Idle"/> when the subchannel is about to be used. In that situation
    /// the gRPC call waits for the load balancer to react to the new state and create a new picker.
    /// </para>
    /// </summary>
    /// <param name="subchannel">The picked subchannel</param>
    /// <param name="subchannelCallTracker">An optional interface to track the subchannel call</param>
    /// <returns>The pick result</returns>
    [DebuggerStepThrough]
    public static PickResult ForSubchannel(Subchannel subchannel, ISubchannelCallTracker? subchannelCallTracker = null)
        => new PickResult(PickResultType.Complete, subchannel, Status.DefaultSuccess, subchannelCallTracker);

    /// <summary>
    /// Creates a <see cref="PickResult"/> to report a connectivity error to calls. If the call has
    /// a <see cref="CallOptions.IsWaitForReady"/> value of <c>true</c> then the call will wait.
    /// </summary>
    /// <param name="status">The error status. Must not be <see cref="StatusCode.OK"/>.</param>
    /// <returns>The pick result.</returns>
    [DebuggerStepThrough]
    public static PickResult ForFailure(Status status)
    {
        if (status.StatusCode == StatusCode.OK)
            throw new ArgumentException("Error status code must not be OK.", nameof(status));

        return new PickResult(PickResultType.Fail, subchannel: null, status, subchannelCallTracker: null);
    }

    /// <summary>
    /// Creates a <see cref="PickResult"/> to fail a gRPC call immediately. A result with a type of 
    /// <see cref="PickResultType.Drop"/> causes calls to ignore <see cref="CallOptions.IsWaitForReady"/> and retry.
    /// </summary>
    /// <param name="status">The error status. Must not be <see cref="StatusCode.OK"/></param>
    /// <returns>The pick result</returns>
    [DebuggerStepThrough]
    public static PickResult ForDrop(Status status)
    {
        if (status.StatusCode == StatusCode.OK)
            throw new ArgumentException("Error status code must not be OK.", nameof(status));

        return new PickResult(PickResultType.Drop, subchannel: null, status, subchannelCallTracker: null);
    }

    /// <summary>Creates a <see cref="PickResult"/> to queue gRPC calls</summary>
    /// <returns>The pick result</returns>
    [DebuggerStepThrough]
    public static PickResult ForQueue()
        => new PickResult(PickResultType.Queue, subchannel: null, Status.DefaultSuccess, subchannelCallTracker: null);
}

/// <summary>The <see cref="PickResult"/> type
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public enum PickResultType
{
    /// <summary><see cref="PickResult"/> with a <see cref="Subchannel"/></summary>
    Complete,

    /// <summary>
    /// <see cref="PickResult"/> that was unable to resolve success or failure
    /// This result will queue gRPC calls until a non-queue result is available
    /// </summary>
    Queue,

    /// <summary>
    /// <see cref="PickResult"/> with a connectivity error. gRPC calls fail
    /// unless <see cref="CallOptions.IsWaitForReady"/> is set to <c>true</c>
    /// If <see cref="CallOptions.IsWaitForReady"/> is set to <c>true</c> then gRPC calls will queue
    /// </summary>
    Fail,

    /// <summary>
    /// <see cref="PickResult"/> with an immediate failure. All gRPC calls will fail,
    /// regardless of what <see cref="CallOptions.IsWaitForReady"/> is set to,
    /// and retry logic is bypassed.
    /// </summary>
    Drop,
}
