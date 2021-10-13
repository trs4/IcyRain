using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Compression.LZ4.Internal
{
    /// <summary>
    /// Base class for both <see cref="XXH32"/>. Do not use directly.
    /// </summary>
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    internal unsafe class XXH
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        /// <summary>Protected constructor to prevent instantiation.</summary>
        protected XXH() { }

        [MethodImpl(Flags.HotPath)]
        internal static uint XXH_read32(void* p) => *(uint*)p;

        [MethodImpl(Flags.HotPath)]
        internal static ulong XXH_read64(void* p) => *(ulong*)p;

        internal static void XXH_zero(void* target, int length)
        {
            var targetP = (byte*)target;

            while (length >= sizeof(ulong))
            {
                *(ulong*)targetP = 0;
                targetP += sizeof(ulong);
                length -= sizeof(ulong);
            }

            if (length >= sizeof(uint))
            {
                *(uint*)targetP = 0;
                targetP += sizeof(uint);
                length -= sizeof(uint);
            }

            if (length >= sizeof(ushort))
            {
                *(ushort*)targetP = 0;
                targetP += sizeof(ushort);
                length -= sizeof(ushort);
            }

            if (length > 0)
            {
                *targetP = 0;
                // targetP++;
                // length--;
            }
        }

        internal static void XXH_copy(void* target, void* source, int length)
        {
            var sourceP = (byte*)source;
            var targetP = (byte*)target;

            while (length >= sizeof(ulong))
            {
                *(ulong*)targetP = *(ulong*)sourceP;
                targetP += sizeof(ulong);
                sourceP += sizeof(ulong);
                length -= sizeof(ulong);
            }

            if (length >= sizeof(uint))
            {
                *(uint*)targetP = *(uint*)sourceP;
                targetP += sizeof(uint);
                sourceP += sizeof(uint);
                length -= sizeof(uint);
            }

            if (length >= sizeof(ushort))
            {
                *(ushort*)targetP = *(ushort*)sourceP;
                targetP += sizeof(ushort);
                sourceP += sizeof(ushort);
                length -= sizeof(ushort);
            }


            if (length > 0)
            {
                *targetP = *sourceP;
                // targetP++;
                // sourceP++;
                // length--;
            }
        }

        internal static void Validate(byte[] bytes, int offset, int length)
        {
            if (bytes == null || offset < 0 || length < 0 || offset + length > bytes.Length)
                throw new ArgumentException("Invalid buffer boundaries");
        }
    }
}
