using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IcyRain.Tables;

[DataContract]
public abstract class ArrayDataColumn<T> : DataColumn<T[]>
{
    protected ArrayDataColumn(int capacity) : base(capacity) { }

    public sealed override bool IsArray => true;

    internal sealed override void CleanBottom() => CleanBottom(Values, SetRows, null, Equals);

    protected sealed override bool IsDefault(T[] value) => value is null;

    public sealed override List<T[]> GetValues(int count)
    {
        if (count < 0)
            throw new ArgumentNullException(nameof(count));

        if (Values is null)
            return GetNullValues(count);

        int valuesCount = Values.Count;

        if (count == valuesCount)
            return [.. Values];

        var values = new List<T[]>(count);

        if (count > valuesCount)
        {
            values.AddRange(Values);

            for (int i = valuesCount; i < count; i++)
                values.Add(null);
        }
        else
        {
            for (int i = 0; i < count; i++)
                values.Add(Values[i]);
        }

        return values;
    }

    private static List<T[]> GetNullValues(int count)
    {
        var values = new List<T[]>(count);

        for (int i = 0; i < count; i++)
            values.Add(null);

        return values;
    }

}
