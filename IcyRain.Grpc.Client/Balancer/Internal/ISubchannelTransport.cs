using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace IcyRain.Grpc.Client.Balancer.Internal;

/// <summary>
/// An abstraction for subchannels to create a transport and connect to the server.
/// This abstraction allows the connection to be customized. Used in unit tests.
/// Might be made public in the future to support using load balancing with non-socket transports.
/// </summary>
internal interface ISubchannelTransport : IDisposable
{
    DnsEndPoint? CurrentEndPoint { get; }
    TimeSpan? ConnectTimeout { get; }
    TransportStatus TransportStatus { get; }

    ValueTask<Stream> GetStreamAsync(DnsEndPoint endPoint, CancellationToken token);
    ValueTask<ConnectResult> TryConnectAsync(ConnectContext context, int attempt);

    void Disconnect();
}

internal enum TransportStatus
{
    NotConnected,
    Passive,
    InitialSocket,
    ActiveStream
}

internal enum ConnectResult
{
    Success,
    Failure,
    Timeout
}

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
internal sealed class ConnectContext
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private readonly CancellationTokenSource _cts;
    private readonly CancellationToken _token;

    // This flag allows the transport to determine why the cancellation token was canceled.
    // - Explicit cancellation, e.g. the channel was disposed.
    // - Connection timeout, e.g. SocketsHttpHandler.ConnectTimeout was exceeded.
    public bool IsConnectCanceled { get; private set; }
    public bool Disposed { get; private set; }

    public CancellationToken CancellationToken => _token;

    public ConnectContext(TimeSpan connectTimeout)
    {
        _cts = new CancellationTokenSource(connectTimeout);

        // Take a copy of the token to avoid ObjectDisposedException when accessing _cts.Token after CTS is disposed.
        _token = _cts.Token;
    }

    public void CancelConnect()
    {
        // Check disposed because CTS.Cancel throws if the CTS is disposed.
        ObjectDisposedException.ThrowIf(Disposed, typeof(ConnectContext));

        IsConnectCanceled = true;
        _cts.Cancel();
    }

    public void Dispose()
    {
        // Dispose the CTS because it could be created with an internal timer.
        _cts.Dispose();
        Disposed = true;
    }

}

internal interface ISubchannelTransportFactory
{
    ISubchannelTransport Create(Subchannel subchannel);
}
