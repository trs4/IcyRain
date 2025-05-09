using System;
using System.Collections.Generic;

namespace IcyRain.Tables;

public abstract class FrozenStructDataColumn<T> : FrozenSingleDataColumn<T>
    where T : struct, IEquatable<T>
{
    protected FrozenStructDataColumn(StructDataColumn<T> dataColumn) : base(dataColumn) { }

    public HashSet<T> GetUniqueValues(int count)
    {
        if (count < 0)
            throw new ArgumentNullException(nameof(count));

        var values = new HashSet<T>(count);

        if (count > _valuesCount)
        {
            for (int i = 0; i < _valuesCount; i++)
                values.Add(_values[i]);

            values.Add(_fallback);
        }
        else
        {
            for (int i = 0; i < count; i++)
                values.Add(_values[i]);
        }

        return values;
    }

    protected sealed override bool IsDefault(T value) => value.Equals(default);

    public sealed override bool IsNull(in int row) => false;
}
