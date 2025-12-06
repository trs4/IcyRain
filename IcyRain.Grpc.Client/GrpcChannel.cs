using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client.Internal;
using IcyRain.Grpc.Client.Balancer;
using IcyRain.Grpc.Client.Balancer.Internal;
using IcyRain.Grpc.Client.Configuration;
using IcyRain.Grpc.Client.Internal;
using IcyRain.Grpc.Client.Internal.Http;
using IcyRain.Grpc.Client.Internal.Retry;

namespace IcyRain.Grpc.Client;

/// <summary>
/// Represents a gRPC channel. Channels are an abstraction of long-lived connections to remote servers.
/// Client objects can reuse the same channel. Creating a channel is an expensive operation compared to invoking
/// a remote call so in general you should reuse a single channel for as many calls as possible
/// </summary>
public sealed partial class GrpcChannel : ChannelBase, IDisposable
{
    internal const int DefaultMaxReceiveMessageSize = 1024 * 1024 * 4; // 4 MB
    internal const long DefaultInitialReconnectBackoffTicks = TimeSpan.TicksPerSecond * 1;
    internal const long DefaultMaxReconnectBackoffTicks = TimeSpan.TicksPerSecond * 120;
    internal const int DefaultMaxRetryAttempts = 5;
    internal const long DefaultMaxRetryBufferSize = 1024 * 1024 * 16; // 16 MB
    internal const long DefaultMaxRetryBufferPerCallSize = 1024 * 1024; // 1 MB

    private readonly object _lock;
    private readonly ThreadSafeLookup<IMethod, GrpcMethodInfo> _methodInfoCache;
    private readonly Func<IMethod, GrpcMethodInfo> _createMethodInfoFunc;
    private readonly Dictionary<MethodKey, MethodConfig>? _serviceConfigMethods;
    private readonly bool _isSecure;
    private readonly List<CallCredentials>? _callCredentials;
    private readonly HashSet<IDisposable> _activeCalls;

    internal Uri Address { get; }

    internal HttpMessageInvoker HttpInvoker { get; }

    internal TimeSpan? ConnectTimeout { get; }

    internal TimeSpan? ConnectionIdleTimeout { get; }

    internal HttpHandlerType HttpHandlerType { get; }

    internal TimeSpan InitialReconnectBackoff { get; }

    internal TimeSpan? MaxReconnectBackoff { get; }

    internal int? SendMaxMessageSize { get; }

    internal int? ReceiveMaxMessageSize { get; }

    internal int? MaxRetryAttempts { get; }

    internal long? MaxRetryBufferSize { get; }

    internal long? MaxRetryBufferPerCallSize { get; }

    internal bool ThrowOperationCanceledOnCancellation { get; }

    internal bool UnsafeUseInsecureChannelCallCredentials { get; }

    internal bool IsSecure => _isSecure;

    internal List<CallCredentials>? CallCredentials => _callCredentials;

    internal bool Disposed { get; private set; }

    internal Version HttpVersion { get; }

    internal HttpVersionPolicy HttpVersionPolicy { get; }

    // Load balancing
    internal ConnectionManager ConnectionManager { get; }

    // Set in unit tests
    internal ISubchannelTransportFactory SubchannelTransportFactory;

    // Stateful
    internal ChannelRetryThrottling? RetryThrottling { get; }
    internal long CurrentRetryBufferSize;

    // Options that are set in unit tests
    internal IOperatingSystem OperatingSystem;
    internal IRandomGenerator RandomGenerator;
    internal long MaxTimerDueTime = uint.MaxValue - 1; // Max System.Threading.Timer due time

    private readonly bool _shouldDisposeHttpClient;

    internal GrpcChannel(Uri address, GrpcChannelOptions channelOptions) : base(address.Authority)
    {
        _lock = new object();
        _methodInfoCache = new ThreadSafeLookup<IMethod, GrpcMethodInfo>();

        // Dispose the HTTP client/handler if...
        //   1. No client/handler was specified and so the channel created the client itself
        //   2. User has specified a client/handler and set DisposeHttpClient to true
        _shouldDisposeHttpClient = (channelOptions.HttpClient == null && channelOptions.HttpHandler == null)
            || channelOptions.DisposeHttpClient;

        Address = address;
        OperatingSystem = channelOptions.ResolveService<IOperatingSystem>(Internal.OperatingSystem.Instance);
        RandomGenerator = channelOptions.ResolveService<IRandomGenerator>(new RandomGenerator());

        InitialReconnectBackoff = channelOptions.InitialReconnectBackoff;
        MaxReconnectBackoff = channelOptions.MaxReconnectBackoff;

        var resolverFactory = GetResolverFactory(channelOptions);
        ResolveCredentials(channelOptions, out _isSecure, out _callCredentials);
        (HttpHandlerType, ConnectTimeout, ConnectionIdleTimeout) = CalculateHandlerContext(address, _isSecure, channelOptions);

        SubchannelTransportFactory = channelOptions.ResolveService<ISubchannelTransportFactory>(new SubChannelTransportFactory(this));

        if (!IsHttpOrHttpsAddress(Address) || channelOptions.ServiceConfig?.LoadBalancingConfigs.Count > 0)
            ValidateHttpHandlerSupportsConnectivity();

        var defaultPort = IsSecure ? 443 : 80;
        var resolver = resolverFactory.Create(new ResolverOptions(Address, defaultPort, channelOptions));

        ConnectionManager = new ConnectionManager(
            resolver,
            channelOptions.DisableResolverServiceConfig,
            channelOptions.ResolveService<IBackoffPolicyFactory>(new ExponentialBackoffPolicyFactory(RandomGenerator, InitialReconnectBackoff, MaxReconnectBackoff)),
            SubchannelTransportFactory,
            ResolveLoadBalancerFactories(channelOptions));

        ConnectionManager.ConfigureBalancer(c => new ChildHandlerLoadBalancer(
            c,
            channelOptions.ServiceConfig,
            ConnectionManager));

        HttpInvoker = channelOptions.HttpClient ?? CreateInternalHttpInvoker(channelOptions.HttpHandler);
        SendMaxMessageSize = channelOptions.MaxSendMessageSize;
        ReceiveMaxMessageSize = channelOptions.MaxReceiveMessageSize;
        MaxRetryAttempts = channelOptions.MaxRetryAttempts;
        MaxRetryBufferSize = channelOptions.MaxRetryBufferSize;
        MaxRetryBufferPerCallSize = channelOptions.MaxRetryBufferPerCallSize;
        ThrowOperationCanceledOnCancellation = channelOptions.ThrowOperationCanceledOnCancellation;
        UnsafeUseInsecureChannelCallCredentials = channelOptions.UnsafeUseInsecureChannelCallCredentials;
        _createMethodInfoFunc = CreateMethodInfo;
        _activeCalls = [];

        if (channelOptions.ServiceConfig is { } serviceConfig)
        {
            RetryThrottling = serviceConfig.RetryThrottling != null ? CreateChannelRetryThrottling(serviceConfig.RetryThrottling) : null;
            _serviceConfigMethods = CreateServiceConfigMethods(serviceConfig);
        }

        HttpVersion = channelOptions.HttpVersion ?? GrpcProtocolConstants.Http2Version;
        HttpVersionPolicy = channelOptions.HttpVersionPolicy ?? HttpVersionPolicy.RequestVersionExact;

        if (HttpHandlerType == HttpHandlerType.WinHttpHandler
            && OperatingSystem.IsWindows
            && !ValidateWinHttpHandlerOperatingSystemVersion())
        {
            throw new InvalidOperationException("The channel configuration isn't valid on this operating system. " +
                "The channel is configured to use WinHttpHandler and the current version of Windows " +
                "doesn't support HTTP/2 features required by gRPC. Windows Server 2022 or Windows 11 or later is required. " +
                "For more information, see https://aka.ms/aspnet/grpc/netframework.");
        }
    }

    private bool ValidateWinHttpHandlerOperatingSystemVersion()
    {
        // Grpc.Net.Client + .NET Framework + WinHttpHandler requires features in WinHTTP, shipped in Windows, to work correctly.
        // This scenario is supported in these versions of Windows or later:
        // -Windows Server 2019 and Windows Server 2022 have partial support.
        //    -Unary and server streaming methods are supported.
        //    -Client and bidi streaming methods aren't supported.
        // -Windows 11 has full support.
        const int WinServer2022BuildVersion = 20348;
        const int WinServer2019BuildVersion = 17763;

        // Validate the Windows version is WinServer2022 or later. Win11 version number is greater than WinServer2022.
        // Note that this doesn't block using unsupported client and bidi streaming methods on WinServer2022.
        if (OperatingSystem.OSVersion.Build >= WinServer2022BuildVersion)
            return true;

        // Validate the Windows version is WinServer2019. Its build numbers are mixed with Windows 10, so we must check
        // the OS version is Windows Server and the build number together to avoid allowing Windows 10.
        if (OperatingSystem.IsWindowsServer && OperatingSystem.OSVersion.Build >= WinServer2019BuildVersion)
            return true;

        return false;
    }

    private void ResolveCredentials(GrpcChannelOptions channelOptions, out bool isSecure, out List<CallCredentials>? callCredentials)
    {
        if (channelOptions.Credentials is not null)
        {
            var configurator = new DefaultChannelCredentialsConfigurator(channelOptions.UnsafeUseInsecureChannelCallCredentials);
            channelOptions.Credentials.InternalPopulateConfiguration(configurator, channelOptions.Credentials);

            isSecure = configurator.IsSecure ?? false;
            callCredentials = configurator.CallCredentials;

            ValidateChannelCredentials();
        }
        else
        {
            if (Address.Scheme == Uri.UriSchemeHttp)
                isSecure = false;
            else if (Address.Scheme == Uri.UriSchemeHttps)
                isSecure = true;
            else
            {
                throw new InvalidOperationException($"Unable to determine the TLS configuration of the channel from address '{Address}'. " +
                    $"{nameof(GrpcChannelOptions)}.{nameof(GrpcChannelOptions.Credentials)} must be specified when the address doesn't have a 'http' or 'https' scheme. " +
                    $"To call TLS endpoints, set credentials to '{nameof(ChannelCredentials)}.{nameof(ChannelCredentials.SecureSsl)}'. " +
                    $"To call non-TLS endpoints, set credentials to '{nameof(ChannelCredentials)}.{nameof(ChannelCredentials.Insecure)}'.");
            }

            callCredentials = null;
        }
    }

    private static bool IsHttpOrHttpsAddress(Uri address)
        => address.Scheme == Uri.UriSchemeHttps || address.Scheme == Uri.UriSchemeHttp;

    private static HttpHandlerContext CalculateHandlerContext(Uri address, bool isSecure, GrpcChannelOptions channelOptions)
    {
        if (channelOptions.HttpHandler is null)
        {
            // No way to know what handler a HttpClient is using so assume custom.
            var type = channelOptions.HttpClient == null ? HttpHandlerType.SocketsHttpHandler : HttpHandlerType.Custom;
            return new HttpHandlerContext(type);
        }

        if (HttpRequestHelpers.HasHttpHandlerType(channelOptions.HttpHandler, "System.Net.Http.WinHttpHandler"))
            return new HttpHandlerContext(HttpHandlerType.WinHttpHandler);

        if (HttpRequestHelpers.HasHttpHandlerType(channelOptions.HttpHandler, "System.Net.Http.SocketsHttpHandler"))
        {
            var socketsHttpHandler = HttpRequestHelpers.GetHttpHandlerType<SocketsHttpHandler>(channelOptions.HttpHandler)!;

            // Check if the SocketsHttpHandler is being shared by channels.
            // It has already been setup by another channel (i.e. ConnectCallback is set) then
            // additional channels can use advanced connectivity features.
            if (!BalancerHttpHandler.IsSocketsHttpHandlerSetup(socketsHttpHandler))
            {
                // Someone has already configured the handler callback.
                // This channel can't support advanced connectivity features.
                if (socketsHttpHandler.ConnectCallback is not null)
                    return new HttpHandlerContext(HttpHandlerType.Custom);
            }

            // Load balancing has been disabled on the SocketsHttpHandler.
            if (socketsHttpHandler.Properties.TryGetValue("__GrpcLoadBalancingDisabled", out var value)
                && value is bool loadBalancingDisabled && loadBalancingDisabled)
            {
                return new HttpHandlerContext(HttpHandlerType.Custom);
            }

            return new HttpHandlerContext(HttpHandlerType.SocketsHttpHandler, socketsHttpHandler.ConnectTimeout, GetConnectionIdleTimeout(socketsHttpHandler));
        }

        if (HttpRequestHelpers.GetHttpHandlerType<HttpClientHandler>(channelOptions.HttpHandler) != null)
            return new HttpHandlerContext(HttpHandlerType.HttpClientHandler);

        return new HttpHandlerContext(HttpHandlerType.Custom);

        static TimeSpan? GetConnectionIdleTimeout(SocketsHttpHandler socketsHttpHandler)
        {
            // Check if either TimeSpan is InfiniteTimeSpan, and return the other one.
            if (socketsHttpHandler.PooledConnectionIdleTimeout == Timeout.InfiniteTimeSpan)
                return socketsHttpHandler.PooledConnectionLifetime;

            if (socketsHttpHandler.PooledConnectionLifetime == Timeout.InfiniteTimeSpan)
                return socketsHttpHandler.PooledConnectionIdleTimeout;

            // Return the bigger TimeSpan.
            return socketsHttpHandler.PooledConnectionIdleTimeout > socketsHttpHandler.PooledConnectionLifetime
                ? socketsHttpHandler.PooledConnectionIdleTimeout
                : socketsHttpHandler.PooledConnectionLifetime;
        }
    }

    private ResolverFactory GetResolverFactory(GrpcChannelOptions options)
    {
        // Special case http and https schemes. These schemes don't use a dynamic resolver. An http/https
        // address is always just one address and that is enabled using the static resolver.
        //
        // Even with just one address we still want to use the load balancing infrastructure. This enables
        // the connectivity APIs on channel like GrpcChannel.State and GrpcChannel.WaitForStateChanged.
        if (IsHttpOrHttpsAddress(Address))
            return new StaticResolverFactory(uri => [new BalancerAddress(Address.Host, Address.Port)]);

        var factories = options.ResolveService<IEnumerable<ResolverFactory>>([]);
        factories = factories.Union(ResolverFactory.KnownLoadResolverFactories);

        foreach (var factory in factories)
        {
            if (string.Equals(factory.Name, Address.Scheme, StringComparison.OrdinalIgnoreCase))
                return factory;
        }

        throw new InvalidOperationException($"No address resolver configured for the scheme '{Address.Scheme}'.");
    }

    private void ValidateHttpHandlerSupportsConnectivity()
    {
        if (HttpHandlerType == HttpHandlerType.SocketsHttpHandler)
            return; // SocketsHttpHandler is being used

        if (SubchannelTransportFactory is not SubChannelTransportFactory)
            return; // Custom transport is configured. Probably in a unit test

        // Either the HTTP transport was configured with HttpClient, or SocketsHttpHandler.ConnectCallback is set.
        // We don't know how HTTP requests will be sent so we throw an error
        throw new InvalidOperationException(
            $"Channel is configured with an HTTP transport doesn't support client-side load balancing or connectivity state tracking. " +
            $"The underlying HTTP transport must be a {nameof(SocketsHttpHandler)} with no " +
            $"{nameof(SocketsHttpHandler)}.{nameof(SocketsHttpHandler.ConnectCallback)} configured. " +
            $"The HTTP transport must be configured on the channel using {nameof(GrpcChannelOptions)}.{nameof(GrpcChannelOptions.HttpHandler)}.");
    }

    private static LoadBalancerFactory[] ResolveLoadBalancerFactories(GrpcChannelOptions channelOptions)
    {
        var serviceFactories = channelOptions.ResolveService<IEnumerable<LoadBalancerFactory>?>(defaultValue: null);

        if (serviceFactories is not null)
            return [.. serviceFactories.Union(LoadBalancerFactory.KnownLoadBalancerFactories)];

        return LoadBalancerFactory.KnownLoadBalancerFactories;
    }

    private static ChannelRetryThrottling CreateChannelRetryThrottling(RetryThrottlingPolicy retryThrottling)
    {
        if (retryThrottling.MaxTokens is null)
            throw CreateException(RetryThrottlingPolicy.MaxTokensPropertyName);

        if (retryThrottling.TokenRatio is null)
            throw CreateException(RetryThrottlingPolicy.TokenRatioPropertyName);

        return new ChannelRetryThrottling(retryThrottling.MaxTokens.Value, retryThrottling.TokenRatio.Value);

        static InvalidOperationException CreateException(string propertyName)
            => new InvalidOperationException($"Retry throttling missing required property '{propertyName}'.");
    }

    private static Dictionary<MethodKey, MethodConfig> CreateServiceConfigMethods(ServiceConfig serviceConfig)
    {
        var configs = new Dictionary<MethodKey, MethodConfig>();

        for (var i = 0; i < serviceConfig.MethodConfigs.Count; i++)
        {
            var methodConfig = serviceConfig.MethodConfigs[i];

            for (var j = 0; j < methodConfig.Names.Count; j++)
            {
                var name = methodConfig.Names[j];
                var methodKey = new MethodKey(name.Service, name.Method);

                if (configs.ContainsKey(methodKey))
                    throw new InvalidOperationException($"Duplicate method config found. Service: '{name.Service}', method: '{name.Method}'.");

                configs[methodKey] = methodConfig;
            }
        }

        return configs;
    }

    private HttpMessageInvoker CreateInternalHttpInvoker(HttpMessageHandler? handler)
    {
        // HttpMessageInvoker should always dispose handler if Disposed is called on it
        // Decision to dispose invoker is controlled by _shouldDisposeHttpClient
        if (handler is null)
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            if (!HttpHandlerFactory.TryCreatePrimaryHandler(out handler))
                throw HttpHandlerFactory.CreateUnsupportedHandlerException();
#pragma warning restore CA2000 // Dispose objects before losing scope
        }
        else
        {
            // Validate the user specified handler is compatible with this platform.
            //
            // Android's native handler doesn't fully support HTTP/2 and using it could cause hard to understand errors
            // in advanced gRPC scenarios. We want Android to use SocketsHttpHandler. Throw an error if:
            // 1. Client is running on Android.
            // 2. Channel is created with HttpClientHandler.
            // 3. Channel is not using GrpcWebHandler. grpc-web is compatible with the native handler.
            // 4. UseNativeHttpHandler switch is true.
            if (OperatingSystem.IsAndroid)
            {
                // GetHttpHandlerType recurses through DelegatingHandlers that may wrap the HttpClientHandler.
                var httpClientHandler = HttpRequestHelpers.GetHttpHandlerType<HttpClientHandler>(handler);
                var grpcWebHandler = HttpRequestHelpers.GetHttpHandlerType(handler, "Grpc.Net.Client.Web.GrpcWebHandler");

                if (httpClientHandler is not null && grpcWebHandler is null
                    && RuntimeHelpers.QueryRuntimeSettingSwitch("System.Net.Http.UseNativeHttpHandler", defaultValue: false))
                {
                    throw new InvalidOperationException("The channel configuration isn't valid on Android devices. " +
                        "The channel is configured to use HttpClientHandler and Android's native HTTP/2 library. " +
                        "gRPC isn't fully supported by Android's native HTTP/2 library and it can cause runtime errors. " +
                        "To fix this problem, either configure the channel to use SocketsHttpHandler, or add " +
                        "<UseNativeHttpHandler>false</UseNativeHttpHandler> to the app's project file. " +
                        "For more information, see https://aka.ms/aspnet/grpc/android.");
                }
            }
        }

        BalancerHttpHandler balancerHttpHandler;
#pragma warning disable CA2000 // Dispose objects before losing scope
        handler = balancerHttpHandler = new BalancerHttpHandler(handler, ConnectionManager);
#pragma warning restore CA2000 // Dispose objects before losing scope

        if (HttpHandlerType == HttpHandlerType.SocketsHttpHandler)
        {
            var socketsHttpHandler = HttpRequestHelpers.GetHttpHandlerType<SocketsHttpHandler>(handler);
            Debug.Assert(socketsHttpHandler is not null, "Should have handler with this handler type");

            BalancerHttpHandler.ConfigureSocketsHttpHandlerSetup(socketsHttpHandler, balancerHttpHandler.OnConnect);
        }

        // Use HttpMessageInvoker instead of HttpClient because it is faster
        // and we don't need client's features.
        return new HttpMessageInvoker(handler, disposeHandler: true);
    }

    internal void RegisterActiveCall(IDisposable grpcCall)
    {
        lock (_lock)
        {
            // Test the disposed flag inside the lock to ensure there is no chance of a race and adding a call after dispose.
            // Note that a GrpcCall has been created but hasn't been started. The error will prevent it from starting.
            ObjectDisposedException.ThrowIf(Disposed, typeof(GrpcChannel));

            _activeCalls.Add(grpcCall);
        }
    }

    internal void FinishActiveCall(IDisposable grpcCall)
    {
        lock (_lock)
            _activeCalls.Remove(grpcCall);
    }

    internal GrpcMethodInfo GetCachedGrpcMethodInfo(IMethod method)
        => _methodInfoCache.GetOrAdd(method, _createMethodInfoFunc);

    private GrpcMethodInfo CreateMethodInfo(IMethod method)
    {
        var uri = new Uri(method.FullName, UriKind.Relative);
        var scope = new GrpcCallScope(method.Type, uri);
        var methodConfig = ResolveMethodConfig(method);

        var uriBuilder = new UriBuilder(Address)
        {
            Path = method.FullName,

            // The Uri used to create HttpRequestMessage must have a http or https scheme.
            Scheme = IsSecure ? Uri.UriSchemeHttps : Uri.UriSchemeHttp,
        };

        // A Uri with a http or https scheme requires a host name.
        // Triple slash URIs, e.g. dns:///custom-value, won't have a host and UriBuilder throws an error.
        // Add a temp value as the host. The tempuri.org host may show up in some logging but it will
        // get replaced in the final HTTP request address by the load balancer.
        if (string.IsNullOrEmpty(uriBuilder.Host))
        {
            // .invalid is reserved for temporary host names.
            // https://datatracker.ietf.org/doc/html/rfc2606#section-2
            uriBuilder.Host = "loadbalancer.temporary.invalid";
        }

        return new GrpcMethodInfo(scope, uriBuilder.Uri, methodConfig);
    }

    private MethodConfig? ResolveMethodConfig(IMethod method)
    {
        if (_serviceConfigMethods is not null)
        {
            if (_serviceConfigMethods.TryGetValue(new MethodKey(method.ServiceName, method.Name), out MethodConfig? methodConfig))
                return methodConfig;

            if (_serviceConfigMethods.TryGetValue(new MethodKey(method.ServiceName, null), out methodConfig))
                return methodConfig;

            if (_serviceConfigMethods.TryGetValue(new MethodKey(null, null), out methodConfig))
                return methodConfig;
        }

        return null;
    }

    private void ValidateChannelCredentials()
    {
        if (IsSecure && Address.Scheme == Uri.UriSchemeHttp)
            throw new InvalidOperationException($"Channel is configured with secure channel credentials and can't use a HttpClient with a '{Address.Scheme}' scheme.");

        if (!IsSecure && Address.Scheme == Uri.UriSchemeHttps)
            throw new InvalidOperationException($"Channel is configured with insecure channel credentials and can't use a HttpClient with a '{Address.Scheme}' scheme.");
    }

    /// <summary>Create a new <see cref="CallInvoker"/> for the channel</summary>
    /// <returns>A new <see cref="CallInvoker"/></returns>
    public override CallInvoker CreateCallInvoker()
    {
        ObjectDisposedException.ThrowIf(Disposed, typeof(GrpcChannel));
        return new HttpClientCallInvoker(this);
    }

    /// <summary>Creates a <see cref="GrpcChannel"/> for the specified address</summary>
    /// <param name="address">The address the channel will use</param>
    /// <returns>A new instance of <see cref="GrpcChannel"/></returns>
    public static GrpcChannel ForAddress(string address)
        => ForAddress(address, new GrpcChannelOptions());

    /// <summary>Creates a <see cref="GrpcChannel"/> for the specified address and configuration options</summary>
    /// <param name="address">The address the channel will use</param>
    /// <param name="channelOptions">The channel configuration options</param>
    /// <returns>A new instance of <see cref="GrpcChannel"/></returns>
    public static GrpcChannel ForAddress(string address, GrpcChannelOptions channelOptions)
        => ForAddress(new Uri(address), channelOptions);

    /// <summary>Creates a <see cref="GrpcChannel"/> for the specified address</summary>
    /// <param name="address">The address the channel will use</param>
    /// <returns>A new instance of <see cref="GrpcChannel"/></returns>
    public static GrpcChannel ForAddress(Uri address)
        => ForAddress(address, new GrpcChannelOptions());

    /// <summary>Creates a <see cref="GrpcChannel"/> for the specified address and configuration options</summary>
    /// <param name="address">The address the channel will use</param>
    /// <param name="channelOptions">The channel configuration options</param>
    /// <returns>A new instance of <see cref="GrpcChannel"/></returns>
    public static GrpcChannel ForAddress(Uri address, GrpcChannelOptions channelOptions)
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(channelOptions);

        if (channelOptions.HttpClient is not null && channelOptions.HttpHandler is not null)
        {
            throw new ArgumentException($"{nameof(GrpcChannelOptions.HttpClient)} and {nameof(GrpcChannelOptions.HttpHandler)} have been configured. " +
                $"Only one HTTP caller can be specified");
        }

        return new GrpcChannel(address, channelOptions);
    }

    /// <summary>Creates a <see cref="GrpcChannel"/> for the specified address</summary>
    /// <param name="ipAddress">IP address the channel will use</param>
    /// <param name="port">Port the channel will use</param>
    /// <param name="scheme">Scheme the channel will use</param>
    /// <param name="withoutSSL">Without certificate validation</param>
    /// <returns>A new instance of <see cref="GrpcChannel"/></returns>
    public static GrpcChannel ForAddress(IPAddress ipAddress, int port, string scheme = "https", bool withoutSSL = true)
    {
        ArgumentNullException.ThrowIfNull(ipAddress);
        var httpHandler = new HttpClientHandler();

        if (withoutSSL)
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var channelOptions = new GrpcChannelOptions { HttpHandler = httpHandler };
        return ForAddress($"{scheme}://{ipAddress}:{port}", channelOptions);
    }

    /// <summary>
    /// Allows explicitly requesting channel to connect without starting an RPC.
    /// Returned task completes once <see cref="State"/> Ready was seen.
    /// There is no need to call this explicitly unless your use case requires that.
    /// Starting an RPC on a new channel will request connection implicitly.
    /// <para>
    /// This API is only supported when the channel is configured with a
    /// <see cref="SocketsHttpHandler"/> HTTP transport.
    /// </para>
    /// <para>
    /// Note: Experimental API that can change or be removed without any prior notice.
    /// </para>
    /// </summary>
    /// <param name="token">The cancellation token.</param>
    /// <returns></returns>
    public Task ConnectAsync(CancellationToken token = default)
    {
        ObjectDisposedException.ThrowIf(Disposed, typeof(GrpcChannel));

        ValidateHttpHandlerSupportsConnectivity();
        return ConnectionManager.ConnectAsync(waitForReady: true, token);
    }

    /// <summary>
    /// Gets current connectivity state of this channel.
    /// After the channel has been shutdown, <see cref="ConnectivityState.Shutdown"/> is returned.
    /// <para>
    /// This API is only supported when the channel is configured with a
    /// <see cref="SocketsHttpHandler"/> HTTP transport
    /// </para>
    /// <para>Note: Experimental API that can change or be removed without any prior notice</para>
    /// </summary>
    public ConnectivityState State
    {
        get
        {
            ValidateHttpHandlerSupportsConnectivity();
            return ConnectionManager.State;
        }
    }

    /// <summary>
    /// Wait for channel's state to change. The task completes when <see cref="State"/> becomes
    /// different from <paramref name="lastObservedState"/>.
    /// <para>
    /// This API is only supported when the channel is configured with a
    /// <see cref="SocketsHttpHandler"/> HTTP transport
    /// </para>
    /// <para>Note: Experimental API that can change or be removed without any prior notice</para>
    /// </summary>
    /// <param name="lastObservedState">The last observed state. The task completes when <see cref="State"/> becomes different from this value</param>
    /// <param name="token">The cancellation token</param>
    /// <returns>The task object representing the asynchronous operation</returns>
    public Task WaitForStateChangedAsync(ConnectivityState lastObservedState, CancellationToken token = default)
    {
        ObjectDisposedException.ThrowIf(Disposed, typeof(GrpcChannel));

        ValidateHttpHandlerSupportsConnectivity();
        return ConnectionManager.WaitForStateChangedAsync(lastObservedState, waitForState: null, token);
    }

    /// <summary>
    /// Releases the resources used by the <see cref="GrpcChannel"/> class.
    /// Clients created with the channel can't be used after the channel is disposed
    /// </summary>
    public void Dispose()
    {
        IDisposable[]? activeCallsCopy = null;

        lock (_lock)
        {
            // Check and set disposed flag inside lock.
            if (Disposed)
                return;

            if (_activeCalls.Count > 0)
                activeCallsCopy = [.. _activeCalls];

            Disposed = true;
        }

        // Dispose calls outside of lock to avoid chance of deadlock.
        if (activeCallsCopy is not null)
        {
            foreach (var activeCall in activeCallsCopy)
                activeCall.Dispose();
        }

        if (_shouldDisposeHttpClient)
            HttpInvoker.Dispose();

        ConnectionManager.Dispose();
    }

    internal bool TryAddToRetryBuffer(long messageSize)
    {
        lock (_lock)
        {
            if (CurrentRetryBufferSize + messageSize > MaxRetryBufferSize)
                return false;

            CurrentRetryBufferSize += messageSize;
            return true;
        }
    }

    internal void RemoveFromRetryBuffer(long messageSize)
    {
        lock (_lock)
            CurrentRetryBufferSize -= messageSize;
    }

    internal int GetRandomNumber(int minValue, int maxValue)
    {
        Debug.Assert(RandomGenerator is not null);

        lock (_lock)
            return RandomGenerator.Next(minValue, maxValue);
    }

    private sealed class SubChannelTransportFactory : ISubchannelTransportFactory
    {
        private readonly GrpcChannel _channel;

        public SubChannelTransportFactory(GrpcChannel channel)
            => _channel = channel;

        public ISubchannelTransport Create(Subchannel subchannel)
        {
            if (_channel.HttpHandlerType == HttpHandlerType.SocketsHttpHandler)
            {
                return new SocketConnectivitySubchannelTransport(
                    subchannel,
                    SocketConnectivitySubchannelTransport.SocketPingInterval,
                    _channel.ConnectTimeout,
                    _channel.ConnectionIdleTimeout ?? TimeSpan.FromMinutes(1),
                    socketConnect: null);
            }

            return new PassiveSubchannelTransport(subchannel);
        }
    }

    private readonly struct MethodKey : IEquatable<MethodKey>
    {
        public MethodKey(string? service, string? method)
        {
            Service = service;
            Method = method;
        }

        public string? Service { get; }
        public string? Method { get; }

        public override bool Equals(object? obj) => obj is MethodKey n && Equals(n);

        // Service and method names are case sensitive.
        public bool Equals(MethodKey other) => other.Service == Service && other.Method == Method;

        public override int GetHashCode() =>
            (Service != null ? StringComparer.Ordinal.GetHashCode(Service) : 0) ^
            (Method != null ? StringComparer.Ordinal.GetHashCode(Method) : 0);
    }

    private readonly record struct HttpHandlerContext(HttpHandlerType HttpHandlerType, TimeSpan? ConnectTimeout = null, TimeSpan? ConnectionIdleTimeout = null);
}

internal enum HttpHandlerType
{
    SocketsHttpHandler,
    HttpClientHandler,
    WinHttpHandler,
    Custom
}
