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
}
