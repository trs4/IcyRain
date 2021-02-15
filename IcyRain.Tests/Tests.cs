using System;
using IcyRain.Internal;

namespace IcyRain.Tests
{
    public static class Tests<T>
    {
        public static readonly Func<T, T>[] Functions = new Func<T, T>[]
        {
            Serialization.Tests.DeepClone,
            Serialization.Tests.DeepCloneBytes,
            Serialization.Tests.DeepCloneSegment,

            new Func<T, T>(value =>
            {
                using var buffer = new ArrayBufferWriter();
                Serialization.NonGeneric.Serialize(typeof(T), buffer, value);
                return (T)Serialization.NonGeneric.Deserialize(typeof(T), buffer.ToSequence());
            }),
            new Func<T, T>(value =>
            {
                byte[] bytes = Serialization.NonGeneric.Serialize(typeof(T), value);
                return (T)Serialization.NonGeneric.Deserialize(typeof(T), bytes);
            }),
            new Func<T, T>(value =>
            {
                var segment = Serialization.NonGeneric.SerializeSegment(typeof(T), value);
                return (T)Serialization.NonGeneric.DeserializeSegment(typeof(T), segment);
            }),
        };

    }
}
