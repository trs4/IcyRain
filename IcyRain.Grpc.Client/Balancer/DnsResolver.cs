using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Grpc.Client.Balancer.Internal;
using IcyRain.Grpc.Client.Internal;

namespace IcyRain.Grpc.Client.Balancer;

internal sealed class DnsResolver : PollingResolver
{
    // To prevent excessive re-resolution, we enforce a rate limit on DNS resolution requests.
    private static readonly TimeSpan MinimumDnsResolutionRate = TimeSpan.FromSeconds(15);

    private readonly Uri _originalAddress;
    private readonly string _dnsAddress;
    private readonly int _port;
    private readonly TimeSpan _refreshInterval;

    private Timer? _timer;
    private DateTime _lastResolveStart;

    public DnsResolver(Uri address, int defaultPort, TimeSpan refreshInterval, IBackoffPolicyFactory backoffPolicyFactory)
        : base(backoffPolicyFactory)
    {
        _originalAddress = address;

        // DNS address has the format: dns:[//authority/]host[:port]
        // Because the host is specified in the path, the port needs to be parsed manually
        var addressParsed = new Uri("temp://" + address.AbsolutePath.TrimStart('/'));
        _dnsAddress = addressParsed.Host;
        _port = addressParsed.Port == -1 ? defaultPort : addressParsed.Port;
        _refreshInterval = refreshInterval;
    }

    protected override void OnStarted()
    {
        base.OnStarted();

        if (_refreshInterval != Timeout.InfiniteTimeSpan)
        {
            _timer = NonCapturingTimer.Create(OnTimerCallback, state: null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _timer.Change(_refreshInterval, _refreshInterval);
        }
    }

    protected override async Task ResolveAsync(CancellationToken token)
    {
        try
        {
            var elapsedTimeSinceLastRefresh = DateTime.UtcNow - _lastResolveStart;
            if (elapsedTimeSinceLastRefresh < MinimumDnsResolutionRate)
            {
                var delay = MinimumDnsResolutionRate - elapsedTimeSinceLastRefresh;
                await Task.Delay(delay, token).ConfigureAwait(false);
            }

            var lastResolveStart = DateTime.UtcNow;

            if (string.IsNullOrEmpty(_dnsAddress))
                throw new InvalidOperationException($"Resolver address '{_originalAddress}' is not valid. Please use dns:/// for DNS provider.");

            var addresses = await Dns.GetHostAddressesAsync(_dnsAddress, token).ConfigureAwait(false);

            var hostOverride = $"{_dnsAddress}:{_port}";
            var endpoints = addresses.Select(a =>
            {
                var address = new BalancerAddress(a.ToString(), _port);
                address.Attributes.Set(ConnectionManager.HostOverrideKey, hostOverride);
                return address;
            }).ToArray();
            var resolverResult = ResolverResult.ForResult(endpoints);
            Listener(resolverResult);

            // Only update last resolve start if successful. Backoff will handle limiting resolves on failure.
            _lastResolveStart = lastResolveStart;
        }
        catch (Exception ex)
        {
            var message = $"Error getting DNS hosts for address '{_dnsAddress}'.";
            Listener(ResolverResult.ForFailure(GrpcProtocolHelpers.CreateStatusFromException(message, ex, StatusCode.Unavailable)));
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _timer?.Dispose();
    }

    private void OnTimerCallback(object? state)
    {
        try
        {
            Refresh();
        }
        catch { }
    }
}

/// <summary>
/// A <see cref="ResolverFactory"/> that matches the URI scheme <c>dns</c>
/// and creates <see cref="DnsResolver"/> instances.
/// <para>
/// Note: Experimental API that can change or be removed without any prior notice.
/// </para>
/// </summary>
public sealed class DnsResolverFactory : ResolverFactory
{
    private readonly TimeSpan _refreshInterval;

    /// <summary>
    /// Initializes a new instance of the <see cref="DnsResolverFactory"/> class with a refresh interval.
    /// </summary>
    /// <param name="refreshInterval">An interval for automatically refreshing the DNS hostname.</param>
    public DnsResolverFactory(TimeSpan refreshInterval)
        => _refreshInterval = refreshInterval;

    /// <inheritdoc />
    public override string Name => "dns";

    /// <inheritdoc />
    public override Resolver Create(ResolverOptions options)
    {
        var channelOptions = options.ChannelOptions;

        var randomGenerator = channelOptions.ResolveService<IRandomGenerator>(
            new RandomGenerator());

        var backoffPolicyFactory = channelOptions.ResolveService<IBackoffPolicyFactory>(
            new ExponentialBackoffPolicyFactory(randomGenerator, channelOptions.InitialReconnectBackoff, channelOptions.MaxReconnectBackoff));

        return new DnsResolver(options.Address, options.DefaultPort, _refreshInterval, backoffPolicyFactory);
    }

}
