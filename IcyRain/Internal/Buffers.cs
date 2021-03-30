using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace IcyRain.Internal
{
    internal static class Buffers
    {
        public const int MinArrayLength = 512;
        public const int StreamPartSize = 1024 * 1024; // 1 Mb
        public const int MinCompressSize = 256;

        [MethodImpl(Flags.HotPath)]
        public static byte[] Rent(int sizeHint)
            => MinArrayLength > sizeHint ? new byte[sizeHint] : ArrayPool<byte>.Shared.Rent(sizeHint);

        [MethodImpl(Flags.HotPath)]
        public static void Return(byte[] buffer)
        {
            if (buffer is not null && buffer.Length >= MinArrayLength)
                ArrayPool<byte>.Shared.Return(buffer);
        }

        [MethodImpl(Flags.HotPath)]
        public static void ReturnWithCheck(byte[] buffer)
        {
            if (buffer is not null && buffer.Length >= MinArrayLength && (buffer.Length & (buffer.Length - 1)) == 0)
                ArrayPool<byte>.Shared.Return(buffer);
        }

        [MethodImpl(Flags.HotPath)]
        public static byte[] ToArray(byte[] buffer, int count)
        {
            if (buffer.Length == count)
                return buffer;

            var result = new byte[count];
            Buffer.BlockCopy(buffer, 0, result, 0, count);
            return result;
        }

    }
}
