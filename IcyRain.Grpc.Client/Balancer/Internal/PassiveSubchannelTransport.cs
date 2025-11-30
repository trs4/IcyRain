using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace IcyRain.Grpc.Client.Balancer.Internal;

/// <summary>
/// Subchannel transport used when SocketsHttpHandler isn't configured.
/// This transport will only be used when there is one address.
/// It isn't able to correctly determine connectivity state.
/// </summary>
internal sealed class PassiveSubchannelTransport : ISubchannelTransport, IDisposable
{
    private readonly Subchannel _subchannel;
    private DnsEndPoint? _currentEndPoint;

    public PassiveSubchannelTransport(Subchannel subchannel)
        => _subchannel = subchannel;

    public DnsEndPoint? CurrentEndPoint => _currentEndPoint;
    public TimeSpan? ConnectTimeout { get; }
    public TransportStatus TransportStatus => TransportStatus.Passive;

    public void Disconnect()
    {
        _currentEndPoint = null;
        _subchannel.UpdateConnectivityState(ConnectivityState.Idle, "Disconnected.");
    }

    public ValueTask<ConnectResult> TryConnectAsync(ConnectContext context, int attempt)
    {
        Debug.Assert(_subchannel._addresses.Count == 1);
        Debug.Assert(CurrentEndPoint == null);

        var currentAddress = _subchannel._addresses[0];

        _subchannel.UpdateConnectivityState(ConnectivityState.Connecting, "Passively connecting.");
        _currentEndPoint = currentAddress.EndPoint;
        _subchannel.UpdateConnectivityState(ConnectivityState.Ready, "Passively connected.");

        return new ValueTask<ConnectResult>(ConnectResult.Success);
    }

    public void Dispose()
        => _currentEndPoint = null;

    public ValueTask<Stream> GetStreamAsync(DnsEndPoint endPoint, CancellationToken token) => throw new NotSupportedException();
}
