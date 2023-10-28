using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4.Internal;
using IcyRain.Internal;

namespace IcyRain.Compression.LZ4.Engine;

internal static unsafe class LLxx
{
    [MethodImpl(Flags.HotPath)]
    public static int LZ4_decompress_safe(byte* source, byte* target, int sourceLength, int targetLength)
        => Mem.System32
            ? LL32.LZ4_decompress_safe(source, target, sourceLength, targetLength)
            : LL64.LZ4_decompress_safe(source, target, sourceLength, targetLength);

    [MethodImpl(Flags.HotPath)]
    public static int LZ4_compress_fast(byte* source, byte* target, int sourceLength, int targetLength)
        => Mem.System32
            ? LL32.LZ4_compress_fast(source, target, sourceLength, targetLength)
            : LL64.LZ4_compress_fast(source, target, sourceLength, targetLength);
}
