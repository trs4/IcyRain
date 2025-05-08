using System;
using System.Runtime.Serialization;

namespace IcyRain.Tables;

[DataContract]
public class BooleanDataColumn : StructDataColumn<bool>
{
    public BooleanDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Boolean;

    public sealed override bool GetBool(in int row) => Get(row);

    public sealed override bool? GetNullableBool(in int row) => Get(row);

    public sealed override char GetChar(in int row) => throw new NotSupportedException();

    public sealed override char? GetNullableChar(in int row) => throw new NotSupportedException();

    public sealed override sbyte GetSByte(in int row) => Get(row) ? (sbyte)1 : (sbyte)0;

    public sealed override sbyte? GetNullableSByte(in int row) => Get(row) ? (sbyte)1 : (sbyte)0;

    public sealed override byte GetByte(in int row) => Get(row) ? (byte)1 : (byte)0;

    public sealed override byte? GetNullableByte(in int row) => Get(row) ? (byte)1 : (byte)0;

    public sealed override short GetShort(in int row) => Get(row) ? (short)1 : (short)0;

    public sealed override short? GetNullableShort(in int row) => Get(row) ? (short)1 : (short)0;

    public sealed override ushort GetUShort(in int row) => Get(row) ? (ushort)1 : (ushort)0;

    public sealed override ushort? GetNullableUShort(in int row) => Get(row) ? (ushort)1 : (ushort)0;

    public sealed override int GetInt(in int row) => Get(row) ? 1 : 0;

    public sealed override int? GetNullableInt(in int row) => Get(row) ? 1 : 0;

    public sealed override uint GetUInt(in int row) => Get(row) ? (uint)1 : 0;

    public sealed override uint? GetNullableUInt(in int row) => Get(row) ? (uint)1 : 0;

    public sealed override long GetLong(in int row) => Get(row) ? 1 : 0;

    public sealed override long? GetNullableLong(in int row) => Get(row) ? 1 : 0;

    public sealed override ulong GetULong(in int row) => Get(row) ? (ulong)1 : 0;

    public sealed override ulong? GetNullableULong(in int row) => Get(row) ? (ulong)1 : 0;

    public sealed override float GetFloat(in int row) => Get(row) ? 1 : 0;

    public sealed override float? GetNullableFloat(in int row) => Get(row) ? 1 : 0;

    public sealed override double GetDouble(in int row) => Get(row) ? 1 : 0;

    public sealed override double? GetNullableDouble(in int row) => Get(row) ? 1 : 0;

    public sealed override decimal GetDecimal(in int row) => Get(row) ? 1 : 0;

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row) ? 1 : 0;

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableBooleanDataColumn : NullableDataColumn<bool>
{
    public NullableBooleanDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Boolean;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault();

    public sealed override bool? GetNullableBool(in int row) => Get(row);

    public sealed override char GetChar(in int row) => throw new NotSupportedException();

    public sealed override char? GetNullableChar(in int row) => throw new NotSupportedException();

    public sealed override sbyte GetSByte(in int row) => Get(row) == true ? (sbyte)1 : (sbyte)0;

    public sealed override sbyte? GetNullableSByte(in int row) => Get(row) == true ? (sbyte)1 : (sbyte)0;

    public sealed override byte GetByte(in int row) => Get(row) == true ? (byte)1 : (byte)0;

    public sealed override byte? GetNullableByte(in int row) => Get(row) == true ? (byte)1 : (byte)0;

    public sealed override short GetShort(in int row) => Get(row) == true ? (short)1 : (short)0;

    public sealed override short? GetNullableShort(in int row) => Get(row) == true ? (short)1 : (short)0;

    public sealed override ushort GetUShort(in int row) => Get(row) == true ? (ushort)1 : (ushort)0;

    public sealed override ushort? GetNullableUShort(in int row) => Get(row) == true ? (ushort)1 : (ushort)0;

    public sealed override int GetInt(in int row) => Get(row) == true ? 1 : 0;

    public sealed override int? GetNullableInt(in int row) => Get(row) == true ? 1 : 0;

    public sealed override uint GetUInt(in int row) => Get(row) == true ? (uint)1 : 0;

    public sealed override uint? GetNullableUInt(in int row) => Get(row) == true ? (uint)1 : 0;

    public sealed override long GetLong(in int row) => Get(row) == true ? 1 : 0;

    public sealed override long? GetNullableLong(in int row) => Get(row) == true ? 1 : 0;

    public sealed override ulong GetULong(in int row) => Get(row) == true ? (ulong)1 : 0;

    public sealed override ulong? GetNullableULong(in int row) => Get(row) == true ? (ulong)1 : 0;

    public sealed override float GetFloat(in int row) => Get(row) == true ? 1 : 0;

    public sealed override float? GetNullableFloat(in int row) => Get(row) == true ? 1 : 0;

    public sealed override double GetDouble(in int row) => Get(row) == true ? 1 : 0;

    public sealed override double? GetNullableDouble(in int row) => Get(row) == true ? 1 : 0;

    public sealed override decimal GetDecimal(in int row) => Get(row) == true ? 1 : 0;

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row) == true ? 1 : 0;

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class CharDataColumn : StructDataColumn<char>
{
    public CharDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Char;

    public sealed override bool GetBool(in int row) => Get(row) != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row) != 0;

    public sealed override char GetChar(in int row) => Get(row);

    public sealed override char? GetNullableChar(in int row) => Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row);

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row);

    public sealed override byte? GetNullableByte(in int row) => (byte)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row);

    public sealed override short? GetNullableShort(in int row) => (short)Get(row);

    public sealed override ushort GetUShort(in int row) => Get(row);

    public sealed override ushort? GetNullableUShort(in int row) => Get(row);

    public sealed override int GetInt(in int row) => Get(row);

    public sealed override int? GetNullableInt(in int row) => Get(row);

    public sealed override uint GetUInt(in int row) => Get(row);

    public sealed override uint? GetNullableUInt(in int row) => Get(row);

    public sealed override long GetLong(in int row) => Get(row);

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => Get(row);

    public sealed override ulong? GetNullableULong(in int row) => Get(row);

    public sealed override float GetFloat(in int row) => Get(row);

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row);

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row);

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableCharDataColumn : NullableDataColumn<char>
{
    public NullableCharDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Char;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override char GetChar(in int row) => Get(row).GetValueOrDefault();

    public sealed override char? GetNullableChar(in int row) => Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row).GetValueOrDefault();

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte?)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row).GetValueOrDefault();

    public sealed override byte? GetNullableByte(in int row) => (byte?)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row).GetValueOrDefault();

    public sealed override short? GetNullableShort(in int row) => (short?)Get(row);

    public sealed override ushort GetUShort(in int row) => Get(row).GetValueOrDefault();

    public sealed override ushort? GetNullableUShort(in int row) => Get(row);

    public sealed override int GetInt(in int row) => Get(row).GetValueOrDefault();

    public sealed override int? GetNullableInt(in int row) => Get(row);

    public sealed override uint GetUInt(in int row) => Get(row).GetValueOrDefault();

    public sealed override uint? GetNullableUInt(in int row) => Get(row);

    public sealed override long GetLong(in int row) => Get(row).GetValueOrDefault();

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => Get(row).GetValueOrDefault();

    public sealed override ulong? GetNullableULong(in int row) => Get(row);

    public sealed override float GetFloat(in int row) => Get(row).GetValueOrDefault();

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row).GetValueOrDefault();

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row).GetValueOrDefault();

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class SByteDataColumn : StructDataColumn<sbyte>
{
    public SByteDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.SByte;

    public sealed override bool GetBool(in int row) => Get(row) != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row) != 0;

    public sealed override char GetChar(in int row) => (char)Get(row);

    public sealed override char? GetNullableChar(in int row) => (char)Get(row);

    public sealed override sbyte GetSByte(in int row) => Get(row);

    public sealed override sbyte? GetNullableSByte(in int row) => Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row);

    public sealed override byte? GetNullableByte(in int row) => (byte)Get(row);

    public sealed override short GetShort(in int row) => Get(row);

    public sealed override short? GetNullableShort(in int row) => Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row);

    public sealed override ushort? GetNullableUShort(in int row) => (ushort)Get(row);

    public sealed override int GetInt(in int row) => Get(row);

    public sealed override int? GetNullableInt(in int row) => Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row);

    public sealed override uint? GetNullableUInt(in int row) => (uint)Get(row);

    public sealed override long GetLong(in int row) => Get(row);

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row);

    public sealed override ulong? GetNullableULong(in int row) => (ulong)Get(row);

    public sealed override float GetFloat(in int row) => Get(row);

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row);

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row);

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableSByteDataColumn : NullableDataColumn<sbyte>
{
    public NullableSByteDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.SByte;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override char GetChar(in int row) => (char)Get(row).GetValueOrDefault();

    public sealed override char? GetNullableChar(in int row) => (char?)Get(row);

    public sealed override sbyte GetSByte(in int row) => Get(row).GetValueOrDefault();

    public sealed override sbyte? GetNullableSByte(in int row) => Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row).GetValueOrDefault();

    public sealed override byte? GetNullableByte(in int row) => (byte?)Get(row);

    public sealed override short GetShort(in int row) => Get(row).GetValueOrDefault();

    public sealed override short? GetNullableShort(in int row) => Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row).GetValueOrDefault();

    public sealed override ushort? GetNullableUShort(in int row) => (ushort?)Get(row);

    public sealed override int GetInt(in int row) => Get(row).GetValueOrDefault();

    public sealed override int? GetNullableInt(in int row) => Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row).GetValueOrDefault();

    public sealed override uint? GetNullableUInt(in int row) => (uint?)Get(row);

    public sealed override long GetLong(in int row) => Get(row).GetValueOrDefault();

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row).GetValueOrDefault();

    public sealed override ulong? GetNullableULong(in int row) => (ulong?)Get(row);

    public sealed override float GetFloat(in int row) => Get(row).GetValueOrDefault();

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row).GetValueOrDefault();

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row).GetValueOrDefault();

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class ByteDataColumn : StructDataColumn<byte>
{
    public ByteDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Byte;

    public sealed override bool GetBool(in int row) => Get(row) != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row) != 0;

    public sealed override char GetChar(in int row) => (char)Get(row);

    public sealed override char? GetNullableChar(in int row) => (char)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row);

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte)Get(row);

    public sealed override byte GetByte(in int row) => Get(row);

    public sealed override byte? GetNullableByte(in int row) => Get(row);

    public sealed override short GetShort(in int row) => Get(row);

    public sealed override short? GetNullableShort(in int row) => Get(row);

    public sealed override ushort GetUShort(in int row) => Get(row);

    public sealed override ushort? GetNullableUShort(in int row) => Get(row);

    public sealed override int GetInt(in int row) => Get(row);

    public sealed override int? GetNullableInt(in int row) => Get(row);

    public sealed override uint GetUInt(in int row) => Get(row);

    public sealed override uint? GetNullableUInt(in int row) => Get(row);

    public sealed override long GetLong(in int row) => Get(row);

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => Get(row);

    public sealed override ulong? GetNullableULong(in int row) => Get(row);

    public sealed override float GetFloat(in int row) => Get(row);

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row);

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row);

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableByteDataColumn : NullableDataColumn<byte>
{
    public NullableByteDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Byte;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override char GetChar(in int row) => (char)Get(row).GetValueOrDefault();

    public sealed override char? GetNullableChar(in int row) => (char?)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row).GetValueOrDefault();

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte?)Get(row);

    public sealed override byte GetByte(in int row) => Get(row).GetValueOrDefault();

    public sealed override byte? GetNullableByte(in int row) => Get(row);

    public sealed override short GetShort(in int row) => Get(row).GetValueOrDefault();

    public sealed override short? GetNullableShort(in int row) => Get(row);

    public sealed override ushort GetUShort(in int row) => Get(row).GetValueOrDefault();

    public sealed override ushort? GetNullableUShort(in int row) => Get(row);

    public sealed override int GetInt(in int row) => Get(row).GetValueOrDefault();

    public sealed override int? GetNullableInt(in int row) => Get(row);

    public sealed override uint GetUInt(in int row) => Get(row).GetValueOrDefault();

    public sealed override uint? GetNullableUInt(in int row) => Get(row);

    public sealed override long GetLong(in int row) => Get(row).GetValueOrDefault();

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => Get(row).GetValueOrDefault();

    public sealed override ulong? GetNullableULong(in int row) => Get(row);

    public sealed override float GetFloat(in int row) => Get(row).GetValueOrDefault();

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row).GetValueOrDefault();

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row).GetValueOrDefault();

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class Int16DataColumn : StructDataColumn<short>
{
    public Int16DataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Int16;

    public sealed override bool GetBool(in int row) => Get(row) != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row) != 0;

    public sealed override char GetChar(in int row) => (char)Get(row);

    public sealed override char? GetNullableChar(in int row) => (char)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row);

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row);

    public sealed override byte? GetNullableByte(in int row) => (byte)Get(row);

    public sealed override short GetShort(in int row) => Get(row);

    public sealed override short? GetNullableShort(in int row) => Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row);

    public sealed override ushort? GetNullableUShort(in int row) => (ushort)Get(row);

    public sealed override int GetInt(in int row) => Get(row);

    public sealed override int? GetNullableInt(in int row) => Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row);

    public sealed override uint? GetNullableUInt(in int row) => (uint)Get(row);

    public sealed override long GetLong(in int row) => Get(row);

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row);

    public sealed override ulong? GetNullableULong(in int row) => (ulong)Get(row);

    public sealed override float GetFloat(in int row) => Get(row);

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row);

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row);

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableInt16DataColumn : NullableDataColumn<short>
{
    public NullableInt16DataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Int16;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override char GetChar(in int row) => (char)Get(row).GetValueOrDefault();

    public sealed override char? GetNullableChar(in int row) => (char?)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row).GetValueOrDefault();

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte?)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row).GetValueOrDefault();

    public sealed override byte? GetNullableByte(in int row) => (byte?)Get(row);

    public sealed override short GetShort(in int row) => Get(row).GetValueOrDefault();

    public sealed override short? GetNullableShort(in int row) => Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row).GetValueOrDefault();

    public sealed override ushort? GetNullableUShort(in int row) => (ushort?)Get(row);

    public sealed override int GetInt(in int row) => Get(row).GetValueOrDefault();

    public sealed override int? GetNullableInt(in int row) => Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row).GetValueOrDefault();

    public sealed override uint? GetNullableUInt(in int row) => (uint?)Get(row);

    public sealed override long GetLong(in int row) => Get(row).GetValueOrDefault();

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row).GetValueOrDefault();

    public sealed override ulong? GetNullableULong(in int row) => (ulong?)Get(row);

    public sealed override float GetFloat(in int row) => Get(row).GetValueOrDefault();

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row).GetValueOrDefault();

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row).GetValueOrDefault();

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class UInt16DataColumn : StructDataColumn<ushort>
{
    public UInt16DataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.UInt16;

    public sealed override bool GetBool(in int row) => Get(row) != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row) != 0;

    public sealed override char GetChar(in int row) => (char)Get(row);

    public sealed override char? GetNullableChar(in int row) => (char)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row);

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row);

    public sealed override byte? GetNullableByte(in int row) => (byte)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row);

    public sealed override short? GetNullableShort(in int row) => (short)Get(row);

    public sealed override ushort GetUShort(in int row) => Get(row);

    public sealed override ushort? GetNullableUShort(in int row) => Get(row);

    public sealed override int GetInt(in int row) => Get(row);

    public sealed override int? GetNullableInt(in int row) => Get(row);

    public sealed override uint GetUInt(in int row) => Get(row);

    public sealed override uint? GetNullableUInt(in int row) => Get(row);

    public sealed override long GetLong(in int row) => Get(row);

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => Get(row);

    public sealed override ulong? GetNullableULong(in int row) => Get(row);

    public sealed override float GetFloat(in int row) => Get(row);

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row);

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row);

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableUInt16DataColumn : NullableDataColumn<ushort>
{
    public NullableUInt16DataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.UInt16;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override char GetChar(in int row) => (char)Get(row).GetValueOrDefault();

    public sealed override char? GetNullableChar(in int row) => (char?)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row).GetValueOrDefault();

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte?)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row).GetValueOrDefault();

    public sealed override byte? GetNullableByte(in int row) => (byte?)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row).GetValueOrDefault();

    public sealed override short? GetNullableShort(in int row) => (short?)Get(row);

    public sealed override ushort GetUShort(in int row) => Get(row).GetValueOrDefault();

    public sealed override ushort? GetNullableUShort(in int row) => Get(row);

    public sealed override int GetInt(in int row) => Get(row).GetValueOrDefault();

    public sealed override int? GetNullableInt(in int row) => Get(row);

    public sealed override uint GetUInt(in int row) => Get(row).GetValueOrDefault();

    public sealed override uint? GetNullableUInt(in int row) => Get(row);

    public sealed override long GetLong(in int row) => Get(row).GetValueOrDefault();

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => Get(row).GetValueOrDefault();

    public sealed override ulong? GetNullableULong(in int row) => Get(row);

    public sealed override float GetFloat(in int row) => Get(row).GetValueOrDefault();

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row).GetValueOrDefault();

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row).GetValueOrDefault();

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class Int32DataColumn : StructDataColumn<int>
{
    public Int32DataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Int32;

    public sealed override bool GetBool(in int row) => Get(row) != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row) != 0;

    public sealed override char GetChar(in int row) => (char)Get(row);

    public sealed override char? GetNullableChar(in int row) => (char)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row);

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row);

    public sealed override byte? GetNullableByte(in int row) => (byte)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row);

    public sealed override short? GetNullableShort(in int row) => (short)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row);

    public sealed override ushort? GetNullableUShort(in int row) => (ushort)Get(row);

    public sealed override int GetInt(in int row) => Get(row);

    public sealed override int? GetNullableInt(in int row) => Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row);

    public sealed override uint? GetNullableUInt(in int row) => (uint)Get(row);

    public sealed override long GetLong(in int row) => Get(row);

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row);

    public sealed override ulong? GetNullableULong(in int row) => (ulong)Get(row);

    public sealed override float GetFloat(in int row) => Get(row);

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row);

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row);

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableInt32DataColumn : NullableDataColumn<int>
{
    public NullableInt32DataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Int32;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override char GetChar(in int row) => (char)Get(row).GetValueOrDefault();

    public sealed override char? GetNullableChar(in int row) => (char?)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row).GetValueOrDefault();

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte?)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row).GetValueOrDefault();

    public sealed override byte? GetNullableByte(in int row) => (byte?)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row).GetValueOrDefault();

    public sealed override short? GetNullableShort(in int row) => (short?)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row).GetValueOrDefault();

    public sealed override ushort? GetNullableUShort(in int row) => (ushort?)Get(row);

    public sealed override int GetInt(in int row) => Get(row).GetValueOrDefault();

    public sealed override int? GetNullableInt(in int row) => Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row).GetValueOrDefault();

    public sealed override uint? GetNullableUInt(in int row) => (uint?)Get(row);

    public sealed override long GetLong(in int row) => Get(row).GetValueOrDefault();

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row).GetValueOrDefault();

    public sealed override ulong? GetNullableULong(in int row) => (ulong?)Get(row);

    public sealed override float GetFloat(in int row) => Get(row).GetValueOrDefault();

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row).GetValueOrDefault();

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row).GetValueOrDefault();

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class UInt32DataColumn : StructDataColumn<uint>
{
    public UInt32DataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.UInt32;

    public sealed override bool GetBool(in int row) => Get(row) != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row) != 0;

    public sealed override char GetChar(in int row) => (char)Get(row);

    public sealed override char? GetNullableChar(in int row) => (char)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row);

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row);

    public sealed override byte? GetNullableByte(in int row) => (byte)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row);

    public sealed override short? GetNullableShort(in int row) => (short)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row);

    public sealed override ushort? GetNullableUShort(in int row) => (ushort)Get(row);

    public sealed override int GetInt(in int row) => (int)Get(row);

    public sealed override int? GetNullableInt(in int row) => (int)Get(row);

    public sealed override uint GetUInt(in int row) => Get(row);

    public sealed override uint? GetNullableUInt(in int row) => Get(row);

    public sealed override long GetLong(in int row) => Get(row);

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => Get(row);

    public sealed override ulong? GetNullableULong(in int row) => Get(row);

    public sealed override float GetFloat(in int row) => Get(row);

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row);

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row);

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableUInt32DataColumn : NullableDataColumn<uint>
{
    public NullableUInt32DataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.UInt32;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override char GetChar(in int row) => (char)Get(row).GetValueOrDefault();

    public sealed override char? GetNullableChar(in int row) => (char?)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row).GetValueOrDefault();

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte?)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row).GetValueOrDefault();

    public sealed override byte? GetNullableByte(in int row) => (byte?)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row).GetValueOrDefault();

    public sealed override short? GetNullableShort(in int row) => (short?)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row).GetValueOrDefault();

    public sealed override ushort? GetNullableUShort(in int row) => (ushort?)Get(row);

    public sealed override int GetInt(in int row) => (int)Get(row).GetValueOrDefault();

    public sealed override int? GetNullableInt(in int row) => (int?)Get(row);

    public sealed override uint GetUInt(in int row) => Get(row).GetValueOrDefault();

    public sealed override uint? GetNullableUInt(in int row) => Get(row);

    public sealed override long GetLong(in int row) => Get(row).GetValueOrDefault();

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => Get(row).GetValueOrDefault();

    public sealed override ulong? GetNullableULong(in int row) => Get(row);

    public sealed override float GetFloat(in int row) => Get(row).GetValueOrDefault();

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row).GetValueOrDefault();

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row).GetValueOrDefault();

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class Int64DataColumn : StructDataColumn<long>
{
    public Int64DataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Int64;

    public sealed override bool GetBool(in int row) => Get(row) != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row) != 0;

    public sealed override char GetChar(in int row) => (char)Get(row);

    public sealed override char? GetNullableChar(in int row) => (char?)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row);

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte?)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row);

    public sealed override byte? GetNullableByte(in int row) => (byte)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row);

    public sealed override short? GetNullableShort(in int row) => (short)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row);

    public sealed override ushort? GetNullableUShort(in int row) => (ushort)Get(row);

    public sealed override int GetInt(in int row) => (int)Get(row);

    public sealed override int? GetNullableInt(in int row) => (int)Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row);

    public sealed override uint? GetNullableUInt(in int row) => (uint)Get(row);

    public sealed override long GetLong(in int row) => Get(row);

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row);

    public sealed override ulong? GetNullableULong(in int row) => (ulong)Get(row);

    public sealed override float GetFloat(in int row) => Get(row);

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row);

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row);

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableInt64DataColumn : NullableDataColumn<long>
{
    public NullableInt64DataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Int64;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override char GetChar(in int row) => (char)Get(row).GetValueOrDefault();

    public sealed override char? GetNullableChar(in int row) => (char?)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row).GetValueOrDefault();

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte?)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row).GetValueOrDefault();

    public sealed override byte? GetNullableByte(in int row) => (byte?)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row).GetValueOrDefault();

    public sealed override short? GetNullableShort(in int row) => (short?)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row).GetValueOrDefault();

    public sealed override ushort? GetNullableUShort(in int row) => (ushort?)Get(row);

    public sealed override int GetInt(in int row) => (int)Get(row).GetValueOrDefault();

    public sealed override int? GetNullableInt(in int row) => (int)Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row).GetValueOrDefault();

    public sealed override uint? GetNullableUInt(in int row) => (uint?)Get(row);

    public sealed override long GetLong(in int row) => Get(row).GetValueOrDefault();

    public sealed override long? GetNullableLong(in int row) => Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row).GetValueOrDefault();

    public sealed override ulong? GetNullableULong(in int row) => (ulong?)Get(row);

    public sealed override float GetFloat(in int row) => Get(row).GetValueOrDefault();

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row).GetValueOrDefault();

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row).GetValueOrDefault();

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class UInt64DataColumn : StructDataColumn<ulong>
{
    public UInt64DataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.UInt64;

    public sealed override bool GetBool(in int row) => Get(row) != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row) != 0;

    public sealed override char GetChar(in int row) => (char)Get(row);

    public sealed override char? GetNullableChar(in int row) => (char)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row);

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row);

    public sealed override byte? GetNullableByte(in int row) => (byte)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row);

    public sealed override short? GetNullableShort(in int row) => (short)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row);

    public sealed override ushort? GetNullableUShort(in int row) => (ushort)Get(row);

    public sealed override int GetInt(in int row) => (int)Get(row);

    public sealed override int? GetNullableInt(in int row) => (int)Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row);

    public sealed override uint? GetNullableUInt(in int row) => (uint)Get(row);

    public sealed override long GetLong(in int row) => (long)Get(row);

    public sealed override long? GetNullableLong(in int row) => (long)Get(row);

    public sealed override ulong GetULong(in int row) => Get(row);

    public sealed override ulong? GetNullableULong(in int row) => Get(row);

    public sealed override float GetFloat(in int row) => Get(row);

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row);

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row);

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableUInt64DataColumn : NullableDataColumn<ulong>
{
    public NullableUInt64DataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.UInt64;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override char GetChar(in int row) => (char)Get(row).GetValueOrDefault();

    public sealed override char? GetNullableChar(in int row) => (char?)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row).GetValueOrDefault();

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte?)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row).GetValueOrDefault();

    public sealed override byte? GetNullableByte(in int row) => (byte?)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row).GetValueOrDefault();

    public sealed override short? GetNullableShort(in int row) => (short?)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row).GetValueOrDefault();

    public sealed override ushort? GetNullableUShort(in int row) => (ushort?)Get(row);

    public sealed override int GetInt(in int row) => (int)Get(row).GetValueOrDefault();

    public sealed override int? GetNullableInt(in int row) => (int?)Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row).GetValueOrDefault();

    public sealed override uint? GetNullableUInt(in int row) => (uint?)Get(row);

    public sealed override long GetLong(in int row) => (long)Get(row).GetValueOrDefault();

    public sealed override long? GetNullableLong(in int row) => (long?)Get(row);

    public sealed override ulong GetULong(in int row) => Get(row).GetValueOrDefault();

    public sealed override ulong? GetNullableULong(in int row) => Get(row);

    public sealed override float GetFloat(in int row) => Get(row).GetValueOrDefault();

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row).GetValueOrDefault();

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row).GetValueOrDefault();

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class SingleDataColumn : StructDataColumn<float>
{
    public SingleDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Single;

    public sealed override bool GetBool(in int row) => Get(row) != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row) != 0;

    public sealed override char GetChar(in int row) => (char)Get(row);

    public sealed override char? GetNullableChar(in int row) => (char)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row);

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row);

    public sealed override byte? GetNullableByte(in int row) => (byte)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row);

    public sealed override short? GetNullableShort(in int row) => (short)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row);

    public sealed override ushort? GetNullableUShort(in int row) => (ushort)Get(row);

    public sealed override int GetInt(in int row) => (int)Get(row);

    public sealed override int? GetNullableInt(in int row) => (int)Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row);

    public sealed override uint? GetNullableUInt(in int row) => (uint)Get(row);

    public sealed override long GetLong(in int row) => (long)Get(row);

    public sealed override long? GetNullableLong(in int row) => (long)Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row);

    public sealed override ulong? GetNullableULong(in int row) => (ulong)Get(row);

    public sealed override float GetFloat(in int row) => Get(row);

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row);

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => (decimal)Get(row);

    public sealed override decimal? GetNullableDecimal(in int row) => (decimal)Get(row);

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableSingleDataColumn : NullableDataColumn<float>
{
    public NullableSingleDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Single;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override char GetChar(in int row) => (char)Get(row).GetValueOrDefault();

    public sealed override char? GetNullableChar(in int row) => (char?)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row).GetValueOrDefault();

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte?)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row).GetValueOrDefault();

    public sealed override byte? GetNullableByte(in int row) => (byte?)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row).GetValueOrDefault();

    public sealed override short? GetNullableShort(in int row) => (short?)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row).GetValueOrDefault();

    public sealed override ushort? GetNullableUShort(in int row) => (ushort?)Get(row);

    public sealed override int GetInt(in int row) => (int)Get(row).GetValueOrDefault();

    public sealed override int? GetNullableInt(in int row) => (int?)Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row).GetValueOrDefault();

    public sealed override uint? GetNullableUInt(in int row) => (uint?)Get(row);

    public sealed override long GetLong(in int row) => (long)Get(row).GetValueOrDefault();

    public sealed override long? GetNullableLong(in int row) => (long?)Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row).GetValueOrDefault();

    public sealed override ulong? GetNullableULong(in int row) => (ulong?)Get(row);

    public sealed override float GetFloat(in int row) => Get(row).GetValueOrDefault();

    public sealed override float? GetNullableFloat(in int row) => Get(row);

    public sealed override double GetDouble(in int row) => Get(row).GetValueOrDefault();

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => (decimal)Get(row).GetValueOrDefault();

    public sealed override decimal? GetNullableDecimal(in int row) => (decimal?)Get(row);

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class DoubleDataColumn : StructDataColumn<double>
{
    public DoubleDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Double;

    public sealed override bool GetBool(in int row) => Get(row) != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row) != 0;

    public sealed override char GetChar(in int row) => (char)Get(row);

    public sealed override char? GetNullableChar(in int row) => (char)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row);

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row);

    public sealed override byte? GetNullableByte(in int row) => (byte)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row);

    public sealed override short? GetNullableShort(in int row) => (short)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row);

    public sealed override ushort? GetNullableUShort(in int row) => (ushort)Get(row);

    public sealed override int GetInt(in int row) => (int)Get(row);

    public sealed override int? GetNullableInt(in int row) => (int)Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row);

    public sealed override uint? GetNullableUInt(in int row) => (uint)Get(row);

    public sealed override long GetLong(in int row) => (long)Get(row);

    public sealed override long? GetNullableLong(in int row) => (long)Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row);

    public sealed override ulong? GetNullableULong(in int row) => (ulong)Get(row);

    public sealed override float GetFloat(in int row) => (float)Get(row);

    public sealed override float? GetNullableFloat(in int row) => (float)Get(row);

    public sealed override double GetDouble(in int row) => Get(row);

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => (decimal)Get(row);

    public sealed override decimal? GetNullableDecimal(in int row) => (decimal)Get(row);

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableDoubleDataColumn : NullableDataColumn<double>
{
    public NullableDoubleDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Double;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override char GetChar(in int row) => (char)Get(row).GetValueOrDefault();

    public sealed override char? GetNullableChar(in int row) => (char?)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row).GetValueOrDefault();

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte?)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row).GetValueOrDefault();

    public sealed override byte? GetNullableByte(in int row) => (byte?)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row).GetValueOrDefault();

    public sealed override short? GetNullableShort(in int row) => (short?)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row).GetValueOrDefault();

    public sealed override ushort? GetNullableUShort(in int row) => (ushort?)Get(row);

    public sealed override int GetInt(in int row) => (int)Get(row).GetValueOrDefault();

    public sealed override int? GetNullableInt(in int row) => (int?)Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row).GetValueOrDefault();

    public sealed override uint? GetNullableUInt(in int row) => (uint?)Get(row);

    public sealed override long GetLong(in int row) => (long)Get(row).GetValueOrDefault();

    public sealed override long? GetNullableLong(in int row) => (long?)Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row).GetValueOrDefault();

    public sealed override ulong? GetNullableULong(in int row) => (ulong?)Get(row);

    public sealed override float GetFloat(in int row) => (float)Get(row).GetValueOrDefault();

    public sealed override float? GetNullableFloat(in int row) => (float?)Get(row);

    public sealed override double GetDouble(in int row) => Get(row).GetValueOrDefault();

    public sealed override double? GetNullableDouble(in int row) => Get(row);

    public sealed override decimal GetDecimal(in int row) => (decimal)Get(row).GetValueOrDefault();

    public sealed override decimal? GetNullableDecimal(in int row) => (decimal?)Get(row);

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class DecimalDataColumn : StructDataColumn<decimal>
{
    public DecimalDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Decimal;

    public sealed override bool GetBool(in int row) => Get(row) != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row) != 0;

    public sealed override char GetChar(in int row) => (char)Get(row);

    public sealed override char? GetNullableChar(in int row) => (char)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row);

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row);

    public sealed override byte? GetNullableByte(in int row) => (byte)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row);

    public sealed override short? GetNullableShort(in int row) => (short)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row);

    public sealed override ushort? GetNullableUShort(in int row) => (ushort)Get(row);

    public sealed override int GetInt(in int row) => (int)Get(row);

    public sealed override int? GetNullableInt(in int row) => (int)Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row);

    public sealed override uint? GetNullableUInt(in int row) => (uint)Get(row);

    public sealed override long GetLong(in int row) => (long)Get(row);

    public sealed override long? GetNullableLong(in int row) => (long)Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row);

    public sealed override ulong? GetNullableULong(in int row) => (ulong)Get(row);

    public sealed override float GetFloat(in int row) => (float)Get(row);

    public sealed override float? GetNullableFloat(in int row) => (float)Get(row);

    public sealed override double GetDouble(in int row) => (double)Get(row);

    public sealed override double? GetNullableDouble(in int row) => (double)Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row);

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableDecimalDataColumn : NullableDataColumn<decimal>
{
    public NullableDecimalDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Decimal;

    public sealed override bool GetBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override bool? GetNullableBool(in int row) => Get(row).GetValueOrDefault() != 0;

    public sealed override char GetChar(in int row) => (char)Get(row).GetValueOrDefault();

    public sealed override char? GetNullableChar(in int row) => (char?)Get(row);

    public sealed override sbyte GetSByte(in int row) => (sbyte)Get(row).GetValueOrDefault();

    public sealed override sbyte? GetNullableSByte(in int row) => (sbyte?)Get(row);

    public sealed override byte GetByte(in int row) => (byte)Get(row).GetValueOrDefault();

    public sealed override byte? GetNullableByte(in int row) => (byte?)Get(row);

    public sealed override short GetShort(in int row) => (short)Get(row).GetValueOrDefault();

    public sealed override short? GetNullableShort(in int row) => (short?)Get(row);

    public sealed override ushort GetUShort(in int row) => (ushort)Get(row).GetValueOrDefault();

    public sealed override ushort? GetNullableUShort(in int row) => (ushort?)Get(row);

    public sealed override int GetInt(in int row) => (int)Get(row).GetValueOrDefault();

    public sealed override int? GetNullableInt(in int row) => (int?)Get(row);

    public sealed override uint GetUInt(in int row) => (uint)Get(row).GetValueOrDefault();

    public sealed override uint? GetNullableUInt(in int row) => (uint?)Get(row);

    public sealed override long GetLong(in int row) => (long)Get(row).GetValueOrDefault();

    public sealed override long? GetNullableLong(in int row) => (long?)Get(row);

    public sealed override ulong GetULong(in int row) => (ulong)Get(row).GetValueOrDefault();

    public sealed override ulong? GetNullableULong(in int row) => (ulong?)Get(row);

    public sealed override float GetFloat(in int row) => (float)Get(row).GetValueOrDefault();

    public sealed override float? GetNullableFloat(in int row) => (float?)Get(row);

    public sealed override double GetDouble(in int row) => (double)Get(row).GetValueOrDefault();

    public sealed override double? GetNullableDouble(in int row) => (double?)Get(row);

    public sealed override decimal GetDecimal(in int row) => Get(row).GetValueOrDefault();

    public sealed override decimal? GetNullableDecimal(in int row) => Get(row);

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class GuidDataColumn : StructDataColumn<Guid>
{
    public GuidDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Guid;

    public sealed override bool GetBool(in int row) => throw new NotSupportedException();

    public sealed override bool? GetNullableBool(in int row) => throw new NotSupportedException();

    public sealed override char GetChar(in int row) => throw new NotSupportedException();

    public sealed override char? GetNullableChar(in int row) => throw new NotSupportedException();

    public sealed override sbyte GetSByte(in int row) => throw new NotSupportedException();

    public sealed override sbyte? GetNullableSByte(in int row) => throw new NotSupportedException();

    public sealed override byte GetByte(in int row) => throw new NotSupportedException();

    public sealed override byte? GetNullableByte(in int row) => throw new NotSupportedException();

    public sealed override short GetShort(in int row) => throw new NotSupportedException();

    public sealed override short? GetNullableShort(in int row) => throw new NotSupportedException();

    public sealed override ushort GetUShort(in int row) => throw new NotSupportedException();

    public sealed override ushort? GetNullableUShort(in int row) => throw new NotSupportedException();

    public sealed override int GetInt(in int row) => throw new NotSupportedException();

    public sealed override int? GetNullableInt(in int row) => throw new NotSupportedException();

    public sealed override uint GetUInt(in int row) => throw new NotSupportedException();

    public sealed override uint? GetNullableUInt(in int row) => throw new NotSupportedException();

    public sealed override long GetLong(in int row) => throw new NotSupportedException();

    public sealed override long? GetNullableLong(in int row) => throw new NotSupportedException();

    public sealed override ulong GetULong(in int row) => throw new NotSupportedException();

    public sealed override ulong? GetNullableULong(in int row) => throw new NotSupportedException();

    public sealed override float GetFloat(in int row) => throw new NotSupportedException();

    public sealed override float? GetNullableFloat(in int row) => throw new NotSupportedException();

    public sealed override double GetDouble(in int row) => throw new NotSupportedException();

    public sealed override double? GetNullableDouble(in int row) => throw new NotSupportedException();

    public sealed override decimal GetDecimal(in int row) => throw new NotSupportedException();

    public sealed override decimal? GetNullableDecimal(in int row) => throw new NotSupportedException();

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => Get(row);

    public sealed override Guid? GetNullableGuid(in int row) => Get(row);

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => Get(row).ToByteArray();
}

[DataContract]
public class NullableGuidDataColumn : NullableDataColumn<Guid>
{
    public NullableGuidDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Guid;

    public sealed override bool GetBool(in int row) => throw new NotSupportedException();

    public sealed override bool? GetNullableBool(in int row) => throw new NotSupportedException();

    public sealed override char GetChar(in int row) => throw new NotSupportedException();

    public sealed override char? GetNullableChar(in int row) => throw new NotSupportedException();

    public sealed override sbyte GetSByte(in int row) => throw new NotSupportedException();

    public sealed override sbyte? GetNullableSByte(in int row) => throw new NotSupportedException();

    public sealed override byte GetByte(in int row) => throw new NotSupportedException();

    public sealed override byte? GetNullableByte(in int row) => throw new NotSupportedException();

    public sealed override short GetShort(in int row) => throw new NotSupportedException();

    public sealed override short? GetNullableShort(in int row) => throw new NotSupportedException();

    public sealed override ushort GetUShort(in int row) => throw new NotSupportedException();

    public sealed override ushort? GetNullableUShort(in int row) => throw new NotSupportedException();

    public sealed override int GetInt(in int row) => throw new NotSupportedException();

    public sealed override int? GetNullableInt(in int row) => throw new NotSupportedException();

    public sealed override uint GetUInt(in int row) => throw new NotSupportedException();

    public sealed override uint? GetNullableUInt(in int row) => throw new NotSupportedException();

    public sealed override long GetLong(in int row) => throw new NotSupportedException();

    public sealed override long? GetNullableLong(in int row) => throw new NotSupportedException();

    public sealed override ulong GetULong(in int row) => throw new NotSupportedException();

    public sealed override ulong? GetNullableULong(in int row) => throw new NotSupportedException();

    public sealed override float GetFloat(in int row) => throw new NotSupportedException();

    public sealed override float? GetNullableFloat(in int row) => throw new NotSupportedException();

    public sealed override double GetDouble(in int row) => throw new NotSupportedException();

    public sealed override double? GetNullableDouble(in int row) => throw new NotSupportedException();

    public sealed override decimal GetDecimal(in int row) => throw new NotSupportedException();

    public sealed override decimal? GetNullableDecimal(in int row) => throw new NotSupportedException();

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => Get(row).GetValueOrDefault();

    public sealed override Guid? GetNullableGuid(in int row) => Get(row);

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => Get(row)?.ToByteArray();
}

[DataContract]
public class DateTimeDataColumn : StructDataColumn<DateTime>
{
    public DateTimeDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.DateTime;

    public sealed override bool GetBool(in int row) => throw new NotSupportedException();

    public sealed override bool? GetNullableBool(in int row) => throw new NotSupportedException();

    public sealed override char GetChar(in int row) => throw new NotSupportedException();

    public sealed override char? GetNullableChar(in int row) => throw new NotSupportedException();

    public sealed override sbyte GetSByte(in int row) => throw new NotSupportedException();

    public sealed override sbyte? GetNullableSByte(in int row) => throw new NotSupportedException();

    public sealed override byte GetByte(in int row) => throw new NotSupportedException();

    public sealed override byte? GetNullableByte(in int row) => throw new NotSupportedException();

    public sealed override short GetShort(in int row) => throw new NotSupportedException();

    public sealed override short? GetNullableShort(in int row) => throw new NotSupportedException();

    public sealed override ushort GetUShort(in int row) => throw new NotSupportedException();

    public sealed override ushort? GetNullableUShort(in int row) => throw new NotSupportedException();

    public sealed override int GetInt(in int row) => throw new NotSupportedException();

    public sealed override int? GetNullableInt(in int row) => throw new NotSupportedException();

    public sealed override uint GetUInt(in int row) => throw new NotSupportedException();

    public sealed override uint? GetNullableUInt(in int row) => throw new NotSupportedException();

    public sealed override long GetLong(in int row) => Get(row).Ticks;

    public sealed override long? GetNullableLong(in int row) => Get(row).Ticks;

    public sealed override ulong GetULong(in int row) => throw new NotSupportedException();

    public sealed override ulong? GetNullableULong(in int row) => throw new NotSupportedException();

    public sealed override float GetFloat(in int row) => throw new NotSupportedException();

    public sealed override float? GetNullableFloat(in int row) => throw new NotSupportedException();

    public sealed override double GetDouble(in int row) => throw new NotSupportedException();

    public sealed override double? GetNullableDouble(in int row) => throw new NotSupportedException();

    public sealed override decimal GetDecimal(in int row) => throw new NotSupportedException();

    public sealed override decimal? GetNullableDecimal(in int row) => throw new NotSupportedException();

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => Get(row);

    public sealed override DateTime? GetNullableDateTime(in int row) => Get(row);

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableDateTimeDataColumn : NullableDataColumn<DateTime>
{
    public NullableDateTimeDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.DateTime;

    public sealed override bool GetBool(in int row) => throw new NotSupportedException();

    public sealed override bool? GetNullableBool(in int row) => throw new NotSupportedException();

    public sealed override char GetChar(in int row) => throw new NotSupportedException();

    public sealed override char? GetNullableChar(in int row) => throw new NotSupportedException();

    public sealed override sbyte GetSByte(in int row) => throw new NotSupportedException();

    public sealed override sbyte? GetNullableSByte(in int row) => throw new NotSupportedException();

    public sealed override byte GetByte(in int row) => throw new NotSupportedException();

    public sealed override byte? GetNullableByte(in int row) => throw new NotSupportedException();

    public sealed override short GetShort(in int row) => throw new NotSupportedException();

    public sealed override short? GetNullableShort(in int row) => throw new NotSupportedException();

    public sealed override ushort GetUShort(in int row) => throw new NotSupportedException();

    public sealed override ushort? GetNullableUShort(in int row) => throw new NotSupportedException();

    public sealed override int GetInt(in int row) => throw new NotSupportedException();

    public sealed override int? GetNullableInt(in int row) => throw new NotSupportedException();

    public sealed override uint GetUInt(in int row) => throw new NotSupportedException();

    public sealed override uint? GetNullableUInt(in int row) => throw new NotSupportedException();

    public sealed override long GetLong(in int row) => Get(row).GetValueOrDefault().Ticks;

    public sealed override long? GetNullableLong(in int row) => Get(row)?.Ticks;

    public sealed override ulong GetULong(in int row) => throw new NotSupportedException();

    public sealed override ulong? GetNullableULong(in int row) => throw new NotSupportedException();

    public sealed override float GetFloat(in int row) => throw new NotSupportedException();

    public sealed override float? GetNullableFloat(in int row) => throw new NotSupportedException();

    public sealed override double GetDouble(in int row) => throw new NotSupportedException();

    public sealed override double? GetNullableDouble(in int row) => throw new NotSupportedException();

    public sealed override decimal GetDecimal(in int row) => throw new NotSupportedException();

    public sealed override decimal? GetNullableDecimal(in int row) => throw new NotSupportedException();

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => Get(row).GetValueOrDefault();

    public sealed override DateTime? GetNullableDateTime(in int row) => Get(row);

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class TimeSpanDataColumn : StructDataColumn<TimeSpan>
{
    public TimeSpanDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.TimeSpan;

    public sealed override bool GetBool(in int row) => throw new NotSupportedException();

    public sealed override bool? GetNullableBool(in int row) => throw new NotSupportedException();

    public sealed override char GetChar(in int row) => throw new NotSupportedException();

    public sealed override char? GetNullableChar(in int row) => throw new NotSupportedException();

    public sealed override sbyte GetSByte(in int row) => throw new NotSupportedException();

    public sealed override sbyte? GetNullableSByte(in int row) => throw new NotSupportedException();

    public sealed override byte GetByte(in int row) => throw new NotSupportedException();

    public sealed override byte? GetNullableByte(in int row) => throw new NotSupportedException();

    public sealed override short GetShort(in int row) => throw new NotSupportedException();

    public sealed override short? GetNullableShort(in int row) => throw new NotSupportedException();

    public sealed override ushort GetUShort(in int row) => throw new NotSupportedException();

    public sealed override ushort? GetNullableUShort(in int row) => throw new NotSupportedException();

    public sealed override int GetInt(in int row) => throw new NotSupportedException();

    public sealed override int? GetNullableInt(in int row) => throw new NotSupportedException();

    public sealed override uint GetUInt(in int row) => throw new NotSupportedException();

    public sealed override uint? GetNullableUInt(in int row) => throw new NotSupportedException();

    public sealed override long GetLong(in int row) => Get(row).Ticks;

    public sealed override long? GetNullableLong(in int row) => Get(row).Ticks;

    public sealed override ulong GetULong(in int row) => throw new NotSupportedException();

    public sealed override ulong? GetNullableULong(in int row) => throw new NotSupportedException();

    public sealed override float GetFloat(in int row) => throw new NotSupportedException();

    public sealed override float? GetNullableFloat(in int row) => throw new NotSupportedException();

    public sealed override double GetDouble(in int row) => throw new NotSupportedException();

    public sealed override double? GetNullableDouble(in int row) => throw new NotSupportedException();

    public sealed override decimal GetDecimal(in int row) => throw new NotSupportedException();

    public sealed override decimal? GetNullableDecimal(in int row) => throw new NotSupportedException();

    public sealed override string GetString(in int row) => Get(row).ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => Get(row);

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => Get(row);

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

[DataContract]
public class NullableTimeSpanDataColumn : NullableDataColumn<TimeSpan>
{
    public NullableTimeSpanDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.TimeSpan;

    public sealed override bool GetBool(in int row) => throw new NotSupportedException();

    public sealed override bool? GetNullableBool(in int row) => throw new NotSupportedException();

    public sealed override char GetChar(in int row) => throw new NotSupportedException();

    public sealed override char? GetNullableChar(in int row) => throw new NotSupportedException();

    public sealed override sbyte GetSByte(in int row) => throw new NotSupportedException();

    public sealed override sbyte? GetNullableSByte(in int row) => throw new NotSupportedException();

    public sealed override byte GetByte(in int row) => throw new NotSupportedException();

    public sealed override byte? GetNullableByte(in int row) => throw new NotSupportedException();

    public sealed override short GetShort(in int row) => throw new NotSupportedException();

    public sealed override short? GetNullableShort(in int row) => throw new NotSupportedException();

    public sealed override ushort GetUShort(in int row) => throw new NotSupportedException();

    public sealed override ushort? GetNullableUShort(in int row) => throw new NotSupportedException();

    public sealed override int GetInt(in int row) => throw new NotSupportedException();

    public sealed override int? GetNullableInt(in int row) => throw new NotSupportedException();

    public sealed override uint GetUInt(in int row) => throw new NotSupportedException();

    public sealed override uint? GetNullableUInt(in int row) => throw new NotSupportedException();

    public sealed override long GetLong(in int row) => Get(row).GetValueOrDefault().Ticks;

    public sealed override long? GetNullableLong(in int row) => Get(row)?.Ticks;

    public sealed override ulong GetULong(in int row) => throw new NotSupportedException();

    public sealed override ulong? GetNullableULong(in int row) => throw new NotSupportedException();

    public sealed override float GetFloat(in int row) => throw new NotSupportedException();

    public sealed override float? GetNullableFloat(in int row) => throw new NotSupportedException();

    public sealed override double GetDouble(in int row) => throw new NotSupportedException();

    public sealed override double? GetNullableDouble(in int row) => throw new NotSupportedException();

    public sealed override decimal GetDecimal(in int row) => throw new NotSupportedException();

    public sealed override decimal? GetNullableDecimal(in int row) => throw new NotSupportedException();

    public sealed override string GetString(in int row) => Get(row)?.ToString();

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => Get(row).GetValueOrDefault();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => Get(row);

    public sealed override byte[] GetByteArray(in int row) => throw new NotSupportedException();
}

