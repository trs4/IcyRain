using System;

namespace IcyRain
{
    /// <summary>Deserialize options</summary>
    public sealed class DeserializeOptions
    {
        /// <summary>Default options</summary>
        public static readonly DeserializeOptions Default = new DeserializeOptions(DateTimeKind.Unspecified);

        public DeserializeOptions(DateTimeKind dateTimeZone)
            => DateTimeZone = dateTimeZone;

        /// <summary>DateTime zone mode</summary>
        public DateTimeKind DateTimeZone { get; }

        internal bool IsUtcZone { get; }
    }
}
