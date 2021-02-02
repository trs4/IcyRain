using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using IcyRain.Internal;
using MessagePack;
using ZeroFormatter;

namespace IcyRain.Benchmarks
{
    [MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class StringBenchmarks
    {
        private static readonly Google.Protobuf.MessageParser<Google.Protobuf.WellKnownTypes.StringValue> _parser = new(() => new());

        [Params("test", "тест5test4", "h47h89dhn_wy8hnasdf_njas0")]
        public string Value { get; set; }

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Serialize")]
        public void IcyRain_Ser()
            => Serialization.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Deep clone")]
        public void IcyRain_DeepClone()
            => new TestBufferWriter().DeepClone(Value, v => Serialization.Serialize(v), b => Serialization.Deserialize<string>(b));


        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Serialize")]
        public void ZeroFormatter_Ser()
            => new TestBufferWriter().Write(ZeroFormatterSerializer.Serialize(Value));

        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Deep clone")]
        public void ZeroFormatter_DeepClone()
            => new TestBufferWriter().DeepClone(Value, v => ZeroFormatterSerializer.Serialize(v), b => ZeroFormatterSerializer.Deserialize<string>(b));


        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Serialize")]
        public void MessagePack_Ser()
            => MessagePackSerializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Deep clone")]
        public void MessagePack_DeepClone()
            => new TestBufferWriter().DeepClone(Value, (b, v) => MessagePackSerializer.Serialize(b, v), s => MessagePackSerializer.Deserialize<string>(s));


        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Serialize")]
        public void ProtoBufNet_Ser()
            => ProtoBuf.Serializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Deep clone")]
        public void ProtoBufNet_DeepClone()
            => new TestBufferWriter().DeepClone(Value, (b, v) => ProtoBuf.Serializer.Serialize(b, v), s => ProtoBuf.Serializer.Deserialize<string>(s));


        [Benchmark(Description = "Google.Protobuf"), BenchmarkCategory("Serialize")]
        public void GoogleProtobuf_Ser()
        {
            var value = new Google.Protobuf.WellKnownTypes.StringValue() { Value = Value };
            Google.Protobuf.MessageExtensions.WriteTo(value, new TestBufferWriter());
        }

        [Benchmark(Description = "Google.Protobuf"), BenchmarkCategory("Deep clone")]
        public void GoogleProtobuf_DeepClone()
        {
            var value = new Google.Protobuf.WellKnownTypes.StringValue() { Value = Value };
            new TestBufferWriter().DeepClone(value, (b, v) => Google.Protobuf.MessageExtensions.WriteTo(v, b), s => _parser.ParseFrom(s));
        }

    }
}