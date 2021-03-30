using IcyRain.Internal;
using K4os.Compression.LZ4;
using MessagePack;
using ZeroFormatter;

namespace IcyRain.Benchmarks
{
    internal static class Benchmark
    {
        public static class IcyRain
        {
            public static void Serialize<T>(T value)
                => Serialization.Serialize(new TestBufferWriter(), value);

            public static T DeepClone<T>(T value)
                => new TestBufferWriter().DeepCloneBuffer(value,
                    (b, v) => Serialization.Serialize(b, v),
                    b => Serialization.Deserialize<T>(b));

            public static void SerializeLZ4<T>(T value)
                => Serialization.SerializeWithLZ4(new TestBufferWriter(), value, out _);

            public static T DeepCloneLZ4<T>(T value)
                => new TestBufferWriter().DeepCloneBuffer(value,
                    (b, v) => Serialization.SerializeWithLZ4(b, v, out _),
                    (b, l) => Serialization.DeserializeWithLZ4<T>(b, l, out _));

            public static T DeepCloneLZ4UTC<T>(T value)
                => new TestBufferWriter().DeepCloneBuffer(value,
                    (b, v) => Serialization.SerializeWithLZ4(b, v, out _),
                    (b, l) => Serialization.DeserializeInUTCWithLZ4<T>(b, l, out _));
        }

        public static class ZeroFormatter
        {
            public static void Serialize<T>(T value)
                => new TestBufferWriter().Write(ZeroFormatterSerializer.Serialize(value));

            public static T DeepClone<T>(T value)
                => new TestBufferWriter().DeepCloneBytes(value,
                    v => ZeroFormatterSerializer.Serialize(v),
                    b => ZeroFormatterSerializer.Deserialize<T>(b));

            public static void SerializeLZ4<T>(T value)
                => new TestBufferWriter().Write(LZ4Pickler.Pickle(ZeroFormatterSerializer.Serialize(value)));

            public static T DeepCloneLZ4<T>(T value)
                => new TestBufferWriter().DeepCloneBytes(value,
                    v => LZ4Pickler.Pickle(ZeroFormatterSerializer.Serialize(v)),
                    b => ZeroFormatterSerializer.Deserialize<T>(LZ4Pickler.Unpickle(b)));
        }

        public static class MessagePack
        {
            public static void Serialize<T>(T value)
                => MessagePackSerializer.Serialize(new TestBufferWriter(), value);

            public static T DeepClone<T>(T value)
                => new TestBufferWriter().DeepCloneBuffer(value,
                    (b, v) => MessagePackSerializer.Serialize(b, v),
                    s => MessagePackSerializer.Deserialize<T>(s));
        }

        public static class ProtobufNet
        {
            public static void Serialize<T>(T value)
                => ProtoBuf.Serializer.Serialize(new TestBufferWriter(), value);

            public static T DeepClone<T>(T value)
                => new TestBufferWriter().DeepCloneBuffer(value,
                    (b, v) => ProtoBuf.Serializer.Serialize(b, v),
                    s => ProtoBuf.Serializer.Deserialize<T>(s));
        }

    }
}
