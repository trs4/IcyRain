using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using IcyRain.Tables.Internal;

namespace IcyRain.Tables;

[DataContract]
public sealed class NullableStringDataColumn : SingleDataColumn<string>
{
    public NullableStringDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.String;

    public sealed override bool IsNullable => true;

    public sealed override FrozenDataColumn<string> ToFrozen() => new FrozenNullableStringDataColumn(this);

    public sealed override bool GetBool(in int row) => bool.Parse(base.Get(row) ?? string.Empty);

    public sealed override bool? GetNullableBool(in int row) => bool.Parse(base.Get(row) ?? string.Empty);

    public sealed override char GetChar(in int row) => char.Parse(base.Get(row) ?? string.Empty);

    public sealed override char? GetNullableChar(in int row) => char.Parse(base.Get(row) ?? string.Empty);

    public sealed override sbyte GetSByte(in int row) => sbyte.Parse(base.Get(row) ?? string.Empty);

    public sealed override sbyte? GetNullableSByte(in int row) => sbyte.Parse(base.Get(row) ?? string.Empty);

    public sealed override byte GetByte(in int row) => byte.Parse(base.Get(row) ?? string.Empty);

    public sealed override byte? GetNullableByte(in int row) => byte.Parse(base.Get(row) ?? string.Empty);

    public sealed override short GetShort(in int row) => short.Parse(base.Get(row) ?? string.Empty);

    public sealed override short? GetNullableShort(in int row) => short.Parse(base.Get(row) ?? string.Empty);

    public sealed override ushort GetUShort(in int row) => ushort.Parse(base.Get(row) ?? string.Empty);

    public sealed override ushort? GetNullableUShort(in int row) => ushort.Parse(base.Get(row) ?? string.Empty);

    public sealed override int GetInt(in int row) => int.Parse(Get(row) ?? string.Empty);

    public sealed override int? GetNullableInt(in int row) => int.Parse(base.Get(row) ?? string.Empty);

    public sealed override uint GetUInt(in int row) => uint.Parse(base.Get(row) ?? string.Empty);

    public sealed override uint? GetNullableUInt(in int row) => uint.Parse(base.Get(row) ?? string.Empty);

    public sealed override long GetLong(in int row) => long.Parse(base.Get(row) ?? string.Empty);

    public sealed override long? GetNullableLong(in int row) => long.Parse(base.Get(row) ?? string.Empty);

    public sealed override ulong GetULong(in int row) => ulong.Parse(base.Get(row) ?? string.Empty);

    public sealed override ulong? GetNullableULong(in int row) => ulong.Parse(base.Get(row) ?? string.Empty);

    public sealed override float GetFloat(in int row) => float.Parse(base.Get(row) ?? string.Empty);

    public sealed override float? GetNullableFloat(in int row) => float.Parse(base.Get(row) ?? string.Empty);

    public sealed override double GetDouble(in int row) => double.Parse(base.Get(row) ?? string.Empty);

    public sealed override double? GetNullableDouble(in int row) => double.Parse(base.Get(row) ?? string.Empty);

    public sealed override decimal GetDecimal(in int row) => decimal.Parse(base.Get(row) ?? string.Empty);

    public sealed override decimal? GetNullableDecimal(in int row) => decimal.Parse(base.Get(row) ?? string.Empty);

    public sealed override string GetString(in int row) => Get(row);

    public sealed override Guid GetGuid(in int row) => Guid.Parse(base.Get(row) ?? string.Empty);

    public sealed override Guid? GetNullableGuid(in int row) => Guid.Parse(base.Get(row) ?? string.Empty);

    public sealed override DateTime GetDateTime(in int row) => DateTime.Parse(base.Get(row) ?? string.Empty);

    public sealed override DateTime? GetNullableDateTime(in int row) => DateTime.Parse(base.Get(row) ?? string.Empty);

    public sealed override TimeSpan GetTimeSpan(in int row) => TimeSpan.Parse(base.Get(row) ?? string.Empty);

    public sealed override TimeSpan? GetNullableTimeSpan(in int row) => TimeSpan.Parse(base.Get(row) ?? string.Empty);

    public sealed override byte[] GetByteArray(in int row) => Encoding.UTF8.GetBytes(Get(row) ?? string.Empty);

    [MethodImpl(Flags.HotPath)]
    protected sealed override bool Equals(string x, string y) => x == y;

    [MethodImpl(Flags.HotPath)]
    protected sealed override bool IsDefault(string value) => value is null;
}
