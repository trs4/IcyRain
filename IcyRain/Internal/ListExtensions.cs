using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IcyRain.Internal
{
    internal static class ListExtensions
    {
        [MethodImpl(Flags.HotPath)]
        public static T[] GetArray<T>(this List<T> list) => (T[])new ListStruct { List = list }.ListAccessor.Items;

        [MethodImpl(Flags.HotPath)]
        public static bool TryGetArray<T>(this ReadOnlyCollection<T> collection, out T[] array, out IList<T> list, out int count)
        {
            list = (IList<T>)new ListStruct { List = collection }.ListAccessor.Items;
            array = list as T[];

            if (array is not null)
            {
                count = array.Length;
                return true;
            }
            else if (list is List<T> nativeList)
            {
                array = nativeList.GetArray();
                count = nativeList.Count;
                return true;
            }

            count = list.Count;
            return false;
        }

        [MethodImpl(Flags.HotPath)]
        public static bool TryGetArray<T>(this IEnumerable<T> enumerable, out T[] array)
        {
            array = enumerable as T[];

            if (array is null)
                array = (enumerable as List<T>)?.GetArray();

            return array is not null;
        }

        [MethodImpl(Flags.HotPath)]
        public static bool TryGetArray<T>(this IEnumerable<T> enumerable, out T[] array, out int count)
        {
            array = enumerable as T[];

            if (array is not null)
            {
                count = array.Length;
                return true;
            }
            else if (enumerable is List<T> list)
            {
                array = list.GetArray();
                count = list.Count;
                return true;
            }

            count = 0;
            return false;
        }

        [MethodImpl(Flags.HotPath)]
        public static int CalculateLength<T>(this IEnumerable<T> enumerable)
        {
            using var enumerator = enumerable.GetEnumerator();
            int length = 0;

            while (enumerator.MoveNext())
                length++;

            return length;
        }

        [MethodImpl(Flags.HotPath)]
        public static List<T> CreateList<T>(this T[] array)
#if NETFRAMEWORK
            => new List<T>(array);
#else
        {
            var list = new List<T>();
            var wrapper = new ListStruct { List = list };
            wrapper.ListAccessor.Items = array;
            wrapper.ListAccessor.Size = array.Length;
            return list;
        }
#endif

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
}
