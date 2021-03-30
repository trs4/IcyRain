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

        public abstract ArraySegment<byte> SerializeWithLZ4(T value, out int serializedLength);


        public abstract T Deserialize(ArraySegment<byte> segment);

        public abstract T DeserializeInUTC(ArraySegment<byte> segment);

        public abstract T DeserializeWithLZ4(ArraySegment<byte> segment, out int decodedLength);

        public abstract T DeserializeInUTCWithLZ4(ArraySegment<byte> segment, out int decodedLength);
    }
}
