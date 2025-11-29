using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal static class GrpcProtocolConstants
{
    internal const string GrpcContentType = "application/grpc";
    internal const string GrpcWebContentType = "application/grpc-web";
    internal const string GrpcWebTextContentType = "application/grpc-web-text";
    internal static readonly string Http2Protocol = HttpProtocol.Http2;
    internal static readonly string TimeoutHeader = HeaderNames.GrpcTimeout;
    internal static readonly string MessageEncodingHeader = HeaderNames.GrpcEncoding;
    internal static readonly string MessageAcceptEncodingHeader = HeaderNames.GrpcAcceptEncoding;
    internal const string CompressionRequestAlgorithmHeader = "grpc-internal-encoding-request";
    internal static readonly string StatusTrailer = HeaderNames.GrpcStatus;
    internal static readonly string MessageTrailer = HeaderNames.GrpcMessage;
    internal const int Http2ResetStreamCancel = 0x8;
    internal const int Http3ResetStreamCancel = 0x010c;

    internal static readonly HashSet<string> FilteredHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        MessageEncodingHeader,
        MessageAcceptEncodingHeader,
        TimeoutHeader,
        HeaderNames.ContentEncoding,
        HeaderNames.ContentType,
        HeaderNames.TE,
        HeaderNames.Host,
        HeaderNames.AcceptEncoding
    };

    internal const string X509SubjectAlternativeNameId = "2.5.29.17";
    internal const string X509SubjectAlternativeNameKey = "x509_subject_alternative_name";
    internal const string X509CommonNameKey = "x509_common_name";

    // Maxmimum deadline of 99999999s is consistent with Grpc.Core
    // https://github.com/grpc/grpc/blob/907a1313a87723774bf59d04ed432602428245c3/src/core/lib/transport/timeout_encoding.h#L32-L34
    internal const long MaxDeadlineTicks = 99999999 * TimeSpan.TicksPerSecond;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsHttp2(string protocol) => HttpProtocol.IsHttp2(protocol);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsHttp3(string protocol) => HttpProtocol.IsHttp3(protocol);

    internal static int GetCancelErrorCode(string protocol) => IsHttp3(protocol) ? Http3ResetStreamCancel : Http2ResetStreamCancel;

    internal const DynamicallyAccessedMemberTypes ServiceAccessibility = ServerDynamicAccessConstants.ServiceAccessibility;
}
