namespace IcyRain.Grpc.Client;

/// <summary>
/// The connectivity state
/// <para>Note: Experimental API that can change or be removed without any prior notice</para>
/// </summary>
public enum ConnectivityState
{
    /// <summary>Not trying to create a connection</summary>
    Idle,

    /// <summary>Establishing a connection</summary>
    Connecting,

    /// <summary>Connection ready</summary>
    Ready,

    /// <summary>A transient failure on connection</summary>
    TransientFailure,

    /// <summary>Connection shutdown</summary>
    Shutdown,
}
