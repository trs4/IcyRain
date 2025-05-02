using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;
using ZeroFormatter;

namespace IcyRain.Data.Objects;

[DataContract]
[KnownType(typeof(TestA2)), KnownType(typeof(TestA3)), KnownType(typeof(TestA4)), KnownType(typeof(TestA5)), KnownType(typeof(TestA6)),
    KnownType(typeof(TestA7)), KnownType(typeof(TestA8)), KnownType(typeof(TestA9)), KnownType(typeof(TestA10)), KnownType(typeof(TestA11)),
    KnownType(typeof(TestA12)), KnownType(typeof(TestA13)), KnownType(typeof(TestA14)), KnownType(typeof(TestA15)), KnownType(typeof(TestA16)),
    KnownType(typeof(TestA17)), KnownType(typeof(TestA18)), KnownType(typeof(TestA19)), KnownType(typeof(TestA20)), KnownType(typeof(TestA21)),
    KnownType(typeof(TestA22)), KnownType(typeof(TestA23)), KnownType(typeof(TestA24)), KnownType(typeof(TestA25))]
[ZeroFormattable]
[DynamicUnion]
public abstract class TestA1
{
    static TestA1()
    {
        ZeroFormatter.Formatters.Formatter.AppendDynamicUnionResolver((unionType, resolver) =>
        {
            if (unionType == typeof(TestA1))
            {
                resolver.RegisterUnionKeyType(typeof(byte));
                resolver.RegisterSubType(key: (byte)1, subType: typeof(TestA2));
                resolver.RegisterSubType(key: (byte)2, subType: typeof(TestA3));
                resolver.RegisterSubType(key: (byte)3, subType: typeof(TestA4));
                resolver.RegisterSubType(key: (byte)4, subType: typeof(TestA5));
                resolver.RegisterSubType(key: (byte)5, subType: typeof(TestA6));
                resolver.RegisterSubType(key: (byte)6, subType: typeof(TestA7));
                resolver.RegisterSubType(key: (byte)7, subType: typeof(TestA8));
                resolver.RegisterSubType(key: (byte)8, subType: typeof(TestA9));
                resolver.RegisterSubType(key: (byte)9, subType: typeof(TestA10));
                resolver.RegisterSubType(key: (byte)10, subType: typeof(TestA11));

                resolver.RegisterSubType(key: (byte)11, subType: typeof(TestA12));
                resolver.RegisterSubType(key: (byte)12, subType: typeof(TestA13));
                resolver.RegisterSubType(key: (byte)13, subType: typeof(TestA14));
                resolver.RegisterSubType(key: (byte)14, subType: typeof(TestA15));
                resolver.RegisterSubType(key: (byte)15, subType: typeof(TestA16));
                resolver.RegisterSubType(key: (byte)16, subType: typeof(TestA17));
                resolver.RegisterSubType(key: (byte)17, subType: typeof(TestA18));
                resolver.RegisterSubType(key: (byte)18, subType: typeof(TestA19));
                resolver.RegisterSubType(key: (byte)19, subType: typeof(TestA20));
                resolver.RegisterSubType(key: (byte)20, subType: typeof(TestA21));

                resolver.RegisterSubType(key: (byte)21, subType: typeof(TestA22));
                resolver.RegisterSubType(key: (byte)22, subType: typeof(TestA23));
                resolver.RegisterSubType(key: (byte)23, subType: typeof(TestA24));
                resolver.RegisterSubType(key: (byte)24, subType: typeof(TestA25));
            }
        });
    }

    [DataMember(Order = 1)]
    [Index(0)]
    public virtual bool Property11 { get; set; }
}

[DataContract]
[ZeroFormattable]
public class TestA2 : TestA1
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
public class TestA3 : TestA1
{
    [DataMember(Order = 4)]
    [Index(3)]
    public virtual double Property31 { get; set; }

    [DataMember(Order = 5)]
    [Index(4)]
    public virtual DateTime Property32 { get; set; }

    [DataMember(Order = 6)]
    [Index(5)]
    public virtual Guid? Property33 { get; set; }

    [DataMember(Order = 7)]
    [Index(6)]
    public virtual string Property34 { get; set; }

    [DataMember(Order = 8)]
    [Index(7)]
    public virtual TestB1 Property35 { get; set; }
}

[DataContract]
[ZeroFormattable]
public class TestA4 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA5 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA6 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA7 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA8 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA9 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA10 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA11 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA12 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA13 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA14 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA15 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA16 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA17 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA18 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA19 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA20 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA21 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA22 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA23 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA24 : TestA3 { }

[DataContract]
[ZeroFormattable]
public class TestA25 : TestA3 { }


public sealed class TestA1Serializer : Serializer<Resolver, TestA1>
{
    private readonly Serializer<Resolver, bool> _s_Property11Serializer;

    public TestA1Serializer()
    {
        _s_Property11Serializer = Serializer<Resolver, bool>.Instance;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 50;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TestA1 value)
        => value is null ? 1 : 50;

    public override sealed void Serialize(ref Writer writer, TestA1 value)
    {
        if (value is null)
        {
            writer.WriteByte(255); // Empty object
            return;
        }

        if (value.Property11 != default)
        {
            writer.WriteByte(1);
            _s_Property11Serializer.SerializeSpot(ref writer, value.Property11);
        }

        writer.WriteByte(0); // End object
    }

    public override sealed void SerializeSpot(ref Writer writer, TestA1 value)
    {
        throw new NotImplementedException();
    }

    public override sealed TestA1 Deserialize(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 255 or 1 or 0

        if (index == 255)
            return null;

#if NET8_0_OR_GREATER
        var obj = (TestA1)RuntimeHelpers.GetUninitializedObject(typeof(TestA1));
#else
        var obj = (TestA1)FormatterServices.GetUninitializedObject(typeof(TestA1));
#endif
        if (index == 1)
        {
            obj.Property11 = _s_Property11Serializer.DeserializeSpot(ref reader);
            reader.ReadByte(); // Read 0
        }

        return obj;
    }

    public override sealed TestA1 DeserializeInUTC(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 255 or 1 or 0

        if (index == 255)
            return null;

#if NET8_0_OR_GREATER
        var obj = (TestA1)RuntimeHelpers.GetUninitializedObject(typeof(TestA1));
#else
        var obj = (TestA1)FormatterServices.GetUninitializedObject(typeof(TestA1));
#endif
        if (index == 1)
        {
            obj.Property11 = _s_Property11Serializer.DeserializeInUTCSpot(ref reader);
            reader.ReadByte(); // Read 0
        }

        return obj;
    }

    public override sealed TestA1 DeserializeSpot(ref Reader reader)
    {
        throw new NotImplementedException();
    }

    public override sealed TestA1 DeserializeInUTCSpot(ref Reader reader)
    {
        throw new NotImplementedException();
    }

}

public sealed class TestA2Serializer : Serializer<Resolver, TestA2>
{
    private readonly Serializer<Resolver, bool> _s_Property11Serializer;
    private readonly Serializer<Resolver, bool> _s_Property21Serializer;
    private readonly Serializer<Resolver, int> _s_Property22Serializer;

    public TestA2Serializer()
    {
        _s_Property11Serializer = Serializer<Resolver, bool>.Instance;
        _s_Property21Serializer = Serializer<Resolver, bool>.Instance;
        _s_Property22Serializer = Serializer<Resolver, int>.Instance;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TestA2 value)
        => value is null ? 1 : 50;

    public override sealed void Serialize(ref Writer writer, TestA2 value)
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

    public override sealed void SerializeSpot(ref Writer writer, TestA2 value)
    {
        throw new NotImplementedException();
    }

    public override sealed TestA2 Deserialize(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 0)
            return null;

#if NET8_0_OR_GREATER
        var obj = (TestA2)RuntimeHelpers.GetUninitializedObject(typeof(TestA2));
#else
        var obj = (TestA2)FormatterServices.GetUninitializedObject(typeof(TestA2));
#endif
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

    public override sealed TestA2 DeserializeInUTC(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 0)
            return null;

#if NET8_0_OR_GREATER
        var obj = (TestA2)RuntimeHelpers.GetUninitializedObject(typeof(TestA2));
#else
        var obj = (TestA2)FormatterServices.GetUninitializedObject(typeof(TestA2));
#endif
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

    public override sealed TestA2 DeserializeSpot(ref Reader reader)
    {
        throw new NotImplementedException();
    }

    public override sealed TestA2 DeserializeInUTCSpot(ref Reader reader)
    {
        throw new NotImplementedException();
    }

}

public sealed class TestA3Serializer : Serializer<Resolver, TestA3>
{
    private readonly Serializer<Resolver, bool> _s_Property11Serializer;
    private readonly Serializer<Resolver, double> _s_Property31Serializer;
    private readonly Serializer<Resolver, DateTime> _s_Property32Serializer;
    private readonly Serializer<Resolver, Guid> _s_Property33Serializer;
    private readonly Serializer<Resolver, string> _s_Property34Serializer;
    private readonly Serializer<UnionResolver, TestB1> _s_Property35Serializer;

    public TestA3Serializer()
    {
        _s_Property11Serializer = Serializer<Resolver, bool>.Instance;
        _s_Property31Serializer = Serializer<Resolver, double>.Instance;
        _s_Property32Serializer = Serializer<Resolver, DateTime>.Instance;
        _s_Property33Serializer = Serializer<Resolver, Guid>.Instance;
        _s_Property34Serializer = Serializer<Resolver, string>.Instance;
        _s_Property35Serializer = Serializer<UnionResolver, TestB1>.Instance;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TestA3 value)
        => value is null ? 1 : 50;

    public override sealed void Serialize(ref Writer writer, TestA3 value)
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

        if (!value.Property32.Equals(DateTime.MinValue))
        {
            writer.WriteUShort(3);
            _s_Property32Serializer.SerializeSpot(ref writer, value.Property32);
        }

        var localProperty33 = value.Property33;

        if (localProperty33.HasValue)
        {
            writer.WriteUShort(4);
            _s_Property33Serializer.SerializeSpot(ref writer, localProperty33.Value);
        }

        if (value.Property34 is not null)
        {
            writer.WriteUShort(5);
            _s_Property34Serializer.SerializeSpot(ref writer, value.Property34);
        }

        if (value.Property35 is not null)
        {
            writer.WriteUShort(6);
            _s_Property35Serializer.SerializeSpot(ref writer, value.Property35);
        }

        writer.WriteUShort(65535); // End object
    }

    public override sealed void SerializeSpot(ref Writer writer, TestA3 value)
    {
        throw new NotImplementedException();
    }

    public override sealed TestA3 Deserialize(ref Reader reader)
    {
        ushort index = reader.ReadUShort(); // Read 0 or 1 or 65535

        if (index == 0)
            return null;

#if NET8_0_OR_GREATER
        var obj = (TestA3)RuntimeHelpers.GetUninitializedObject(typeof(TestA3));
#else
        var obj = (TestA3)FormatterServices.GetUninitializedObject(typeof(TestA3));
#endif
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
            reader.ReadUShort(); // Read 4
        }

        if (index == 4)
        {
            obj.Property33 = _s_Property33Serializer.DeserializeSpot(ref reader);
            reader.ReadUShort(); // Read 5
        }

        if (index == 5)
        {
            obj.Property34 = _s_Property34Serializer.DeserializeSpot(ref reader);
            reader.ReadUShort(); // Read 6
        }

        if (index == 6)
        {
            obj.Property35 = _s_Property35Serializer.DeserializeSpot(ref reader);
            reader.ReadUShort(); // Read 65535
        }

        return obj;
    }

    public override sealed TestA3 DeserializeInUTC(ref Reader reader)
    {
        ushort index = reader.ReadUShort(); // Read 0 or 1 or 65535

        if (index == 0)
            return null;

#if NET8_0_OR_GREATER
        var obj = (TestA3)RuntimeHelpers.GetUninitializedObject(typeof(TestA3));
#else
        var obj = (TestA3)FormatterServices.GetUninitializedObject(typeof(TestA3));
#endif
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
            reader.ReadUShort(); // Read 4
        }

        if (index == 4)
        {
            obj.Property33 = _s_Property33Serializer.DeserializeInUTCSpot(ref reader);
            reader.ReadUShort(); // Read 5
        }

        if (index == 5)
        {
            obj.Property34 = _s_Property34Serializer.DeserializeInUTCSpot(ref reader);
            reader.ReadUShort(); // Read 6
        }

        if (index == 6)
        {
            obj.Property35 = _s_Property35Serializer.DeserializeInUTCSpot(ref reader);
            reader.ReadUShort(); // Read 65535
        }

        return obj;
    }

    public override sealed TestA3 DeserializeSpot(ref Reader reader)
    {
        throw new NotImplementedException();
    }

    public override sealed TestA3 DeserializeInUTCSpot(ref Reader reader)
    {
        throw new NotImplementedException();
    }

}


public sealed class TestA1DynamicSerializer : Serializer<UnionResolver, TestA1>
{
    public TestA1DynamicSerializer() { }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

#pragma warning disable IDE0038 // Use pattern matching
    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TestA1 value)
    {
        if (value is null)
            return 1;
        else if (value is TestA2)
            return Serializer<Resolver, TestA2>.Instance.GetCapacity((TestA2)value) + 1;
        else if (value is TestA3)
            return Serializer<Resolver, TestA3>.Instance.GetCapacity((TestA3)value) + 1;
        else
            throw new InvalidOperationException("Unknown type: " + value.GetType().FullName);
    }
#pragma warning restore IDE0038 // Use pattern matching

    public override sealed void Serialize(ref Writer writer, TestA1 value)
    {
        if (value is null)
            writer.WriteByte(0); // Empty object
        else
            SerializeSpot(ref writer, value);
    }

#pragma warning disable IDE0038 // Use pattern matching
    public override sealed void SerializeSpot(ref Writer writer, TestA1 value)
    {
        if (value is TestA2)
        {
            writer.WriteByte(1);
            Serializer<Resolver, TestA2>.Instance.SerializeSpot(ref writer, (TestA2)value);
        }
        else if (value is TestA3)
        {
            writer.WriteByte(2);
            Serializer<Resolver, TestA3>.Instance.SerializeSpot(ref writer, (TestA3)value);
        }
        else
        {
            throw new InvalidOperationException("Unknown type: " + value.GetType().FullName);
        }
    }
#pragma warning restore IDE0038 // Use pattern matching

    public override sealed TestA1 Deserialize(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 0)
            return null;
        else if (index == 1)
            return Serializer<Resolver, TestA2>.Instance.DeserializeSpot(ref reader);
        else if (index == 2)
            return Serializer<Resolver, TestA3>.Instance.DeserializeSpot(ref reader);
        else
            throw new InvalidOperationException("Unknown index: " + index);
    }

    public override sealed TestA1 DeserializeInUTC(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 0)
            return null;
        else if (index == 1)
            return Serializer<Resolver, TestA2>.Instance.DeserializeInUTCSpot(ref reader);
        else if (index == 2)
            return Serializer<Resolver, TestA3>.Instance.DeserializeInUTCSpot(ref reader);
        else
            throw new InvalidOperationException("Unknown index: " + index);
    }

    public override sealed TestA1 DeserializeSpot(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 1)
            return Serializer<Resolver, TestA2>.Instance.DeserializeSpot(ref reader);
        else if (index == 2)
            return Serializer<Resolver, TestA3>.Instance.DeserializeSpot(ref reader);
        else
            throw new InvalidOperationException("Unknown index: " + index);
    }

    public override sealed TestA1 DeserializeInUTCSpot(ref Reader reader)
    {
        byte index = reader.ReadByte(); // Read 0 or 1 or 255

        if (index == 1)
            return Serializer<Resolver, TestA2>.Instance.DeserializeInUTCSpot(ref reader);
        else if (index == 2)
            return Serializer<Resolver, TestA3>.Instance.DeserializeInUTCSpot(ref reader);
        else
            throw new InvalidOperationException("Unknown index: " + index);
    }

}
