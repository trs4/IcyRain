using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace IcyRain.Tables;

[DataContract, Serializable]
public class DataSet : Dictionary<string, DataTable>
{
    public DataSet(int capacity = 4) : base(capacity) { }

    protected DataSet(SerializationInfo serializationInfo, StreamingContext streamingContext) { }

    public DataTable AddTable(string name, int capacity = 4)
    {
        var table = new DataTable(capacity);
        Add(name, table);
        return table;
    }

    public void Compress()
    {
        foreach (var table in Values.Where(t => t is not null))
            table.Compress();
    }

    public string GetView()
    {
        if (Count == 0)
            return null;

        if (Count == 1)
            return Values.First()?.GetView();

        var builder = new StringBuilder(2048);

        foreach (var pair in this)
        {
            if (pair.Value is null)
                continue;

            if (pair.Key.Length > 0)
                builder.Append(pair.Key).AppendLine();

            pair.Value.BuildView(builder);
            builder.AppendLine().AppendLine();
        }

        return builder.ToString();
    }

}