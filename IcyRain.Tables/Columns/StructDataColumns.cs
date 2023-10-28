using System;
using System.Runtime.Serialization;

namespace IcyRain.Tables;

[DataContract]
public class BooleanDataColumn : StructDataColumn<bool>
{
    public BooleanDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Boolean;
}

[DataContract]
public class NullableBooleanDataColumn : NullableDataColumn<bool>
{
    public NullableBooleanDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Boolean;
}

[DataContract]
public class CharDataColumn : StructDataColumn<char>
{
    public CharDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Char;
}

[DataContract]
public class NullableCharDataColumn : NullableDataColumn<char>
{
    public NullableCharDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Char;
}

[DataContract]
public class SByteDataColumn : StructDataColumn<sbyte>
{
    public SByteDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.SByte;
}

[DataContract]
public class NullableSByteDataColumn : NullableDataColumn<sbyte>
{
    public NullableSByteDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.SByte;
}

[DataContract]
public class ByteDataColumn : StructDataColumn<byte>
{
    public ByteDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Byte;
}

[DataContract]
public class NullableByteDataColumn : NullableDataColumn<byte>
{
    public NullableByteDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Byte;
}

[DataContract]
public class Int16DataColumn : StructDataColumn<short>
{
    public Int16DataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Int16;
}

[DataContract]
public class NullableInt16DataColumn : NullableDataColumn<short>
{
    public NullableInt16DataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Int16;
}

[DataContract]
public class UInt16DataColumn : StructDataColumn<ushort>
{
    public UInt16DataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.UInt16;
}

[DataContract]
public class NullableUInt16DataColumn : NullableDataColumn<ushort>
{
    public NullableUInt16DataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.UInt16;
}

[DataContract]
public class Int32DataColumn : StructDataColumn<int>
{
    public Int32DataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Int32;
}

[DataContract]
public class NullableInt32DataColumn : NullableDataColumn<int>
{
    public NullableInt32DataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Int32;
}

[DataContract]
public class UInt32DataColumn : StructDataColumn<uint>
{
    public UInt32DataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.UInt32;
}

[DataContract]
public class NullableUInt32DataColumn : NullableDataColumn<uint>
{
    public NullableUInt32DataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.UInt32;
}

[DataContract]
public class Int64DataColumn : StructDataColumn<long>
{
    public Int64DataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Int64;
}

[DataContract]
public class NullableInt64DataColumn : NullableDataColumn<long>
{
    public NullableInt64DataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Int64;
}

[DataContract]
public class UInt64DataColumn : StructDataColumn<ulong>
{
    public UInt64DataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.UInt64;
}

[DataContract]
public class NullableUInt64DataColumn : NullableDataColumn<ulong>
{
    public NullableUInt64DataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.UInt64;
}

[DataContract]
public class SingleDataColumn : StructDataColumn<float>
{
    public SingleDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Single;
}

[DataContract]
public class NullableSingleDataColumn : NullableDataColumn<float>
{
    public NullableSingleDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Single;
}

[DataContract]
public class DoubleDataColumn : StructDataColumn<double>
{
    public DoubleDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Double;
}

[DataContract]
public class NullableDoubleDataColumn : NullableDataColumn<double>
{
    public NullableDoubleDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Double;
}

[DataContract]
public class DecimalDataColumn : StructDataColumn<decimal>
{
    public DecimalDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Decimal;
}

[DataContract]
public class NullableDecimalDataColumn : NullableDataColumn<decimal>
{
    public NullableDecimalDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Decimal;
}

[DataContract]
public class GuidDataColumn : StructDataColumn<Guid>
{
    public GuidDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Guid;
}

[DataContract]
public class NullableGuidDataColumn : NullableDataColumn<Guid>
{
    public NullableGuidDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.Guid;
}

[DataContract]
public class DateTimeDataColumn : StructDataColumn<DateTime>
{
    public DateTimeDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.DateTime;
}

[DataContract]
public class NullableDateTimeDataColumn : NullableDataColumn<DateTime>
{
    public NullableDateTimeDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.DateTime;
}

[DataContract]
public class TimeSpanDataColumn : StructDataColumn<TimeSpan>
{
    public TimeSpanDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.TimeSpan;
}

[DataContract]
public class NullableTimeSpanDataColumn : NullableDataColumn<TimeSpan>
{
    public NullableTimeSpanDataColumn(int capacity) : base(capacity) { }

    public override DataType Type => DataType.TimeSpan;
}

