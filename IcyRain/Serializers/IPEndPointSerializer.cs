using System.Net;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers
{
    internal sealed class IPEndPointSerializer<TResolver> : Serializer<TResolver, IPEndPoint>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(IPEndPoint value)
            => value is null ? 1 : StringEncoding.GetSize(value.Address?.ToString()) + 5;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, IPEndPoint value)
        {
            writer.WriteBool(value is not null);

            if (value is null)
                return;

            writer.WriteString(value.Address?.ToString());
            writer.WriteInt(value.Port);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, IPEndPoint value)
        {
            writer.WriteBool(true);
            writer.WriteString(value.Address?.ToString());
            writer.WriteInt(value.Port);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed IPEndPoint Deserialize(ref Reader reader)
        {
            if (!reader.ReadBool())
                return null;

            string version = reader.ReadString();
            var address = version is null ? null : IPAddress.Parse(version);
            return new IPEndPoint(address, reader.ReadInt());
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed IPEndPoint DeserializeInUTC(ref Reader reader)
        {
            if (!reader.ReadBool())
                return null;

            string version = reader.ReadString();
            var address = version is null ? null : IPAddress.Parse(version);
            return new IPEndPoint(address, reader.ReadInt());
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed IPEndPoint DeserializeSpot(ref Reader reader)
        {
            reader.ReadBool();
            string version = reader.ReadString();
            var address = version is null ? null : IPAddress.Parse(version);
            return new IPEndPoint(address, reader.ReadInt());
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed IPEndPoint DeserializeInUTCSpot(ref Reader reader)
        {
            reader.ReadBool();
            string version = reader.ReadString();
            var address = version is null ? null : IPAddress.Parse(version);
            return new IPEndPoint(address, reader.ReadInt());
        }

    }
}
