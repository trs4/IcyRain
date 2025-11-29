using System;
using Microsoft.AspNetCore.Http;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal static class HttpResponseExtensions
{
    public static void ConsolidateTrailers(this HttpResponse httpResponse, HttpContextServerCallContext context)
    {
        var trailersDestination = GrpcProtocolHelpers.GetTrailersDestination(httpResponse);

        if (context.HasResponseTrailers)
        {
            foreach (var trailer in context.ResponseTrailers)
            {
                var value = (trailer.IsBinary) ? Convert.ToBase64String(trailer.ValueBytes) : trailer.Value;

                try
                {
                    trailersDestination.Append(trailer.Key, value);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error adding response trailer '{trailer.Key}'.", ex);
                }
            }
        }

        // Append status trailers, these overwrite any existing status trailers set via ServerCallContext.ResponseTrailers
        GrpcProtocolHelpers.SetStatus(trailersDestination, context.Status);
    }

}
