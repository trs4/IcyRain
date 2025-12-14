using System;
using System.Buffers;
using IcyRain.Data.Objects;

#pragma warning disable IDE0059 // Unnecessary assignment of a value
namespace IcyRain.Data;

public static class SerializationExample
{
    public static void Example()
    {
        var data = new TestData
        {
            Property1 = true,
            Property2 = 25,
            Property3 = 7.0,
            Property4 = DateTime.Now,
            Property5 = "test",
            Property6 = 4,
        };

        // byte array serialization
        byte[] bytes = Serialization.Serialize(data);
        TestData bytesClone = Serialization.Deserialize<TestData>(bytes);

        // array segment serialization
        ArraySegment<byte> segment = Serialization.SerializeSegment(data);
        TestData segmentClone = Serialization.DeserializeSegment<TestData>(segment);

        // IBufferWriter<byte> & ReadOnlySequence<byte> serialization
        IBufferWriter<byte> buffer = null;
        Serialization.Serialize(buffer, data);
        ReadOnlySequence<byte> sequence = default;
        TestData sequenceClone = Serialization.Deserialize<TestData>(sequence);
    }

}
#pragma warning restore IDE0059 // Unnecessary assignment of a value
