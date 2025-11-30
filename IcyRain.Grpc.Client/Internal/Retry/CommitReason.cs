namespace IcyRain.Grpc.Client.Internal.Retry;

internal enum CommitReason
{
    ResponseHeadersReceived,
    FatalStatusCode,
    ExceededAttemptCount,
    DeadlineExceeded,
    Throttled,
    BufferExceeded,
    PushbackStop,
    UnexpectedError,
    Canceled,
    Drop,
}
