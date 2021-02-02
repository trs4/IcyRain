using System;
using IcyRain.Internal;

namespace IcyRain.Data
{
    public class TestStructures
    {
        public bool Property1 { get; }
        public char Property2 { get; }
        public sbyte Property3 { get; }
        public byte Property4 { get; }
        public short Property5 { get; }
        public ushort Property6 { get; }
        public int Property7 { get; }
        public uint Property8 { get; }
        public long Property9 { get; }
        public ulong Property10 { get; }
        public float Property11 { get; }
        public double Property12 { get; }
        public decimal Property13 { get; }
        public string Property14 { get; }
        public Guid Property15 { get; }
        public DateTime Property16 { get; }
        public DateTimeOffset Property17 { get; }
        public TimeSpan Property18 { get; }
        public IntPtr Property19 { get; }
        public UIntPtr Property20 { get; }
        public ArraySegment<byte> Property21 { get; }
        public bool? Property22 { get; }
        public int? Property23 { get; }
        public Guid? Property24 { get; }

        public int Test()
        {
            int i = 0;

            if (Property1 != default)
                i++;

            if (Property2 != default)
                i++;

            if (Property3 != default)
                i++;

            if (Property4 != default)
                i++;

            if (Property5 != default)
                i++;

            if (Property6 != default)
                i++;

            if (Property7 != default)
                i++;

            if (Property8 != default)
                i++;

            if (Property9 != default)
                i++;

            if (Property10 != default)
                i++;

            if (Property11 != default)
                i++;

            if (Property12 != default)
                i++;

            if (Property13 != default)
                i++;

            if (Property14 != default)
                i++;

            if (!Property15.IsEmptyEqutableStruct())
                i++;

            if (!Property16.IsEmptyEqutableStruct())
                i++;

            if (!Property17.IsEmptyEqutableStruct())
                i++;

            if (!Property18.IsEmptyEqutableStruct())
                i++;

            if (Property19 != IntPtr.Zero)
                i++;

            if (Property20 != UIntPtr.Zero)
                i++;

            if (Property21.Count > 0)
                i++;

            if (Property22.HasValue)
                i++;

            if (Property23.HasValue)
                i++;

            if (Property24.HasValue)
                i++;

            return i;
        }

    }
}
