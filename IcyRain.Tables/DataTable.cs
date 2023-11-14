using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace IcyRain.Tables;

[DataContract, Serializable]
public class DataTable : Dictionary<string, DataColumn>
{
    private int _rowCapacity = 4;
    private Dictionary<string, object> _tags;

    public DataTable(int capacity = 4) : base(capacity) { }

    protected DataTable(SerializationInfo serializationInfo, StreamingContext streamingContext) { }

    [DataMember(Order = 1, EmitDefaultValue = false)]
    public virtual int RowCount { get; set; }

    public int RowCapacity
    {
        get => Math.Max(RowCount, _rowCapacity);
        set => _rowCapacity = Math.Max(0, value);
    }

    public Dictionary<string, object> Tags => _tags ??= new();

    public BooleanDataColumn AddBooleanColumn(string name)
        => AddColumn(name, new BooleanDataColumn(_rowCapacity));

    public NullableBooleanDataColumn AddNullableBooleanColumn(string name)
        => AddColumn(name, new NullableBooleanDataColumn(_rowCapacity));

    public CharDataColumn AddCharColumn(string name)
        => AddColumn(name, new CharDataColumn(_rowCapacity));

    public NullableCharDataColumn AddNullableCharColumn(string name)
        => AddColumn(name, new NullableCharDataColumn(_rowCapacity));

    public SByteDataColumn AddSByteColumn(string name)
        => AddColumn(name, new SByteDataColumn(_rowCapacity));

    public NullableSByteDataColumn AddNullableSByteColumn(string name)
        => AddColumn(name, new NullableSByteDataColumn(_rowCapacity));

    public ByteDataColumn AddByteColumn(string name)
        => AddColumn(name, new ByteDataColumn(_rowCapacity));

    public NullableByteDataColumn AddNullableByteColumn(string name)
        => AddColumn(name, new NullableByteDataColumn(_rowCapacity));

    public Int16DataColumn AddInt16Column(string name)
        => AddColumn(name, new Int16DataColumn(_rowCapacity));

    public NullableInt16DataColumn AddNullableInt16Column(string name)
        => AddColumn(name, new NullableInt16DataColumn(_rowCapacity));

    public UInt16DataColumn AddUInt16Column(string name)
        => AddColumn(name, new UInt16DataColumn(_rowCapacity));

    public NullableUInt16DataColumn AddNullableUInt16Column(string name)
        => AddColumn(name, new NullableUInt16DataColumn(_rowCapacity));

    public Int32DataColumn AddInt32Column(string name)
        => AddColumn(name, new Int32DataColumn(_rowCapacity));

    public NullableInt32DataColumn AddNullableInt32Column(string name)
        => AddColumn(name, new NullableInt32DataColumn(_rowCapacity));

    public UInt32DataColumn AddUInt32Column(string name)
        => AddColumn(name, new UInt32DataColumn(_rowCapacity));

    public NullableUInt32DataColumn AddNullableUInt32Column(string name)
        => AddColumn(name, new NullableUInt32DataColumn(_rowCapacity));

    public Int64DataColumn AddInt64Column(string name)
        => AddColumn(name, new Int64DataColumn(_rowCapacity));

    public NullableInt64DataColumn AddNullableInt64Column(string name)
        => AddColumn(name, new NullableInt64DataColumn(_rowCapacity));

    public UInt64DataColumn AddUInt64Column(string name)
        => AddColumn(name, new UInt64DataColumn(_rowCapacity));

    public NullableUInt64DataColumn AddNullableUInt64Column(string name)
        => AddColumn(name, new NullableUInt64DataColumn(_rowCapacity));

    public SingleDataColumn AddSingleColumn(string name)
        => AddColumn(name, new SingleDataColumn(_rowCapacity));

    public NullableSingleDataColumn AddNullableSingleColumn(string name)
        => AddColumn(name, new NullableSingleDataColumn(_rowCapacity));

    public DoubleDataColumn AddDoubleColumn(string name)
        => AddColumn(name, new DoubleDataColumn(_rowCapacity));

    public NullableDoubleDataColumn AddNullableDoubleColumn(string name)
        => AddColumn(name, new NullableDoubleDataColumn(_rowCapacity));

    public DecimalDataColumn AddDecimalColumn(string name)
        => AddColumn(name, new DecimalDataColumn(_rowCapacity));

    public NullableDecimalDataColumn AddNullableDecimalColumn(string name)
        => AddColumn(name, new NullableDecimalDataColumn(_rowCapacity));

    public StringDataColumn AddStringColumn(string name)
        => AddColumn(name, new StringDataColumn(_rowCapacity));

    public NullableStringDataColumn AddNullableStringColumn(string name)
        => AddColumn(name, new NullableStringDataColumn(_rowCapacity));

    public DateTimeDataColumn AddDateTimeColumn(string name)
        => AddColumn(name, new DateTimeDataColumn(_rowCapacity));

    public NullableDateTimeDataColumn AddNullableDateTimeColumn(string name)
        => AddColumn(name, new NullableDateTimeDataColumn(_rowCapacity));

    public TimeSpanDataColumn AddTimeSpanColumn(string name)
        => AddColumn(name, new TimeSpanDataColumn(_rowCapacity));

    public NullableTimeSpanDataColumn AddNullableTimeSpanColumn(string name)
        => AddColumn(name, new NullableTimeSpanDataColumn(_rowCapacity));

    public GuidDataColumn AddGuidColumn(string name)
        => AddColumn(name, new GuidDataColumn(_rowCapacity));

    public NullableGuidDataColumn AddNullableGuidColumn(string name)
        => AddColumn(name, new NullableGuidDataColumn(_rowCapacity));

    public ByteArrayDataColumn AddByteArrayColumn(string name)
        => AddColumn(name, new ByteArrayDataColumn(_rowCapacity));

    private TColumn AddColumn<TColumn>(string name, TColumn column)
        where TColumn : DataColumn
    {
        Add(name, column);
        return column;
    }

    public void Compress()
    {
        if (RowCount == 0)
        {
            foreach (var column in Values.Where(c => c is not null))
                column.SetNullRowsIfEmpty();
        }
        else
        {
            foreach (var column in Values.Where(c => c is not null))
                column.Compress(RowCount);
        }
    }

    public string GetView()
    {
        if (Count == 0)
            return null;

        var builder = new StringBuilder(2048);
        BuildView(builder);
        return builder.ToString();
    }

    internal void BuildView(StringBuilder builder)
    {
        int count = Count;

        if (count == 0)
            return;

        var columns = new string[count][];
        int[] pads = new int[count];
        int index = 0;
        int rowSize = RowCount + 1;

        foreach (var pair in this)
        {
            var column = new string[rowSize];
            column[0] = pair.Key;
            columns[index] = column;
            int padSize = pair.Key.Length;

            if (pair.Value is not null)
            {
                for (int row = 0; row < RowCount; row++)
                {
                    string cell = pair.Value.GetString(row);

                    if (cell is null)
                        cell = "NULL";
                    else if (pair.Value.Type == DataType.String)
                    {
                        if (cell.Contains('\n'))
                        {
                            cell = cell.Replace(Environment.NewLine, " ");

                            if (cell.Contains('\n'))
                                cell = cell.Replace('\n', ' ');
                        }
                    }

                    column[row + 1] = cell;
                    padSize = Math.Max(padSize, cell.Length);
                }
            }

            pads[index] = padSize + 2;
            index++;
        }

        for (int i = 0; i < rowSize; i++)
        {
            for (int j = 0; j < count; j++)
            {
                string cell = columns[i][j];
                int padSize = pads[j];
                builder.Append(cell);

                for (int k = cell.Length; k < padSize; k++)
                    builder.Append(' ');
            }

            builder.AppendLine();
        }
    }

}
