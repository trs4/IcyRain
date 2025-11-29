namespace IcyRain.Grpc.AspNetCore.Internal;

internal static class GrpcServerConstants
{
    internal const string HostActivityName = "Microsoft.AspNetCore.Hosting.HttpRequestIn";
    internal const string HostActivityChanged = HostActivityName + ".Changed";

    internal const string ActivityStatusCodeTag = "grpc.status_code";
    internal const string ActivityMethodTag = "grpc.method";

    internal const string GrpcUnimplementedConstraintPrefix = "grpcunimplemented";
}
