using System.Reflection;
using System.Runtime.CompilerServices;

namespace IcyRain.Internal
{
    internal static class Flags
    {
#if NETFRAMEWORK
        public const MethodImplOptions HotPath = MethodImplOptions.AggressiveInlining;
#else
        public const MethodImplOptions HotPath = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;
#endif

        public const BindingFlags StaticPublicBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>Public Sealed BeforeFieldInit AutoClass AnsiClass</summary>
        public const TypeAttributes Type = TypeAttributes.Public | TypeAttributes.Sealed
            | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoClass | TypeAttributes.AnsiClass;

        /// <summary>Public HideBySig</summary>
        public const MethodAttributes Constructor = MethodAttributes.Public | MethodAttributes.HideBySig;

        /// <summary>Private InitOnly</summary>
        public const FieldAttributes PrivateReadOnlyField = FieldAttributes.Private | FieldAttributes.InitOnly;

        /// <summary>Public HideBySig Virtual Final</summary>
        public const MethodAttributes PublicOverrideMethod = MethodAttributes.Public | MethodAttributes.HideBySig
            | MethodAttributes.Virtual | MethodAttributes.Final;
    }
}
