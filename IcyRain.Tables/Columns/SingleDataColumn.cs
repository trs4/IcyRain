using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using IcyRain.Tables.Internal;

namespace IcyRain.Tables;

[DataContract]
public abstract class SingleDataColumn<T> : DataColumn<T>
{
    protected SingleDataColumn(int capacity) : base(capacity) { }

    [DataMember(Order = 2, EmitDefaultValue = false)]
    public virtual T Fallback { get; set; }

    public override List<T> GetValues(int count)
    {
        if (count < 0)
            throw new ArgumentNullException(nameof(count));

        if (Values is null)
            return GetNullValues(count);

        int valuesCount = Values.Count;

        if (count == valuesCount)
            return new(Values);

        var values = new List<T>(count);

        if (count > valuesCount)
        {
            values.AddRange(Values);

            for (int i = valuesCount; i < count; i++)
                values.Add(Fallback);
        }
        else
        {
            for (int i = 0; i < count; i++)
                values.Add(Values[i]);
        }

        return values;
    }

    private List<T> GetNullValues(int count)
    {
        var values = new List<T>(count);

        for (int i = 0; i < count; i++)
            values.Add(Fallback);

        return values;
    }

    [MethodImpl(Flags.HotPath)]
    public override T Get(in int row)
    {
        var rowValues = Values;
        return rowValues is not null && rowValues.Count > row ? rowValues[row] : Fallback;
    }

    [MethodImpl(Flags.HotPath)]
    public override void Set(in int row, in T value) => Set(row, value, Values, Fallback, IsDefault);

    [MethodImpl(Flags.HotPath)]
    public override void SetObject(in int row, object value) => Set(row, (T)value, Values, Fallback, IsDefault);

    [MethodImpl(Flags.HotPath)]
    public sealed override object GetObjectFallback() => Fallback;

    protected sealed override T GetDefaultValue() => Fallback;

    internal sealed override void CleanBottom() => CleanBottom(Values, SetRows, Fallback, Equals);

    internal sealed override void CompressFallback(in int tableRowCount)
        => SetFallback(tableRowCount, Values, SetRows, SetFallbackValue, Equals);

    public sealed override void ClearRows(in int capacity)
    {
        base.ClearRows(capacity);
        Fallback = default;
    }

    [MethodImpl(Flags.HotPath)]
    private void SetFallbackValue(T newValue) => Fallback = newValue;

    protected abstract bool Equals(T x, T y);
}
