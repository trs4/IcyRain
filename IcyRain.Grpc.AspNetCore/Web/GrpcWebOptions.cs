namespace IcyRain.Grpc.AspNetCore.Web;

/// <summary>Provides programmatic configuration for gRPC-Web</summary>
public class GrpcWebOptions
{
    /// <summary>Gets or sets a flag indicating whether gRPC-Web is enabled by default for endpoints that have not specifically opted in or out</summary>
    public bool DefaultEnabled { get; set; }
}
