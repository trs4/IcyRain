using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;
using ZeroFormatter;

namespace IcyRain.Data.Objects;

[DataContract]
[ZeroFormattable]
public class TestData
{
    [DataMember(Order = 1)]
    [Index(0)]
    public virtual bool Property1 { get; set; }

    [DataMember(Order = 2)]
    [Index(1)]
    public virtual int Property2 { get; set; }

    [DataMember(Order = 3)]
    [Index(2)]
    public virtual double Property3 { get; set; }

    [DataMember(Order = 4)]
    [Index(3)]
    public virtual DateTime Property4 { get; set; }

    [DataMember(Order = 5)]
    [Index(4)]
    public virtual string Property5 { get; set; }
}

public sealed class TestDataSerializer : Serializer<Resolver, TestData>
{
    private readonly Type _s_Type = typeof(TestData);
    private readonly Serializer<Resolver, bool> _s_Property1Serializer;
    private readonly Serializer<Resolver, int> _s_Property2Serializer;
    private readonly Serializer<Resolver, double> _s_Property3Serializer;
    private readonly Serializer<Resolver, DateTime> _s_Property4Serializer;
    private readonly Serializer<Resolver, string> _s_Property5Serializer;

    public TestDataSerializer()
    {
        _s_Property1Serializer = Serializer<Resolver, bool>.Instance;
        _s_Property2Serializer = Serializer<Resolver, int>.Instance;
        _s_Property3Serializer = Serializer<Resolver, double>.Instance;
        _s_Property4Serializer = Serializer<Resolver, DateTime>.Instance;
        _s_Property5Serializer = Serializer<Resolver, string>.Instance;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TestData value)
        => value is null ? 1 : 50 + _s_Property3Serializer.GetCapacity(value.Property3) + _s_Property4Serializer.GetCapacity(value.Property4)
        + _s_Property5Serializer.GetCapacity(value.Property5);

    public override sealed void Serialize(ref Writer writer, TestData value)
    {
        if (value is null)
            writer.WriteByte(0); // Empty object
        else
            SerializeSpot(ref writer, value);
    }

    public override sealed void SerializeSpot(ref Writer writer, TestData value)
    {
        if (value.Property1 != default)
        {
            writer.WriteByte(1);
            _s_Property1Serializer.SerializeSpot(ref writer, value.Property1);
        }

        if (value.Property2 != default)
        {
            writer.WriteByte(2);
            _s_Property2Serializer.SerializeSpot(ref writer, value.Property2);
        }

        if (value.Property3 != default)
        {
            writer.WriteInt(3);
            _s_Property3Serializer.SerializeSpot(ref writer, value.Property3);
        }

        if (!value.Property4.IsEmptyEqutableStruct())
        {
            writer.WriteByte(4);
            _s_Property4Serializer.SerializeSpot(ref writer, value.Property4);
        }

        if (value.Property5 is not null)
        {
            writer.WriteByte(5);
            _s_Property5Serializer.SerializeSpot(ref writer, value.Property5);
        }

        writer.WriteByte(255); // End object
    }

    public override sealed TestData Deserialize(ref Reader reader)
    {
        int index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 0)
            return null;

#if NET8_0_OR_GREATER
        var obj = (TestData)RuntimeHelpers.GetUninitializedObject(_s_Type);
#else
        var obj = (TestData)FormatterServices.GetUninitializedObject(_s_Type);
#endif

        if (index == 1)
        {
            obj.Property1 = _s_Property1Serializer.DeserializeSpot(ref reader);
            index = reader.ReadByte(); // Read 2
        }

        if (index == 2)
        {
            obj.Property2 = _s_Property2Serializer.DeserializeSpot(ref reader);
            index = reader.ReadByte(); // Read 3
        }

        if (index == 3)
        {
            obj.Property3 = _s_Property3Serializer.DeserializeSpot(ref reader);
            index = reader.ReadInt(); // Read 4
        }

        if (index == 4)
        {
            obj.Property4 = _s_Property4Serializer.DeserializeSpot(ref reader);
            index = reader.ReadByte(); // Read 5
        }

        if (index == 5)
        {
            obj.Property5 = _s_Property5Serializer.DeserializeSpot(ref reader);
            reader.ReadByte(); // Read 255
        }

        return obj;
    }

    public override sealed TestData DeserializeInUTC(ref Reader reader)
    {
        int index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 0)
            return null;

#if NET8_0_OR_GREATER
        var obj = (TestData)RuntimeHelpers.GetUninitializedObject(_s_Type);
#else
        var obj = (TestData)FormatterServices.GetUninitializedObject(_s_Type);
#endif

        if (index == 1)
        {
            obj.Property1 = _s_Property1Serializer.DeserializeInUTCSpot(ref reader);
            index = reader.ReadByte(); // Read 2
        }

        if (index == 2)
        {
            obj.Property2 = _s_Property2Serializer.DeserializeInUTCSpot(ref reader);
            index = reader.ReadByte(); // Read 3
        }

        if (index == 3)
        {
            obj.Property3 = _s_Property3Serializer.DeserializeInUTCSpot(ref reader);
            index = reader.ReadInt(); // Read 4
        }

        if (index == 4)
        {
            obj.Property4 = _s_Property4Serializer.DeserializeInUTCSpot(ref reader);
            index = reader.ReadByte(); // Read 5
        }

        if (index == 5)
        {
            obj.Property5 = _s_Property5Serializer.DeserializeInUTCSpot(ref reader);
            reader.ReadByte(); // Read 255
        }

        return obj;
    }

    public override sealed TestData DeserializeSpot(ref Reader reader)
    {
#if NET8_0_OR_GREATER
        var obj = (TestData)RuntimeHelpers.GetUninitializedObject(_s_Type);
#else
        var obj = (TestData)FormatterServices.GetUninitializedObject(_s_Type);
#endif
        int index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 1)
        {
            obj.Property1 = _s_Property1Serializer.DeserializeSpot(ref reader);
            index = reader.ReadByte(); // Read 2
        }

        if (index == 2)
        {
            obj.Property2 = _s_Property2Serializer.DeserializeSpot(ref reader);
            index = reader.ReadByte(); // Read 3
        }

        if (index == 3)
        {
            obj.Property3 = _s_Property3Serializer.DeserializeSpot(ref reader);
            index = reader.ReadInt(); // Read 4
        }

        if (index == 4)
        {
            obj.Property4 = _s_Property4Serializer.DeserializeSpot(ref reader);
            index = reader.ReadByte(); // Read 5
        }

        if (index == 5)
        {
            obj.Property5 = _s_Property5Serializer.DeserializeSpot(ref reader);
            reader.ReadByte(); // Read 255
        }

        return obj;
    }

    public override sealed TestData DeserializeInUTCSpot(ref Reader reader)
    {
#if NET8_0_OR_GREATER
        var obj = (TestData)RuntimeHelpers.GetUninitializedObject(_s_Type);
#else
        var obj = (TestData)FormatterServices.GetUninitializedObject(_s_Type);
#endif
        int index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 1)
        {
            obj.Property1 = _s_Property1Serializer.DeserializeInUTCSpot(ref reader);
            index = reader.ReadByte(); // Read 2
        }

        if (index == 2)
        {
            obj.Property2 = _s_Property2Serializer.DeserializeInUTCSpot(ref reader);
            index = reader.ReadByte(); // Read 3
        }

        if (index == 3)
        {
            obj.Property3 = _s_Property3Serializer.DeserializeInUTCSpot(ref reader);
            index = reader.ReadInt(); // Read 4
        }

        if (index == 4)
        {
            obj.Property4 = _s_Property4Serializer.DeserializeInUTCSpot(ref reader);
            index = reader.ReadByte(); // Read 5
        }

        if (index == 5)
        {
            obj.Property5 = _s_Property5Serializer.DeserializeInUTCSpot(ref reader);
            reader.ReadByte(); // Read 255
        }

        return obj;
    }

}
