using System.Collections.Generic;

namespace IcyRain.Comparers;

internal static class SerializerComparer<T>
{
    public static IEqualityComparer<T> Instance { get; }

    static SerializerComparer()
        => Instance = (IEqualityComparer<T>)Builder.Get<T>() ?? EqualityComparer<T>.Default;
}
