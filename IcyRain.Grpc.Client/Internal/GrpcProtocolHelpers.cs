using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Grpc.Client.Internal.Http;

namespace IcyRain.Grpc.Client.Internal;

internal static class GrpcProtocolHelpers
{
    public static byte[] ParseBinaryHeader(string base64)
    {
        string decodable = (base64.Length % 4) switch // base64 has the required padding 
        {
            0 => base64, // base64 has the required padding 
            2 => base64 + "==", // 2 chars padding
            3 => base64 + "=", // 3 chars padding
            _ => throw new FormatException("Invalid Base-64 header value."), // length%4 == 1 should be illegal
        };

        return Convert.FromBase64String(decodable);
    }

    public static Metadata BuildMetadata(HttpHeaders responseHeaders)
    {
        var headers = new Metadata();

        // Use NonValidated to avoid race-conditions and because it is faster.
        foreach (var header in responseHeaders.NonValidated)
        {
            if (ShouldSkipHeader(header.Key))
                continue;

            foreach (var value in header.Value)
            {
                if (header.Key.EndsWith(Metadata.BinaryHeaderSuffix, StringComparison.OrdinalIgnoreCase))
                    headers.Add(header.Key, ParseBinaryHeader(value));
                else
                    headers.Add(header.Key, value);
            }
        }

        return headers;
    }

    internal static bool ShouldSkipHeader(string name)
    {
        if (name.Length == 0)
            return false;

        return name[0] switch
        {
            // ASP.NET Core includes pseudo headers in the set of request headers
            // whereas, they are not in gRPC implementations. We will filter them
            // out when we construct the list of headers on the context
            ':' => true,
            // Exclude known grpc headers. This matches Grpc.Core client behavior
            'g' or 'G' => string.Equals(name, GrpcProtocolConstants.StatusTrailer, StringComparison.OrdinalIgnoreCase)
                       || string.Equals(name, GrpcProtocolConstants.MessageTrailer, StringComparison.OrdinalIgnoreCase)
                       || string.Equals(name, GrpcProtocolConstants.MessageEncodingHeader, StringComparison.OrdinalIgnoreCase)
                       || string.Equals(name, GrpcProtocolConstants.MessageAcceptEncodingHeader, StringComparison.OrdinalIgnoreCase),
            // Exclude known HTTP headers. This matches Grpc.Core client behavior
            'c' or 'C' => string.Equals(name, "content-encoding", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(name, "content-type", StringComparison.OrdinalIgnoreCase),
            _ => false,
        };
    }

    private const int MillisecondsPerSecond = 1000;

    /* round an integer up to the next value with three significant figures */
    private static long TimeoutRoundUpToThreeSignificantFigures(long x)
    {
        if (x < 1000)
            return x;

        if (x < 10000)
            return RoundUp(x, 10);

        if (x < 100000)
            return RoundUp(x, 100);

        if (x < 1000000)
            return RoundUp(x, 1000);

        if (x < 10000000)
            return RoundUp(x, 10000);

        if (x < 100000000)
            return RoundUp(x, 100000);

        if (x < 1000000000)
            return RoundUp(x, 1000000);

        return RoundUp(x, 10000000);

        static long RoundUp(long x, long divisor)
            => (x / divisor + Convert.ToInt32(x % divisor != 0)) * divisor;
    }

    private static string FormatTimeout(long value, char ext)
        => value.ToString(CultureInfo.InvariantCulture) + ext;

    private static string EncodeTimeoutSeconds(long sec)
    {
        if (sec % 3600 == 0)
            return FormatTimeout(sec / 3600, 'H');
        else if (sec % 60 == 0)
            return FormatTimeout(sec / 60, 'M');
        else
            return FormatTimeout(sec, 'S');
    }

    private static string EncodeTimeoutMilliseconds(long x)
    {
        x = TimeoutRoundUpToThreeSignificantFigures(x);

        if (x < MillisecondsPerSecond)
            return FormatTimeout(x, 'm');
        else if (x % MillisecondsPerSecond == 0)
            return EncodeTimeoutSeconds(x / MillisecondsPerSecond);
        else
            return FormatTimeout(x, 'm');
    }

    public static string EncodeTimeout(long timeout)
    {
        if (timeout <= 0)
            return "1n";
        else if (timeout < 1000 * MillisecondsPerSecond)
            return EncodeTimeoutMilliseconds(timeout);
        else
            return EncodeTimeoutSeconds(timeout / MillisecondsPerSecond + Convert.ToInt32(timeout % MillisecondsPerSecond != 0));
    }

    internal static string GetRequestEncoding(HttpRequestHeaders headers)
    {
        var grpcRequestEncoding = HttpRequestHelpers.GetHeaderValue(
            headers,
            GrpcProtocolConstants.MessageEncodingHeader,
            first: true);

        return grpcRequestEncoding ?? GrpcProtocolConstants.IdentityGrpcEncoding;
    }

    internal static string GetGrpcEncoding(HttpResponseMessage response)
    {
        var grpcEncoding = HttpRequestHelpers.GetHeaderValue(
            response.Headers,
            GrpcProtocolConstants.MessageEncodingHeader,
            first: true);

        return grpcEncoding ?? GrpcProtocolConstants.IdentityGrpcEncoding;
    }

    internal static AuthInterceptorContext CreateAuthInterceptorContext(Uri baseAddress, IMethod method, CancellationToken token)
    {
        var authority = baseAddress.Authority;

        if (baseAddress.Scheme == Uri.UriSchemeHttps && authority.EndsWith(":443", StringComparison.Ordinal))
        {
            // The service URL can be used by auth libraries to construct the "aud" fields of the JWT token,
            // so not producing serviceUrl compatible with other gRPC implementations can lead to auth failures.
            // For https and the default port 443, the port suffix should be stripped.
            // https://github.com/grpc/grpc/blob/39e982a263e5c48a650990743ed398c1c76db1ac/src/core/lib/security/transport/client_auth_filter.cc#L205
            authority = authority.Substring(0, authority.Length - 4);
        }

        var serviceUrl = baseAddress.Scheme + "://" + authority + baseAddress.AbsolutePath;

        if (!serviceUrl.EndsWith('/'))
            serviceUrl += "/";

        serviceUrl += method.ServiceName;
        return new AuthInterceptorContext(serviceUrl, method.Name, token);
    }

    internal static async Task ReadCredentialMetadata(
        DefaultCallCredentialsConfigurator configurator,
        GrpcChannel channel,
        HttpRequestMessage message,
        IMethod method,
        CallCredentials credentials,
        CancellationToken token)
    {
        credentials.InternalPopulateConfiguration(configurator, null);

        if (configurator.Interceptor != null)
        {
            // Multiple auth interceptors can be called for a gRPC call.
            // These all have the same data: address, method and cancellation token.
            // Lazily allocate the context if it is needed.
            // Stored on the configurator instead of a ref parameter because ref parameters are not supported in async methods.
            configurator.CachedContext ??= CreateAuthInterceptorContext(channel.Address, method, token);

            var metadata = new Metadata();
            await configurator.Interceptor(configurator.CachedContext, metadata).ConfigureAwait(false);

            foreach (var entry in metadata)
                AddHeader(message.Headers, entry);
        }

        if (configurator.CompositeCredentials != null)
        {
            // Copy credentials locally. ReadCredentialMetadata will update it.
            var compositeCredentials = configurator.CompositeCredentials;

            foreach (var callCredentials in compositeCredentials)
            {
                configurator.ResetPerCallCredentialState();
                await ReadCredentialMetadata(configurator, channel, message, method, callCredentials, token).ConfigureAwait(false);
            }
        }
    }

    public static void AddHeader(HttpRequestHeaders headers, Metadata.Entry entry)
    {
        var value = entry.IsBinary ? Convert.ToBase64String(entry.ValueBytes) : entry.Value;
        headers.TryAddWithoutValidation(entry.Key, value);
    }

    public static Status GetResponseStatus(HttpResponseMessage httpResponse, bool isBrowser, bool isWinHttp)
    {
        Status? status;

        try
        {
            if (!TryGetStatusCore(httpResponse.TrailingHeaders, out status))
            {
                var detail = "No grpc-status found on response.";

                if (isBrowser)
                    detail += " If the gRPC call is cross domain then CORS must be correctly configured. Access-Control-Expose-Headers needs to include 'grpc-status' and 'grpc-message'.";
                
                if (isWinHttp)
                    detail += " Using gRPC with WinHttp has Windows and package version requirements. See https://aka.ms/aspnet/grpc/netstandard for details.";
                
                status = new Status(StatusCode.Cancelled, detail);
            }
        }
        catch (Exception ex)
        {
            // Handle error from parsing badly formed status
            status = new Status(StatusCode.Cancelled, ex.Message, ex);
        }

        return status.Value;
    }

    public static bool TryGetStatusCore(HttpHeaders headers, [NotNullWhen(true)] out Status? status)
    {
        var grpcStatus = HttpRequestHelpers.GetHeaderValue(headers, GrpcProtocolConstants.StatusTrailer);

        // grpc-status is a required trailer
        if (grpcStatus is null)
        {
            status = null;
            return false;
        }

        if (!int.TryParse(grpcStatus, out int statusValue))
            throw new InvalidOperationException("Unexpected grpc-status value: " + grpcStatus);

        // grpc-message is optional
        // Always read the gRPC message from the same headers collection as the status
        var grpcMessage = HttpRequestHelpers.GetHeaderValue(headers, GrpcProtocolConstants.MessageTrailer);

        if (!string.IsNullOrEmpty(grpcMessage))
        {
            // https://github.com/grpc/grpc/blob/master/doc/PROTOCOL-HTTP2.md#responses
            // The value portion of Status-Message is conceptually a Unicode string description of the error,
            // physically encoded as UTF-8 followed by percent-encoding.
            grpcMessage = Uri.UnescapeDataString(grpcMessage);
        }

        status = new Status((StatusCode)statusValue, grpcMessage ?? string.Empty);
        return true;
    }

    /// <summary>
    /// Resolve the exception from HttpClient to a gRPC status code.
    /// <param name="ex">The <see cref="Exception"/> to resolve a <see cref="StatusCode"/> from.</param>
    /// </summary>
    public static StatusCode ResolveRpcExceptionStatusCode(Exception ex)
    {
        StatusCode? statusCode = null;
        var hasIOException = false;
        var hasSocketException = false;
        var current = ex;

        do
        {
            // Grpc.Core tends to return Unavailable if there is a problem establishing the connection.
            // Additional changes here are likely required for cases when Unavailable is being returned
            // when it shouldn't be.
            if (current is SocketException)
            {
                // SocketError.ConnectionRefused happens when port is not available.
                // SocketError.HostNotFound happens when unknown host is specified.
                hasSocketException = true;
            }
            else if (current is HttpProtocolException httpProtocolException)
            {
                if (httpProtocolException.ErrorCode >= (long)Http3ErrorCode.H3_NO_ERROR)
                    statusCode = MapHttp3ErrorCodeToStatus(httpProtocolException.ErrorCode);
                else
                    statusCode = MapHttp2ErrorCodeToStatus(httpProtocolException.ErrorCode);
            }
            else if (current is IOException)
                hasIOException = true; // IOException happens if there is a protocol mismatch
        } while ((current = current.InnerException) != null);

        if (statusCode == null && (hasSocketException || hasIOException))
            statusCode = StatusCode.Unavailable;

        return statusCode ?? StatusCode.Internal;

        static StatusCode MapHttp2ErrorCodeToStatus(long protocolError)
        {
            // Mapping from error codes to gRPC status codes is from the gRPC spec.
            return protocolError switch
            {
                (long)Http2ErrorCode.NO_ERROR => StatusCode.Internal,
                (long)Http2ErrorCode.PROTOCOL_ERROR => StatusCode.Internal,
                (long)Http2ErrorCode.INTERNAL_ERROR => StatusCode.Internal,
                (long)Http2ErrorCode.FLOW_CONTROL_ERROR => StatusCode.Internal,
                (long)Http2ErrorCode.SETTINGS_TIMEOUT => StatusCode.Internal,
                (long)Http2ErrorCode.STREAM_CLOSED => StatusCode.Internal,
                (long)Http2ErrorCode.FRAME_SIZE_ERROR => StatusCode.Internal,
                (long)Http2ErrorCode.REFUSED_STREAM => StatusCode.Unavailable,
                (long)Http2ErrorCode.CANCEL => StatusCode.Cancelled,
                (long)Http2ErrorCode.COMPRESSION_ERROR => StatusCode.Internal,
                (long)Http2ErrorCode.CONNECT_ERROR => StatusCode.Internal,
                (long)Http2ErrorCode.ENHANCE_YOUR_CALM => StatusCode.ResourceExhausted,
                (long)Http2ErrorCode.INADEQUATE_SECURITY => StatusCode.PermissionDenied,
                (long)Http2ErrorCode.HTTP_1_1_REQUIRED => StatusCode.Internal,
                _ => StatusCode.Internal
            };
        }

        static StatusCode MapHttp3ErrorCodeToStatus(long protocolError)
        {
            // Mapping from error codes to gRPC status codes is from the gRPC spec.
            return protocolError switch
            {
                (long)Http3ErrorCode.H3_NO_ERROR => StatusCode.Internal,
                (long)Http3ErrorCode.H3_GENERAL_PROTOCOL_ERROR => StatusCode.Internal,
                (long)Http3ErrorCode.H3_INTERNAL_ERROR => StatusCode.Internal,
                (long)Http3ErrorCode.H3_STREAM_CREATION_ERROR => StatusCode.Internal,
                (long)Http3ErrorCode.H3_CLOSED_CRITICAL_STREAM => StatusCode.Internal,
                (long)Http3ErrorCode.H3_FRAME_UNEXPECTED => StatusCode.Internal,
                (long)Http3ErrorCode.H3_FRAME_ERROR => StatusCode.Internal,
                (long)Http3ErrorCode.H3_EXCESSIVE_LOAD => StatusCode.ResourceExhausted,
                (long)Http3ErrorCode.H3_ID_ERROR => StatusCode.Internal,
                (long)Http3ErrorCode.H3_SETTINGS_ERROR => StatusCode.Internal,
                (long)Http3ErrorCode.H3_MISSING_SETTINGS => StatusCode.Internal,
                (long)Http3ErrorCode.H3_REQUEST_REJECTED => StatusCode.Unavailable,
                (long)Http3ErrorCode.H3_REQUEST_CANCELLED => StatusCode.Cancelled,
                (long)Http3ErrorCode.H3_REQUEST_INCOMPLETE => StatusCode.Internal,
                (long)Http3ErrorCode.H3_MESSAGE_ERROR => StatusCode.Internal,
                (long)Http3ErrorCode.H3_CONNECT_ERROR => StatusCode.Internal,
                (long)Http3ErrorCode.H3_VERSION_FALLBACK => StatusCode.Internal,
                _ => StatusCode.Internal
            };
        }
    }

    public static Status CreateStatusFromException(string summary, Exception ex, StatusCode? statusCode = null)
    {
        var exceptionMessage = CommonGrpcProtocolHelpers.ConvertToRpcExceptionMessage(ex);
        statusCode ??= ResolveRpcExceptionStatusCode(ex);

        return new Status(statusCode.Value, summary + " " + exceptionMessage, ex);
    }

}
