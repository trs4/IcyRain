using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IcyRain.Tables;

[DataContract]
public abstract class StructDataColumn<T> : SingleDataColumn<T>
    where T : struct, IEquatable<T>
{
    protected StructDataColumn(int capacity) : base(capacity) { }

    public HashSet<T> GetUniqueValues(int count)
    {
        if (count < 0)
            throw new ArgumentNullException(nameof(count));

        if (Values is null)
            return [Fallback];

        int valuesCount = Values.Count;
        var values = new HashSet<T>(count);

        if (count > valuesCount)
        {
            for (int i = 0; i < Values.Count; i++)
                values.Add(Values[i]);

            values.Add(Fallback);
        }
        else
        {
            for (int i = 0; i < count; i++)
                values.Add(Values[i]);
        }

        return values;
    }

    protected sealed override bool Equals(T x, T y) => x.Equals(y);

    protected sealed override bool IsDefault(T value) => value.Equals(default);

    public sealed override bool IsNull(in int row) => false;
}
