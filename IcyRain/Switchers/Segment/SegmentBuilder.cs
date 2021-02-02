using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Switchers
{
    internal static class SegmentBuilder
    {
        public static SegmentSwitcher<T> Get<T>()
        {
            var type = typeof(T);

            if (type == Types.Bytes)
                return (SegmentSwitcher<T>)(object)new BytesSegmentSwitcher();
            else if (type == Types.BytesSegment)
                return (SegmentSwitcher<T>)(object)new SegmentSegmentSwitcher();
            else if (type == Types.BytesReadOnlySequence)
                return (SegmentSwitcher<T>)(object)new SequenceSegmentSwitcher();
            else if (type == Types.BytesMemory)
                return (SegmentSwitcher<T>)(object)new MemorySegmentSwitcher();
            else if (type == Types.BytesReadOnlyMemory)
                return (SegmentSwitcher<T>)(object)new ReadOnlyMemorySegmentSwitcher();

            return ResolverHelper.IsUnionResolver(type) ? new UnionSegmentSwitcher<T>() : new DefaultSegmentSwitcher<T>();
        }

    }
}
