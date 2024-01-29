using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IcyRain.Tables;

[DataContract]
public sealed class StringDataColumn : SingleDataColumn<string>
{
    public StringDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.String;

    public override string Fallback
    {
        get => base.Fallback;
        set => base.Fallback = string.IsNullOrEmpty(value) ? null : value;
    }

    public sealed override string GetString(in int row) => base.Get(row) ?? string.Empty;

    public sealed override string Get(in int row) => base.Get(row) ?? string.Empty;

    public sealed override void Set(in int row, in string value) => SetWithCheck(row, value, Values, Fallback, IsDefault);

    public sealed override void SetObject(in int row, object value) => SetWithCheck(row, (string)value, Values, Fallback, IsDefault);

    protected sealed override bool Equals(string x, string y) => x == y;

    protected sealed override bool IsDefault(string value) => string.IsNullOrEmpty(value);

    public sealed override bool IsNull(in int row) => false;

    public sealed override List<string> GetValues(int count)
    {
        if (count < 0)
            throw new ArgumentNullException(nameof(count));

        if (Values is null)
            return GetNullValues(count);

        int valuesCount = Values.Count;
        var values = new List<string>(count);

        if (count > valuesCount)
        {
            for (int i = 0; i < valuesCount; i++)
                values.Add(Values[i] ?? string.Empty);

            for (int i = valuesCount; i < count; i++)
                values.Add(base.Fallback ?? string.Empty);
        }
        else
        {
            for (int i = 0; i < count; i++)
                values.Add(Values[i] ?? string.Empty);
        }

        return values;
    }

    private List<string> GetNullValues(int count)
    {
        var values = new List<string>(count);

        for (int i = 0; i < count; i++)
            values.Add(base.Fallback ?? string.Empty);

        return values;
    }

}
