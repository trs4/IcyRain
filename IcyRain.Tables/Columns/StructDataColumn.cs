using System;
using System.Runtime.Serialization;

namespace IcyRain.Tables;

[DataContract]
public abstract class StructDataColumn<T> : SingleDataColumn<T>
    where T : struct, IEquatable<T>
{
    protected StructDataColumn(int capacity) : base(capacity) { }

    protected sealed override bool Equals(T x, T y) => x.Equals(y);

    protected sealed override bool IsDefault(T value) => value.Equals(default);

    public sealed override bool IsNull(in int row) => false;
}
