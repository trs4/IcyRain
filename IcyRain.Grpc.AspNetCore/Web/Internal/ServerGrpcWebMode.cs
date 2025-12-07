namespace IcyRain.Grpc.AspNetCore.Web.Internal;

internal readonly record struct ServerGrpcWebContext(ServerGrpcWebMode Request, ServerGrpcWebMode Response);

internal enum ServerGrpcWebMode : byte
{
    None,
    GrpcWeb,
    GrpcWebText
}
