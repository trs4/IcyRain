using System.Runtime.CompilerServices;
using IcyRain.Internal;
using Mem = IcyRain.Compression.LZ4.Internal.Mem64;

namespace IcyRain.Compression.LZ4.Engine;

internal unsafe partial class LL64
{
    [MethodImpl(Flags.HotPath)]
    public static int LZ4_decompress_generic(
        byte* src,
        byte* dst,
        int srcSize,
        int outputSize,
        endCondition_directive endOnInput,
        earlyEnd_directive partialDecoding,
        dict_directive dict,
        byte* lowPrefix,
        byte* dictStart,
        uint dictSize)
    {
        return LZ4_decompress_generic(
            src, dst, srcSize, outputSize,
            endOnInput == endCondition_directive.endOnInputSize,
            partialDecoding == earlyEnd_directive.partial,
            dict,
            lowPrefix,
            dictStart,
            dictSize
        );
    }

    [MethodImpl(Flags.HotPath)]
    public static int LZ4_decompress_generic(
        byte* src,
        byte* dst,
        int srcSize,
        int outputSize,
        bool endOnInput, // endCondition_directive
        bool partialDecoding, // earlyEnd_directive
        dict_directive dict,
        byte* lowPrefix,
        byte* dictStart,
        uint dictSize)
    {
        if (src is null) { return -1; }

        {
            byte* ip = src;
            byte* iend = ip + srcSize;

            byte* op = dst;
            byte* oend = op + outputSize;
            byte* cpy;

            byte* dictEnd = dictStart is null ? null : dictStart + dictSize;

            bool safeDecode = endOnInput;
            bool checkOffset = safeDecode && (dictSize < 64 * KB);

            /* Set up the "end" pointers for the shortcut. */
            byte* shortiend = iend - (endOnInput ? 14 : 8) /*maxLL*/ - 2 /*offset*/;
            byte* shortoend = oend - (endOnInput ? 14 : 8) /*maxLL*/ - 18 /*maxML*/;

            byte* match;
            uint offset;
            uint token;
            uint length;

            /* Special cases */
#if DEBUG
            Assert(lowPrefix <= op);
#endif
            if ((endOnInput) && ((outputSize == 0)))
            {
                /* Empty output buffer */
                if (partialDecoding) return 0;

                return ((srcSize == 1) && (*ip == 0)) ? 0 : -1;
            }

            if ((!endOnInput) && ((outputSize == 0))) { return (*ip == 0 ? 1 : -1); }

            if ((endOnInput) && (srcSize == 0)) { return -1; }

            /* Main Loop : decode remaining sequences where output < FASTLOOP_SAFE_DISTANCE */
            while (true)
            {
                token = *ip++;
                length = (uint)(token >> ML_BITS); /* literal length */

#if DEBUG
                Assert(!endOnInput || ip <= iend); /* ip < iend before the increment */
#endif

                /* A two-stage shortcut for the most common case:
					* 1) If the literal length is 0..14, and there is enough space,
					* enter the shortcut and copy 16 bytes on behalf of the literals
					* (in the fast mode, only 8 bytes can be safely copied this way).
					* 2) Further if the match length is 4..18, copy 18 bytes in a similar
					* manner; but we ensure that there's enough space in the output for
					* those 18 bytes earlier, upon entering the shortcut (in other words,
					* there is a combined check for both stages).
					*/
                if ((endOnInput ? length != RUN_MASK : length <= 8)
                    /* strictly "less than" on input, to re-enter the loop with at least one byte */
                    && ((!endOnInput || ip < shortiend) & (op <= shortoend)))
                {
                    /* Copy the literals */
                    if (endOnInput) Mem.Copy16(op, ip);
                    else Mem.Copy8(op, ip);
                    // Mem.Copy(op, ip, endOnInput ? 16 : 8);
                    op += length;
                    ip += length;

                    /* The second stage: prepare for match copying, decode full info.
						* If it doesn't work out, the info won't be wasted. */
                    length = token & ML_MASK; /* match length */
                    offset = Mem.Peek2(ip);
                    ip += 2;
                    match = op - offset;
#if DEBUG
                    Assert(match <= op); /* check overflow */
#endif

                    /* Do not deal with overlapping matches. */
                    if ((length != ML_MASK)
                        && (offset >= 8)
                        && (dict == dict_directive.withPrefix64k || match >= lowPrefix))
                    {
                        /* Copy the match. */
                        Mem.Copy18(op, match);
                        op += length + MINMATCH;
                        /* Both stages worked, load the next token. */
                        continue;
                    }

                    /* The second stage didn't work out, but the info is ready.
						* Propel it right to the point of match copying. */
                    goto _copy_match;
                }

                /* decode literal length */
                if (length == RUN_MASK)
                {
                    variable_length_error error = variable_length_error.ok;
                    length += LZ4_readVLE(&ip, iend - RUN_MASK, endOnInput, endOnInput, &error);
#pragma warning disable CA1508 // Avoid dead conditional code
                    if (error == variable_length_error.initial_error) { goto _output_error; }
#pragma warning restore CA1508 // Avoid dead conditional code

                    if ((safeDecode) && ((op) + length < (op)))
                    {
                        goto _output_error;
                    } /* overflow detection */

                    if ((safeDecode) && ((ip) + length < (ip)))
                    {
                        goto _output_error;
                    } /* overflow detection */
                }

                /* copy literals */
                cpy = op + length;
                if (((endOnInput) && ((cpy > oend - MFLIMIT)
                        || (ip + length > iend - (2 + 1 + LASTLITERALS))))
                    || ((!endOnInput) && (cpy > oend - WILDCOPYLENGTH)))
                {
                    /* We've either hit the input parsing restriction or the output parsing restriction.
						* If we've hit the input parsing condition then this must be the last sequence.
						* If we've hit the output parsing condition then we are either using partialDecoding
						* or we've hit the output parsing condition.
						*/
                    if (partialDecoding)
                    {
                        /* Since we are partial decoding we may be in this block because of the output parsing
							* restriction, which is not valid since the output buffer is allowed to be undersized.
							*/
#if DEBUG
                        Assert(endOnInput);
#endif
                        /* If we're in this block because of the input parsing condition, then we must be on the
							* last sequence (or invalid), so we must check that we exactly consume the input.
							*/
                        if ((ip + length > iend - (2 + 1 + LASTLITERALS))
                            && (ip + length != iend)) { goto _output_error; }

#if DEBUG
                        Assert(ip + length <= iend);
#endif
                        /* We are finishing in the middle of a literals segment.
							* Break after the copy.
							*/
                        if (cpy > oend)
                        {
                            cpy = oend;
#if DEBUG
                            Assert(op <= oend);
#endif
                            length = (uint)(oend - op);
                        }

#if DEBUG
                        Assert(ip + length <= iend);
#endif
                    }
                    else
                    {
                        /* We must be on the last sequence because of the parsing limitations so check
							* that we exactly regenerate the original size (must be exact when !endOnInput).
							*/
                        if ((!endOnInput) && (cpy != oend)) { goto _output_error; }

                        /* We must be on the last sequence (or invalid) because of the parsing limitations
							* so check that we exactly consume the input and don't overrun the output buffer.
							*/
                        if ((endOnInput) && ((ip + length != iend) || (cpy > oend)))
                        {
                            goto _output_error;
                        }
                    }

                    Mem.Move(op, ip, (int)length); /* supports overlapping memory regions, which only matters for in-place decompression scenarios */
                    ip += length;
                    op += length;
                    /* Necessarily EOF when !partialDecoding. When partialDecoding
						* it is EOF if we've either filled the output buffer or hit
						* the input parsing restriction.
						*/
                    if (!partialDecoding || (cpy == oend) || (ip == iend))
                    {
                        break;
                    }
                }
                else
                {
                    Mem.WildCopy8(
                        op, ip, cpy); /* may overwrite up to WILDCOPYLENGTH beyond cpy */
                    ip += length;
                    op = cpy;
                }

                /* get offset */
                offset = Mem.Peek2(ip);
                ip += 2;
                match = op - offset;

                /* get matchlength */
                length = token & ML_MASK;

            _copy_match:
                if (length == ML_MASK)
                {
                    variable_length_error error = variable_length_error.ok;
                    length += LZ4_readVLE(
                        &ip, iend - LASTLITERALS + 1, endOnInput, false, &error);
#pragma warning disable CA1508 // Avoid dead conditional code
                    if (error != variable_length_error.ok) goto _output_error;
#pragma warning restore CA1508 // Avoid dead conditional code
                    if ((safeDecode) && ((op) + length < op))
                        goto _output_error; /* overflow detection */
                }

                length += MINMATCH;

                if ((checkOffset) && ((match + dictSize < lowPrefix)))
                    goto _output_error; /* Error : offset outside buffers */

                /* match starting within external dictionary */
                if ((dict == dict_directive.usingExtDict) && (match < lowPrefix))
                {
                    if ((op + length > oend - LASTLITERALS))
                    {
                        if (partialDecoding) length = MIN(length, (uint)(oend - op));
                        else goto _output_error; /* doesn't respect parsing restriction */
                    }

                    if (length <= (uint)(lowPrefix - match))
                    {
                        /* match fits entirely within external dictionary : just copy */
                        Mem.Move(op, dictEnd - (lowPrefix - match), (int)length);
                        op += length;
                    }
                    else
                    {
                        /* match stretches into both external dictionary and current block */
                        uint copySize = (uint)(lowPrefix - match);
                        uint restSize = length - copySize;
                        Mem.Copy(op, dictEnd - copySize, (int)copySize);
                        op += copySize;
                        if (restSize > (uint)(op - lowPrefix))
                        {
                            /* overlap copy */
                            byte* endOfMatch = op + restSize;
                            byte* copyFrom = lowPrefix;
                            while (op < endOfMatch) *op++ = *copyFrom++;
                        }
                        else
                        {
                            Mem.Copy(op, lowPrefix, (int)restSize);
                            op += restSize;
                        }
                    }

                    continue;
                }

#if DEBUG
                Assert(match >= lowPrefix);
#endif

                /* copy match within block */
                cpy = op + length;

                /* partialDecoding : may end anywhere within the block */
#if DEBUG
                Assert(op <= oend);
#endif
                if (partialDecoding && (cpy > oend - MATCH_SAFEGUARD_DISTANCE))
                {
                    uint mlen = MIN(length, (uint)(oend - op));
                    byte* matchEnd = match + mlen;
                    byte* copyEnd = op + mlen;
                    if (matchEnd > op)
                    {
                        /* overlap copy */
                        while (op < copyEnd) { *op++ = *match++; }
                    }
                    else
                    {
                        Mem.Copy(op, match, (int)mlen);
                    }

                    op = copyEnd;
                    if (op == oend) { break; }

                    continue;
                }

                if ((offset < 8))
                {
                    // Mem.Poke4(op, 0); /* silence msan warning when offset==0 */
                    op[0] = match[0];
                    op[1] = match[1];
                    op[2] = match[2];
                    op[3] = match[3];
                    match += inc32table[offset];
                    Mem.Copy4(op + 4, match);
                    match -= dec64table[offset];
                }
                else
                {
                    Mem.Copy8(op, match);
                    match += 8;
                }

                op += 8;

                if ((cpy > oend - MATCH_SAFEGUARD_DISTANCE))
                {
                    byte* oCopyLimit = oend - (WILDCOPYLENGTH - 1);
                    if (cpy > oend - LASTLITERALS)
                    {
                        goto _output_error;
                    } /* Error : last LASTLITERALS bytes must be literals (uncompressed) */

                    if (op < oCopyLimit)
                    {
                        Mem.WildCopy8(op, match, oCopyLimit);
                        match += oCopyLimit - op;
                        op = oCopyLimit;
                    }

                    while (op < cpy) { *op++ = *match++; }
                }
                else
                {
                    Mem.Copy8(op, match);
                    if (length > 16) { Mem.WildCopy8(op + 8, match + 8, cpy); }
                }

                op = cpy; /* wildcopy correction */
            }

            /* end of decoding */
            if (endOnInput)
            {
                return (int)(((byte*)op) - dst); /* Nb of output bytes decoded */
            }
            else
            {
                return (int)(((byte*)ip) - src); /* Nb of input bytes read */
            }

        /* Overflow error detected */
        _output_error:
            return (int)(-(((byte*)ip) - src)) - 1;
        }
    }

    public static int LZ4_decompress_safe(
        byte* source, byte* dest, int compressedSize, int maxDecompressedSize)
    {
        return LZ4_decompress_generic(
            source, dest, compressedSize, maxDecompressedSize,
            endCondition_directive.endOnInputSize, earlyEnd_directive.full,
            dict_directive.noDict,
            (byte*)dest, null, 0);
    }

}
