using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Compression.LZ4.Internal
{
    /// <summary>Unsafe memory operations</summary>
    internal unsafe class Mem32 : Mem
    {
        /// <summary>Reads 4 bytes from given address</summary>
        /// <param name="p">Address</param>
        /// <returns>4 bytes at given address</returns>
        [MethodImpl(Flags.HotPath)]
        public static uint PeekW(void* p) => Peek4(p);

        /// <summary>Writes 4 or 8 bytes to given address</summary>
        /// <param name="p">Address</param>
        /// <param name="v">Value</param>
        [MethodImpl(Flags.HotPath)]
        public static void PokeW(void* p, uint v) => Poke4(p, v);

        /// <summary>Copies exactly 16 bytes from source to target</summary>
        /// <param name="target">Target address</param>
        /// <param name="source">Source address</param>
        [MethodImpl(Flags.HotPath)]
        public static void Copy16(byte* target, byte* source)
        {
            Copy8(target + 0, source + 0);
            Copy8(target + 8, source + 8);
        }

        /// <summary>Copies exactly 18 bytes from source to target</summary>
        /// <param name="target">Target address</param>
        /// <param name="source">Source address</param>
        [MethodImpl(Flags.HotPath)]
        public static void Copy18(byte* target, byte* source)
        {
            Copy8(target + 0, source + 0);
            Copy8(target + 8, source + 8);
            Copy2(target + 16, source + 16);
        }

        /// <summary>
        /// Copies memory block for <paramref name="source"/> to <paramref name="target"/>
        /// up to (around) <paramref name="limit"/>
        /// It does not handle overlapping blocks and may copy up to 8 bytes more than expected
        /// </summary>
        /// <param name="target">The target block address</param>
        /// <param name="source">The source block address</param>
        /// <param name="limit">The limit (in target block)</param>
        [MethodImpl(Flags.HotPath)]
        public static void WildCopy8(byte* target, byte* source, void* limit)
        {
            do
            {
                Copy8(target, source);
                target += sizeof(ulong);
                source += sizeof(ulong);
            }
            while (target < limit);
        }

        /// <summary>
        /// Copies memory block for <paramref name="source"/> to <paramref name="target"/>
        /// up to (around) <paramref name="limit"/>
        /// It does not handle overlapping blocks and may copy up to 32 bytes more than expected
        /// This version copies two times 16 bytes (instead of one time 32 bytes)
        /// because it must be compatible with offsets >= 16
        /// </summary>
        /// <param name="target">The target block address</param>
        /// <param name="source">The source block address</param>
        /// <param name="limit">The limit (in target block)</param>
        [MethodImpl(Flags.HotPath)]
        public static void WildCopy32(byte* target, byte* source, void* limit)
        {
            do
            {
                Copy16(target + 0, source + 0);
                Copy16(target + 16, source + 16);
                target += 32;
                source += 32;
            }
            while (target < limit);
        }

    }
}
