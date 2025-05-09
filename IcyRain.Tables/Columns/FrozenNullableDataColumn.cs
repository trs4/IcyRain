using System;
using System.Collections.Generic;

namespace IcyRain.Tables;

public abstract class FrozenNullableDataColumn<T> : FrozenSingleDataColumn<T?>
    where T : struct
{
    protected FrozenNullableDataColumn(NullableDataColumn<T> dataColumn) : base(dataColumn) { }

    public sealed override bool IsNullable => true;

    public HashSet<T> GetNonNullableUniqueValues(int count)
    {
        if (count < 0)
            throw new ArgumentNullException(nameof(count));

        var values = new HashSet<T>(count);

        if (count > _valuesCount)
        {
            for (int i = 0; i < _valuesCount; i++)
            {
                var value = _values[i];

                if (value.HasValue)
                    values.Add(value.Value);
            }

            if (_fallback.HasValue)
                values.Add(_fallback.Value);
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                var value = _values[i];

                if (value.HasValue)
                    values.Add(value.Value);
            }
        }

        return values;
    }

    public List<T> GetNonNullableValues(int count)
    {
        if (count < 0)
            throw new ArgumentNullException(nameof(count));

        var values = new List<T>(count);

        if (count > _valuesCount)
        {
            for (int i = 0; i < _valuesCount; i++)
            {
                var value = _values[i];

                if (value.HasValue)
                    values.Add(value.Value);
            }

            if (_fallback.HasValue)
            {
                for (int i = _valuesCount; i < count; i++)
                    values.Add(_fallback.Value);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                var value = _values[i];

                if (value.HasValue)
                    values.Add(value.Value);
            }
        }

        return values;
    }

    protected sealed override bool IsDefault(T? value) => !value.HasValue;
}
