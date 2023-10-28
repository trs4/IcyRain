using System.Runtime.CompilerServices;
using IcyRain.Internal;
using Mem = IcyRain.Compression.LZ4.Internal.Mem32;
using size_t = System.UInt32;

namespace IcyRain.Compression.LZ4.Engine;

internal unsafe partial class LL32
{
    #region LZ4_compress_generic

    [MethodImpl(Flags.HotPath)]
    protected static int LZ4_compress_generic(
        LZ4_stream_t* cctx,
        byte* source,
        byte* dest,
        int inputSize,
        int* inputConsumed, /* only written when outputDirective == fillOutput */
        int maxOutputSize,
        limitedOutput_directive outputDirective,
        tableType_t tableType,
        dict_directive dictDirective,
        dictIssue_directive dictIssue,
        int acceleration = 1)
    {
        int result;
        byte* ip = source;

        uint startIndex = cctx->currentOffset;
        byte* @base = source - startIndex;
        byte* lowLimit;

        LZ4_stream_t* dictCtx = cctx->dictCtx;

        byte* dictionary = dictDirective == dict_directive.usingDictCtx ? dictCtx->dictionary : cctx->dictionary;
        uint dictSize = dictDirective == dict_directive.usingDictCtx ? dictCtx->dictSize : cctx->dictSize;
        uint dictDelta = (dictDirective == dict_directive.usingDictCtx) ? startIndex - dictCtx->currentOffset : 0;
        bool maybe_extMem = (dictDirective == dict_directive.usingExtDict) || (dictDirective == dict_directive.usingDictCtx);
        uint prefixIdxLimit = startIndex - dictSize;
        byte* dictEnd = dictionary + dictSize;
        byte* anchor = source;
        byte* iend = ip + inputSize;
        byte* mflimitPlusOne = iend - MFLIMIT + 1;
        byte* matchlimit = iend - LASTLITERALS;

        /* the dictCtx currentOffset is indexed on the start of the dictionary,
			 * while a dictionary in the current context precedes the currentOffset */
        byte* dictBase = (dictDirective == dict_directive.usingDictCtx) ?
            dictionary + dictSize - dictCtx->currentOffset :
            dictionary + dictSize - startIndex;

        byte* op = dest;
        byte* olimit = op + maxOutputSize;

        uint offset = 0;
        uint forwardH;

        if (outputDirective == limitedOutput_directive.fillOutput && maxOutputSize < 1)
            return 0;

        if ((uint)inputSize > LZ4_MAX_INPUT_SIZE)
            return 0;

        if ((tableType == tableType_t.byU16) && (inputSize >= LZ4_64Klimit))
            return 0;

#if DEBUG
        if (tableType == tableType_t.byPtr)
            Assert(dictDirective == dict_directive.noDict);

        Assert(acceleration >= 1);
#endif

        lowLimit = source - (dictDirective == dict_directive.withPrefix64k ? dictSize : 0);

        /* Update context state */
        if (dictDirective == dict_directive.usingDictCtx)
        {
            /* Subsequent linked blocks can't use the dictionary. */
            /* Instead, they use the block we just compressed. */
            cctx->dictCtx = null;
            cctx->dictSize = (uint)inputSize;
        }
        else
        {
            cctx->dictSize += (uint)inputSize;
        }

        cctx->currentOffset += (uint)inputSize;
        cctx->tableType = tableType;

        if (inputSize < LZ4_minLength)
            goto _last_literals;

        /* First Byte */
        LZ4_putPosition(ip, cctx->hashTable, tableType, @base);
        ip++;
        forwardH = LZ4_hashPosition(ip, tableType);

        /* Main Loop */
        for (; ; )
        {
            byte* match;
            byte* token;
            byte* filledIp;

            /* Find a match */
            if (tableType == tableType_t.byPtr)
            {
                byte* forwardIp = ip;
                int step = 1;
                int searchMatchNb = acceleration << LZ4_skipTrigger;
                do
                {
                    uint h = forwardH;
                    ip = forwardIp;
                    forwardIp += step;
                    step = (searchMatchNb++ >> LZ4_skipTrigger);

                    if (forwardIp > mflimitPlusOne)
                        goto _last_literals;

#if DEBUG
                    Assert(ip < mflimitPlusOne);
#endif

                    match = LZ4_getPositionOnHash(h, cctx->hashTable, tableType, @base);
                    forwardH = LZ4_hashPosition(forwardIp, tableType);
                    LZ4_putPositionOnHash(ip, h, cctx->hashTable, tableType, @base);
                }
                while ((match + LZ4_DISTANCE_MAX < ip) || (Internal.Mem.Peek4(match) != Internal.Mem.Peek4(ip)));
            }
            else
            {
                /* byU32, byU16 */

                byte* forwardIp = ip;
                int step = 1;
                int searchMatchNb = acceleration << LZ4_skipTrigger;
                do
                {
                    uint h = forwardH;
                    uint current = (uint)(forwardIp - @base);
                    uint matchIndex = LZ4_getIndexOnHash(h, cctx->hashTable, tableType);
#if DEBUG
                    Assert(matchIndex <= current);
                    Assert(forwardIp - @base < (2 * GB - 1));
#endif
                    ip = forwardIp;
                    forwardIp += step;
                    step = (searchMatchNb++ >> LZ4_skipTrigger);

                    if (forwardIp > mflimitPlusOne)
                        goto _last_literals;

#if DEBUG
                    Assert(ip < mflimitPlusOne);
#endif

                    if (dictDirective == dict_directive.usingDictCtx)
                    {
                        if (matchIndex < startIndex)
                        {
#if DEBUG
                            Assert(tableType == tableType_t.byU32);
#endif

                            matchIndex = LZ4_getIndexOnHash(
                                h, dictCtx->hashTable, tableType_t.byU32);

                            match = dictBase + matchIndex;
                            matchIndex += dictDelta;
                            lowLimit = dictionary;
                        }
                        else
                        {
                            match = @base + matchIndex;
                            lowLimit = source;
                        }
                    }
                    else if (dictDirective == dict_directive.usingExtDict)
                    {
                        if (matchIndex < startIndex)
                        {
#if DEBUG
                            Assert(startIndex - matchIndex >= MINMATCH);
#endif
                            match = dictBase + matchIndex;
                            lowLimit = dictionary;
                        }
                        else
                        {
                            match = @base + matchIndex;
                            lowLimit = source;
                        }
                    }
                    else
                    {
                        match = @base + matchIndex;
                    }

                    forwardH = LZ4_hashPosition(forwardIp, tableType);
                    LZ4_putIndexOnHash(current, h, cctx->hashTable, tableType);

                    if ((dictIssue == dictIssue_directive.dictSmall) && (matchIndex < prefixIdxLimit))
                        continue;

#if DEBUG
                    Assert(matchIndex < current);
#endif
                    if (((tableType != tableType_t.byU16)
                            || (LZ4_DISTANCE_MAX < LZ4_DISTANCE_ABSOLUTE_MAX))
                        && (matchIndex + LZ4_DISTANCE_MAX < current))
                    {
                        continue;
                    }

#if DEBUG
                    Assert((current - matchIndex) <= LZ4_DISTANCE_MAX);
#endif

                    if (Internal.Mem.Peek4(match) == Internal.Mem.Peek4(ip))
                    {
                        if (maybe_extMem)
                            offset = current - matchIndex;

                        break;
                    }
                }
                while (true);
            }

            filledIp = ip;
            while (((ip > anchor) & (match > lowLimit)) && ((ip[-1] == match[-1])))
            {
                ip--;
                match--;
            }

            {
                var litLength = (uint)(ip - anchor);
                token = op++;

                if ((outputDirective == limitedOutput_directive.limitedOutput) &&
                    ((op + litLength + (2 + 1 + LASTLITERALS) + (litLength / 255) > olimit)))
                {
                    return 0;
                }

                if ((outputDirective == limitedOutput_directive.fillOutput) &&
                    ((op + (litLength + 240) / 255 + litLength + 2 + 1 + MFLIMIT - MINMATCH
                        > olimit)))
                {
                    op--;
                    goto _last_literals;
                }

                if (litLength >= RUN_MASK)
                {
                    int len = (int)(litLength - RUN_MASK);
                    *token = (byte)(RUN_MASK << ML_BITS);
                    for (; len >= 255; len -= 255) *op++ = 255;
                    *op++ = (byte)len;
                }
                else
                    *token = (byte)(litLength << ML_BITS);

                Mem.WildCopy8(op, anchor, op + litLength);
                op += litLength;
            }

        _next_match:
            /* at this stage, the following variables must be correctly set :
				 * - ip : at start of LZ operation
				 * - match : at start of previous pattern occurence; can be within current prefix, or within extDict
				 * - offset : if maybe_ext_memSegment==1 (constant)
				 * - lowLimit : must be == dictionary to mean "match is within extDict"; must be == source otherwise
				 * - token and *token : position to write 4-bits for match length; higher 4-bits for literal length supposed already written
				 */

            if ((outputDirective == limitedOutput_directive.fillOutput) &&
                (op + 2 + 1 + MFLIMIT - MINMATCH > olimit))
            {
                /* the match was too close to the end, rewind and go to last literals */
                op = token;
                goto _last_literals;
            }

            /* Encode Offset */
            if (maybe_extMem)
            {
                /* static test */
#if DEBUG
                Assert(offset <= LZ4_DISTANCE_MAX && offset > 0);
#endif
                Internal.Mem.Poke2(op, (ushort)offset);
                op += 2;
            }
            else
            {
#if DEBUG
                Assert(ip - match <= LZ4_DISTANCE_MAX);
#endif
                Internal.Mem.Poke2(op, (ushort)(ip - match));
                op += 2;
            }

            /* Encode MatchLength */
            {
                uint matchCode;

                if ((dictDirective == dict_directive.usingExtDict
                        || dictDirective == dict_directive.usingDictCtx)
                    && (lowLimit == dictionary) /* match within extDict */)
                {
                    byte* limit = ip + (dictEnd - match);
#if DEBUG
                    Assert(dictEnd > match);
#endif
                    if (limit > matchlimit)
                        limit = matchlimit;

                    matchCode = LZ4_count(ip + MINMATCH, match + MINMATCH, limit);
                    ip += matchCode + MINMATCH;

                    if (ip == limit)
                    {
                        uint more = LZ4_count(limit, source, matchlimit);
                        matchCode += more;
                        ip += more;
                    }
                }
                else
                {
                    matchCode = LZ4_count(ip + MINMATCH, match + MINMATCH, matchlimit);
                    ip += matchCode + MINMATCH;
                }

                if ((outputDirective != 0)
                    && ((op + (1 + LASTLITERALS) + (matchCode + 240) / 255 > olimit)))
                {
                    if (outputDirective == limitedOutput_directive.fillOutput)
                    {
                        /* Match description too long : reduce it */
                        uint newMatchCode = 15 - 1 + ((uint)(olimit - op) - 1 - LASTLITERALS) * 255;
                        ip -= matchCode - newMatchCode;
#if DEBUG
                        Assert(newMatchCode < matchCode);
#endif
                        matchCode = newMatchCode;

                        if (ip <= filledIp)
                        {
                            /* We have already filled up to filledIp so if ip ends up less than filledIp
								 * we have positions in the hash table beyond the current position. This is
								 * a problem if we reuse the hash table. So we have to remove these positions
								 * from the hash table.
								 */
                            byte* ptr;
                            for (ptr = ip; ptr <= filledIp; ++ptr)
                            {
                                uint h = LZ4_hashPosition(ptr, tableType);
                                LZ4_clearHash(h, cctx->hashTable, tableType);
                            }
                        }
                    }
                    else
                    {
#if DEBUG
                        Assert(outputDirective == limitedOutput_directive.limitedOutput);
#endif
                        return 0;
                    }
                }

                if (matchCode >= ML_MASK)
                {
                    *token += (byte)ML_MASK; //!!!
                    matchCode -= ML_MASK;
                    Internal.Mem.Poke4(op, 0xFFFFFFFF);
                    while (matchCode >= 4 * 255)
                    {
                        op += 4;
                        Internal.Mem.Poke4(op, 0xFFFFFFFF);
                        matchCode -= 4 * 255;
                    }

                    op += matchCode / 255;
                    *op++ = (byte)(matchCode % 255);
                }
                else
                    *token += (byte)(matchCode);
            }

            /* Ensure we have enough space for the last literals. */
#if DEBUG
            Assert(
                !(outputDirective == limitedOutput_directive.fillOutput
                    && op + 1 + LASTLITERALS > olimit));
#endif

            anchor = ip;

            /* Test end of chunk */
            if (ip >= mflimitPlusOne)
                break;

            /* Fill table */
            LZ4_putPosition(ip - 2, cctx->hashTable, tableType, @base);

            /* Test next position */
            if (tableType == tableType_t.byPtr)
            {
                match = LZ4_getPosition(ip, cctx->hashTable, tableType, @base);
                LZ4_putPosition(ip, cctx->hashTable, tableType, @base);

                if ((match + LZ4_DISTANCE_MAX >= ip) && (Internal.Mem.Peek4(match) == Internal.Mem.Peek4(ip)))
                {
                    token = op++;
                    *token = 0;
                    goto _next_match;
                }
            }
            else
            {
                /* byU32, byU16 */

                uint h = LZ4_hashPosition(ip, tableType);
                uint current = (uint)(ip - @base);
                uint matchIndex = LZ4_getIndexOnHash(h, cctx->hashTable, tableType);
#if DEBUG
                Assert(matchIndex < current);
#endif

                if (dictDirective == dict_directive.usingDictCtx)
                {
                    if (matchIndex < startIndex)
                    {
                        matchIndex = LZ4_getIndexOnHash(
                            h, dictCtx->hashTable, tableType_t.byU32);
                        match = dictBase + matchIndex;
                        lowLimit = dictionary;
                        matchIndex += dictDelta;
                    }
                    else
                    {
                        match = @base + matchIndex;
                        lowLimit = source;
                    }
                }
                else if (dictDirective == dict_directive.usingExtDict)
                {
                    if (matchIndex < startIndex)
                    {
                        match = dictBase + matchIndex;
                        lowLimit = dictionary;
                    }
                    else
                    {
                        match = @base + matchIndex;
                        lowLimit = source;
                    }
                }
                else
                {
                    match = @base + matchIndex;
                }

                LZ4_putIndexOnHash(current, h, cctx->hashTable, tableType);
#if DEBUG
                Assert(matchIndex < current);
#endif

                if (((dictIssue != dictIssue_directive.dictSmall)
                        || (matchIndex >= prefixIdxLimit))
                    && (((tableType == tableType_t.byU16)
                            /*&& (LZ4_DISTANCE_MAX == LZ4_DISTANCE_ABSOLUTE_MAX)*/)
                        || (matchIndex + LZ4_DISTANCE_MAX >= current))
                    && (Internal.Mem.Peek4(match) == Internal.Mem.Peek4(ip)))
                {
                    token = op++;
                    *token = 0;

                    if (maybe_extMem)
                        offset = current - matchIndex;

                    goto _next_match;
                }
            }

            forwardH = LZ4_hashPosition(++ip, tableType);
        }

    _last_literals:
        {
            var lastRun = (size_t)(iend - anchor);
            if ((outputDirective != 0) &&
                (op + lastRun + 1 + ((lastRun + 255 - RUN_MASK) / 255) > olimit))
            {
                if (outputDirective == limitedOutput_directive.fillOutput)
                {
#if DEBUG
                    Assert(olimit >= op);
#endif
                    lastRun = (size_t)(olimit - op) - 1;
                    lastRun -= (lastRun + 240) / 255;
                }
                else
                {
#if DEBUG
                    Assert(outputDirective == limitedOutput_directive.limitedOutput);
#endif
                    return 0;
                }
            }

            if (lastRun >= RUN_MASK)
            {
                var accumulator = lastRun - RUN_MASK;
                *op++ = (byte)(RUN_MASK << ML_BITS);

                for (; accumulator >= 255; accumulator -= 255)
                    *op++ = 255;

                *op++ = (byte)accumulator;
            }
            else
            {
                *op++ = (byte)(lastRun << ML_BITS);
            }

            Internal.Mem.Copy(op, anchor, (int)lastRun);
            ip = anchor + lastRun;
            op += lastRun;
        }

        if (outputDirective == limitedOutput_directive.fillOutput)
        {
            *inputConsumed = (int)(ip - source);
        }

        result = (int)(op - dest);
#if DEBUG
        Assert(result > 0);
#endif
        return result;
    }

    #endregion

    public static int LZ4_compress_fast(byte* source, byte* dest, int inputSize, int maxOutputSize)
    {
        LZ4_stream_t state;
        var ctx = LZ4_initStream(&state);
#if DEBUG
        Assert(ctx != null);
#endif

        if (maxOutputSize >= LZ4_compressBound(inputSize))
        {
            if (inputSize < LZ4_64Klimit)
            {
                return LZ4_compress_generic(
                    ctx, source, dest,
                    inputSize, null, 0, limitedOutput_directive.notLimited,
                    tableType_t.byU16, dict_directive.noDict, dictIssue_directive.noDictIssue);
            }
            else
            {
                var tableType = sizeof(void*) < 8 && source > (byte*)LZ4_DISTANCE_MAX
                    ? tableType_t.byPtr
                    : tableType_t.byU32;

                return LZ4_compress_generic(
                    ctx, source, dest,
                    inputSize, null, 0, limitedOutput_directive.notLimited,
                    tableType, dict_directive.noDict, dictIssue_directive.noDictIssue);
            }
        }
        else
        {
            if (inputSize < LZ4_64Klimit)
            {
                return LZ4_compress_generic(
                    ctx, source, dest,
                    inputSize, null, maxOutputSize, limitedOutput_directive.limitedOutput,
                    tableType_t.byU16, dict_directive.noDict, dictIssue_directive.noDictIssue);
            }
            else
            {
                var tableType = sizeof(void*) < 8 && source > (byte*)LZ4_DISTANCE_MAX
                    ? tableType_t.byPtr
                    : tableType_t.byU32;

                return LZ4_compress_generic(
                    ctx, source, dest,
                    inputSize, null, maxOutputSize, limitedOutput_directive.limitedOutput,
                    tableType, dict_directive.noDict, dictIssue_directive.noDictIssue);
            }
        }
    }

}
