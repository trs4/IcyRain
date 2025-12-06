namespace IcyRain.Grpc.Client.Web;

/// <summary>The gRPC-Web mode</summary>
public enum GrpcWebMode
{
    /// <summary>Calls are made using the <c>application/grpc-web</c> content type. Request content is not translated to base64</summary>
    GrpcWeb,

    /// <summary>Calls are made using the <c>application/grpc-web-text</c> content type. Request content is translated to base64</summary>
    GrpcWebText,
}
