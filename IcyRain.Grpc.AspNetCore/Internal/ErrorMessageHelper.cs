using System;
using IcyRain.Grpc.Service.Internal;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal static class ErrorMessageHelper
{
    internal static string BuildErrorMessage(string message, Exception exception, bool? includeExceptionDetails)
    {
        if (includeExceptionDetails ?? false)
            return message + " " + CommonGrpcProtocolHelpers.ConvertToRpcExceptionMessage(exception);

        return message;
    }

}
