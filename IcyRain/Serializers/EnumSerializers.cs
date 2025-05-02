using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers;

internal sealed class EnumSerializer<TResolver, T> : Serializer<TResolver, T>
    where TResolver : Resolver
    where T : struct
{
    private readonly int _size;
    private readonly SerializeDelegate<T> _serializer;
    private readonly DeserializeDelegate<T> _deserializer;

    public EnumSerializer()
    {
        var underlyingCode = Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T)));
        var type = EnumHelper.GetType<T>(underlyingCode);
        _size = type.GetSize();
        _serializer = type.GetSerializer<T>(underlyingCode);
        _deserializer = type.GetDeserializer<T>(underlyingCode);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => _size;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(T value) => _size;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, T value)
        => _serializer(ref writer, value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, T value)
        => _serializer(ref writer, value);

    [MethodImpl(Flags.HotPath)]
    public override sealed T Deserialize(ref Reader reader)
        => _deserializer(ref reader);

    [MethodImpl(Flags.HotPath)]
    public override sealed T DeserializeInUTC(ref Reader reader)
        => _deserializer(ref reader);

    [MethodImpl(Flags.HotPath)]
    public override sealed T DeserializeSpot(ref Reader reader)
        => _deserializer(ref reader);

    [MethodImpl(Flags.HotPath)]
    public override sealed T DeserializeInUTCSpot(ref Reader reader)
        => _deserializer(ref reader);
}

internal sealed class NullableEnumSerializer<TResolver, T> : Serializer<TResolver, T?>
    where TResolver : Resolver
    where T : struct
{
    private readonly int _size;
    private readonly SerializeDelegate<T> _serializer;
    private readonly DeserializeDelegate<T> _deserializer;

    public NullableEnumSerializer()
    {
        var underlyingCode = Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T)));
        var type = EnumHelper.GetType<T>(underlyingCode);
        _size = type.GetSize() + 1;
        _serializer = type.GetSerializer<T>(underlyingCode);
        _deserializer = type.GetDeserializer<T>(underlyingCode);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => _size;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(T? value) => _size;

    public override sealed void Serialize(ref Writer writer, T? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            _serializer(ref writer, value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, T? value)
        => throw new NotSupportedException();

    public override sealed T? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? _deserializer(ref reader) : null;
    }

    public override sealed T? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? _deserializer(ref reader) : null;
    }

    public override sealed T? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed T? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class ArrayEnumSerializer<TResolver, T> : Serializer<TResolver, T[]>
    where TResolver : Resolver
    where T : struct
{
    private readonly int _size;
    private readonly SerializeDelegate<T> _serializer;
    private readonly DeserializeDelegate<T> _deserializer;

    public ArrayEnumSerializer()
    {
        var underlyingCode = Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T)));
        var type = EnumHelper.GetType<T>(underlyingCode);
        _size = type.GetSize();
        _serializer = type.GetSerializer<T>(underlyingCode);
        _deserializer = type.GetDeserializer<T>(underlyingCode);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(T[] value)
        => value is null ? 4 : value.Length * _size + 4;

    public override sealed void Serialize(ref Writer writer, T[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
        {
            for (int i = 0; i < value.Length; i++)
                _serializer(ref writer, value[i]);
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, T[] value)
    {
        writer.WriteInt(value.Length);

        for (int i = 0; i < value.Length; i++)
            _serializer(ref writer, value[i]);
    }

    public override sealed T[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _deserializer(ref reader);

            return value;
        }

        return length == 0 ? [] : null;
    }

    public override sealed T[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _deserializer(ref reader);

            return value;
        }

        return length == 0 ? [] : null;
    }

    public override sealed T[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return [];

        var value = new T[length];

        for (int i = 0; i < length; i++)
            value[i] = _deserializer(ref reader);

        return value;
    }

    public override sealed T[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return [];

        var value = new T[length];

        for (int i = 0; i < length; i++)
            value[i] = _deserializer(ref reader);

        return value;
    }

}

internal sealed class ListEnumSerializer<TResolver, T> : Serializer<TResolver, List<T>>
    where TResolver : Resolver
    where T : struct
{
    private readonly int _size;
    private readonly SerializeDelegate<T> _serializer;
    private readonly DeserializeDelegate<T> _deserializer;

    public ListEnumSerializer()
    {
        var underlyingCode = Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T)));
        var type = EnumHelper.GetType<T>(underlyingCode);
        _size = type.GetSize();
        _serializer = type.GetSerializer<T>(underlyingCode);
        _deserializer = type.GetDeserializer<T>(underlyingCode);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(List<T> value)
        => value is null ? 4 : value.Count * _size + 4;

    public override sealed void Serialize(ref Writer writer, List<T> value)
    {
        int length = value is null ? -1 : value.Count;
        writer.WriteInt(length);

        if (length > 0)
        {
            var array = value.GetArray();

            for (int i = 0; i < length; i++)
                _serializer(ref writer, array[i]);
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, List<T> value)
    {
        writer.WriteInt(value.Count);
        var array = value.GetArray();

        for (int i = 0; i < value.Count; i++)
            _serializer(ref writer, array[i]);
    }

    public override sealed List<T> Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _deserializer(ref reader);

            return value.CreateList();
        }

        return length == 0 ? [] : null;
    }

    public override sealed List<T> DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new T[length];

            for (int i = 0; i < length; i++)
                value[i] = _deserializer(ref reader);

            return value.CreateList();
        }

        return length == 0 ? [] : null;
    }

    public override sealed List<T> DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return [];

        var value = new T[length];

        for (int i = 0; i < length; i++)
            value[i] = _deserializer(ref reader);

        return value.CreateList();
    }

    public override sealed List<T> DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return [];

        var value = new T[length];

        for (int i = 0; i < length; i++)
            value[i] = _deserializer(ref reader);

        return value.CreateList();
    }

}

#region Cast

internal enum EnumType
{
    Byte,
    SByte,
    Short,
    UShort,
    Int,
    UInt,
    Long,
    ULong,
}

internal delegate void SerializeDelegate<T>(ref Writer writer, T value);
internal delegate T DeserializeDelegate<T>(ref Reader reader);

internal static class EnumHelper
{
    public static EnumType GetType<T>(TypeCode underlyingCode)
    {
        switch (underlyingCode)
        {
            case TypeCode.SByte: return EnumType.SByte;
            case TypeCode.Byte: return EnumType.Byte;
            case TypeCode.Int16: return EnumType.Short;
            case TypeCode.UInt16: return EnumType.UShort;
            case TypeCode.Int64: return EnumType.Long;
            case TypeCode.UInt64: return EnumType.ULong;
        }

        var values = Enum.GetValues(typeof(T));
        var numValues = new List<long>(values.Length);

        foreach (object item in values)
        {
            switch (underlyingCode)
            {
                case TypeCode.Int32:
                    numValues.Add((int)item);
                    break;
                case TypeCode.UInt32:
                    numValues.Add((uint)item);
                    break;
            }
        }

        long minValue = numValues.Min();
        long maxValue = numValues.Max();

        if (minValue < 0)
        {
            maxValue = Math.Max(maxValue, -minValue);

            if (maxValue <= sbyte.MaxValue)
                return EnumType.SByte;
            else if (maxValue <= short.MaxValue)
                return EnumType.Short;
            else
                return EnumType.Int;
        }
        else
        {
            if (maxValue <= byte.MaxValue)
                return EnumType.Byte;

            return underlyingCode switch
            {
                TypeCode.UInt32 => maxValue <= ushort.MaxValue ? EnumType.UShort : EnumType.UInt,
                TypeCode.Int32 => maxValue <= short.MaxValue ? EnumType.Short : EnumType.Int,
                _ => throw new NotSupportedException(typeof(T).FullName),
            };
        }
    }

    public static int GetSize(this EnumType type) => type switch
    {
        EnumType.Byte or EnumType.SByte => 1,
        EnumType.Short or EnumType.UShort => 2,
        EnumType.Int or EnumType.UInt => 4,
        EnumType.Long or EnumType.ULong => 8,
        _ => throw new NotImplementedException(type.ToString()),
    };

    public static SerializeDelegate<T> GetSerializer<T>(this EnumType type, TypeCode underlyingCode)
    {
        switch (underlyingCode)
        {
            case TypeCode.Byte: return WriteByteToByte;
            case TypeCode.SByte: return WriteSByteToSByte;
            case TypeCode.Int16: return WriteShortToShort;
            case TypeCode.UInt16: return WriteUShortToUShort;
            case TypeCode.Int32:
                {
                    switch (type)
                    {
                        case EnumType.Byte: return WriteIntToByte;
                        case EnumType.SByte: return WriteIntToSByte;
                        case EnumType.Short: return WriteIntToShort;
                        case EnumType.Int: return WriteIntToInt;
                    }
                }
                break;
            case TypeCode.UInt32:
                {
                    switch (type)
                    {
                        case EnumType.Byte: return WriteUIntToByte;
                        case EnumType.SByte: return WriteUIntToSByte;
                        case EnumType.UShort: return WriteUIntToUShort;
                        case EnumType.UInt: return WriteUIntToUInt;
                    }
                }
                break;
            case TypeCode.Int64: return WriteLongToLong;
            case TypeCode.UInt64: return WriteULongToULong;
        }

        throw new NotSupportedException($"{type} {underlyingCode}");
    }

    public static DeserializeDelegate<T> GetDeserializer<T>(this EnumType type, TypeCode underlyingCode)
    {
        switch (underlyingCode)
        {
            case TypeCode.Byte: return ReadByteToByte<T>;
            case TypeCode.SByte: return ReadSByteToSByte<T>;
            case TypeCode.Int16: return ReadShortToShort<T>;
            case TypeCode.UInt16: return ReadUShortToUShort<T>;
            case TypeCode.Int32:
                {
                    switch (type)
                    {
                        case EnumType.Byte: return ReadByteToInt<T>;
                        case EnumType.SByte: return ReadSByteToInt<T>;
                        case EnumType.Short: return ReadShortToInt<T>;
                        case EnumType.Int: return ReadIntToInt<T>;
                    }
                }
                break;
            case TypeCode.UInt32:
                {
                    switch (type)
                    {
                        case EnumType.Byte: return ReadByteToUInt<T>;
                        case EnumType.SByte: return ReadSByteToUInt<T>;
                        case EnumType.UShort: return ReadUShortToUInt<T>;
                        case EnumType.UInt: return ReadUIntToUInt<T>;
                    }
                }
                break;
            case TypeCode.Int64: return ReadLongToLong<T>;
            case TypeCode.UInt64: return ReadULongToULong<T>;
        }

        throw new NotSupportedException($"{type} {underlyingCode}");
    }

    #region Read / Write

    [MethodImpl(Flags.HotPath)]
    private static void WriteByteToByte<T>(ref Writer writer, T value) => writer.WriteByte(EnumTransformByteToByte<T>.Serialize(value));

    [MethodImpl(Flags.HotPath)]
    private static T ReadByteToByte<T>(ref Reader reader) => EnumTransformByteToByte<T>.Deserialize(reader.ReadByte());


    [MethodImpl(Flags.HotPath)]
    private static T ReadByteToInt<T>(ref Reader reader) => EnumTransformByteToInt<T>.Deserialize(reader.ReadByte());


    [MethodImpl(Flags.HotPath)]
    private static T ReadByteToUInt<T>(ref Reader reader) => EnumTransformByteToUInt<T>.Deserialize(reader.ReadByte());


    [MethodImpl(Flags.HotPath)]
    private static void WriteSByteToSByte<T>(ref Writer writer, T value) => writer.WriteSByte(EnumTransformSByteToSByte<T>.Serialize(value));

    [MethodImpl(Flags.HotPath)]
    private static T ReadSByteToSByte<T>(ref Reader reader) => EnumTransformSByteToSByte<T>.Deserialize(reader.ReadSByte());


    [MethodImpl(Flags.HotPath)]
    private static T ReadSByteToInt<T>(ref Reader reader) => EnumTransformSByteToInt<T>.Deserialize(reader.ReadSByte());


    [MethodImpl(Flags.HotPath)]
    private static T ReadSByteToUInt<T>(ref Reader reader) => EnumTransformSByteToUInt<T>.Deserialize(reader.ReadSByte());


    [MethodImpl(Flags.HotPath)]
    private static void WriteShortToShort<T>(ref Writer writer, T value) => writer.WriteShort(EnumTransformShortToShort<T>.Serialize(value));

    [MethodImpl(Flags.HotPath)]
    private static T ReadShortToShort<T>(ref Reader reader) => EnumTransformShortToShort<T>.Deserialize(reader.ReadShort());


    [MethodImpl(Flags.HotPath)]
    private static T ReadShortToInt<T>(ref Reader reader) => EnumTransformShortToInt<T>.Deserialize(reader.ReadShort());


    [MethodImpl(Flags.HotPath)]
    private static T ReadUShortToUInt<T>(ref Reader reader) => EnumTransformUShortToUInt<T>.Deserialize(reader.ReadUShort());


    [MethodImpl(Flags.HotPath)]
    private static void WriteUShortToUShort<T>(ref Writer writer, T value) => writer.WriteUShort(EnumTransformUShortToUShort<T>.Serialize(value));

    [MethodImpl(Flags.HotPath)]
    private static T ReadUShortToUShort<T>(ref Reader reader) => EnumTransformUShortToUShort<T>.Deserialize(reader.ReadUShort());


    [MethodImpl(Flags.HotPath)]
    private static void WriteIntToByte<T>(ref Writer writer, T value) => writer.WriteByte(EnumTransformIntToByte<T>.Serialize(value));


    [MethodImpl(Flags.HotPath)]
    private static void WriteIntToSByte<T>(ref Writer writer, T value) => writer.WriteSByte(EnumTransformIntToSByte<T>.Serialize(value));


    [MethodImpl(Flags.HotPath)]
    private static void WriteIntToShort<T>(ref Writer writer, T value) => writer.WriteShort(EnumTransformIntToShort<T>.Serialize(value));


    [MethodImpl(Flags.HotPath)]
    private static void WriteIntToInt<T>(ref Writer writer, T value) => writer.WriteInt(EnumTransformIntToInt<T>.Serialize(value));

    [MethodImpl(Flags.HotPath)]
    private static T ReadIntToInt<T>(ref Reader reader) => EnumTransformIntToInt<T>.Deserialize(reader.ReadInt());


    [MethodImpl(Flags.HotPath)]
    private static void WriteUIntToByte<T>(ref Writer writer, T value) => writer.WriteByte(EnumTransformUIntToByte<T>.Serialize(value));


    [MethodImpl(Flags.HotPath)]
    private static void WriteUIntToSByte<T>(ref Writer writer, T value) => writer.WriteSByte(EnumTransformUIntToSByte<T>.Serialize(value));


    [MethodImpl(Flags.HotPath)]
    private static void WriteUIntToUShort<T>(ref Writer writer, T value) => writer.WriteUShort(EnumTransformUIntToUShort<T>.Serialize(value));


    [MethodImpl(Flags.HotPath)]
    private static void WriteUIntToUInt<T>(ref Writer writer, T value) => writer.WriteUInt(EnumTransformUIntToUInt<T>.Serialize(value));

    [MethodImpl(Flags.HotPath)]
    private static T ReadUIntToUInt<T>(ref Reader reader) => EnumTransformUIntToUInt<T>.Deserialize(reader.ReadUInt());


    [MethodImpl(Flags.HotPath)]
    private static void WriteLongToLong<T>(ref Writer writer, T value) => writer.WriteLong(EnumTransformLongToLong<T>.Serialize(value));

    [MethodImpl(Flags.HotPath)]
    private static T ReadLongToLong<T>(ref Reader reader) => EnumTransformLongToLong<T>.Deserialize(reader.ReadLong());


    [MethodImpl(Flags.HotPath)]
    private static void WriteULongToULong<T>(ref Writer writer, T value) => writer.WriteULong(EnumTransformULongToULong<T>.Serialize(value));

    [MethodImpl(Flags.HotPath)]
    private static T ReadULongToULong<T>(ref Reader reader) => EnumTransformULongToULong<T>.Deserialize(reader.ReadULong());

    #endregion
}

#endregion
#region Transform

internal static class EnumTransformByteToByte<T>
{
    [MethodImpl(Flags.HotPath)]
    private static byte Transform(byte x) => x;

    public static readonly Func<T, byte> Serialize;
    public static readonly Func<byte, T> Deserialize;

    static EnumTransformByteToByte()
    {
        Func<byte, byte> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, byte>)) as Func<T, byte>;
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<byte, T>)) as Func<byte, T>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, byte>>();
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<byte, T>>();
#endif
    }
}

internal static class EnumTransformByteToInt<T>
{
    [MethodImpl(Flags.HotPath)]
    private static int Transform(byte x) => x;

    public static readonly Func<byte, T> Deserialize;

    static EnumTransformByteToInt()
    {
        Func<byte, int> transform = Transform;
#if NETFRAMEWORK
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<byte, T>)) as Func<byte, T>;
#else
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<byte, T>>();
#endif
    }
}

internal static class EnumTransformByteToUInt<T>
{
    [MethodImpl(Flags.HotPath)]
    private static uint Transform(byte x) => x;

    public static readonly Func<byte, T> Deserialize;

    static EnumTransformByteToUInt()
    {
        Func<byte, uint> transform = Transform;
#if NETFRAMEWORK
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<byte, T>)) as Func<byte, T>;
#else
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<byte, T>>();
#endif
    }
}

internal static class EnumTransformSByteToSByte<T>
{
    [MethodImpl(Flags.HotPath)]
    private static sbyte Transform(sbyte x) => x;

    public static readonly Func<T, sbyte> Serialize;
    public static readonly Func<sbyte, T> Deserialize;

    static EnumTransformSByteToSByte()
    {
        Func<sbyte, sbyte> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, sbyte>)) as Func<T, sbyte>;
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<sbyte, T>)) as Func<sbyte, T>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, sbyte>>();
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<sbyte, T>>();
#endif
    }
}

internal static class EnumTransformSByteToInt<T>
{
    [MethodImpl(Flags.HotPath)]
    private static int Transform(sbyte x) => x;

    public static readonly Func<sbyte, T> Deserialize;

    static EnumTransformSByteToInt()
    {
        Func<sbyte, int> transform = Transform;
#if NETFRAMEWORK
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<sbyte, T>)) as Func<sbyte, T>;
#else
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<sbyte, T>>();
#endif
    }
}

internal static class EnumTransformSByteToUInt<T>
{
    [MethodImpl(Flags.HotPath)]
    private static uint Transform(sbyte x) => (uint)x;

    public static readonly Func<sbyte, T> Deserialize;

    static EnumTransformSByteToUInt()
    {
        Func<sbyte, uint> transform = Transform;
#if NETFRAMEWORK
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<sbyte, T>)) as Func<sbyte, T>;
#else
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<sbyte, T>>();
#endif
    }
}

internal static class EnumTransformShortToShort<T>
{
    [MethodImpl(Flags.HotPath)]
    private static short Transform(short x) => x;

    public static readonly Func<T, short> Serialize;
    public static readonly Func<short, T> Deserialize;

    static EnumTransformShortToShort()
    {
        Func<short, short> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, short>)) as Func<T, short>;
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<short, T>)) as Func<short, T>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, short>>();
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<short, T>>();
#endif
    }
}

internal static class EnumTransformShortToInt<T>
{
    [MethodImpl(Flags.HotPath)]
    private static int Transform(short x) => x;

    public static readonly Func<short, T> Deserialize;

    static EnumTransformShortToInt()
    {
        Func<short, int> transform = Transform;
#if NETFRAMEWORK
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<short, T>)) as Func<short, T>;
#else
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<short, T>>();
#endif
    }
}

internal static class EnumTransformUShortToUShort<T>
{
    [MethodImpl(Flags.HotPath)]
    private static ushort Transform(ushort x) => x;

    public static readonly Func<T, ushort> Serialize;
    public static readonly Func<ushort, T> Deserialize;

    static EnumTransformUShortToUShort()
    {
        Func<ushort, ushort> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, ushort>)) as Func<T, ushort>;
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<ushort, T>)) as Func<ushort, T>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, ushort>>();
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<ushort, T>>();
#endif
    }
}

internal static class EnumTransformUShortToUInt<T>
{
    [MethodImpl(Flags.HotPath)]
    private static uint Transform(ushort x) => x;

    public static readonly Func<ushort, T> Deserialize;

    static EnumTransformUShortToUInt()
    {
        Func<ushort, uint> transform = Transform;
#if NETFRAMEWORK
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<ushort, T>)) as Func<ushort, T>;
#else
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<ushort, T>>();
#endif
    }
}

internal static class EnumTransformIntToByte<T>
{
    [MethodImpl(Flags.HotPath)]
    private static byte Transform(int x) => (byte)x;

    public static readonly Func<T, byte> Serialize;

    static EnumTransformIntToByte()
    {
        Func<int, byte> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, byte>)) as Func<T, byte>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, byte>>();
#endif
    }
}

internal static class EnumTransformIntToSByte<T>
{
    [MethodImpl(Flags.HotPath)]
    private static sbyte Transform(int x) => (sbyte)x;

    public static readonly Func<T, sbyte> Serialize;

    static EnumTransformIntToSByte()
    {
        Func<int, sbyte> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, sbyte>)) as Func<T, sbyte>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, sbyte>>();
#endif
    }
}

internal static class EnumTransformIntToShort<T>
{
    [MethodImpl(Flags.HotPath)]
    private static short Transform(int x) => (short)x;

    public static readonly Func<T, short> Serialize;

    static EnumTransformIntToShort()
    {
        Func<int, short> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, short>)) as Func<T, short>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, short>>();
#endif
    }
}

internal static class EnumTransformUIntToByte<T>
{
    [MethodImpl(Flags.HotPath)]
    private static byte Transform(uint x) => (byte)x;

    public static readonly Func<T, byte> Serialize;

    static EnumTransformUIntToByte()
    {
        Func<uint, byte> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, byte>)) as Func<T, byte>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, byte>>();
#endif
    }
}

internal static class EnumTransformUIntToSByte<T>
{
    [MethodImpl(Flags.HotPath)]
    private static sbyte Transform(uint x) => (sbyte)x;

    public static readonly Func<T, sbyte> Serialize;

    static EnumTransformUIntToSByte()
    {
        Func<uint, sbyte> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, sbyte>)) as Func<T, sbyte>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, sbyte>>();
#endif
    }
}

internal static class EnumTransformUIntToUShort<T>
{
    [MethodImpl(Flags.HotPath)]
    private static ushort Transform(uint x) => (ushort)x;

    public static readonly Func<T, ushort> Serialize;

    static EnumTransformUIntToUShort()
    {
        Func<uint, ushort> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, ushort>)) as Func<T, ushort>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, ushort>>();
#endif
    }
}

internal static class EnumTransformIntToInt<T>
{
    [MethodImpl(Flags.HotPath)]
    private static int Transform(int x) => x;

    public static readonly Func<T, int> Serialize;
    public static readonly Func<int, T> Deserialize;

    static EnumTransformIntToInt()
    {
        Func<int, int> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, int>)) as Func<T, int>;
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<int, T>)) as Func<int, T>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, int>>();
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<int, T>>();
#endif
    }
}

internal static class EnumTransformUIntToUInt<T>
{
    [MethodImpl(Flags.HotPath)]
    private static uint Transform(uint x) => x;

    public static readonly Func<T, uint> Serialize;
    public static readonly Func<uint, T> Deserialize;

    static EnumTransformUIntToUInt()
    {
        Func<uint, uint> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, uint>)) as Func<T, uint>;
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<uint, T>)) as Func<uint, T>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, uint>>();
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<uint, T>>();
#endif
    }
}

internal static class EnumTransformLongToLong<T>
{
    [MethodImpl(Flags.HotPath)]
    private static long Transform(long x) => x;

    public static readonly Func<T, long> Serialize;
    public static readonly Func<long, T> Deserialize;

    static EnumTransformLongToLong()
    {
        Func<long, long> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, long>)) as Func<T, long>;
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<long, T>)) as Func<long, T>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, long>>();
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<long, T>>();
#endif
    }
}

internal static class EnumTransformULongToULong<T>
{
    [MethodImpl(Flags.HotPath)]
    private static ulong Transform(ulong x) => x;

    public static readonly Func<T, ulong> Serialize;
    public static readonly Func<ulong, T> Deserialize;

    static EnumTransformULongToULong()
    {
        Func<ulong, ulong> transform = Transform;
#if NETFRAMEWORK
        Serialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<T, ulong>)) as Func<T, ulong>;
        Deserialize = transform.GetMethodInfo().CreateDelegate(typeof(Func<ulong, T>)) as Func<ulong, T>;
#else
        Serialize = transform.GetMethodInfo().CreateDelegate<Func<T, ulong>>();
        Deserialize = transform.GetMethodInfo().CreateDelegate<Func<ulong, T>>();
#endif
    }
}

#endregion
