using System.Runtime.CompilerServices;
using IcyRain.Internal;
using Mem = IcyRain.Compression.LZ4.Internal.Mem64;

namespace IcyRain.Compression.LZ4.Engine
{
    internal unsafe partial class LL64 : LL
    {
        protected const int ALGORITHM_ARCH = 8;

#if NET5_0_OR_GREATER

        [MethodImpl(Flags.HotPath)]
        protected static uint LZ4_NbCommonBytes(ulong val) =>
			((uint)System.Numerics.BitOperations.TrailingZeroCount(val) >> 3) & 0x07;

#else // NET5_0

        private static readonly uint[] _DeBruijnBytePos = {
            0, 0, 0, 0, 0, 1, 1, 2,
            0, 3, 1, 3, 1, 4, 2, 7,
            0, 2, 3, 6, 1, 5, 3, 5,
            1, 3, 4, 4, 2, 5, 6, 7,
            7, 0, 1, 2, 3, 3, 4, 6,
            2, 6, 5, 5, 3, 4, 5, 6,
            7, 1, 2, 4, 6, 4, 4, 5,
            7, 2, 6, 5, 7, 6, 7, 7,
        };

        private static readonly uint* DeBruijnBytePos = Mem.CloneArray(_DeBruijnBytePos);

        [MethodImpl(Flags.HotPath)]
        protected static uint LZ4_NbCommonBytes(ulong val)
            => DeBruijnBytePos[unchecked((ulong)((long)val & -(long)val) * 0x0218A392CDABBD3Ful >> 58)];

#endif // NET5_0

        [MethodImpl(Flags.HotPath)]
        protected static uint LZ4_count(byte* pIn, byte* pMatch, byte* pInLimit)
        {
            const int STEPSIZE = ALGORITHM_ARCH;

            var pStart = pIn;

            if (pIn < pInLimit - (STEPSIZE - 1))
            {
                var diff = Mem.PeekW(pMatch) ^ Mem.PeekW(pIn);
                if (diff != 0)
                    return LZ4_NbCommonBytes(diff);

                pIn += STEPSIZE;
                pMatch += STEPSIZE;
            }

            while (pIn < pInLimit - (STEPSIZE - 1))
            {
                var diff = Mem.PeekW(pMatch) ^ Mem.PeekW(pIn);
                if (diff != 0)
                    return (uint)(pIn + LZ4_NbCommonBytes(diff) - pStart);

                pIn += STEPSIZE;
                pMatch += STEPSIZE;
            }

#if !BIT32

            if (pIn < pInLimit - 3 && Mem.Peek4(pMatch) == Mem.Peek4(pIn))
            {
                pIn += 4;
                pMatch += 4;
            }

#endif

            if (pIn < pInLimit - 1 && Mem.Peek2(pMatch) == Mem.Peek2(pIn))
            {
                pIn += 2;
                pMatch += 2;
            }

            if (pIn < pInLimit && *pMatch == *pIn)
                pIn++;

            return (uint)(pIn - pStart);
        }

        [MethodImpl(Flags.HotPath)]
        protected static uint LZ4_hashPosition(void* p, tableType_t tableType)
        {
#if !BIT32
            if (tableType != tableType_t.byU16)
                return LZ4_hash5(Mem.PeekW(p), tableType);
#endif
            return LZ4_hash4(Mem.Peek4(p), tableType);
        }

        [MethodImpl(Flags.HotPath)]
        protected static void LZ4_putPosition(
            byte* p, void* tableBase, tableType_t tableType, byte* srcBase) =>
            LZ4_putPositionOnHash(p, LZ4_hashPosition(p, tableType), tableBase, tableType, srcBase);

        [MethodImpl(Flags.HotPath)]
        protected static byte* LZ4_getPosition(
            byte* p, void* tableBase, tableType_t tableType, byte* srcBase) =>
            LZ4_getPositionOnHash(LZ4_hashPosition(p, tableType), tableBase, tableType, srcBase);

    }
}
