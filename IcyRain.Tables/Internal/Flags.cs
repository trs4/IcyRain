using System.Runtime.CompilerServices;

namespace IcyRain.Tables.Internal;

internal static class Flags
{
#if NETFRAMEWORK
    public const MethodImplOptions HotPath = MethodImplOptions.AggressiveInlining;
#else
    public const MethodImplOptions HotPath = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;
#endif
}
