using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IcyRain.Tables.Internal;

namespace IcyRain.Tables;

public abstract class FrozenSingleDataColumn<T> : FrozenDataColumn<T>
{
#pragma warning disable CA1051 // Do not declare visible instance fields
    protected readonly T _fallback;
#pragma warning restore CA1051 // Do not declare visible instance fields

    protected FrozenSingleDataColumn(SingleDataColumn<T> dataColumn)
        : base(dataColumn)
        => _fallback = dataColumn.Fallback;

    public override List<T> GetValues(int count)
    {
        if (count < 0)
            throw new ArgumentNullException(nameof(count));

        if (count == _valuesCount)
            return [.. _values];

        var values = new List<T>(count);

        if (count > _valuesCount)
        {
            values.AddRange(_values);

            for (int i = _valuesCount; i < count; i++)
                values.Add(_fallback);
        }
        else
        {
            for (int i = 0; i < count; i++)
                values.Add(_values[i]);
        }

        return values;
    }

    [MethodImpl(Flags.HotPath)]
    public override T Get(in int row) => _valuesCount > row ? _values[row] : _fallback;

    [MethodImpl(Flags.HotPath)]
    public sealed override object GetObjectFallback() => _fallback;
}
