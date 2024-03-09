using System;
using System.Runtime.Serialization;

namespace IcyRain.Tables;

[DataContract]
public sealed class ByteArrayDataColumn : ArrayDataColumn<byte>
{
    public ByteArrayDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Byte;

    public sealed override int GetInt(in int row) => throw new NotSupportedException();
    
    public sealed override string GetString(in int row) => "byte[]";
}
