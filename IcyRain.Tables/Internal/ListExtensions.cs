using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IcyRain.Tables.Internal;

internal static class ListExtensions
{
    [MethodImpl(Flags.HotPath)]
    public static T[] GetArray<T>(this List<T> list) => (T[])new ListStruct { List = list }.ListAccessor.Items;

    [StructLayout(LayoutKind.Explicit)]
    private struct ListStruct
    {
        [FieldOffset(0)]
        public object List;

        [FieldOffset(0)]
        public ListWrapper ListAccessor;
    }

    public class ListWrapper
    {
#if NETFRAMEWORK
        public int DefaultCapacity;
#endif
        public object Items;
        public int Size;
    }

}
