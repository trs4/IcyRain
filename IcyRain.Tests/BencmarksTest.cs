using System;
using IcyRain.Benchmarks;
using NUnit.Framework;

namespace IcyRain.Tests;

public class BencmarksTest
{
    [Test]
    public void Test()
    {
        const int maxAttemp = 3;

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

        foreach (var benchmarkType in benchmarkTypes)
        {
            var benchmark = benchmarkType.GetConstructor(Type.EmptyTypes);

            if (benchmark is IIcyRainBenchmark icyRainBenchmark)
            {
                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    icyRainBenchmark.IcyRain_Ser();

                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    icyRainBenchmark.IcyRain_DeepClone();

                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    icyRainBenchmark.IcyRainLZ4_Ser();

                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    icyRainBenchmark.IcyRainLZ4_DeepClone();

                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    icyRainBenchmark.IcyRainLZ4UTC_DeepClone();
            }

            if (benchmark is IZeroFormatterBenchmark zeroFormatterBenchmark)
            {
                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    zeroFormatterBenchmark.ZeroFormatter_Ser();

                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    zeroFormatterBenchmark.ZeroFormatter_DeepClone();

                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    zeroFormatterBenchmark.ZeroFormatterLZ4_Ser();

                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    zeroFormatterBenchmark.ZeroFormatterLZ4_DeepClone();
            }

            if (benchmark is IMessagePackBenchmark messagePackBenchmark)
            {
                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    messagePackBenchmark.MessagePack_Ser();

                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    messagePackBenchmark.MessagePack_DeepClone();
            }

            if (benchmark is IProtoBufNetBenchmark protoBufNetBenchmark)
            {
                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    protoBufNetBenchmark.ProtoBufNet_Ser();

                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    protoBufNetBenchmark.ProtoBufNet_DeepClone();
            }

            if (benchmark is IGoogleProtobufBenchmark googleProtobufBenchmark)
            {
                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    googleProtobufBenchmark.GoogleProtobuf_Ser();

                for (int attempt = 0; attempt < maxAttemp; attempt++)
                    googleProtobufBenchmark.GoogleProtobuf_DeepClone();
            }
        }
    }

}
