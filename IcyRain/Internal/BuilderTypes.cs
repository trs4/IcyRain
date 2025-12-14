using System;
using System.Reflection;

namespace IcyRain.Internal;

public static class BuilderTypes
{
    public static readonly Type[] IntArray = [typeof(int)];

    internal static readonly FieldInfo IntArrayField = typeof(BuilderTypes).GetField(nameof(IntArray));
}
