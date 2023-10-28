using System;
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

    public abstract object GetObject(in int row);

    public abstract void SetObject(in int row, object value);

    public abstract void Clear(in int row);

    public abstract bool IsEmpty(in int row);

    public abstract bool IsNull(in int row);

    public virtual object GetObjectFallback() => null;

    internal virtual void CleanBottom() { }

    internal virtual void CompressFallback(in int tableRowCount) { }

    internal abstract void SetNullRowsIfEmpty();

    public abstract void ClearRows(in int capacity);

    protected abstract int GetCount();

    [MethodImpl(Flags.HotPath)]
    protected static void Set<T>(in int row, in T value, List<T> rowValues, in T fallbackValue, Func<T, bool> isDefault)
    {
        if (rowValues.Count == row)
            rowValues.Add(value);
        else if (rowValues.Count > row)
            rowValues[row] = value;
        else if (!isDefault(fallbackValue) || !isDefault(value))
        {
            while (rowValues.Count < row)
                rowValues.Add(fallbackValue);

            rowValues.Add(value);
        }
    }

    [MethodImpl(Flags.HotPath)]
    protected static void SetWithCheck<T>(in int row, T value, List<T> rowValues,
        in T fallbackValue, Func<T, bool> isDefault)
    {
        if (isDefault(value))
            value = default;

        Set(row, value, rowValues, fallbackValue, isDefault);
    }

    protected static void CleanBottom<T>(List<T> rowValues, Action<List<T>> setRows,
        T fallbackValue, Func<T, T, bool> equals)
    {
        if (rowValues.Count == 0)
            return;

        int index = rowValues.FindLastIndex(r => !equals(r, fallbackValue));

        if (index < 0)
        {
            if (rowValues.All(r => equals(r, fallbackValue)))
                setRows(null);

            return;
        }

        if (++index == rowValues.Count)
            return;

        rowValues.RemoveRange(index, rowValues.Count - index);
    }

    protected static void SetFallback<T>(in int tableRowCount, List<T> rowValues,
        Action<List<T>> setRows, Action<T> setFallbackValue, Func<T, T, bool> equals)
    {
        if (rowValues is null || rowValues.Count != tableRowCount || rowValues.Count == 0)
            return;

        int index = rowValues.Count - 1;

        if (index == 0)
        {
            setFallbackValue(rowValues[0]);
            setRows(null);
        }
        else if (index > 0)
        {
            var fallbackValue = rowValues[index];

            if (equals(fallbackValue, rowValues[index - 1]))
            {
                setFallbackValue(fallbackValue);
                CleanBottom(rowValues, setRows, fallbackValue, equals);
            }
        }
    }

    protected static void SetNullRowsIfEmpty<T>(List<T> rowValues, Action<List<T>> setRows)
    {
        if (rowValues is not null && rowValues.Count == 0)
            setRows(null);
    }

    protected static void ClearRows<T>(List<T> rowValues, Action<List<T>> setRows, in int capacity)
    {
        if (rowValues is null || rowValues.Capacity < capacity)
            setRows(new List<T>(capacity));
        else
            rowValues.Clear();
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

    private static DataColumn CreateArray(DataType type, int capacity) => type switch
    {
        DataType.Byte => new ByteArrayDataColumn(capacity),
        _ => throw new NotSupportedException(type.ToString()),
    };

    public void Compress(in int tableRowCount)
    {
        CleanBottom();
        CompressFallback(tableRowCount);
        SetNullRowsIfEmpty();
    }

}
