using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IcyRain.Grpc.Client.Internal.Http;

namespace IcyRain.Grpc.Client.Balancer.Internal;

internal sealed partial class BalancerHttpHandler : DelegatingHandler
{
    private static readonly object SetupLock = new object();

    internal const string WaitForReadyKey = "WaitForReady";
    internal const string SubchannelKey = "Subchannel";
    internal const string CurrentAddressKey = "CurrentAddress";
    internal const string IsSocketsHttpHandlerSetupKey = "IsSocketsHttpHandlerSetup";

    private readonly ConnectionManager _manager;

    public BalancerHttpHandler(HttpMessageHandler innerHandler, ConnectionManager manager)
        : base(innerHandler)
        => _manager = manager;

    internal static bool IsSocketsHttpHandlerSetup(SocketsHttpHandler socketsHttpHandler)
    {
        lock (SetupLock)
        {
            return socketsHttpHandler.Properties.TryGetValue(IsSocketsHttpHandlerSetupKey, out var value) &&
                value is bool isEnabled &&
                isEnabled;
        }
    }

    internal static void ConfigureSocketsHttpHandlerSetup(
        SocketsHttpHandler socketsHttpHandler,
        Func<SocketsHttpConnectionContext, CancellationToken, ValueTask<Stream>> connectCallback)
    {
        // We're modifying the SocketsHttpHandler and nothing prevents two threads from creating a
        // channel with the same handler on different threads.
        // Place handler reads and modifications in a lock to ensure there is no chance of race conditions.
        // This is a static lock but it is only called once when a channel is created and the logic
        // inside it will complete straight away. Shouldn't have any performance impact.
        lock (SetupLock)
        {
            if (!IsSocketsHttpHandlerSetup(socketsHttpHandler))
            {
                Debug.Assert(socketsHttpHandler.ConnectCallback == null, "ConnectCallback should be null to get to this point.");

                socketsHttpHandler.ConnectCallback = connectCallback;
                socketsHttpHandler.Properties[IsSocketsHttpHandlerSetupKey] = true;
            }
        }
    }

    internal async ValueTask<Stream> OnConnect(SocketsHttpConnectionContext context, CancellationToken token)
    {
        if (!context.InitialRequestMessage.TryGetOption<Subchannel>(SubchannelKey, out var subchannel))
            throw new InvalidOperationException($"Unable to get subchannel from {nameof(HttpRequestMessage)}.");

        if (!context.InitialRequestMessage.TryGetOption<BalancerAddress>(CurrentAddressKey, out var currentAddress))
            throw new InvalidOperationException($"Unable to get current address from {nameof(HttpRequestMessage)}.");

        Debug.Assert(context.DnsEndPoint.Equals(currentAddress.EndPoint), "Context endpoint should equal address endpoint.");
        return await subchannel.Transport.GetStreamAsync(currentAddress.EndPoint, token).ConfigureAwait(false);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        if (request.RequestUri is null)
            throw new InvalidOperationException("Request message URI not set.");

        var waitForReady = false;

        if (request.TryGetOption<bool>(WaitForReadyKey, out var value))
            waitForReady = value;

        await _manager.ConnectAsync(waitForReady: false, token).ConfigureAwait(false);
        var pickContext = new PickContext { Request = request };
        var result = await _manager.PickAsync(pickContext, waitForReady, token).ConfigureAwait(false);
        var address = result.Address;
        var addressEndpoint = address.EndPoint;

        // Update request host if required.
        if (!request.RequestUri.IsAbsoluteUri ||
            request.RequestUri.Host != addressEndpoint.Host ||
            request.RequestUri.Port != addressEndpoint.Port)
        {
            var uriBuilder = new UriBuilder(request.RequestUri)
            {
                Host = addressEndpoint.Host,
                Port = addressEndpoint.Port
            };

            request.RequestUri = uriBuilder.Uri;

            if (address.Attributes.TryGetValue(ConnectionManager.HostOverrideKey, out var hostOverride))
                request.Headers.TryAddWithoutValidation("Host", hostOverride);
        }

        // Set sub-connection onto request.
        // Will be used to get a stream in SocketsHttpHandler.ConnectCallback.
        request.SetOption(SubchannelKey, result.Subchannel);
        request.SetOption(CurrentAddressKey, address);

        var responseMessageTask = base.SendAsync(request, token);
        result.SubchannelCallTracker?.Start();

        try
        {
            var responseMessage = await responseMessageTask.ConfigureAwait(false);

            // TODO(JamesNK): This doesn't take into account long running streams.
            // If there is response content then we need to wait until it is read to the end
            // or the request is disposed.
            result.SubchannelCallTracker?.Complete(new CompletionContext
            {
                Address = address
            });

            return responseMessage;
        }
        catch (Exception ex)
        {
            result.SubchannelCallTracker?.Complete(new CompletionContext
            {
                Address = address,
                Error = ex
            });

            throw;
        }
    }

}
