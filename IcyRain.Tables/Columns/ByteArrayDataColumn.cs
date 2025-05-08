using System;
using System.Runtime.Serialization;

namespace IcyRain.Tables;

[DataContract]
public sealed class ByteArrayDataColumn : ArrayDataColumn<byte>
{
    public ByteArrayDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.Byte;

    public sealed override bool GetBool(in int row) => Get(row) is not null;

    public sealed override bool? GetNullableBool(in int row) => Get(row) is not null;

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

    public sealed override string GetString(in int row) => "byte[]";

    public sealed override Guid GetGuid(in int row) => throw new NotSupportedException();

    public sealed override Guid? GetNullableGuid(in int row) => throw new NotSupportedException();

    public sealed override DateTime GetDateTime(in int row) => throw new NotSupportedException();

    public sealed override DateTime? GetNullableDateTime(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan GetTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => throw new NotSupportedException();

    public sealed override byte[] GetByteArray(in int row) => Get(row);
}
