using System.Net.Http;

namespace IcyRain.Grpc.Client.Web.Internal;

internal static class HttpRequestHelpers
{
    public static void SetOption<T>(this HttpRequestMessage requestMessage, string key, T value)
        => requestMessage.Options.Set(new HttpRequestOptionsKey<T>(key), value);
}
