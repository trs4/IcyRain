using System;
using System.Collections.Generic;

namespace IcyRain.Tables;

public abstract class FrozenArrayDataColumn<T> : FrozenDataColumn<T[]>
{
    protected FrozenArrayDataColumn(ArrayDataColumn<T> dataColumn) : base(dataColumn) { }

    public sealed override bool IsArray => true;

    protected sealed override bool IsDefault(T[] value) => value is null;

    public sealed override List<T[]> GetValues(int count)
    {
        if (count < 0)
            throw new ArgumentNullException(nameof(count));

        if (count == _valuesCount)
            return [.. _values];

        var values = new List<T[]>(count);

        if (count > _valuesCount)
        {
            values.AddRange(_values);

            for (int i = _valuesCount; i < count; i++)
                values.Add(null);
        }
        else
        {
            for (int i = 0; i < count; i++)
                values.Add(_values[i]);
        }

        return values;
    }

}
