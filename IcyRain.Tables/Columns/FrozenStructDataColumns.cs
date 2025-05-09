using System;

namespace IcyRain.Tables;

public class FrozenBooleanDataColumn : FrozenStructDataColumn<bool>
{
    internal FrozenBooleanDataColumn(BooleanDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableBooleanDataColumn : FrozenNullableDataColumn<bool>
{
    internal FrozenNullableBooleanDataColumn(NullableBooleanDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenCharDataColumn : FrozenStructDataColumn<char>
{
    internal FrozenCharDataColumn(CharDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableCharDataColumn : FrozenNullableDataColumn<char>
{
    internal FrozenNullableCharDataColumn(NullableCharDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenSByteDataColumn : FrozenStructDataColumn<sbyte>
{
    internal FrozenSByteDataColumn(SByteDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableSByteDataColumn : FrozenNullableDataColumn<sbyte>
{
    internal FrozenNullableSByteDataColumn(NullableSByteDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenByteDataColumn : FrozenStructDataColumn<byte>
{
    internal FrozenByteDataColumn(ByteDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableByteDataColumn : FrozenNullableDataColumn<byte>
{
    internal FrozenNullableByteDataColumn(NullableByteDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenInt16DataColumn : FrozenStructDataColumn<short>
{
    internal FrozenInt16DataColumn(Int16DataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableInt16DataColumn : FrozenNullableDataColumn<short>
{
    internal FrozenNullableInt16DataColumn(NullableInt16DataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenUInt16DataColumn : FrozenStructDataColumn<ushort>
{
    internal FrozenUInt16DataColumn(UInt16DataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableUInt16DataColumn : FrozenNullableDataColumn<ushort>
{
    internal FrozenNullableUInt16DataColumn(NullableUInt16DataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenInt32DataColumn : FrozenStructDataColumn<int>
{
    internal FrozenInt32DataColumn(Int32DataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableInt32DataColumn : FrozenNullableDataColumn<int>
{
    internal FrozenNullableInt32DataColumn(NullableInt32DataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenUInt32DataColumn : FrozenStructDataColumn<uint>
{
    internal FrozenUInt32DataColumn(UInt32DataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableUInt32DataColumn : FrozenNullableDataColumn<uint>
{
    internal FrozenNullableUInt32DataColumn(NullableUInt32DataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenInt64DataColumn : FrozenStructDataColumn<long>
{
    internal FrozenInt64DataColumn(Int64DataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableInt64DataColumn : FrozenNullableDataColumn<long>
{
    internal FrozenNullableInt64DataColumn(NullableInt64DataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenUInt64DataColumn : FrozenStructDataColumn<ulong>
{
    internal FrozenUInt64DataColumn(UInt64DataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableUInt64DataColumn : FrozenNullableDataColumn<ulong>
{
    internal FrozenNullableUInt64DataColumn(NullableUInt64DataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenSingleDataColumn : FrozenStructDataColumn<float>
{
    internal FrozenSingleDataColumn(SingleDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableSingleDataColumn : FrozenNullableDataColumn<float>
{
    internal FrozenNullableSingleDataColumn(NullableSingleDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenDoubleDataColumn : FrozenStructDataColumn<double>
{
    internal FrozenDoubleDataColumn(DoubleDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableDoubleDataColumn : FrozenNullableDataColumn<double>
{
    internal FrozenNullableDoubleDataColumn(NullableDoubleDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenDecimalDataColumn : FrozenStructDataColumn<decimal>
{
    internal FrozenDecimalDataColumn(DecimalDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableDecimalDataColumn : FrozenNullableDataColumn<decimal>
{
    internal FrozenNullableDecimalDataColumn(NullableDecimalDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenGuidDataColumn : FrozenStructDataColumn<Guid>
{
    internal FrozenGuidDataColumn(GuidDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableGuidDataColumn : FrozenNullableDataColumn<Guid>
{
    internal FrozenNullableGuidDataColumn(NullableGuidDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenDateTimeDataColumn : FrozenStructDataColumn<DateTime>
{
    internal FrozenDateTimeDataColumn(DateTimeDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableDateTimeDataColumn : FrozenNullableDataColumn<DateTime>
{
    internal FrozenNullableDateTimeDataColumn(NullableDateTimeDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenTimeSpanDataColumn : FrozenStructDataColumn<TimeSpan>
{
    internal FrozenTimeSpanDataColumn(TimeSpanDataColumn dataColumn) : base(dataColumn) { }

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

public class FrozenNullableTimeSpanDataColumn : FrozenNullableDataColumn<TimeSpan>
{
    internal FrozenNullableTimeSpanDataColumn(NullableTimeSpanDataColumn dataColumn) : base(dataColumn) { }

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
