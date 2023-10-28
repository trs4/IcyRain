using System.Runtime.Serialization;

namespace IcyRain.Tables;

[DataContract]
public abstract class ArrayDataColumn<T> : DataColumn<T[]>
{
    protected ArrayDataColumn(int capacity) : base(capacity) { }

    public sealed override bool IsArray => true;

    internal sealed override void CleanBottom() => CleanBottom(Values, SetRows, null, Equals);

    protected sealed override bool IsDefault(T[] value) => value is null;
}
