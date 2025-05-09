﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using IcyRain.Tables.Internal;

namespace IcyRain.Tables;

[DataContract, KnownType(nameof(GetKnownTypes))]
public abstract class DataColumn
{
    public abstract Type LanguageType { get; }

    public abstract DataType Type { get; }

    public virtual bool IsNullable => false;

    public virtual bool IsArray => false;

    public abstract FrozenDataColumn ToFrozenObject();

    [MethodImpl(Flags.HotPath)]
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

    public abstract void SetObject(in int row, object value);

    public abstract void Clear(in int row);

    public abstract bool IsEmpty(in int row);

    public abstract bool IsNull(in int row);

    public virtual object GetObjectFallback() => null;

    internal virtual void CleanBottom() { }

    internal virtual void CompressFallback(in int tableRowCount) { }

    internal abstract void SetNullRowsIfEmpty();

    public abstract void ClearRows(in int capacity);

    [MethodImpl(Flags.HotPath)]
    protected static void Set<T>(in int row, in T value, List<T> values, in T fallback, Func<T, bool> isDefault)
    {
        if (values.Count == row)
            values.Add(value);
        else if (values.Count > row)
            values[row] = value;
        else if (!isDefault(fallback) || !isDefault(value))
        {
            while (values.Count < row)
                values.Add(fallback);

            values.Add(value);
        }
    }

    [MethodImpl(Flags.HotPath)]
    protected static void SetWithCheck<T>(in int row, T value, List<T> values, in T fallback, Func<T, bool> isDefault)
    {
        if (isDefault(value))
            value = default;

        Set(row, value, values, fallback, isDefault);
    }

    protected static void CleanBottom<T>(List<T> values, Action<List<T>> setRows, T fallback, Func<T, T, bool> equals)
    {
        if (values.Count == 0)
            return;

        int index = values.FindLastIndex(r => !equals(r, fallback));

        if (index < 0)
        {
            if (values.All(r => equals(r, fallback)))
                setRows(null);

            return;
        }

        if (++index == values.Count)
            return;

        values.RemoveRange(index, values.Count - index);
    }

    protected static void SetFallback<T>(in int tableRowCount, List<T> values, Action<List<T>> setRows, Action<T> setFallback, Func<T, T, bool> equals)
    {
        if (values is null || values.Count != tableRowCount || values.Count == 0)
            return;

        int index = values.Count - 1;

        if (index == 0)
        {
            setFallback(values[0]);
            setRows(null);
        }
        else if (index > 0)
        {
            var fallback = values[index];

            if (equals(fallback, values[index - 1]))
            {
                setFallback(fallback);
                CleanBottom(values, setRows, fallback, equals);
            }
        }
    }

    protected static void SetNullRowsIfEmpty<T>(List<T> values, Action<List<T>> setRows)
    {
        if (values is not null && values.Count == 0)
            setRows(null);
    }

    protected static void ClearRows<T>(List<T> values, Action<List<T>> setRows, in int capacity)
    {
        if (values is null || values.Capacity < capacity)
            setRows(new List<T>(capacity));
        else
            values.Clear();
    }

    public static IEnumerable<Type> GetKnownTypes()
    {
        yield return typeof(BooleanDataColumn);
        yield return typeof(NullableBooleanDataColumn);
        yield return typeof(CharDataColumn);
        yield return typeof(NullableCharDataColumn);
        yield return typeof(SByteDataColumn);
        yield return typeof(NullableSByteDataColumn);
        yield return typeof(ByteDataColumn);
        yield return typeof(NullableByteDataColumn);
        yield return typeof(Int16DataColumn);
        yield return typeof(NullableInt16DataColumn);
        yield return typeof(UInt16DataColumn);
        yield return typeof(NullableUInt16DataColumn);
        yield return typeof(Int32DataColumn);
        yield return typeof(NullableInt32DataColumn);
        yield return typeof(UInt32DataColumn);
        yield return typeof(NullableUInt32DataColumn);
        yield return typeof(Int64DataColumn);
        yield return typeof(NullableInt64DataColumn);
        yield return typeof(UInt64DataColumn);
        yield return typeof(NullableUInt64DataColumn);
        yield return typeof(SingleDataColumn);
        yield return typeof(NullableSingleDataColumn);
        yield return typeof(DoubleDataColumn);
        yield return typeof(NullableDoubleDataColumn);
        yield return typeof(DecimalDataColumn);
        yield return typeof(NullableDecimalDataColumn);
        yield return typeof(StringDataColumn);
        yield return typeof(NullableStringDataColumn);
        yield return typeof(GuidDataColumn);
        yield return typeof(NullableGuidDataColumn);
        yield return typeof(DateTimeDataColumn);
        yield return typeof(NullableDateTimeDataColumn);
        yield return typeof(TimeSpanDataColumn);
        yield return typeof(NullableTimeSpanDataColumn);
        yield return typeof(ByteArrayDataColumn);
    }

    public static DataColumn Create(DataType type, int capacity = 4, bool nullable = false, bool isArray = false)
    {
        if (isArray)
            return CreateArray(type, capacity);

        return nullable ? CreateNullable(type, capacity) : CreateCore(type, capacity);
    }

    private static DataColumn CreateCore(DataType type, int capacity) => type switch
    {
        DataType.Boolean => new BooleanDataColumn(capacity),
        DataType.Char => new CharDataColumn(capacity),
        DataType.SByte => new SByteDataColumn(capacity),
        DataType.Byte => new ByteDataColumn(capacity),
        DataType.Int16 => new Int16DataColumn(capacity),
        DataType.UInt16 => new UInt16DataColumn(capacity),
        DataType.Int32 => new Int32DataColumn(capacity),
        DataType.UInt32 => new UInt32DataColumn(capacity),
        DataType.Int64 => new Int64DataColumn(capacity),
        DataType.UInt64 => new UInt64DataColumn(capacity),
        DataType.Single => new SingleDataColumn(capacity),
        DataType.Double => new DoubleDataColumn(capacity),
        DataType.Decimal => new DecimalDataColumn(capacity),
        DataType.String => new StringDataColumn(capacity),
        DataType.Guid => new GuidDataColumn(capacity),
        DataType.DateTime => new DateTimeDataColumn(capacity),
        DataType.TimeSpan => new TimeSpanDataColumn(capacity),
        _ => throw new NotSupportedException(type.ToString()),
    };

    private static DataColumn CreateNullable(DataType type, int capacity) => type switch
    {
        DataType.Boolean => new NullableBooleanDataColumn(capacity),
        DataType.Char => new NullableCharDataColumn(capacity),
        DataType.SByte => new NullableSByteDataColumn(capacity),
        DataType.Byte => new NullableByteDataColumn(capacity),
        DataType.Int16 => new NullableInt16DataColumn(capacity),
        DataType.UInt16 => new NullableUInt16DataColumn(capacity),
        DataType.Int32 => new NullableInt32DataColumn(capacity),
        DataType.UInt32 => new NullableUInt32DataColumn(capacity),
        DataType.Int64 => new NullableInt64DataColumn(capacity),
        DataType.UInt64 => new NullableUInt64DataColumn(capacity),
        DataType.Single => new NullableSingleDataColumn(capacity),
        DataType.Double => new NullableDoubleDataColumn(capacity),
        DataType.Decimal => new NullableDecimalDataColumn(capacity),
        DataType.String => new NullableStringDataColumn(capacity),
        DataType.Guid => new NullableGuidDataColumn(capacity),
        DataType.DateTime => new NullableDateTimeDataColumn(capacity),
        DataType.TimeSpan => new NullableTimeSpanDataColumn(capacity),
        _ => throw new NotSupportedException(type.ToString()),
    };

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private static DataColumn CreateArray(DataType type, int capacity) => type switch
    {
        DataType.Byte => new ByteArrayDataColumn(capacity),
        _ => throw new NotSupportedException(type.ToString()),
    };
#pragma warning restore CA1859 // Use concrete types when possible for improved performance

    public void Compress(in int tableRowCount)
    {
        CleanBottom();
        CompressFallback(tableRowCount);
        SetNullRowsIfEmpty();
    }

}
