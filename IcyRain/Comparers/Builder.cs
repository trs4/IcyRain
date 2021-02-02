using System;
using System.Collections.Generic;

namespace IcyRain.Comparers
{
    internal static class Builder
    {
        public static object Get<T>()
            => _primitiveMap.TryGetValue(typeof(T), out object comparer) ? comparer : null;

        private static readonly Dictionary<Type, object> _primitiveMap = new Dictionary<Type, object>
        {
            { typeof(bool), BoolEqualityComparer.Instance },
            { typeof(char), CharEqualityComparer.Instance },
            { typeof(sbyte), SByteEqualityComparer.Instance },
            { typeof(byte), ByteEqualityComparer.Instance },
            { typeof(short), ShortEqualityComparer.Instance },
            { typeof(ushort), UShortEqualityComparer.Instance },
            { typeof(int), IntEqualityComparer.Instance },
            { typeof(uint), UIntEqualityComparer.Instance },
            { typeof(long), LongEqualityComparer.Instance },
            { typeof(ulong), ULongEqualityComparer.Instance },
            { typeof(float), FloatEqualityComparer.Instance },
            { typeof(double), DoubleEqualityComparer.Instance },
            { typeof(decimal), DecimalEqualityComparer.Instance },
            { typeof(string), StringEqualityComparer.Instance },
            { typeof(Guid), GuidEqualityComparer.Instance },
            { typeof(DateTime), DateTimeEqualityComparer.Instance },
            { typeof(DateTimeOffset), DateTimeOffsetEqualityComparer.Instance },
            { typeof(TimeSpan), TimeSpanEqualityComparer.Instance },
        };

    }
}
