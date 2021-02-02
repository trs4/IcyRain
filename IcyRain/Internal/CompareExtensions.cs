using System;
using System.Runtime.CompilerServices;

namespace IcyRain.Internal
{
    public static class CompareExtensions
    {
        [MethodImpl(Flags.HotPath)]
        public static bool IsEmptyStruct<T>(this T value) where T : struct
            => Equals(value, default(T));

        [MethodImpl(Flags.HotPath)]
        public static bool IsEmptyEqutableStruct<T>(this T value) where T : struct, IEquatable<T>
            => value.Equals(default);
    }
}
