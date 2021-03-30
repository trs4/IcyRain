using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IcyRain.Compression.LZ4.Internal
{
    internal unsafe partial class XXH32 : XXH
    {
        private const uint PRIME32_1 = 2654435761u;
        private const uint PRIME32_2 = 2246822519u;
        private const uint PRIME32_3 = 3266489917u;
        private const uint PRIME32_4 = 668265263u;
        private const uint PRIME32_5 = 374761393u;

        [StructLayout(LayoutKind.Sequential)]
        private struct XXH32_state
        {
            public uint total_len_32;
            public bool large_len;
            public uint v1;
            public uint v2;
            public uint v3;
            public uint v4;
            public fixed uint mem32[4];
            public uint memsize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint XXH32_rotl(uint x, int r) => (x << r) | (x >> (32 - r));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint XXH32_round(uint seed, uint input) =>
            XXH32_rotl(seed + input * PRIME32_2, 13) * PRIME32_1;

        private static uint XXH32_hash(void* input, int len, uint seed)
        {
            var p = (byte*)input;
            var bEnd = p + len;
            uint h32;

            if (len >= 16)
            {
                var limit = bEnd - 16;
                var v1 = seed + PRIME32_1 + PRIME32_2;
                var v2 = seed + PRIME32_2;
                var v3 = seed + 0;
                var v4 = seed - PRIME32_1;

                do
                {
                    v1 = XXH32_round(v1, XXH_read32(p + 0));
                    v2 = XXH32_round(v2, XXH_read32(p + 4));
                    v3 = XXH32_round(v3, XXH_read32(p + 8));
                    v4 = XXH32_round(v4, XXH_read32(p + 12));
                    p += 16;
                }
                while (p <= limit);

                h32 = XXH32_rotl(v1, 1) + XXH32_rotl(v2, 7) + XXH32_rotl(v3, 12) + XXH32_rotl(v4, 18);
            }
            else
            {
                h32 = seed + PRIME32_5;
            }

            h32 += (uint)len;

            while (p + 4 <= bEnd)
            {
                h32 = XXH32_rotl(h32 + XXH_read32(p) * PRIME32_3, 17) * PRIME32_4;
                p += 4;
            }

            while (p < bEnd)
            {
                h32 = XXH32_rotl(h32 + *p * PRIME32_5, 11) * PRIME32_1;
                p++;
            }

            h32 ^= h32 >> 15;
            h32 *= PRIME32_2;
            h32 ^= h32 >> 13;
            h32 *= PRIME32_3;
            h32 ^= h32 >> 16;

            return h32;
        }

        private static void XXH32_reset(XXH32_state* state, uint seed)
        {
            XXH_zero(state, sizeof(XXH32_state));
            state->v1 = seed + PRIME32_1 + PRIME32_2;
            state->v2 = seed + PRIME32_2;
            state->v3 = seed + 0;
            state->v4 = seed - PRIME32_1;
        }

        private static void XXH32_update(XXH32_state* state, void* input, int len)
        {
            var p = (byte*)input;
            var bEnd = p + len;

            state->total_len_32 += (uint)len;
            state->large_len |= len >= 16 || state->total_len_32 >= 16;

            if (state->memsize + len < 16)
            {
                /* fill in tmp buffer */
                XXH_copy((byte*)state->mem32 + state->memsize, input, len);
                state->memsize += (uint)len;
                return;
            }

            if (state->memsize > 0)
            {
                /* some data left from previous update */
                XXH_copy((byte*)state->mem32 + state->memsize, input, (int)(16 - state->memsize));
                var p32 = state->mem32;
                state->v1 = XXH32_round(state->v1, XXH_read32(p32 + 0));
                state->v2 = XXH32_round(state->v2, XXH_read32(p32 + 1));
                state->v3 = XXH32_round(state->v3, XXH_read32(p32 + 2));
                state->v4 = XXH32_round(state->v4, XXH_read32(p32 + 3));
                p += 16 - state->memsize;
                state->memsize = 0;
            }

            if (p <= bEnd - 16)
            {
                var limit = bEnd - 16;
                var v1 = state->v1;
                var v2 = state->v2;
                var v3 = state->v3;
                var v4 = state->v4;

                do
                {
                    v1 = XXH32_round(v1, XXH_read32(p + 0));
                    v2 = XXH32_round(v2, XXH_read32(p + 4));
                    v3 = XXH32_round(v3, XXH_read32(p + 8));
                    v4 = XXH32_round(v4, XXH_read32(p + 12));
                    p += 16;
                }
                while (p <= limit);

                state->v1 = v1;
                state->v2 = v2;
                state->v3 = v3;
                state->v4 = v4;
            }

            if (p < bEnd)
            {
                XXH_copy(state->mem32, p, (int)(bEnd - p));
                state->memsize = (uint)(bEnd - p);
            }
        }

        private static uint XXH32_digest(XXH32_state* state)
        {
            var p = (byte*)state->mem32;
            var bEnd = (byte*)state->mem32 + state->memsize;
            uint h32;

            if (state->large_len)
            {
                h32 = XXH32_rotl(state->v1, 1)
                    + XXH32_rotl(state->v2, 7)
                    + XXH32_rotl(state->v3, 12)
                    + XXH32_rotl(state->v4, 18);
            }
            else
            {
                h32 = state->v3 + PRIME32_5;
            }

            h32 += state->total_len_32;

            while (p + 4 <= bEnd)
            {
                h32 += XXH_read32(p) * PRIME32_3;
                h32 = XXH32_rotl(h32, 17) * PRIME32_4;
                p += 4;
            }

            while (p < bEnd)
            {
                h32 += (*p) * PRIME32_5;
                h32 = XXH32_rotl(h32, 11) * PRIME32_1;
                p++;
            }

            h32 ^= h32 >> 15;
            h32 *= PRIME32_2;
            h32 ^= h32 >> 13;
            h32 *= PRIME32_3;
            h32 ^= h32 >> 16;

            return h32;
        }


        /// <summary>Hash of empty buffer.</summary>
        public const uint EmptyHash = 46947589;

        /// <summary>Hash of provided buffer.</summary>
        /// <param name="bytes">Buffer.</param>
        /// <param name="length">Length of buffer.</param>
        /// <returns>Digest.</returns>
        public static unsafe uint DigestOf(void* bytes, int length) =>
            XXH32_hash(bytes, length, 0);

        /// <summary>Hash of provided buffer.</summary>
        /// <param name="bytes">Buffer.</param>
        /// <returns>Digest.</returns>
        public static unsafe uint DigestOf(ReadOnlySpan<byte> bytes)
        {
            fixed (byte* bytesP = &MemoryMarshal.GetReference(bytes))
                return DigestOf(bytesP, bytes.Length);
        }

        /// <summary>Hash of provided buffer.</summary>
        /// <param name="bytes">Buffer.</param>
        /// <param name="offset">Starting offset.</param>
        /// <param name="length">Length of buffer.</param>
        /// <returns>Digest.</returns>
        public static unsafe uint DigestOf(byte[] bytes, int offset, int length)
        {
            Validate(bytes, offset, length);

            fixed (byte* bytes0 = bytes)
                return DigestOf(bytes0 + offset, length);
        }

        private XXH32_state _state;

        /// <summary>Creates xxHash instance.</summary>
        public XXH32() => Reset();

        /// <summary>Resets hash calculation.</summary>
        public unsafe void Reset()
        {
            fixed (XXH32_state* stateP = &_state)
                XXH32_reset(stateP, 0);
        }

        /// <summary>Updates the has using given buffer.</summary>
        /// <param name="bytes">Buffer.</param>
        /// <param name="length">Length of buffer.</param>
        public unsafe void Update(byte* bytes, int length)
        {
            fixed (XXH32_state* stateP = &_state)
                XXH32_update(stateP, bytes, length);
        }

        /// <summary>Updates the has using given buffer.</summary>
        /// <param name="bytes">Buffer.</param>
        public unsafe void Update(ReadOnlySpan<byte> bytes)
        {
            fixed (byte* bytesP = &MemoryMarshal.GetReference(bytes))
                Update(bytesP, bytes.Length);
        }

        /// <summary>Updates the has using given buffer.</summary>
        /// <param name="bytes">Buffer.</param>
        /// <param name="offset">Starting offset.</param>
        /// <param name="length">Length of buffer.</param>
        public unsafe void Update(byte[] bytes, int offset, int length)
        {
            Validate(bytes, offset, length);

            fixed (byte* bytesP = bytes)
                Update(bytesP + offset, length);
        }

        /// <summary>Hash so far.</summary>
        /// <returns>Hash so far.</returns>
        public unsafe uint Digest()
        {
            fixed (XXH32_state* stateP = &_state)
                return XXH32_digest(stateP);
        }

        /// <summary>Hash so far, as byte array.</summary>
        /// <returns>Hash so far.</returns>
        public byte[] DigestBytes() => BitConverter.GetBytes(Digest());

        ///// <summary>Converts this class to <see cref="HashAlgorithm"/></summary>
        ///// <returns><see cref="HashAlgorithm"/></returns>
        //public HashAlgorithm AsHashAlgorithm() =>
        //	new HashAlgorithmAdapter(sizeof(uint), Reset, Update, DigestBytes);

    }
}
