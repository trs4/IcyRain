using System;
using System.Collections.Generic;
using Grpc.Core;
using IcyRain.Grpc.Client.Internal.Configuration;

namespace IcyRain.Grpc.Client.Configuration;

/// <summary>
/// The hedging policy for outgoing calls. Hedged calls may execute more than
/// once on the server, so only idempotent methods should specify a hedging
/// policy.
/// </summary>
/// <remarks>
/// <para>
/// Represents the <c>HedgingPolicy</c> message in <see href="https://github.com/grpc/grpc-proto/blob/master/grpc/service_config/service_config.proto"/>.
/// </para>
/// </remarks>
public sealed class HedgingPolicy : ConfigObject
{
    internal const string MaxAttemptsPropertyName = "maxAttempts";
    internal const string HedgingDelayPropertyName = "hedgingDelay";
    internal const string NonFatalStatusCodesPropertyName = "nonFatalStatusCodes";

    private readonly ConfigProperty<Values<StatusCode, object>, IList<object>> _nonFatalStatusCodes =
        new(i => new Values<StatusCode, object>(i ?? [],
            s => ConvertHelpers.ConvertStatusCode(s),
            s => ConvertHelpers.ConvertStatusCode(s.ToString()!)), NonFatalStatusCodesPropertyName);

    /// <summary>Initializes a new instance of the <see cref="HedgingPolicy"/> class</summary>
    public HedgingPolicy() { }

    internal HedgingPolicy(IDictionary<string, object> inner) : base(inner) { }

    /// <summary>
    /// Gets or sets the maximum number of call attempts. This value includes the original attempt.
    /// The hedging policy will send up to this number of calls.
    ///
    /// This property is required and must be 2 or greater.
    /// This value is limited by <see cref="GrpcChannelOptions.MaxRetryAttempts"/>
    /// </summary>
    public int? MaxAttempts
    {
        get => GetValue<int>(MaxAttemptsPropertyName);
        set => SetValue(MaxAttemptsPropertyName, value);
    }

    /// <summary>
    /// Gets or sets the hedging delay.
    /// The first call will be sent immediately, but the subsequent
    /// hedged call will be sent at intervals of the specified delay.
    /// Set this to 0 or <c>null</c> to immediately send all hedged calls
    /// </summary>
    public TimeSpan? HedgingDelay
    {
        get => ConvertHelpers.ConvertDurationText(GetValue<string>(HedgingDelayPropertyName));
        set => SetValue(HedgingDelayPropertyName, ConvertHelpers.ToDurationText(value));
    }

    /// <summary>
    /// Gets a collection of status codes which indicate other hedged calls may still
    /// succeed. If a non-fatal status code is returned by the server, hedged
    /// calls will continue. Otherwise, outstanding requests will be canceled and
    /// the error returned to the client application layer.
    ///
    /// Specifying status codes is optional
    /// </summary>
    public IList<StatusCode> NonFatalStatusCodes => _nonFatalStatusCodes.GetValue(this)!;
}
