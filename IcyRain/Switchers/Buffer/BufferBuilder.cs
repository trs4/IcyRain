using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Switchers;

internal static class BufferBuilder
{
    public static BufferSwitcher<T> Get<T>()
    {
        var type = typeof(T);

        if (type == Types.Bytes)
            return (BufferSwitcher<T>)(object)new BytesBufferSwitcher();
        else if (type == Types.BytesSegment)
            return (BufferSwitcher<T>)(object)new SegmentBufferSwitcher();
        else if (type == Types.BytesReadOnlySequence)
            return (BufferSwitcher<T>)(object)new SequenceBufferSwitcher();
        else if (type == Types.BytesMemory)
            return (BufferSwitcher<T>)(object)new MemoryBufferSwitcher();
        else if (type == Types.BytesReadOnlyMemory)
            return (BufferSwitcher<T>)(object)new ReadOnlyMemoryBufferSwitcher();

        return ResolverHelper.IsUnionResolver(type) ? new UnionBufferSwitcher<T>() : new DefaultBufferSwitcher<T>();
    }

}
