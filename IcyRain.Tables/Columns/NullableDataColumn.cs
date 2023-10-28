using System.Runtime.Serialization;

namespace IcyRain.Tables;

[DataContract]
public abstract class NullableDataColumn<T> : SingleDataColumn<T?>
    where T : struct
{
    protected NullableDataColumn(int capacity) : base(capacity) { }

    public sealed override bool IsNullable => true;

    protected sealed override bool IsDefault(T? value) => !value.HasValue;

    protected sealed override bool Equals(T? x, T? y) => (x.HasValue && y.HasValue) ? x.Value.Equals(y.Value) : !x.HasValue && !y.HasValue;
}
