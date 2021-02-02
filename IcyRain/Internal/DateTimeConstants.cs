using System;

namespace IcyRain.Internal
{
    internal static class DateTimeConstants
    {
        public const long BclSecondsAtUnixEpoch = 62135596800;
        public const int NanosecondsPerTick = 100;
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
