using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain.Comparers
{
    internal sealed class BoolEqualityComparer : IEqualityComparer<bool>
    {
        public static readonly BoolEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(bool x, bool y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(bool obj) => obj ? 1 : 0;
    }

    internal sealed class CharEqualityComparer : IEqualityComparer<char>
    {
        public static readonly CharEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(char x, char y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(char obj) => obj | obj << 16;
    }

    internal sealed class SByteEqualityComparer : IEqualityComparer<sbyte>
    {
        public static readonly SByteEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(sbyte x, sbyte y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(sbyte obj) => obj ^ obj << 8;
    }

    internal sealed class ByteEqualityComparer : IEqualityComparer<byte>
    {
        public static readonly ByteEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(byte x, byte y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(byte obj) => obj;
    }

    internal sealed class ShortEqualityComparer : IEqualityComparer<short>
    {
        public static readonly ShortEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(short x, short y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(short obj) => (ushort)obj | obj << 16;
    }

    internal sealed class UShortEqualityComparer : IEqualityComparer<ushort>
    {
        public static readonly UShortEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(ushort x, ushort y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(ushort obj) => obj;
    }

    internal sealed class IntEqualityComparer : IEqualityComparer<int>
    {
        public static readonly IntEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(int x, int y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(int obj) => obj;
    }

    internal sealed class UIntEqualityComparer : IEqualityComparer<uint>
    {
        public static readonly UIntEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(uint x, uint y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(uint obj) => (int)obj;
    }

    internal sealed class LongEqualityComparer : IEqualityComparer<long>
    {
        public static readonly LongEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(long x, long y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(long obj) => (int)obj ^ (int)(obj >> 32);
    }

    internal sealed class ULongEqualityComparer : IEqualityComparer<ulong>
    {
        public static readonly ULongEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(ulong x, ulong y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(ulong obj) => (int)obj ^ (int)(obj >> 32);
    }

    internal sealed class FloatEqualityComparer : IEqualityComparer<float>
    {
        public static readonly FloatEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(float x, float y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public unsafe int GetHashCode(float obj) => *(int*)(&obj);
    }

    internal sealed class DoubleEqualityComparer : IEqualityComparer<double>
    {
        public static readonly DoubleEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(double x, double y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public unsafe int GetHashCode(double obj)
        {
            long num = *(long*)(&obj);
            return (int)num ^ (int)(num >> 32);
        }

    }

    internal sealed class DecimalEqualityComparer : IEqualityComparer<decimal>
    {
        public static readonly DecimalEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(decimal x, decimal y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(decimal obj)
        {
            int[] bits = decimal.GetBits(obj);
            return bits[3] ^ bits[2] ^ bits[0] ^ bits[1];
        }

    }

    internal sealed class StringEqualityComparer : IEqualityComparer<string>
    {
        public static readonly StringEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(string x, string y) => string.Equals(x, y);

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(string obj) => obj.GetHashCode();
    }

    internal sealed class GuidEqualityComparer : IEqualityComparer<Guid>
    {
        public static readonly GuidEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(Guid x, Guid y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(Guid obj)
        {
            byte[] b = obj.ToByteArray();

            int g_a = (b[3] << 24) | (b[2] << 16) | (b[1] << 8) | b[0];
            short g_b = (short)((b[5] << 8) | b[4]);
            short g_c = (short)((b[7] << 8) | b[6]);
            byte g_f = b[10];
            byte g_k = b[15];

            return g_a ^ ((g_b << 16) | (ushort)g_c) ^ ((g_f << 24) | g_k);
        }

    }

    internal sealed class DateTimeEqualityComparer : IEqualityComparer<DateTime>
    {
        public static readonly DateTimeEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(DateTime x, DateTime y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(DateTime dateTime)
        {
            dateTime = dateTime.ToUniversalTime();

            long secondsSinceBclEpoch = dateTime.Ticks / TimeSpan.TicksPerSecond;
            long seconds = secondsSinceBclEpoch - DateTimeConstants.BclSecondsAtUnixEpoch;
            int nanoseconds = (int)(dateTime.Ticks % TimeSpan.TicksPerSecond);

            int hash = (int)seconds ^ (int)(seconds >> 32);
            hash = (hash << 5) + hash ^ nanoseconds;
            return hash;
        }

    }

    internal sealed class DateTimeOffsetEqualityComparer : IEqualityComparer<DateTimeOffset>
    {
        public static readonly DateTimeOffsetEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(DateTimeOffset x, DateTimeOffset y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(DateTimeOffset dateTime)
        {
            long ticks = dateTime.UtcTicks;
            long secondsSinceBclEpoch = ticks / TimeSpan.TicksPerSecond;
            long seconds = secondsSinceBclEpoch - DateTimeConstants.BclSecondsAtUnixEpoch;
            int nanoseconds = (int)(ticks % TimeSpan.TicksPerSecond);

            int hash = (int)seconds ^ (int)(seconds >> 32);
            hash = (hash << 5) + hash ^ nanoseconds;
            return hash;
        }

    }

    internal sealed class TimeSpanEqualityComparer : IEqualityComparer<TimeSpan>
    {
        public static readonly TimeSpanEqualityComparer Instance = new();

        [MethodImpl(Flags.HotPath)]
        public bool Equals(TimeSpan x, TimeSpan y) => x == y;

        [MethodImpl(Flags.HotPath)]
        public int GetHashCode(TimeSpan timeSpan)
        {
            long ticks = timeSpan.Ticks;
            long seconds = ticks / TimeSpan.TicksPerSecond;
            int nanos = (int)(ticks % TimeSpan.TicksPerSecond) * DateTimeConstants.NanosecondsPerTick;

            int hash = (int)seconds ^ (int)(seconds >> 32);
            hash = (hash << 5) + hash ^ nanos;
            return hash;
        }

    }

}
