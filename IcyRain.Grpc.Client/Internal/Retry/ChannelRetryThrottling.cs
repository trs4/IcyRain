using System;
using System.Diagnostics;
using System.Threading;

namespace IcyRain.Grpc.Client.Internal.Retry;

internal sealed partial class ChannelRetryThrottling
{
    private readonly object _lock = new object();
    private readonly double _tokenRatio;
    private readonly int _maxTokens;

    private double _tokenCount;
    private readonly double _tokenThreshold;
    private bool _isRetryThrottlingActive;

    public ChannelRetryThrottling(int maxTokens, double tokenRatio)
    {
        // Truncate token ratio to 3 decimal places
        _tokenRatio = Math.Truncate(tokenRatio * 1000) / 1000;

        _maxTokens = maxTokens;
        _tokenCount = maxTokens;
        _tokenThreshold = _tokenCount / 2;
    }

    public bool IsRetryThrottlingActive()
    {
        lock (_lock)
            return _isRetryThrottlingActive;
    }

    public void CallSuccess()
    {
        lock (_lock)
        {
            _tokenCount = Math.Min(_tokenCount + _tokenRatio, _maxTokens);
            UpdateRetryThrottlingActive();
        }
    }

    public void CallFailure()
    {
        lock (_lock)
        {
            _tokenCount = Math.Max(_tokenCount - 1, 0);
            UpdateRetryThrottlingActive();
        }
    }

    private void UpdateRetryThrottlingActive()
    {
        Debug.Assert(Monitor.IsEntered(_lock));

        var newRetryThrottlingActive = _tokenCount <= _tokenThreshold;

        if (newRetryThrottlingActive != _isRetryThrottlingActive)
            _isRetryThrottlingActive = newRetryThrottlingActive;
    }

}
