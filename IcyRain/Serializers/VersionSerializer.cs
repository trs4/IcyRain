using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers
{
    internal sealed class VersionSerializer<TResolver> : Serializer<TResolver, Version>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(Version value)
            => StringEncoding.GetSize(value?.ToString());

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, Version value)
            => writer.WriteString(value?.ToString());

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, Version value)
            => writer.WriteNotNullString(value.ToString());

        [MethodImpl(Flags.HotPath)]
        public override sealed Version Deserialize(ref Reader reader)
        {
            string version = reader.ReadString();
            return version is null ? null : Version.Parse(version);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed Version DeserializeInUTC(ref Reader reader)
        {
            string version = reader.ReadString();
            return version is null ? null : Version.Parse(version);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed Version DeserializeSpot(ref Reader reader)
            => Version.Parse(reader.ReadNotNullString());

        [MethodImpl(Flags.HotPath)]
        public override sealed Version DeserializeInUTCSpot(ref Reader reader)
            => Version.Parse(reader.ReadNotNullString());
    }
}
