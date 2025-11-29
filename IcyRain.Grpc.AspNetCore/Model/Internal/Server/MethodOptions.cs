using System.Collections.Generic;
using System.Linq;

namespace IcyRain.Grpc.AspNetCore.Internal;

/// <summary>Options used to execute a gRPC method</summary>
internal sealed class MethodOptions
{
    // Default to no send limit and 4mb receive limit.
    // Matches the gRPC C impl defaults
    // https://github.com/grpc/grpc/blob/977df7208a6e3f9a62a6369af5cd6e4b69b4fdec/include/grpc/impl/codegen/grpc_types.h#L413-L416
    internal const int DefaultReceiveMaxMessageSize = 4 * 1024 * 1024;

    /// <summary>Gets the maximum message size in bytes that can be sent from the server</summary>
    public int? MaxSendMessageSize { get; }

    /// <summary>Gets the maximum message size in bytes that can be received by the server</summary>
    public int? MaxReceiveMessageSize { get; }

    /// <summary>
    /// Gets a value indicating whether detailed error messages are sent to the peer.
    /// Detailed error messages include details from exceptions thrown on the server.
    /// </summary>
    public bool? EnableDetailedErrors { get; }

    internal bool SuppressCreatingService { get; }

    private MethodOptions(
        int? maxSendMessageSize,
        int? maxReceiveMessageSize,
        bool? enableDetailedErrors,
        bool suppressCreatingService)
    {
        MaxSendMessageSize = maxSendMessageSize;
        MaxReceiveMessageSize = maxReceiveMessageSize;
        EnableDetailedErrors = enableDetailedErrors;
        SuppressCreatingService = suppressCreatingService;
    }

    /// <summary>
    /// Creates method options by merging together the settings the specificed <see cref="GrpcServiceOptions"/> collection.
    /// The <see cref="GrpcServiceOptions"/> should be ordered with items arranged in ascending order of precedence.
    /// </summary>
    /// <param name="serviceOptions">A collection of <see cref="GrpcServiceOptions"/> instances, arranged in ascending order of precedence.</param>
    /// <returns>A new <see cref="MethodOptions"/> instanced with settings merged from specifid <see cref="GrpcServiceOptions"/> collection.</returns>
    public static MethodOptions Create(IEnumerable<GrpcServiceOptions> serviceOptions)
    {
        int? maxSendMessageSize = null;
        var maxSendMessageSizeConfigured = false;
        int? maxReceiveMessageSize = DefaultReceiveMaxMessageSize;
        var maxReceiveMessageSizeConfigured = false;
        bool? enableDetailedErrors = null;
        bool? suppressCreatingService = null;

        foreach (var options in serviceOptions.Reverse())
        {
            if (!maxSendMessageSizeConfigured && options.MaxSendMessageSizeSpecified)
            {
                maxSendMessageSize = options.MaxSendMessageSize;
                maxSendMessageSizeConfigured = true;
            }

            if (!maxReceiveMessageSizeConfigured && options.MaxReceiveMessageSizeSpecified)
            {
                maxReceiveMessageSize = options.MaxReceiveMessageSize;
                maxReceiveMessageSizeConfigured = true;
            }

            enableDetailedErrors ??= options.EnableDetailedErrors;
            suppressCreatingService ??= options.SuppressCreatingService;
        }

        return new MethodOptions
        (
            maxSendMessageSize: maxSendMessageSize,
            maxReceiveMessageSize: maxReceiveMessageSize,
            enableDetailedErrors: enableDetailedErrors,
            suppressCreatingService: suppressCreatingService ?? false
        );
    }

}
