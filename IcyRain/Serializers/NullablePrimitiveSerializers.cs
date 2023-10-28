using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers;

internal sealed class NullableBoolSerializer<TResolver> : Serializer<TResolver, bool?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 2;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(bool? value)
        => value.HasValue ? 2 : 1;

    public override sealed void Serialize(ref Writer writer, bool? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteBool(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, bool? value)
        => throw new NotSupportedException();

    public override sealed bool? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadBool() : null;
    }

    public override sealed bool? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadBool() : null;
    }

    public override sealed bool? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed bool? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableCharSerializer<TResolver> : Serializer<TResolver, char?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 3;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(char? value)
        => value.HasValue ? 3 : 1;

    public override sealed void Serialize(ref Writer writer, char? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteChar(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, char? value)
        => throw new NotSupportedException();

    public override sealed char? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadChar() : null;
    }

    public override sealed char? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadChar() : null;
    }

    public override sealed char? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed char? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableSByteSerializer<TResolver> : Serializer<TResolver, sbyte?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 2;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(sbyte? value)
        => value.HasValue ? 2 : 1;

    public override sealed void Serialize(ref Writer writer, sbyte? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteSByte(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, sbyte? value)
        => throw new NotSupportedException();

    public override sealed sbyte? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadSByte() : null;
    }

    public override sealed sbyte? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadSByte() : null;
    }

    public override sealed sbyte? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed sbyte? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableByteSerializer<TResolver> : Serializer<TResolver, byte?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 2;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(byte? value)
        => value.HasValue ? 2 : 1;

    public override sealed void Serialize(ref Writer writer, byte? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteByte(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, byte? value)
        => throw new NotSupportedException();

    public override sealed byte? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadByte() : null;
    }

    public override sealed byte? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadByte() : null;
    }

    public override sealed byte? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed byte? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableShortSerializer<TResolver> : Serializer<TResolver, short?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 3;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(short? value)
        => value.HasValue ? 3 : 1;

    public override sealed void Serialize(ref Writer writer, short? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteShort(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, short? value)
        => throw new NotSupportedException();

    public override sealed short? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadShort() : null;
    }

    public override sealed short? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadShort() : null;
    }

    public override sealed short? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed short? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableUShortSerializer<TResolver> : Serializer<TResolver, ushort?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 3;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(ushort? value)
        => value.HasValue ? 3 : 1;

    public override sealed void Serialize(ref Writer writer, ushort? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteUShort(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, ushort? value)
        => throw new NotSupportedException();

    public override sealed ushort? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadUShort() : null;
    }

    public override sealed ushort? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadUShort() : null;
    }

    public override sealed ushort? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed ushort? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableIntSerializer<TResolver> : Serializer<TResolver, int?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 5;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(int? value)
        => value.HasValue ? 5 : 1;

    public override sealed void Serialize(ref Writer writer, int? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteInt(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, int? value)
        => throw new NotSupportedException();

    public override sealed int? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadInt() : null;
    }

    public override sealed int? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadInt() : null;
    }

    public override sealed int? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed int? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableUIntSerializer<TResolver> : Serializer<TResolver, uint?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 5;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(uint? value)
        => value.HasValue ? 5 : 1;

    public override sealed void Serialize(ref Writer writer, uint? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteUInt(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, uint? value)
        => throw new NotSupportedException();

    public override sealed uint? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadUInt() : null;
    }

    public override sealed uint? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadUInt() : null;
    }

    public override sealed uint? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed uint? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableLongSerializer<TResolver> : Serializer<TResolver, long?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 9;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(long? value)
        => value.HasValue ? 9 : 1;

    public override sealed void Serialize(ref Writer writer, long? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteLong(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, long? value)
        => throw new NotSupportedException();

    public override sealed long? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadLong() : null;
    }

    public override sealed long? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadLong() : null;
    }

    public override sealed long? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed long? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableULongSerializer<TResolver> : Serializer<TResolver, ulong?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 9;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(ulong? value)
        => value.HasValue ? 9 : 1;

    public override sealed void Serialize(ref Writer writer, ulong? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteULong(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, ulong? value)
        => throw new NotSupportedException();

    public override sealed ulong? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadULong() : null;
    }

    public override sealed ulong? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadULong() : null;
    }

    public override sealed ulong? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed ulong? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableFloatSerializer<TResolver> : Serializer<TResolver, float?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 5;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(float? value)
        => value.HasValue ? 5 : 1;

    public override sealed void Serialize(ref Writer writer, float? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteFloat(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, float? value)
        => throw new NotSupportedException();

    public override sealed float? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadFloat() : null;
    }

    public override sealed float? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadFloat() : null;
    }

    public override sealed float? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed float? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableDoubleSerializer<TResolver> : Serializer<TResolver, double?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 9;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(double? value)
        => value.HasValue ? 9 : 1;

    public override sealed void Serialize(ref Writer writer, double? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteDouble(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, double? value)
        => throw new NotSupportedException();

    public override sealed double? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadDouble() : null;
    }

    public override sealed double? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadDouble() : null;
    }

    public override sealed double? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed double? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableDecimalSerializer<TResolver> : Serializer<TResolver, decimal?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 17;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(decimal? value)
        => value.HasValue ? 17 : 1;

    public override sealed void Serialize(ref Writer writer, decimal? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteDecimal(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, decimal? value)
        => throw new NotSupportedException();

    public override sealed decimal? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadDecimal() : null;
    }

    public override sealed decimal? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadDecimal() : null;
    }

    public override sealed decimal? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed decimal? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableGuidSerializer<TResolver> : Serializer<TResolver, Guid?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 17;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(Guid? value)
        => value.HasValue ? 17 : 1;

    public override sealed void Serialize(ref Writer writer, Guid? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteGuid(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, Guid? value)
        => throw new NotSupportedException();

    public override sealed Guid? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadGuid() : null;
    }

    public override sealed Guid? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadGuid() : null;
    }

    public override sealed Guid? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed Guid? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableDateTimeSerializer<TResolver> : Serializer<TResolver, DateTime?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 10;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(DateTime? value)
        => value.HasValue ? 10 : 1;

    public override sealed void Serialize(ref Writer writer, DateTime? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteDateTime(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, DateTime? value)
        => throw new NotSupportedException();

    public override sealed DateTime? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadDateTime() : null;
    }

    public override sealed DateTime? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadDateTimeInUTC() : null;
    }

    public override sealed DateTime? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed DateTime? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableDateTimeOffsetSerializer<TResolver> : Serializer<TResolver, DateTimeOffset?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 11;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(DateTimeOffset? value)
        => value.HasValue ? 11 : 1;

    public override sealed void Serialize(ref Writer writer, DateTimeOffset? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteDateTimeWithoutZone(new DateTime(value.Value.Ticks, DateTimeKind.Utc));
            writer.WriteShort((short)value.Value.Offset.TotalMinutes);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, DateTimeOffset? value)
        => throw new NotSupportedException();

    public override sealed DateTimeOffset? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort())) : null;
    }

    public override sealed DateTimeOffset? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort())) : null;
    }

    public override sealed DateTimeOffset? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed DateTimeOffset? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableTimeSpanSerializer<TResolver> : Serializer<TResolver, TimeSpan?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 9;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TimeSpan? value)
        => value.HasValue ? 9 : 1;

    public override sealed void Serialize(ref Writer writer, TimeSpan? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteTimeSpan(value.Value);
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, TimeSpan? value)
        => throw new NotSupportedException();

    public override sealed TimeSpan? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadTimeSpan() : null;
    }

    public override sealed TimeSpan? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? reader.ReadTimeSpan() : null;
    }

    public override sealed TimeSpan? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed TimeSpan? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableIntPtrSerializer<TResolver> : Serializer<TResolver, IntPtr?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 9;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(IntPtr? value) => 9;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, IntPtr? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteLong(value.Value.ToInt64());
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, IntPtr? value)
        => throw new NotSupportedException();

    [MethodImpl(Flags.HotPath)]
    public override sealed IntPtr? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? new IntPtr(reader.ReadLong()) : null;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed IntPtr? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? new IntPtr(reader.ReadLong()) : null;
    }

    public override sealed IntPtr? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed IntPtr? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}

internal sealed class NullableUIntPtrSerializer<TResolver> : Serializer<TResolver, UIntPtr?>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 9;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(UIntPtr? value) => 9;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, UIntPtr? value)
    {
        if (value.HasValue)
        {
            writer.WriteBoolTrue();
            writer.WriteULong(value.Value.ToUInt64());
        }
        else
        {
            writer.WriteBoolFalse();
        }
    }

    public override sealed void SerializeSpot(ref Writer writer, UIntPtr? value)
        => throw new NotSupportedException();

    [MethodImpl(Flags.HotPath)]
    public override sealed UIntPtr? Deserialize(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? new UIntPtr(reader.ReadULong()) : null;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed UIntPtr? DeserializeInUTC(ref Reader reader)
    {
        bool hasValue = reader.ReadBool();
        return hasValue ? new UIntPtr(reader.ReadULong()) : null;
    }

    public override sealed UIntPtr? DeserializeSpot(ref Reader reader)
        => throw new NotSupportedException();

    public override sealed UIntPtr? DeserializeInUTCSpot(ref Reader reader)
        => throw new NotSupportedException();
}
