using System;

namespace IcyRain.Tests
{
    public static class Tests<T>
    {
        public static readonly Func<T, T>[] Functions = new Func<T, T>[]
        {
            Serialization.Tests.DeepClone,
            Serialization.Tests.DeepCloneBytes,
            Serialization.Tests.DeepCloneSegment,
        };

    }
}
