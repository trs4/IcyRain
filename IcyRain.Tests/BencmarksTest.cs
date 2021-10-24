using System;
using IcyRain.Benchmarks;
using NUnit.Framework;

namespace IcyRain.Tests
{
    public class BencmarksTest
    {
        [Test]
        public void Test()
        {
            var benchmarkTypes = new[]
            {
                typeof(BoolBenchmarks),
                typeof(BytesBenchmarks),
                typeof(DateTimeBenchmarks),
                typeof(DateTimeListBenchmarks),
                typeof(DictionaryBenchmarks),
                typeof(DoubleListBenchmarks),
                typeof(HashSetBenchmarks),
                typeof(IntBenchmarks),
                typeof(IntListBenchmarks),
                typeof(IReadOnlyIntCollectionBenchmarks),
                typeof(IReadOnlyIntListBenchmarks),
                typeof(SequenceBenchmarks),
                typeof(StringBenchmarks),
                typeof(StringListBenchmarks),
            };

            for (int attempt = 0; attempt < 3; attempt++)
            {
                foreach (var benchmarkType in benchmarkTypes)
                {
                    var benchmark = benchmarkType.GetConstructor(Type.EmptyTypes);

                    if (benchmark is IIcyRainBenchmark icyRainBenchmark)
                    {
                        icyRainBenchmark.IcyRain_Ser();
                        icyRainBenchmark.IcyRain_DeepClone();
                        icyRainBenchmark.IcyRainLZ4_Ser();
                        icyRainBenchmark.IcyRainLZ4_DeepClone();
                        icyRainBenchmark.IcyRainLZ4UTC_DeepClone();
                    }

                    if (benchmark is IZeroFormatterBenchmark zeroFormatterBenchmark)
                    {
                        zeroFormatterBenchmark.ZeroFormatter_Ser();
                        zeroFormatterBenchmark.ZeroFormatter_DeepClone();
                        zeroFormatterBenchmark.ZeroFormatterLZ4_Ser();
                        zeroFormatterBenchmark.ZeroFormatterLZ4_DeepClone();
                    }

                    if (benchmark is IMessagePackBenchmark messagePackBenchmark)
                    {
                        messagePackBenchmark.MessagePack_Ser();
                        messagePackBenchmark.MessagePack_DeepClone();
                    }

                    if (benchmark is IProtoBufNetBenchmark protoBufNetBenchmark)
                    {
                        protoBufNetBenchmark.ProtoBufNet_Ser();
                        protoBufNetBenchmark.ProtoBufNet_DeepClone();
                    }

                    if (benchmark is IGoogleProtobufBenchmark googleProtobufBenchmark)
                    {
                        googleProtobufBenchmark.GoogleProtobuf_Ser();
                        googleProtobufBenchmark.GoogleProtobuf_DeepClone();
                    }
                }
            }
        }

    }
}
