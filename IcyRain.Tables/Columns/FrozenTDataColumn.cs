using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IcyRain.Tables.Internal;

namespace IcyRain.Tables;

public abstract class FrozenDataColumn<T> : FrozenDataColumn
{
#pragma warning disable CA1051 // Do not declare visible instance fields
    protected readonly T[] _values;
    protected readonly int _valuesCount;
#pragma warning restore CA1051 // Do not declare visible instance fields

    protected FrozenDataColumn(DataColumn<T> dataColumn)
    {
        _values = dataColumn.Values?.GetArray() ?? [];
        _valuesCount = dataColumn.Values?.Count ?? 0;
    }

    public sealed override Type LanguageType => typeof(T);

    [MethodImpl(Flags.HotPath)]
    public virtual T Get(in int row) => _valuesCount > row ? _values[row] : default;

    [MethodImpl(Flags.HotPath)]
    public sealed override object GetObject(in int row) => Get(row);

    public sealed override bool IsEmpty(in int row) => IsDefault(Get(row));

    public override bool IsNull(in int row) => Get(row) is null;

    public abstract List<T> GetValues(int count);

    protected abstract bool IsDefault(T value);
}
