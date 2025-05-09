using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using IcyRain.Tables.Internal;

namespace IcyRain.Tables;

[DataContract]
public abstract class DataColumn<T> : DataColumn
{
    protected DataColumn(int capacity)
        => Values = new(capacity);

    [DataMember(Order = 1, EmitDefaultValue = false)]
    public virtual List<T> Values { get; set; }

    public sealed override Type LanguageType => typeof(T);

    public abstract FrozenDataColumn<T> ToFrozen();

    public sealed override FrozenDataColumn ToFrozenObject() => ToFrozen();

    [MethodImpl(Flags.HotPath)]
    public virtual T Get(in int row)
    {
        var values = Values;
        return values is not null && values.Count > row ? values[row] : default;
    }

    [MethodImpl(Flags.HotPath)]
    public virtual void Set(in int row, in T value) => Set(row, value, Values, default, IsDefault);

    [MethodImpl(Flags.HotPath)]
    public sealed override object GetObject(in int row) => Get(row);

    [MethodImpl(Flags.HotPath)]
    public override void SetObject(in int row, object value) => Set(row, (T)value, Values, default, IsDefault);

    public sealed override void Clear(in int row)
    {
        if (Values.Count == row + 1)
            Values.RemoveAt(row);
        else if (Values.Count >= row)
            Values[row] = default;
    }

    public sealed override bool IsEmpty(in int row) => IsDefault(Get(row));

    public override bool IsNull(in int row) => Get(row) is null;

    public abstract List<T> GetValues(int count);

    internal sealed override void SetNullRowsIfEmpty() => SetNullRowsIfEmpty(Values, SetRows);

    public override void ClearRows(in int capacity) => ClearRows(Values, SetRows, capacity);

    [MethodImpl(Flags.HotPath)]
    protected void SetRows(List<T> newValue) => Values = newValue;

    protected abstract bool IsDefault(T value);
}
