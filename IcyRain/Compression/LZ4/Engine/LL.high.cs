using System.Runtime.CompilerServices;
using IcyRain.Compression.LZ4.Internal;
using IcyRain.Internal;

namespace IcyRain.Compression.LZ4.Engine;

internal unsafe partial class LL
{
    public static void LZ4_setCompressionLevel(
        LZ4_streamHC_t* LZ4_streamHCPtr, int compressionLevel)
    {
        if (compressionLevel < 1) compressionLevel = LZ4HC_CLEVEL_DEFAULT;
        if (compressionLevel > LZ4HC_CLEVEL_MAX) compressionLevel = LZ4HC_CLEVEL_MAX;
        LZ4_streamHCPtr->compressionLevel = (short)compressionLevel;
    }

    public static LZ4_streamHC_t* LZ4_initStreamHC(void* buffer, int size)
    {
        LZ4_streamHC_t* LZ4_streamHCPtr = (LZ4_streamHC_t*)buffer;
        if (buffer is null) return null;
        if (size < sizeof(LZ4_streamHC_t)) return null;

        LZ4_streamHCPtr->end = (byte*)-1;
        LZ4_streamHCPtr->@base = null;
        LZ4_streamHCPtr->dictCtx = null;
        LZ4_streamHCPtr->favorDecSpeed = false;
        LZ4_streamHCPtr->dirty = false;
        LZ4_setCompressionLevel(LZ4_streamHCPtr, LZ4HC_CLEVEL_DEFAULT);
        return LZ4_streamHCPtr;
    }

    public static LZ4_streamHC_t* LZ4_initStreamHC(LZ4_streamHC_t* stream) =>
        LZ4_initStreamHC(stream, sizeof(LZ4_streamHC_t));

    public static void LZ4_resetStreamHC_fast(
        LZ4_streamHC_t* LZ4_streamHCPtr, int compressionLevel)
    {
        if (LZ4_streamHCPtr->dirty)
        {
            LZ4_initStreamHC(LZ4_streamHCPtr);
        }
        else
        {
            LZ4_streamHCPtr->end -= LZ4_streamHCPtr->@base;
            LZ4_streamHCPtr->@base = null;
            LZ4_streamHCPtr->dictCtx = null;
        }

        LZ4_setCompressionLevel(LZ4_streamHCPtr, compressionLevel);
    }

    [MethodImpl(Flags.HotPath)]
    private static uint HASH_FUNCTION(uint value) =>
        (value * 2654435761U) >> (MINMATCH * 8 - LZ4HC_HASH_LOG);

    [MethodImpl(Flags.HotPath)]
    protected static ref ushort DELTANEXTU16(ushort* table, uint pos) =>
        ref table[(ushort)(pos)];

    [MethodImpl(Flags.HotPath)]
    public static uint LZ4HC_hashPtr(void* ptr) => HASH_FUNCTION(Mem.Peek4(ptr));

    [MethodImpl(Flags.HotPath)]
    public static void LZ4HC_Insert(LZ4_streamHC_t* hc4, byte* ip)
    {
        ushort* chainTable = hc4->chainTable;
        uint* hashTable = hc4->hashTable;
        byte* @base = hc4->@base;
        uint target = (uint)(ip - @base);
        uint idx = hc4->nextToUpdate;

        while (idx < target)
        {
            uint h = LZ4HC_hashPtr(@base + idx);
            uint delta = idx - hashTable[h];
            if (delta > LZ4_DISTANCE_MAX) delta = LZ4_DISTANCE_MAX;
            DELTANEXTU16(chainTable, idx) = (ushort)delta;
            hashTable[h] = idx;
            idx++;
        }

        hc4->nextToUpdate = target;
    }

    public static void LZ4HC_setExternalDict(LZ4_streamHC_t* ctxPtr, byte* newBlock)
    {
        if (ctxPtr->end >= ctxPtr->@base + ctxPtr->dictLimit + 4)
            LZ4HC_Insert(
                ctxPtr, ctxPtr->end - 3); /* Referencing remaining dictionary content */

        /* Only one memory segment for extDict, so any previous extDict is lost at this stage */
        ctxPtr->lowLimit = ctxPtr->dictLimit;
        ctxPtr->dictLimit = (uint)(ctxPtr->end - ctxPtr->@base);
        ctxPtr->dictBase = ctxPtr->@base;
        ctxPtr->@base = newBlock - ctxPtr->dictLimit;
        ctxPtr->end = newBlock;
        ctxPtr->nextToUpdate = ctxPtr->dictLimit; /* match referencing will resume from there */

        /* cannot reference an extDict and a dictCtx at the same time */
        ctxPtr->dictCtx = null;
    }

    public static void LZ4HC_clearTables(LZ4_streamHC_t* hc4)
    {
        // uint hashTable[LZ4HC_HASHTABLESIZE];
        // ushort chainTable[LZ4HC_MAXD];
        Mem.Fill((byte*)hc4->hashTable, 0, sizeof(uint) * LZ4HC_HASHTABLESIZE);
        Mem.Fill((byte*)hc4->chainTable, 0xFF, sizeof(ushort) * LZ4HC_MAXD);
    }

    public static void LZ4HC_init_internal(LZ4_streamHC_t* hc4, byte* start)
    {
        var startingOffset = (hc4->end - hc4->@base);
        if (startingOffset < 0 || startingOffset > 1 * GB)
        {
            LZ4HC_clearTables(hc4);
            startingOffset = 0;
        }

        startingOffset += 64 * KB;
        hc4->nextToUpdate = (uint)startingOffset;
        hc4->@base = start - startingOffset;
        hc4->end = start;
        hc4->dictBase = start - startingOffset;
        hc4->dictLimit = (uint)startingOffset;
        hc4->lowLimit = (uint)startingOffset;
    }

    public static int LZ4_loadDictHC(LZ4_streamHC_t* LZ4_streamHCPtr, byte* dictionary, int dictSize)
    {
        LZ4_streamHC_t* ctxPtr = LZ4_streamHCPtr;
#if DEBUG
        Assert(LZ4_streamHCPtr is not null);
#endif

        if (dictSize > 64 * KB)
        {
            dictionary += dictSize - 64 * KB;
            dictSize = 64 * KB;
        }

        /* need a full initialization, there are bad side-effects when using resetFast() */
        {
            int cLevel = ctxPtr->compressionLevel;
            LZ4_initStreamHC(LZ4_streamHCPtr);
            LZ4_setCompressionLevel(LZ4_streamHCPtr, cLevel);
        }
        LZ4HC_init_internal(ctxPtr, (byte*)dictionary);
        ctxPtr->end = (byte*)dictionary + dictSize;
        if (dictSize >= 4) LZ4HC_Insert(ctxPtr, ctxPtr->end - 3);
        return dictSize;
    }

    [MethodImpl(Flags.HotPath)]
    public static uint LZ4HC_rotl32(uint x, int r) => ((x << r) | (x >> (32 - r)));

    [MethodImpl(Flags.HotPath)]
    public static bool LZ4HC_protectDictEnd(uint dictLimit, uint matchIndex) =>
        ((dictLimit - 1) - matchIndex >= 3);

    [MethodImpl(Flags.HotPath)]
    public static int LZ4HC_countBack(byte* ip, byte* match, byte* iMin, byte* mMin)
    {
        int back = 0;
        int min = (int)MAX(iMin - ip, mMin - match);
#if DEBUG
        Assert(min <= 0);
        Assert(ip >= iMin);
        Assert((uint)(ip - iMin) < (1U << 31));
        Assert(match >= mMin);
        Assert((uint)(match - mMin) < (1U << 31));
#endif
        while ((back > min) && (ip[back - 1] == match[back - 1]))
            back--;

        return back;
    }

    [MethodImpl(Flags.HotPath)]
    public static uint LZ4HC_reverseCountPattern(byte* ip, byte* iLow, uint pattern)
    {
        byte* iStart = ip;

        while ((ip >= iLow + 4))
        {
            if (Mem.Peek4(ip - 4) != pattern)
                break;

            ip -= 4;
        }

        {
            byte* bytePtr = (byte*)(&pattern) + 3; /* works for any endianess */
            while ((ip > iLow))
            {
                if (ip[-1] != *bytePtr) break;

                ip--;
                bytePtr--;
            }
        }
        return (uint)(iStart - ip);
    }

    [MethodImpl(Flags.HotPath)]
    public static uint LZ4HC_rotatePattern(uint rotate, uint pattern)
    {
        uint bitsToRotate = (rotate & (sizeof(uint) - 1)) << 3;

        if (bitsToRotate == 0)
            return pattern;

        return LZ4HC_rotl32(pattern, (int)bitsToRotate);
    }

    [MethodImpl(Flags.HotPath)]
    public static int LZ4HC_literalsPrice(int litlen)
    {
        int price = litlen;
#if DEBUG
        Assert(litlen >= 0);
#endif
        if (litlen >= (int)RUN_MASK)
            price += 1 + ((litlen - (int)RUN_MASK) / 255);

        return price;
    }

    [MethodImpl(Flags.HotPath)]
    public static int LZ4HC_sequencePrice(int litlen, int mlen)
    {
        int price = 1 + 2; /* token + 16-bit offset */
#if DEBUG
        Assert(litlen >= 0);
        Assert(mlen >= MINMATCH);
#endif

        price += LZ4HC_literalsPrice(litlen);

        if (mlen >= (int)(ML_MASK + MINMATCH))
            price += 1 + ((mlen - (int)(ML_MASK + MINMATCH)) / 255);

        return price;
    }

}
