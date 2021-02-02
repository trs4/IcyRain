using System;
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
    public class DateTimeBenchmarks
    {
        private static readonly DateTime Value = DateTime.Now;

        private static readonly Google.Protobuf.MessageParser<Google.Protobuf.WellKnownTypes.Timestamp> _parser = new(() => new());

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Serialize")]
        public void IcyRain_Ser()
            => Serialization.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "IcyRain"), BenchmarkCategory("Deep clone")]
        public void IcyRain_DeepClone()
            => new TestBufferWriter().DeepClone(Value, v => Serialization.Serialize(v), b => Serialization.Deserialize<DateTime>(b));


        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Serialize")]
        public void ZeroFormatter_Ser()
            => new TestBufferWriter().Write(ZeroFormatterSerializer.Serialize(Value));

        [Benchmark(Description = "ZeroFormatter"), BenchmarkCategory("Deep clone")]
        public void ZeroFormatter_DeepClone()
            => new TestBufferWriter().DeepClone(Value, v => ZeroFormatterSerializer.Serialize(v), b => ZeroFormatterSerializer.Deserialize<DateTime>(b));


        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Serialize")]
        public void MessagePack_Ser()
            => MessagePackSerializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "MessagePack"), BenchmarkCategory("Deep clone")]
        public void MessagePack_DeepClone()
            => new TestBufferWriter().DeepClone(Value, (b, v) => MessagePackSerializer.Serialize(b, v), s => MessagePackSerializer.Deserialize<DateTime>(s));


        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Serialize")]
        public void ProtoBufNet_Ser()
            => ProtoBuf.Serializer.Serialize(new TestBufferWriter(), Value);

        [Benchmark(Description = "protobuf-net"), BenchmarkCategory("Deep clone")]
        public void ProtoBufNet_DeepClone()
            => new TestBufferWriter().DeepClone(Value, (b, v) => ProtoBuf.Serializer.Serialize(b, v), s => ProtoBuf.Serializer.Deserialize<DateTime>(s));


        [Benchmark(Description = "Google.Protobuf"), BenchmarkCategory("Serialize")]
        public void GoogleProtobuf_Ser()
        {
            var value = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(Value.ToUniversalTime());
            Google.Protobuf.MessageExtensions.WriteTo(value, new TestBufferWriter());
        }

        [Benchmark(Description = "Google.Protobuf"), BenchmarkCategory("Deep clone")]
        public void GoogleProtobuf_DeepClone()
        {
            var value = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(Value.ToUniversalTime());
            new TestBufferWriter().DeepClone(value, (b, v) => Google.Protobuf.MessageExtensions.WriteTo(v, b), s => _parser.ParseFrom(s));
        }

    }
}