using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Switchers
{
    public abstract class SegmentSwitcher<T>
    {
        public static SegmentSwitcher<T> Instance
        {
            [MethodImpl(Flags.HotPath)]
            get;
        }

        static SegmentSwitcher()
            => Instance = SegmentBuilder.Get<T>();

        public abstract ArraySegment<byte> Serialize(T value);

        public abstract T Deserialize(ArraySegment<byte> segment, DeserializeOptions options);
    }
}
