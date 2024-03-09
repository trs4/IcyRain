using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using IcyRain.Tables.Internal;

namespace IcyRain.Tables;

[DataContract]
public sealed class NullableStringDataColumn : SingleDataColumn<string>
{
    public NullableStringDataColumn(int capacity) : base(capacity) { }

    public sealed override DataType Type => DataType.String;

    public sealed override bool IsNullable => true;

    public sealed override int GetInt(in int row) => int.Parse(Get(row));

    public sealed override string GetString(in int row) => Get(row);

    [MethodImpl(Flags.HotPath)]
    protected sealed override bool Equals(string x, string y) => x == y;

    [MethodImpl(Flags.HotPath)]
    protected sealed override bool IsDefault(string value) => value is null;
}
