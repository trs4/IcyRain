using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IcyRain.Tables;

[DataContract]
public abstract class NullableDataColumn<T> : SingleDataColumn<T?>
    where T : struct
{
    protected NullableDataColumn(int capacity) : base(capacity) { }

    public sealed override bool IsNullable => true;

    public HashSet<T> GetNonNullableUniqueValues(int count)
    {
        if (count < 0)
            throw new ArgumentNullException(nameof(count));

        if (Values is null)
            return Fallback.HasValue ? [Fallback.Value] : [];

        int valuesCount = Values.Count;
        var values = new HashSet<T>(count);

        if (count > valuesCount)
        {
            for (int i = 0; i < Values.Count; i++)
            {
                var value = Values[i];

                if (value.HasValue)
                    values.Add(value.Value);
            }

            if (Fallback.HasValue)
                values.Add(Fallback.Value);
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                var value = Values[i];

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

        if (Values is null)
            return GetNullValues(count);

        int valuesCount = Values.Count;
        var values = new List<T>(count);

        if (count > valuesCount)
        {
            for (int i = 0; i < Values.Count; i++)
            {
                var value = Values[i];

                if (value.HasValue)
                    values.Add(value.Value);
            }

            if (Fallback.HasValue)
            {
                for (int i = valuesCount; i < count; i++)
                    values.Add(Fallback.Value);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                var value = Values[i];

                if (value.HasValue)
                    values.Add(value.Value);
            }
        }

        return values;
    }

    private List<T> GetNullValues(int count)
    {
        if (!Fallback.HasValue)
            return [];

        var values = new List<T>(count);

        for (int i = 0; i < count; i++)
            values.Add(Fallback.Value);

        return values;
    }

    protected sealed override bool IsDefault(T? value) => !value.HasValue;

    protected sealed override bool Equals(T? x, T? y) => (x.HasValue && y.HasValue) ? x.Value.Equals(y.Value) : !x.HasValue && !y.HasValue;
}
