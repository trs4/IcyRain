using System;

namespace IcyRain.Tables;

public abstract class FrozenDataColumn
{
    public abstract Type LanguageType { get; }

    public abstract DataType Type { get; }

    public virtual bool IsNullable => false;

    public virtual bool IsArray => false;

    public abstract object GetObject(in int row);

    public abstract bool GetBool(in int row);

    public abstract bool? GetNullableBool(in int row);

    public abstract char GetChar(in int row);

    public abstract char? GetNullableChar(in int row);

    public abstract sbyte GetSByte(in int row);

    public abstract sbyte? GetNullableSByte(in int row);

    public abstract byte GetByte(in int row);

    public abstract byte? GetNullableByte(in int row);

    public abstract short GetShort(in int row);

    public abstract short? GetNullableShort(in int row);

    public abstract ushort GetUShort(in int row);

    public abstract ushort? GetNullableUShort(in int row);

    public abstract int GetInt(in int row);

    public abstract int? GetNullableInt(in int row);

    public abstract uint GetUInt(in int row);

    public abstract uint? GetNullableUInt(in int row);

    public abstract long GetLong(in int row);

    public abstract long? GetNullableLong(in int row);

    public abstract ulong GetULong(in int row);

    public abstract ulong? GetNullableULong(in int row);

    public abstract float GetFloat(in int row);

    public abstract float? GetNullableFloat(in int row);

    public abstract double GetDouble(in int row);

    public abstract double? GetNullableDouble(in int row);

    public abstract decimal GetDecimal(in int row);

    public abstract decimal? GetNullableDecimal(in int row);

    public abstract string GetString(in int row);

    public abstract Guid GetGuid(in int row);

    public abstract Guid? GetNullableGuid(in int row);

    public abstract DateTime GetDateTime(in int row);

    public abstract DateTime? GetNullableDateTime(in int row);

    public abstract TimeSpan GetTimeSpan(in int row);

    public abstract TimeSpan? GetNullableTimeSpan(in int row);

    public abstract byte[] GetByteArray(in int row);

    public abstract bool IsEmpty(in int row);

    public abstract bool IsNull(in int row);

    public virtual object GetObjectFallback() => null;
}
