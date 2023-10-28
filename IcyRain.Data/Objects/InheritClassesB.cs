using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;
using ZeroFormatter;

namespace IcyRain.Data.Objects;

[DataContract]
[KnownType(typeof(TestB2)), KnownType(typeof(TestB3))]
[ZeroFormattable]
[DynamicUnion]
public class TestB1
{
    static TestB1()
    {
        ZeroFormatter.Formatters.Formatter.AppendDynamicUnionResolver((unionType, resolver) =>
        {
            if (unionType == typeof(TestB1))
            {
                resolver.RegisterUnionKeyType(typeof(byte));
                resolver.RegisterSubType(key: (byte)1, subType: typeof(TestB1));
                resolver.RegisterSubType(key: (byte)2, subType: typeof(TestB2));
                resolver.RegisterSubType(key: (byte)3, subType: typeof(TestB3));
            }
        });
    }

    [DataMember(Order = 1)]
    [Index(0)]
    public virtual bool Property11 { get; set; }
}

[DataContract]
[ZeroFormattable]
public class TestB2 : TestB1
{
    [DataMember(Order = 2)]
    [Index(1)]
    public virtual bool Property21 { get; set; }

    [DataMember(Order = 3)]
    [Index(2)]
    public virtual int Property22 { get; set; }
}

[DataContract]
[ZeroFormattable]
public class TestB3 : TestB1
{
    [DataMember(Order = 4)]
    [Index(3)]
    public virtual double Property31 { get; set; }

    [DataMember(Order = 5)]
    [Index(4)]
    public virtual DateTime Property32 { get; set; }
}


public sealed class TestB1Serializer : Serializer<Resolver, TestB1>
{
    private readonly Serializer<Resolver, bool> _s_Property11Serializer;

    public TestB1Serializer()
    {
        _s_Property11Serializer = Serializer<Resolver, bool>.Instance;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 50;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TestB1 value)
        => value is null ? 1 : 50;

    public override sealed void Serialize(ref Writer writer, TestB1 value)
    {
        if (value is null)
        {
            writer.WriteByte(0); // Empty object
            return;
        }

        if (value.Property11 != default)
        {
            writer.WriteByte(1);
            _s_Property11Serializer.SerializeSpot(ref writer, value.Property11);
        }

        writer.WriteByte(255); // End object
    }

    public override sealed void SerializeSpot(ref Writer writer, TestB1 value)
    {
        throw new NotImplementedException();
    }

    public override sealed TestB1 Deserialize(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 0)
            return null;

        var obj = (TestB1)FormatterServices.GetUninitializedObject(typeof(TestB1));

        if (index == 1)
        {
            obj.Property11 = _s_Property11Serializer.DeserializeSpot(ref reader);
            reader.ReadByte(); // Read 255
        }

        return obj;
    }

    public override sealed TestB1 DeserializeInUTC(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 0)
            return null;

        var obj = (TestB1)FormatterServices.GetUninitializedObject(typeof(TestB1));

        if (index == 1)
        {
            obj.Property11 = _s_Property11Serializer.DeserializeInUTCSpot(ref reader);
            reader.ReadByte(); // Read 255
        }

        return obj;
    }

    public override sealed TestB1 DeserializeSpot(ref Reader reader)
    {
        throw new NotImplementedException();
    }

    public override sealed TestB1 DeserializeInUTCSpot(ref Reader reader)
    {
        throw new NotImplementedException();
    }

}

public sealed class TestB2Serializer : Serializer<Resolver, TestB2>
{
    private readonly Serializer<Resolver, bool> _s_Property11Serializer;
    private readonly Serializer<Resolver, bool> _s_Property21Serializer;
    private readonly Serializer<Resolver, int> _s_Property22Serializer;

    public TestB2Serializer()
    {
        _s_Property11Serializer = Serializer<Resolver, bool>.Instance;
        _s_Property21Serializer = Serializer<Resolver, bool>.Instance;
        _s_Property22Serializer = Serializer<Resolver, int>.Instance;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TestB2 value)
        => value is null ? 1 : 50;

    public override sealed void Serialize(ref Writer writer, TestB2 value)
    {
        if (value is null)
        {
            writer.WriteByte(0); // Empty object
            return;
        }

        if (value.Property11 != default)
        {
            writer.WriteByte(1);
            _s_Property11Serializer.SerializeSpot(ref writer, value.Property11);
        }

        if (value.Property21 != default)
        {
            writer.WriteByte(2);
            _s_Property21Serializer.SerializeSpot(ref writer, value.Property21);
        }

        if (value.Property22 != default)
        {
            writer.WriteInt(3);
            _s_Property22Serializer.SerializeSpot(ref writer, value.Property22);
        }


        writer.WriteByte(255); // End object
    }

    public override sealed void SerializeSpot(ref Writer writer, TestB2 value)
    {
        throw new NotImplementedException();
    }

    public override sealed TestB2 Deserialize(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 0)
            return null;

        var obj = (TestB2)FormatterServices.GetUninitializedObject(typeof(TestB2));

        if (index == 1)
        {
            obj.Property11 = _s_Property11Serializer.DeserializeSpot(ref reader);
            index = reader.ReadByte(); // Read 2
        }

        if (index == 2)
        {
            obj.Property21 = _s_Property21Serializer.DeserializeSpot(ref reader);
            index = reader.ReadByte(); // Read 3
        }

        if (index == 3)
        {
            obj.Property22 = _s_Property22Serializer.DeserializeSpot(ref reader);
            reader.ReadInt(); // Read 255
        }

        return obj;
    }

    public override sealed TestB2 DeserializeInUTC(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 0)
            return null;

        var obj = (TestB2)FormatterServices.GetUninitializedObject(typeof(TestB2));

        if (index == 1)
        {
            obj.Property11 = _s_Property11Serializer.DeserializeInUTCSpot(ref reader);
            index = reader.ReadByte(); // Read 2
        }

        if (index == 2)
        {
            obj.Property21 = _s_Property21Serializer.DeserializeInUTCSpot(ref reader);
            index = reader.ReadByte(); // Read 3
        }

        if (index == 3)
        {
            obj.Property22 = _s_Property22Serializer.DeserializeInUTCSpot(ref reader);
            reader.ReadInt(); // Read 255
        }

        return obj;
    }

    public override sealed TestB2 DeserializeSpot(ref Reader reader)
    {
        throw new NotImplementedException();
    }

    public override sealed TestB2 DeserializeInUTCSpot(ref Reader reader)
    {
        throw new NotImplementedException();
    }

}

public sealed class TestB3Serializer : Serializer<Resolver, TestB3>
{
    private readonly Serializer<Resolver, bool> _s_Property11Serializer;
    private readonly Serializer<Resolver, double> _s_Property31Serializer;
    private readonly Serializer<Resolver, DateTime> _s_Property32Serializer;

    public TestB3Serializer()
    {
        _s_Property11Serializer = Serializer<Resolver, bool>.Instance;
        _s_Property31Serializer = Serializer<Resolver, double>.Instance;
        _s_Property32Serializer = Serializer<Resolver, DateTime>.Instance;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TestB3 value)
        => value is null ? 1 : 50;

    public override sealed void Serialize(ref Writer writer, TestB3 value)
    {
        if (value is null)
        {
            writer.WriteUShort(0); // Empty object
            return;
        }

        if (value.Property11 != default)
        {
            writer.WriteUShort(1);
            _s_Property11Serializer.SerializeSpot(ref writer, value.Property11);
        }

        if (value.Property31 != default)
        {
            writer.WriteUShort(2);
            _s_Property31Serializer.SerializeSpot(ref writer, value.Property31);
        }

        if (value.Property32 != default)
        {
            writer.WriteUShort(3);
            _s_Property32Serializer.SerializeSpot(ref writer, value.Property32);
        }


        writer.WriteUShort(65535); // End object
    }

    public override sealed void SerializeSpot(ref Writer writer, TestB3 value)
    {
        throw new NotImplementedException();
    }

    public override sealed TestB3 Deserialize(ref Reader reader)
    {
        ushort index = reader.ReadUShort(); // Read 0 or 1 or 65535

        if (index == 0)
            return null;

        var obj = (TestB3)FormatterServices.GetUninitializedObject(typeof(TestB3));

        if (index == 1)
        {
            obj.Property11 = _s_Property11Serializer.DeserializeSpot(ref reader);
            index = reader.ReadUShort(); // Read 2
        }

        if (index == 2)
        {
            obj.Property31 = _s_Property31Serializer.DeserializeSpot(ref reader);
            index = reader.ReadUShort(); // Read 3
        }

        if (index == 3)
        {
            obj.Property32 = _s_Property32Serializer.DeserializeSpot(ref reader);
            reader.ReadUShort(); // Read 65535
        }

        return obj;
    }

    public override sealed TestB3 DeserializeInUTC(ref Reader reader)
    {
        ushort index = reader.ReadUShort(); // Read 0 or 1 or 65535

        if (index == 0)
            return null;

        var obj = (TestB3)FormatterServices.GetUninitializedObject(typeof(TestB3));

        if (index == 1)
        {
            obj.Property11 = _s_Property11Serializer.DeserializeInUTCSpot(ref reader);
            index = reader.ReadUShort(); // Read 2
        }

        if (index == 2)
        {
            obj.Property31 = _s_Property31Serializer.DeserializeInUTCSpot(ref reader);
            index = reader.ReadUShort(); // Read 3
        }

        if (index == 3)
        {
            obj.Property32 = _s_Property32Serializer.DeserializeInUTCSpot(ref reader);
            reader.ReadUShort(); // Read 65535
        }

        return obj;
    }

    public override sealed TestB3 DeserializeSpot(ref Reader reader)
    {
        throw new NotImplementedException();
    }

    public override sealed TestB3 DeserializeInUTCSpot(ref Reader reader)
    {
        throw new NotImplementedException();
    }

}


public sealed class TestB1DynamicSerializer : Serializer<UnionResolver, TestB1>
{
    public TestB1DynamicSerializer() { }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

#pragma warning disable IDE0038 // Use pattern matching
    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TestB1 value)
    {
        if (value is null)
            return 1;
        else if (value is TestB2)
            return Serializer<Resolver, TestB2>.Instance.GetCapacity((TestB2)value) + 1;
        else if (value is TestB3)
            return Serializer<Resolver, TestB3>.Instance.GetCapacity((TestB3)value) + 1;
        else
            return Serializer<Resolver, TestB1>.Instance.GetCapacity(value) + 1;
    }
#pragma warning restore IDE0038 // Use pattern matching

    public override sealed void Serialize(ref Writer writer, TestB1 value)
    {
        if (value is null)
            writer.WriteByte(0); // Empty object
        else
            SerializeSpot(ref writer, value);
    }

#pragma warning disable IDE0038 // Use pattern matching
    public override sealed void SerializeSpot(ref Writer writer, TestB1 value)
    {
        if (value is TestB2)
        {
            writer.WriteByte(1);
            Serializer<Resolver, TestB2>.Instance.SerializeSpot(ref writer, (TestB2)value);
        }
        else if (value is TestB3)
        {
            writer.WriteByte(2);
            Serializer<Resolver, TestB3>.Instance.SerializeSpot(ref writer, (TestB3)value);
        }
        else
        {
            writer.WriteByte(3);
            Serializer<Resolver, TestB1>.Instance.SerializeSpot(ref writer, value);
        }
    }
#pragma warning restore IDE0038 // Use pattern matching

    public override sealed TestB1 Deserialize(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 0)
            return null;
        else if (index == 1)
            return Serializer<Resolver, TestB2>.Instance.DeserializeSpot(ref reader);
        else if (index == 2)
            return Serializer<Resolver, TestB3>.Instance.DeserializeSpot(ref reader);
        else
            return Serializer<Resolver, TestB1>.Instance.DeserializeSpot(ref reader);
    }

    public override sealed TestB1 DeserializeInUTC(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 0)
            return null;
        else if (index == 1)
            return Serializer<Resolver, TestB2>.Instance.DeserializeInUTCSpot(ref reader);
        else if (index == 2)
            return Serializer<Resolver, TestB3>.Instance.DeserializeInUTCSpot(ref reader);
        else
            return Serializer<Resolver, TestB1>.Instance.DeserializeInUTCSpot(ref reader);
    }

    public override sealed TestB1 DeserializeSpot(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 1)
            return Serializer<Resolver, TestB2>.Instance.DeserializeSpot(ref reader);
        else if (index == 2)
            return Serializer<Resolver, TestB3>.Instance.DeserializeSpot(ref reader);
        else
            return Serializer<Resolver, TestB1>.Instance.DeserializeSpot(ref reader);
    }

    public override sealed TestB1 DeserializeInUTCSpot(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 1)
            return Serializer<Resolver, TestB2>.Instance.DeserializeInUTCSpot(ref reader);
        else if (index == 2)
            return Serializer<Resolver, TestB3>.Instance.DeserializeInUTCSpot(ref reader);
        else
            return Serializer<Resolver, TestB1>.Instance.DeserializeInUTCSpot(ref reader);
    }

}

public sealed class TestB1ByteUnionSerializer : UnionByteMapSerializer<TestB1>
{
    public TestB1ByteUnionSerializer()
        : base(3)
    {
        Add<TestB2>(1);
        Add<TestB3>(2);
        Add<TestB1>(3);
    }

}

public sealed class TestB1UShortUnionSerializer : UnionUShortMapSerializer<TestB1>
{
    public TestB1UShortUnionSerializer()
        : base(3)
    {
        Add<TestB2>(1);
        Add<TestB3>(2);
        Add<TestB1>(3);
    }

}
