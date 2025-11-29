using Microsoft.Extensions.Options;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class GrpcServiceOptionsSetup : IConfigureOptions<GrpcServiceOptions>
{
    public void Configure(GrpcServiceOptions options)
    {
        if (!options.MaxReceiveMessageSizeSpecified)
            options._maxReceiveMessageSize = MethodOptions.DefaultReceiveMaxMessageSize;
    }
}

internal sealed class GrpcServiceOptionsSetup<TService> : IConfigureOptions<GrpcServiceOptions<TService>> where TService : class
{
    private readonly GrpcServiceOptions _options;

    public GrpcServiceOptionsSetup(IOptions<GrpcServiceOptions> options)
        => _options = options.Value;

    public void Configure(GrpcServiceOptions<TService> options)
    {
        options._maxReceiveMessageSize = _options._maxReceiveMessageSize;
        options._maxReceiveMessageSizeSpecified = _options._maxReceiveMessageSizeSpecified;
        options._maxSendMessageSize = _options._maxSendMessageSize;
        options._maxSendMessageSizeSpecified = _options._maxSendMessageSizeSpecified;

        options.EnableDetailedErrors = _options.EnableDetailedErrors;
        options.IgnoreUnknownServices = _options.IgnoreUnknownServices;
    }

}
