namespace IcyRain.Grpc.AspNetCore;

/// <summary>Options used to configure service instances</summary>
public class GrpcServiceOptions
{
    internal int? _maxReceiveMessageSize;
    internal int? _maxSendMessageSize;
    internal bool _maxSendMessageSizeSpecified;
    internal bool _maxReceiveMessageSizeSpecified;

    /// <summary>
    /// Gets or sets the maximum message size in bytes that can be sent from the server.
    /// Attempting to send a message that exceeds the configured maximum message size results in an exception.
    /// <para>
    /// A <c>null</c> value removes the maximum message size limit. Defaults to <c>null</c>.
    /// </para>
    /// </summary>
    public int? MaxSendMessageSize
    {
        get => _maxSendMessageSize;
        set
        {
            _maxSendMessageSize = value;
            MaxSendMessageSizeSpecified = true;
        }
    }

    /// <summary>
    /// Gets or sets a flag indicating whether <see cref="MaxSendMessageSize"/> is specified.
    /// This flag is automatically set to true when <see cref="MaxSendMessageSize"/> is configured.
    /// </summary>
    public bool MaxSendMessageSizeSpecified
    {
        get => _maxSendMessageSizeSpecified;
        set
        {
            _maxSendMessageSizeSpecified = value;
            if (!_maxSendMessageSizeSpecified)
            {
                _maxSendMessageSize = null;
            }
        }
    }

    /// <summary>
    /// Gets or sets the maximum message size in bytes that can be received by the server.
    /// If the server receives a message that exceeds this limit, it throws an exception.
    /// <para>
    /// A <c>null</c> value removes the maximum message size limit. Defaults to 4,194,304 (4 MB).
    /// </para>
    /// </summary>
    public int? MaxReceiveMessageSize
    {
        get => _maxReceiveMessageSize;
        set
        {
            _maxReceiveMessageSize = value;
            MaxReceiveMessageSizeSpecified = true;
        }
    }

    /// <summary>
    /// Gets or sets a flag indicating whether <see cref="MaxReceiveMessageSize"/> is specified.
    /// This flag is automatically set to true when <see cref="MaxReceiveMessageSize"/> is configured.
    /// </summary>
    public bool MaxReceiveMessageSizeSpecified
    {
        get => _maxReceiveMessageSizeSpecified;
        set
        {
            _maxReceiveMessageSizeSpecified = value;
            if (!_maxReceiveMessageSizeSpecified)
            {
                _maxReceiveMessageSize = null;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether detailed error messages are sent to the peer.
    /// Detailed error messages include details from exceptions thrown on the server.
    /// </summary>
    public bool? EnableDetailedErrors { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gRPC should ignore calls to unknown services and methods.
    /// If set to <c>true</c>, calls to unknown services and methods won't return an 'UNIMPLEMENTED' status,
    /// and the request will pass to the next registered middleware in ASP.NET Core.
    /// </summary>
    public bool? IgnoreUnknownServices { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether creating a service is suppressed when handling a gRPC call.
    /// </summary>
    public bool SuppressCreatingService { get; set; }
}

/// <summary>
/// Options used to configure the specified service type instances. These options override globally set options.
/// </summary>
/// <typeparam name="TService">The service type to configure.</typeparam>
public class GrpcServiceOptions<TService> : GrpcServiceOptions where TService : class
{
}
