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

    [MethodImpl(Flags.HotPath)]
    protected sealed override bool Equals(string x, string y) => x == y;

    [MethodImpl(Flags.HotPath)]
    protected sealed override bool IsDefault(string value) => value is null;
}
