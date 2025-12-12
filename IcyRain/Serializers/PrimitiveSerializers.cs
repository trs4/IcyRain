using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers;

internal sealed class BoolSerializer<TResolver> : Serializer<TResolver, bool>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 1;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(bool value) => 1;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, bool value)
        => writer.WriteBool(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, bool value)
        => writer.WriteBool(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed bool Deserialize(ref Reader reader)
        => reader.ReadBool();

    [MethodImpl(Flags.HotPath)]
    public override sealed bool DeserializeInUTC(ref Reader reader)
        => reader.ReadBool();

    [MethodImpl(Flags.HotPath)]
    public override sealed bool DeserializeSpot(ref Reader reader)
        => reader.ReadBool();

    [MethodImpl(Flags.HotPath)]
    public override sealed bool DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadBool();
}

internal sealed class CharSerializer<TResolver> : Serializer<TResolver, char>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 2;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(char value) => 2;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, char value)
        => writer.WriteChar(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, char value)
        => writer.WriteChar(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed char Deserialize(ref Reader reader)
        => reader.ReadChar();

    [MethodImpl(Flags.HotPath)]
    public override sealed char DeserializeInUTC(ref Reader reader)
        => reader.ReadChar();

    [MethodImpl(Flags.HotPath)]
    public override sealed char DeserializeSpot(ref Reader reader)
        => reader.ReadChar();

    [MethodImpl(Flags.HotPath)]
    public override sealed char DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadChar();
}

internal sealed class SByteSerializer<TResolver> : Serializer<TResolver, sbyte>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 1;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(sbyte value) => 1;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, sbyte value)
        => writer.WriteSByte(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, sbyte value)
        => writer.WriteSByte(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed sbyte Deserialize(ref Reader reader)
        => reader.ReadSByte();

    [MethodImpl(Flags.HotPath)]
    public override sealed sbyte DeserializeInUTC(ref Reader reader)
        => reader.ReadSByte();

    [MethodImpl(Flags.HotPath)]
    public override sealed sbyte DeserializeSpot(ref Reader reader)
        => reader.ReadSByte();

    [MethodImpl(Flags.HotPath)]
    public override sealed sbyte DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadSByte();
}

internal sealed class ByteSerializer<TResolver> : Serializer<TResolver, byte>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 1;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(byte value) => 1;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, byte value)
        => writer.WriteByte(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, byte value)
        => writer.WriteByte(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed byte Deserialize(ref Reader reader)
        => reader.ReadByte();

    [MethodImpl(Flags.HotPath)]
    public override sealed byte DeserializeInUTC(ref Reader reader)
        => reader.ReadByte();

    [MethodImpl(Flags.HotPath)]
    public override sealed byte DeserializeSpot(ref Reader reader)
        => reader.ReadByte();

    [MethodImpl(Flags.HotPath)]
    public override sealed byte DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadByte();
}

internal sealed class ShortSerializer<TResolver> : Serializer<TResolver, short>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 2;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(short value) => 2;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, short value)
        => writer.WriteShort(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, short value)
        => writer.WriteShort(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed short Deserialize(ref Reader reader)
        => reader.ReadShort();

    [MethodImpl(Flags.HotPath)]
    public override sealed short DeserializeInUTC(ref Reader reader)
        => reader.ReadShort();

    [MethodImpl(Flags.HotPath)]
    public override sealed short DeserializeSpot(ref Reader reader)
        => reader.ReadShort();

    [MethodImpl(Flags.HotPath)]
    public override sealed short DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadShort();
}

internal sealed class UShortSerializer<TResolver> : Serializer<TResolver, ushort>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 2;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(ushort value) => 2;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, ushort value)
        => writer.WriteUShort(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, ushort value)
        => writer.WriteUShort(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed ushort Deserialize(ref Reader reader)
        => reader.ReadUShort();

    [MethodImpl(Flags.HotPath)]
    public override sealed ushort DeserializeInUTC(ref Reader reader)
        => reader.ReadUShort();

    [MethodImpl(Flags.HotPath)]
    public override sealed ushort DeserializeSpot(ref Reader reader)
        => reader.ReadUShort();

    [MethodImpl(Flags.HotPath)]
    public override sealed ushort DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadUShort();
}

internal sealed class IntSerializer<TResolver> : Serializer<TResolver, int>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(int value) => 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, int value)
        => writer.WriteInt(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, int value)
        => writer.WriteInt(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed int Deserialize(ref Reader reader)
        => reader.ReadInt();

    [MethodImpl(Flags.HotPath)]
    public override sealed int DeserializeInUTC(ref Reader reader)
        => reader.ReadInt();

    [MethodImpl(Flags.HotPath)]
    public override sealed int DeserializeSpot(ref Reader reader)
        => reader.ReadInt();

    [MethodImpl(Flags.HotPath)]
    public override sealed int DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadInt();
}

internal sealed class UIntSerializer<TResolver> : Serializer<TResolver, uint>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(uint value) => 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, uint value)
        => writer.WriteUInt(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, uint value)
        => writer.WriteUInt(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed uint Deserialize(ref Reader reader)
        => reader.ReadUInt();

    [MethodImpl(Flags.HotPath)]
    public override sealed uint DeserializeInUTC(ref Reader reader)
        => reader.ReadUInt();

    [MethodImpl(Flags.HotPath)]
    public override sealed uint DeserializeSpot(ref Reader reader)
        => reader.ReadUInt();

    [MethodImpl(Flags.HotPath)]
    public override sealed uint DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadUInt();
}

internal sealed class LongSerializer<TResolver> : Serializer<TResolver, long>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 8;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(long value) => 8;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, long value)
        => writer.WriteLong(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, long value)
        => writer.WriteLong(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed long Deserialize(ref Reader reader)
        => reader.ReadLong();

    [MethodImpl(Flags.HotPath)]
    public override sealed long DeserializeInUTC(ref Reader reader)
        => reader.ReadLong();

    [MethodImpl(Flags.HotPath)]
    public override sealed long DeserializeSpot(ref Reader reader)
        => reader.ReadLong();

    [MethodImpl(Flags.HotPath)]
    public override sealed long DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadLong();
}

internal sealed class ULongSerializer<TResolver> : Serializer<TResolver, ulong>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 8;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(ulong value) => 8;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, ulong value)
        => writer.WriteULong(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, ulong value)
        => writer.WriteULong(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed ulong Deserialize(ref Reader reader)
        => reader.ReadULong();

    [MethodImpl(Flags.HotPath)]
    public override sealed ulong DeserializeInUTC(ref Reader reader)
        => reader.ReadULong();

    [MethodImpl(Flags.HotPath)]
    public override sealed ulong DeserializeSpot(ref Reader reader)
        => reader.ReadULong();

    [MethodImpl(Flags.HotPath)]
    public override sealed ulong DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadULong();
}

internal sealed class FloatSerializer<TResolver> : Serializer<TResolver, float>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(float value) => 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, float value)
        => writer.WriteFloat(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, float value)
        => writer.WriteFloat(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed float Deserialize(ref Reader reader)
        => reader.ReadFloat();

    [MethodImpl(Flags.HotPath)]
    public override sealed float DeserializeInUTC(ref Reader reader)
        => reader.ReadFloat();

    [MethodImpl(Flags.HotPath)]
    public override sealed float DeserializeSpot(ref Reader reader)
        => reader.ReadFloat();

    [MethodImpl(Flags.HotPath)]
    public override sealed float DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadFloat();
}

internal sealed class DoubleSerializer<TResolver> : Serializer<TResolver, double>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 8;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(double value) => 8;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, double value)
        => writer.WriteDouble(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, double value)
        => writer.WriteDouble(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed double Deserialize(ref Reader reader)
        => reader.ReadDouble();

    [MethodImpl(Flags.HotPath)]
    public override sealed double DeserializeInUTC(ref Reader reader)
        => reader.ReadDouble();

    [MethodImpl(Flags.HotPath)]
    public override sealed double DeserializeSpot(ref Reader reader)
        => reader.ReadDouble();

    [MethodImpl(Flags.HotPath)]
    public override sealed double DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadDouble();
}

internal sealed class DecimalSerializer<TResolver> : Serializer<TResolver, decimal>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 16;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(decimal value) => 16;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, decimal value)
        => writer.WriteDecimal(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, decimal value)
        => writer.WriteDecimal(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed decimal Deserialize(ref Reader reader)
        => reader.ReadDecimal();

    [MethodImpl(Flags.HotPath)]
    public override sealed decimal DeserializeInUTC(ref Reader reader)
        => reader.ReadDecimal();

    [MethodImpl(Flags.HotPath)]
    public override sealed decimal DeserializeSpot(ref Reader reader)
        => reader.ReadDecimal();

    [MethodImpl(Flags.HotPath)]
    public override sealed decimal DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadDecimal();
}

internal sealed class StringSerializer<TResolver> : Serializer<TResolver, string>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(string value)
        => StringEncoding.GetSize(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, string value)
        => writer.WriteString(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, string value)
        => writer.WriteNotNullString(value);

    [MethodImpl(Flags.HotPath)]
    public override sealed string Deserialize(ref Reader reader)
        => reader.ReadString();

    [MethodImpl(Flags.HotPath)]
    public override sealed string DeserializeInUTC(ref Reader reader)
        => reader.ReadString();

    [MethodImpl(Flags.HotPath)]
    public override sealed string DeserializeSpot(ref Reader reader)
        => reader.ReadNotNullString();

    [MethodImpl(Flags.HotPath)]
    public override sealed string DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadNotNullString();
}

internal sealed class GuidSerializer<TResolver> : Serializer<TResolver, Guid>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 16;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(Guid value) => 16;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, Guid value)
        => writer.WriteGuid(in value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, Guid value)
        => writer.WriteGuid(in value);

    [MethodImpl(Flags.HotPath)]
    public override sealed Guid Deserialize(ref Reader reader)
        => reader.ReadGuid();

    [MethodImpl(Flags.HotPath)]
    public override sealed Guid DeserializeInUTC(ref Reader reader)
        => reader.ReadGuid();

    [MethodImpl(Flags.HotPath)]
    public override sealed Guid DeserializeSpot(ref Reader reader)
        => reader.ReadGuid();

    [MethodImpl(Flags.HotPath)]
    public override sealed Guid DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadGuid();
}

internal sealed class DateTimeSerializer<TResolver> : Serializer<TResolver, DateTime>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 9;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(DateTime value) => 9;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, DateTime value)
        => writer.WriteDateTime(in value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, DateTime value)
        => writer.WriteDateTime(in value);

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTime Deserialize(ref Reader reader)
        => reader.ReadDateTime();

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTime DeserializeInUTC(ref Reader reader)
        => reader.ReadDateTimeInUTC();

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTime DeserializeSpot(ref Reader reader)
        => reader.ReadDateTime();

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTime DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadDateTimeInUTC();
}

internal sealed class DateTimeOffsetSerializer<TResolver> : Serializer<TResolver, DateTimeOffset>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 10;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(DateTimeOffset value) => 10;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, DateTimeOffset value)
    {
        var dateTime = new DateTime(value.Ticks, DateTimeKind.Utc);
        writer.WriteDateTimeWithoutZone(in dateTime);
        writer.WriteShort((short)value.Offset.TotalMinutes);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, DateTimeOffset value)
    {
        var dateTime = new DateTime(value.Ticks, DateTimeKind.Utc);
        writer.WriteDateTimeWithoutZone(in dateTime);
        writer.WriteShort((short)value.Offset.TotalMinutes);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTimeOffset Deserialize(ref Reader reader)
        => new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort()));

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTimeOffset DeserializeInUTC(ref Reader reader)
        => new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort()));

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTimeOffset DeserializeSpot(ref Reader reader)
        => new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort()));

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTimeOffset DeserializeInUTCSpot(ref Reader reader)
        => new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort()));
}

internal sealed class TimeSpanSerializer<TResolver> : Serializer<TResolver, TimeSpan>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 8;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TimeSpan value) => 8;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, TimeSpan value)
        => writer.WriteTimeSpan(in value);

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, TimeSpan value)
        => writer.WriteTimeSpan(in value);

    [MethodImpl(Flags.HotPath)]
    public override sealed TimeSpan Deserialize(ref Reader reader)
        => reader.ReadTimeSpan();

    [MethodImpl(Flags.HotPath)]
    public override sealed TimeSpan DeserializeInUTC(ref Reader reader)
        => reader.ReadTimeSpan();

    [MethodImpl(Flags.HotPath)]
    public override sealed TimeSpan DeserializeSpot(ref Reader reader)
        => reader.ReadTimeSpan();

    [MethodImpl(Flags.HotPath)]
    public override sealed TimeSpan DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadTimeSpan();
}

internal sealed class IntPtrSerializer<TResolver> : Serializer<TResolver, IntPtr>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 8;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(IntPtr value) => 8;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, IntPtr value)
        => writer.WriteLong(value.ToInt64());

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, IntPtr value)
        => writer.WriteLong(value.ToInt64());

    [MethodImpl(Flags.HotPath)]
    public override sealed IntPtr Deserialize(ref Reader reader)
        => new IntPtr(reader.ReadLong());

    [MethodImpl(Flags.HotPath)]
    public override sealed IntPtr DeserializeInUTC(ref Reader reader)
        => new IntPtr(reader.ReadLong());

    [MethodImpl(Flags.HotPath)]
    public override sealed IntPtr DeserializeSpot(ref Reader reader)
        => new IntPtr(reader.ReadLong());

    [MethodImpl(Flags.HotPath)]
    public override sealed IntPtr DeserializeInUTCSpot(ref Reader reader)
        => new IntPtr(reader.ReadLong());
}

internal sealed class UIntPtrSerializer<TResolver> : Serializer<TResolver, UIntPtr>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => 8;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(UIntPtr value) => 8;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, UIntPtr value)
        => writer.WriteULong(value.ToUInt64());

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, UIntPtr value)
        => writer.WriteULong(value.ToUInt64());

    [MethodImpl(Flags.HotPath)]
    public override sealed UIntPtr Deserialize(ref Reader reader)
        => new UIntPtr(reader.ReadULong());

    [MethodImpl(Flags.HotPath)]
    public override sealed UIntPtr DeserializeInUTC(ref Reader reader)
        => new UIntPtr(reader.ReadULong());

    [MethodImpl(Flags.HotPath)]
    public override sealed UIntPtr DeserializeSpot(ref Reader reader)
        => new UIntPtr(reader.ReadULong());

    [MethodImpl(Flags.HotPath)]
    public override sealed UIntPtr DeserializeInUTCSpot(ref Reader reader)
        => new UIntPtr(reader.ReadULong());
}

internal sealed class ArraySegmentByteSerializer<TResolver> : Serializer<TResolver, ArraySegment<byte>>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(ArraySegment<byte> value) => value.Count + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, ArraySegment<byte> value)
    {
        writer.WriteInt(value.Count);

        if (value.Count > 0)
            writer.WriteByteArray(value.Array, value.Offset, value.Count);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, ArraySegment<byte> value)
    {
        writer.WriteInt(value.Count);
        writer.WriteByteArray(value.Array, value.Offset, value.Count);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ArraySegment<byte> Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? new ArraySegment<byte>(reader.ReadByteArray(length)) : default;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ArraySegment<byte> DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? new ArraySegment<byte>(reader.ReadByteArray(length)) : default;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ArraySegment<byte> DeserializeSpot(ref Reader reader)
        => new ArraySegment<byte>(reader.ReadByteArray(reader.ReadInt()));

    [MethodImpl(Flags.HotPath)]
    public override sealed ArraySegment<byte> DeserializeInUTCSpot(ref Reader reader)
        => new ArraySegment<byte>(reader.ReadByteArray(reader.ReadInt()));
}

internal sealed class MemorySerializer<TResolver> : Serializer<TResolver, Memory<byte>>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(Memory<byte> value) => value.Length + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, Memory<byte> value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteSpan(value.Span);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, Memory<byte> value)
    {
        writer.WriteInt(value.Length);
        writer.WriteSpan(value.Span);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed Memory<byte> Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadMemory(length) : default;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed Memory<byte> DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadMemory(length) : default;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed Memory<byte> DeserializeSpot(ref Reader reader)
        => reader.ReadMemory(reader.ReadInt());

    [MethodImpl(Flags.HotPath)]
    public override sealed Memory<byte> DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadMemory(reader.ReadInt());
}

internal sealed class ReadOnlyMemorySerializer<TResolver> : Serializer<TResolver, ReadOnlyMemory<byte>>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(ReadOnlyMemory<byte> value) => value.Length + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, ReadOnlyMemory<byte> value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteSpan(value.Span);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, ReadOnlyMemory<byte> value)
    {
        writer.WriteInt(value.Length);
        writer.WriteSpan(value.Span);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ReadOnlyMemory<byte> Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.GetMemory(length) : default;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ReadOnlyMemory<byte> DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.GetMemory(length) : default;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ReadOnlyMemory<byte> DeserializeSpot(ref Reader reader)
        => reader.GetMemory(reader.ReadInt());

    [MethodImpl(Flags.HotPath)]
    public override sealed ReadOnlyMemory<byte> DeserializeInUTCSpot(ref Reader reader)
        => reader.GetMemory(reader.ReadInt());
}

internal sealed class ReadOnlySequenceByteSerializer<TResolver> : Serializer<TResolver, ReadOnlySequence<byte>>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(ReadOnlySequence<byte> value) => (int)value.Length + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, ReadOnlySequence<byte> value)
    {
        int length = (int)value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteReadOnlySequence(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, ReadOnlySequence<byte> value)
    {
        writer.WriteInt((int)value.Length);
        writer.WriteReadOnlySequence(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ReadOnlySequence<byte> Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadReadOnlySequence(length) : default;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ReadOnlySequence<byte> DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadReadOnlySequence(length) : default;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ReadOnlySequence<byte> DeserializeSpot(ref Reader reader)
        => reader.ReadReadOnlySequence(reader.ReadInt());

    [MethodImpl(Flags.HotPath)]
    public override sealed ReadOnlySequence<byte> DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadReadOnlySequence(reader.ReadInt());
}

internal sealed class StreamSerializer<TResolver> : Serializer<TResolver, Stream>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(Stream value) => (int)value.Length + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, Stream value)
    {
        int length = (int)value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteStream(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, Stream value)
    {
        writer.WriteInt((int)value.Length);
        writer.WriteStream(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed Stream Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadStream(length) : Stream.Null;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed Stream DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadStream(length) : Stream.Null;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed Stream DeserializeSpot(ref Reader reader)
        => reader.ReadStream(reader.ReadInt());

    [MethodImpl(Flags.HotPath)]
    public override sealed Stream DeserializeInUTCSpot(ref Reader reader)
        => reader.ReadStream(reader.ReadInt());
}