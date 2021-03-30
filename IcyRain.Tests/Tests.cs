using System;

namespace IcyRain.Tests
{
    public static class Tests<T>
    {
        public static readonly Func<T, T>[] Functions = new Func<T, T>[]
        {
            // Generic
            Serialization.Tests.DeepClone,
            Serialization.Tests.DeepCloneBytes,
            Serialization.Tests.DeepCloneSegment,
            Serialization.NonGeneric.Tests.DeepClone,
            Serialization.NonGeneric.Tests.DeepCloneBytes,
            Serialization.NonGeneric.Tests.DeepCloneSegment,
            
            // Generic in UTC
            Serialization.Tests.DeepCloneInUTC,
            Serialization.Tests.DeepCloneBytesInUTC,
            Serialization.Tests.DeepCloneSegmentInUTC,
            Serialization.NonGeneric.Tests.DeepCloneInUTC,
            Serialization.NonGeneric.Tests.DeepCloneBytesInUTC,
            Serialization.NonGeneric.Tests.DeepCloneSegmentInUTC,

            // Generic via LZ4
            Serialization.Tests.DeepCloneWithLZ4,
            Serialization.Tests.DeepCloneBytesWithLZ4,
            Serialization.Tests.DeepCloneSegmentWithLZ4,
            Serialization.NonGeneric.Tests.DeepCloneWithLZ4,
            Serialization.NonGeneric.Tests.DeepCloneBytesWithLZ4,
            Serialization.NonGeneric.Tests.DeepCloneSegmentWithLZ4,
            
            // Generic in UTC via LZ4
            Serialization.Tests.DeepCloneInUTCWithLZ4,
            Serialization.Tests.DeepCloneBytesInUTCWithLZ4,
            Serialization.Tests.DeepCloneSegmentInUTCWithLZ4,
            Serialization.NonGeneric.Tests.DeepCloneInUTCWithLZ4,
            Serialization.NonGeneric.Tests.DeepCloneBytesInUTCWithLZ4,
            Serialization.NonGeneric.Tests.DeepCloneSegmentInUTCWithLZ4,
        };

    }
}
