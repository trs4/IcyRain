using System;
using System.Collections.Generic;
using Grpc.Core;
using IcyRain.Grpc.Client.Internal.Configuration;

namespace IcyRain.Grpc.Client.Configuration;

/// <summary>The retry policy for outgoing calls</summary>
public sealed class RetryPolicy : ConfigObject
{
    internal const string MaxAttemptsPropertyName = "maxAttempts";
    internal const string InitialBackoffPropertyName = "initialBackoff";
    internal const string MaxBackoffPropertyName = "maxBackoff";
    internal const string BackoffMultiplierPropertyName = "backoffMultiplier";
    internal const string RetryableStatusCodesPropertyName = "retryableStatusCodes";

    private readonly ConfigProperty<Values<StatusCode, object>, IList<object>> _retryableStatusCodes =
        new(i => new Values<StatusCode, object>(i ?? [],
            s => ConvertHelpers.ConvertStatusCode(s),
            s => ConvertHelpers.ConvertStatusCode(s.ToString()!)), RetryableStatusCodesPropertyName);

    /// <summary>Initializes a new instance of the <see cref="RetryPolicy"/> class</summary>
    public RetryPolicy() { }

    internal RetryPolicy(IDictionary<string, object> inner) : base(inner) { }

    /// <summary>
    /// Gets or sets the maximum number of call attempts. This value includes the original attempt.
    /// This property is required and must be greater than 1.
    /// This value is limited by <see cref="GrpcChannelOptions.MaxRetryAttempts"/>
    /// </summary>
    public int? MaxAttempts
    {
        get => GetValue<int>(MaxAttemptsPropertyName);
        set => SetValue(MaxAttemptsPropertyName, value);
    }

    /// <summary>
    /// Gets or sets the initial backoff.
    /// A randomized delay between 0 and the current backoff value will determine when the next
    /// retry attempt is made.
    /// This property is required and must be greater than zero.
    /// <para>
    /// The backoff will be multiplied by <see cref="BackoffMultiplier"/> after each retry
    /// attempt and will increase exponentially when the multiplier is greater than 1.
    /// </para>
    /// </summary>
    public TimeSpan? InitialBackoff
    {
        get => ConvertHelpers.ConvertDurationText(GetValue<string>(InitialBackoffPropertyName));
        set => SetValue(InitialBackoffPropertyName, ConvertHelpers.ToDurationText(value));
    }

    /// <summary>
    /// Gets or sets the maximum backoff.
    /// The maximum backoff places an upper limit on exponential backoff growth.
    /// This property is required and must be greater than zero.
    /// </summary>
    public TimeSpan? MaxBackoff
    {
        get => ConvertHelpers.ConvertDurationText(GetValue<string>(MaxBackoffPropertyName));
        set => SetValue(MaxBackoffPropertyName, ConvertHelpers.ToDurationText(value));
    }

    /// <summary>
    /// Gets or sets the backoff multiplier.
    /// The backoff will be multiplied by <see cref="BackoffMultiplier"/> after each retry
    /// attempt and will increase exponentially when the multiplier is greater than 1.
    /// This property is required and must be greater than 0.
    /// </summary>
    public double? BackoffMultiplier
    {
        get => GetValue<double>(BackoffMultiplierPropertyName);
        set => SetValue(BackoffMultiplierPropertyName, value);
    }

    /// <summary>Gets a collection of status codes which may be retried. At least one status code is required</summary>
    public IList<StatusCode> RetryableStatusCodes => _retryableStatusCodes.GetValue(this)!;
}
