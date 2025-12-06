using System;
using System.Net;
using System.Net.Http.Headers;

namespace IcyRain.Grpc.Client.Web.Internal;

internal static class GrpcWebProtocolConstants
{
    public static readonly Version Http2Version = HttpVersion.Version20;

    public const string GrpcContentType = "application/grpc";
    public const string GrpcWebContentType = "application/grpc-web";
    public const string GrpcWebTextContentType = "application/grpc-web-text";

    public static readonly MediaTypeHeaderValue GrpcHeader = new MediaTypeHeaderValue(GrpcContentType);
    public static readonly MediaTypeHeaderValue GrpcWebHeader = new MediaTypeHeaderValue(GrpcWebContentType);
    public static readonly MediaTypeHeaderValue GrpcWebTextHeader = new MediaTypeHeaderValue(GrpcWebTextContentType);
}
