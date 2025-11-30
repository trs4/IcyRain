using System.Collections.Generic;

namespace IcyRain.Grpc.Client.Configuration;

/// <summary>The retry throttling policy for a server</summary>
public sealed class RetryThrottlingPolicy : ConfigObject
{
    internal const string MaxTokensPropertyName = "maxTokens";
    internal const string TokenRatioPropertyName = "tokenRatio";

    /// <summary>Initializes a new instance of the <see cref="RetryThrottlingPolicy"/> class</summary>
    public RetryThrottlingPolicy() { }

    internal RetryThrottlingPolicy(IDictionary<string, object> inner) : base(inner) { }

    /// <summary>
    /// Gets or sets the maximum number of tokens.
    /// The number of tokens starts at <see cref="MaxTokens"/> and the token count will
    /// always be between 0 and <see cref="MaxTokens"/>.
    /// This property is required and must be greater than zero.
    /// </summary>
    public int? MaxTokens
    {
        get => GetValue<int>(MaxTokensPropertyName);
        set => SetValue(MaxTokensPropertyName, value);
    }

    /// <summary>
    /// Gets or sets the amount of tokens to add on each successful call. Typically this will
    /// be some number between 0 and 1, e.g., 0.1.
    /// This property is required and must be greater than zero. Up to 3 decimal places are supported.
    /// </summary>
    public double? TokenRatio
    {
        get => GetValue<double>(TokenRatioPropertyName);
        set => SetValue(TokenRatioPropertyName, value);
    }

}
