using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;

namespace IcyRain.Grpc.Client.Internal.Http;

internal static class HttpRequestHelpers
{
    public static bool TryGetOption<T>(this HttpRequestMessage requestMessage, string key, [NotNullWhen(true)] out T? value)
        => requestMessage.Options.TryGetValue(new HttpRequestOptionsKey<T>(key), out value!);

    public static void SetOption<T>(this HttpRequestMessage requestMessage, string key, T value)
        => requestMessage.Options.Set(new HttpRequestOptionsKey<T>(key), value);

    public static bool HasHttpHandlerType(HttpMessageHandler handler, string handlerTypeName)
        => GetHttpHandlerType(handler, handlerTypeName) is not null;

    public static HttpMessageHandler? GetHttpHandlerType(HttpMessageHandler handler, string handlerTypeName, bool isFullName = true)
    {
        if (IsType(handler.GetType(), handlerTypeName, isFullName))
            return handler;

        HttpMessageHandler? currentHandler = handler;

        while (currentHandler is DelegatingHandler delegatingHandler)
        {
            currentHandler = delegatingHandler.InnerHandler;

            if (currentHandler is not null && IsType(currentHandler.GetType(), handlerTypeName, isFullName))
                return currentHandler;
        }

        return null;
    }

    private static bool IsType(Type type, string handlerTypeName, bool isFullName)
    {
        Type? currentType = type;

        do
        {
            string? currentName = isFullName ? currentType.FullName : currentType.Name;

            if (currentName == handlerTypeName)
                return true;

            currentType = currentType.BaseType;
        }
        while (currentType is not null);

        return false;
    }

    public static bool HasHttpHandlerType<T>(HttpMessageHandler handler)
        where T : HttpMessageHandler
        => GetHttpHandlerType<T>(handler) != null;

    public static T? GetHttpHandlerType<T>(HttpMessageHandler handler)
        where T : HttpMessageHandler
    {
        if (handler is T t)
            return t;

        HttpMessageHandler? currentHandler = handler;

        while (currentHandler is DelegatingHandler delegatingHandler)
        {
            currentHandler = delegatingHandler.InnerHandler;

            if (currentHandler is T inner)
                return inner;
        }

        return null;
    }

    public static string? GetHeaderValue(HttpHeaders? headers, string name, bool first = false)
    {
        if (headers is null)
            return null;

        if (!headers.NonValidated.TryGetValues(name, out var values))
            return null;

        using (var e = values.GetEnumerator())
        {
            if (!e.MoveNext())
                return null;

            var result = e.Current;

            if (!e.MoveNext())
                return result;

            if (first)
                return result;
        }

        throw new InvalidOperationException($"Multiple {name} headers.");
    }

}
