using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class GrpcUnimplementedConstraint : IRouteConstraint
{
    public GrpcUnimplementedConstraint() { }

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (httpContext is null)
            return false;

        // Constraint needs to be valid when a CORS preflight request is received so that CORS middleware will run
        if (GrpcProtocolHelpers.IsCorsPreflightRequest(httpContext))
            return true;

        if (!HttpMethods.IsPost(httpContext.Request.Method))
            return false;

        return CommonGrpcProtocolHelpers.IsContentType(GrpcProtocolConstants.GrpcContentType, httpContext.Request.ContentType)
            || CommonGrpcProtocolHelpers.IsContentType(GrpcProtocolConstants.GrpcWebContentType, httpContext.Request.ContentType)
            || CommonGrpcProtocolHelpers.IsContentType(GrpcProtocolConstants.GrpcWebTextContentType, httpContext.Request.ContentType);
    }

}
