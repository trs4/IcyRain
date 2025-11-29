using Grpc.Core;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal static class StatusCodeExtensions
{
    public static string ToTrailerString(this StatusCode status) => status switch
    {
        StatusCode.OK => "0",
        StatusCode.Cancelled => "1",
        StatusCode.Unknown => "2",
        StatusCode.InvalidArgument => "3",
        StatusCode.DeadlineExceeded => "4",
        StatusCode.NotFound => "5",
        StatusCode.AlreadyExists => "6",
        StatusCode.PermissionDenied => "7",
        StatusCode.ResourceExhausted => "8",
        StatusCode.FailedPrecondition => "9",
        StatusCode.Aborted => "10",
        StatusCode.OutOfRange => "11",
        StatusCode.Unimplemented => "12",
        StatusCode.Internal => "13",
        StatusCode.Unavailable => "14",
        StatusCode.DataLoss => "15",
        StatusCode.Unauthenticated => "16",
        _ => status.ToString("D"),
    };
}
