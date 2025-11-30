using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal;

internal static class GrpcProtocolConstants
{
    public static readonly Version Http2Version = System.Net.HttpVersion.Version20;

    internal const string GrpcContentType = "application/grpc";
    internal static readonly MediaTypeHeaderValue GrpcContentTypeHeaderValue = new MediaTypeHeaderValue("application/grpc");

    internal const string TimeoutHeader = "grpc-timeout";
    internal const string MessageEncodingHeader = "grpc-encoding";

    internal const string StatusTrailer = "grpc-status";
    internal const string MessageTrailer = "grpc-message";

    internal const string IdentityGrpcEncoding = "identity";

    internal const string MessageAcceptEncodingHeader = "grpc-accept-encoding";
    internal const string CompressionRequestAlgorithmHeader = "grpc-internal-encoding-request";

    internal const string RetryPushbackHeader = "grpc-retry-pushback-ms";
    internal const string RetryPreviousAttemptsHeader = "grpc-previous-rpc-attempts";

    internal const string DropRequestTrailer = "grpc-internal-drop-request";

    internal const int MessageDelimiterSize = 4; // how many bytes it takes to encode "Message-Length"
    internal const int HeaderSize = MessageDelimiterSize + 1; // message length + compression flag

    internal static readonly string UserAgentHeader;
    internal static readonly string UserAgentHeaderValue;
    internal static readonly string TEHeader;
    internal static readonly string TEHeaderValue;

    internal static readonly Status DeadlineExceededStatus = new Status(StatusCode.DeadlineExceeded, string.Empty);
    internal static readonly Status ThrottledStatus = new Status(StatusCode.Cancelled, "Retries stopped because retry throttling is active.");

    internal static Status CreateClientCanceledStatus(Exception? exception) => new Status(StatusCode.Cancelled, "Call canceled by the client.", exception);
    internal static Status CreateDisposeCanceledStatus(Exception? exception) => new Status(StatusCode.Cancelled, "gRPC call disposed.", exception);

    /// <summary>
    /// Gets key value pairs used by debugging. These are provided as an enumerator instead of a dictionary
    /// because it's one method to implement an enumerator on gRPC calls compared to a dozen members for a dictionary.
    /// </summary>
    public static IEnumerator<KeyValuePair<string, object>> GetDebugEnumerator(ChannelBase channel, IMethod method, object? request)
    {
        const string MethodKey = "Method";
        const string ChannelKey = "Channel";
        const string RequestKey = "Request";

        yield return new KeyValuePair<string, object>(ChannelKey, channel);
        yield return new KeyValuePair<string, object>(MethodKey, method);
        if (request != null)
        {
            yield return new KeyValuePair<string, object>(RequestKey, request);
        }
    }

    static GrpcProtocolConstants()
    {
        UserAgentHeader = "User-Agent";
        UserAgentHeaderValue = UserAgentGenerator.GetUserAgentString();
        TEHeader = "TE";
        TEHeaderValue = "trailers";
    }

}
