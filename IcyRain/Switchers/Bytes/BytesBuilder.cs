using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Switchers;

internal static class BytesBuilder
{
    public static BytesSwitcher<T> Get<T>()
    {
        var type = typeof(T);

        if (type == Types.Bytes)
            return (BytesSwitcher<T>)(object)new BytesBytesSwitcher();
        else if (type == Types.BytesSegment)
            return (BytesSwitcher<T>)(object)new SegmentBytesSwitcher();
        else if (type == Types.BytesReadOnlySequence)
            return (BytesSwitcher<T>)(object)new SequenceBytesSwitcher();
        else if (type == Types.BytesMemory)
            return (BytesSwitcher<T>)(object)new MemoryBytesSwitcher();
        else if (type == Types.BytesReadOnlyMemory)
            return (BytesSwitcher<T>)(object)new ReadOnlyMemoryBytesSwitcher();

        return ResolverHelper.IsUnionResolver(type) ? new UnionBytesSwitcher<T>() : new DefaultBytesSwitcher<T>();
    }

}
