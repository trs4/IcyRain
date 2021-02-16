using System.Net;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers
{
    internal sealed class IPAddressSerializer<TResolver> : Serializer<TResolver, IPAddress>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(IPAddress value)
            => StringEncoding.GetSize(value?.ToString());

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, IPAddress value)
            => writer.WriteString(value?.ToString());

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, IPAddress value)
            => writer.WriteNotNullString(value.ToString());

        [MethodImpl(Flags.HotPath)]
        public override sealed IPAddress Deserialize(ref Reader reader, DeserializeOptions options)
        {
            string version = reader.ReadString();
            return version is null ? null : IPAddress.Parse(version);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed IPAddress DeserializeSpot(ref Reader reader, DeserializeOptions options)
            => IPAddress.Parse(reader.ReadNotNullString());
    }
}
