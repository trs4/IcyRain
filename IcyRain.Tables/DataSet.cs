using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

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

}