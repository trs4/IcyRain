using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client.Internal;
using IcyRain.Grpc.Client.Configuration;

namespace IcyRain.Grpc.Client.Internal;

/// <summary>
/// Cached log scope and URI for a gRPC <see cref="IMethod"/>.
/// </summary>
internal sealed class GrpcMethodInfo
{
    public GrpcMethodInfo(GrpcCallScope logScope, Uri callUri, MethodConfig? methodConfig)
    {
        LogScope = logScope;
        CallUri = callUri;
        MethodConfig = CreateMethodConfig(methodConfig);
    }

    private static MethodConfigInfo? CreateMethodConfig(MethodConfig? methodConfig)
    {
        if (methodConfig is null)
            return null;
        
        if (methodConfig.RetryPolicy is not null && methodConfig.HedgingPolicy is not null)
            throw new InvalidOperationException("Method config can't have a retry policy and hedging policy.");

        var m = new MethodConfigInfo();

        if (methodConfig.RetryPolicy is not null)
            m.RetryPolicy = CreateRetryPolicy(methodConfig.RetryPolicy);

        if (methodConfig.HedgingPolicy is not null)
            m.HedgingPolicy = CreateHedgingPolicy(methodConfig.HedgingPolicy);

        return m;
    }

    internal static RetryPolicyInfo CreateRetryPolicy(RetryPolicy r)
    {
        if (!(r.MaxAttempts > 1))
            throw new InvalidOperationException("Retry policy max attempts must be greater than 1.");
        
        if (!(r.InitialBackoff > TimeSpan.Zero))
            throw new InvalidOperationException("Retry policy initial backoff must be greater than zero.");
        
        if (!(r.MaxBackoff > TimeSpan.Zero))
            throw new InvalidOperationException("Retry policy maximum backoff must be greater than zero.");
        
        if (!(r.BackoffMultiplier > 0))
            throw new InvalidOperationException("Retry policy backoff multiplier must be greater than 0.");
        
        if (!(r.RetryableStatusCodes.Count > 0))
            throw new InvalidOperationException("Retry policy must specify at least 1 retryable status code.");
        
        return new RetryPolicyInfo
        {
            MaxAttempts = r.MaxAttempts.Value,
            InitialBackoff = r.InitialBackoff.Value,
            MaxBackoff = r.MaxBackoff.Value,
            BackoffMultiplier = r.BackoffMultiplier.Value,
            RetryableStatusCodes = [.. r.RetryableStatusCodes]
        };
    }

    internal static HedgingPolicyInfo CreateHedgingPolicy(HedgingPolicy h)
    {
        if (!(h.MaxAttempts > 1))
            throw new InvalidOperationException("Hedging policy max attempts must be greater than 1.");
        
        if (h.HedgingDelay is not null && h.HedgingDelay < TimeSpan.Zero)
            throw new InvalidOperationException("Hedging policy delay must be equal or greater than zero.");
        
        return new HedgingPolicyInfo
        {
            MaxAttempts = h.MaxAttempts.Value,
            HedgingDelay = h.HedgingDelay ?? TimeSpan.Zero,
            NonFatalStatusCodes = [.. h.NonFatalStatusCodes]
        };
    }

    public GrpcCallScope LogScope { get; }

    public Uri CallUri { get; }

    public MethodConfigInfo? MethodConfig { get; }
}

internal sealed class MethodConfigInfo
{
    public RetryPolicyInfo? RetryPolicy { get; set; }

    public HedgingPolicyInfo? HedgingPolicy { get; set; }
}

internal sealed class RetryPolicyInfo
{
    public int MaxAttempts { get; init; }

    public TimeSpan InitialBackoff { get; init; }

    public TimeSpan MaxBackoff { get; init; }

    public double BackoffMultiplier { get; init; }

    public List<StatusCode> RetryableStatusCodes { get; init; } = default!;
}

internal sealed class HedgingPolicyInfo
{
    public int MaxAttempts { get; set; }

    public TimeSpan HedgingDelay { get; set; }

    public List<StatusCode> NonFatalStatusCodes { get; init; } = default!;
}
