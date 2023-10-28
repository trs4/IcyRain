using System.Runtime.CompilerServices;

namespace IcyRain.Internal;

internal static class BlockBuilder
{
    [MethodImpl(Flags.HotPath)]
    public static unsafe void NoCompression(byte* destination, byte* source, ref int length)
    {
        *destination = 0;
        Unsafe.CopyBlock(destination + 1, source, (uint)length);
        length++;
    }

}
