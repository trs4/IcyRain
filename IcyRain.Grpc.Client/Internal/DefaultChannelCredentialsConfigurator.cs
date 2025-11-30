using System;
using System.Collections.Generic;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal;

internal sealed class DefaultChannelCredentialsConfigurator : ChannelCredentialsConfiguratorBase
{
    private readonly bool _allowInsecureChannelCallCredentials;

    public DefaultChannelCredentialsConfigurator(bool allowInsecureChannelCallCredentials)
        => _allowInsecureChannelCallCredentials = allowInsecureChannelCallCredentials;

    public bool? IsSecure { get; private set; }

    public List<CallCredentials>? CallCredentials { get; private set; }

    public override void SetCompositeCredentials(object state, ChannelCredentials channelCredentials, CallCredentials callCredentials)
    {
        channelCredentials.InternalPopulateConfiguration(this, state);

        if (!(IsSecure ?? false) && !_allowInsecureChannelCallCredentials)
        {
            throw new InvalidOperationException($"CallCredentials can't be composed with {channelCredentials.GetType().Name}. " +
                $"CallCredentials must be used with secure channel credentials like SslCredentials or by enabling " +
                $"{nameof(GrpcChannelOptions)}.{nameof(GrpcChannelOptions.UnsafeUseInsecureChannelCallCredentials)} on the channel.");
        }

        if (callCredentials is not null)
            (CallCredentials ??= []).Add(callCredentials);
    }

    public override void SetInsecureCredentials(object state) => IsSecure = false;

    public override void SetSslCredentials(object state, string? rootCertificates, KeyCertificatePair? keyCertificatePair, VerifyPeerCallback? verifyPeerCallback)
    {
        if (!string.IsNullOrEmpty(rootCertificates) || keyCertificatePair != null || verifyPeerCallback is not null)
        {
            throw new InvalidOperationException(
                $"{nameof(SslCredentials)} with non-null arguments is not supported by {nameof(GrpcChannel)}. " +
                $"{nameof(GrpcChannel)} uses HttpClient to make gRPC calls and HttpClient automatically loads root certificates from the operating system certificate store. " +
                $"Client certificates should be configured on HttpClient. See https://aka.ms/aspnet/grpc/certauth for details.");
        }

        IsSecure = true;
    }

}
