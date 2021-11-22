using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers
{
    internal sealed class ListBoolSerializer<TResolver> : Serializer<TResolver, List<bool>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<bool> value)
            => value is null ? 4 : value.Count + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<bool> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteBoolArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<bool> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteBoolArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<bool> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadBoolArray(length).CreateList()
                : (length == 0 ? new List<bool>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<bool> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadBoolArray(length).CreateList()
                : (length == 0 ? new List<bool>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<bool> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadBoolArray(length).CreateList() : new List<bool>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<bool> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadBoolArray(length).CreateList() : new List<bool>();
        }

    }

    internal sealed class ListCharSerializer<TResolver> : Serializer<TResolver, List<char>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<char> value)
            => value is null ? 4 : value.Count * 2 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<char> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteCharArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<char> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteCharArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<char> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadCharArray(length).CreateList()
                : (length == 0 ? new List<char>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<char> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadCharArray(length).CreateList()
                : (length == 0 ? new List<char>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<char> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadCharArray(length).CreateList() : new List<char>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<char> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadCharArray(length).CreateList() : new List<char>();
        }

    }

    internal sealed class ListSByteSerializer<TResolver> : Serializer<TResolver, List<sbyte>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<sbyte> value)
            => value is null ? 4 : value.Count + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<sbyte> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteSByteArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<sbyte> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteSByteArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<sbyte> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadSByteArray(length).CreateList()
                : (length == 0 ? new List<sbyte>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<sbyte> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadSByteArray(length).CreateList()
                : (length == 0 ? new List<sbyte>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<sbyte> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadSByteArray(length).CreateList() : new List<sbyte>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<sbyte> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadSByteArray(length).CreateList() : new List<sbyte>();
        }

    }

    internal sealed class ListByteSerializer<TResolver> : Serializer<TResolver, List<byte>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<byte> value)
            => value is null ? 4 : value.Count + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<byte> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteByteArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<byte> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteByteArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<byte> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadByteArray(length).CreateList()
                : (length == 0 ? new List<byte>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<byte> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadByteArray(length).CreateList()
                : (length == 0 ? new List<byte>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<byte> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadByteArray(length).CreateList() : new List<byte>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<byte> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadByteArray(length).CreateList() : new List<byte>();
        }

    }

    internal sealed class ListShortSerializer<TResolver> : Serializer<TResolver, List<short>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<short> value)
            => value is null ? 4 : value.Count * 2 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<short> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteShortArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<short> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteShortArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<short> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadShortArray(length).CreateList()
                : (length == 0 ? new List<short>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<short> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadShortArray(length).CreateList()
                : (length == 0 ? new List<short>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<short> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadShortArray(length).CreateList() : new List<short>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<short> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadShortArray(length).CreateList() : new List<short>();
        }

    }

    internal sealed class ListUShortSerializer<TResolver> : Serializer<TResolver, List<ushort>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<ushort> value)
            => value is null ? 4 : value.Count * 2 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<ushort> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteUShortArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<ushort> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteUShortArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<ushort> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadUShortArray(length).CreateList()
                : (length == 0 ? new List<ushort>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<ushort> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadUShortArray(length).CreateList()
                : (length == 0 ? new List<ushort>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<ushort> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadUShortArray(length).CreateList() : new List<ushort>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<ushort> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadUShortArray(length).CreateList() : new List<ushort>();
        }

    }

    internal sealed class ListIntSerializer<TResolver> : Serializer<TResolver, List<int>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<int> value)
            => value is null ? 4 : value.Count * 4 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<int> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteIntArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<int> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteIntArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<int> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadIntArray(length).CreateList()
                : (length == 0 ? new List<int>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<int> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadIntArray(length).CreateList()
                : (length == 0 ? new List<int>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<int> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadIntArray(length).CreateList() : new List<int>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<int> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadIntArray(length).CreateList() : new List<int>();
        }

    }

    internal sealed class ListUIntSerializer<TResolver> : Serializer<TResolver, List<uint>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<uint> value)
            => value is null ? 4 : value.Count * 4 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<uint> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteUIntArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<uint> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteUIntArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<uint> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadUIntArray(length).CreateList()
                : (length == 0 ? new List<uint>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<uint> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadUIntArray(length).CreateList()
                : (length == 0 ? new List<uint>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<uint> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadUIntArray(length).CreateList() : new List<uint>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<uint> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadUIntArray(length).CreateList() : new List<uint>();
        }

    }

    internal sealed class ListLongSerializer<TResolver> : Serializer<TResolver, List<long>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<long> value)
            => value is null ? 4 : value.Count * 8 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<long> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteLongArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<long> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteLongArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<long> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadLongArray(length).CreateList()
                : (length == 0 ? new List<long>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<long> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadLongArray(length).CreateList()
                : (length == 0 ? new List<long>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<long> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadLongArray(length).CreateList() : new List<long>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<long> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadLongArray(length).CreateList() : new List<long>();
        }

    }

    internal sealed class ListULongSerializer<TResolver> : Serializer<TResolver, List<ulong>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<ulong> value)
            => value is null ? 4 : value.Count * 8 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<ulong> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteULongArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<ulong> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteULongArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<ulong> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadULongArray(length).CreateList()
                : (length == 0 ? new List<ulong>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<ulong> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadULongArray(length).CreateList()
                : (length == 0 ? new List<ulong>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<ulong> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadULongArray(length).CreateList() : new List<ulong>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<ulong> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadULongArray(length).CreateList() : new List<ulong>();
        }

    }

    internal sealed class ListFloatSerializer<TResolver> : Serializer<TResolver, List<float>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<float> value)
            => value is null ? 4 : value.Count * 4 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<float> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteFloatArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<float> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteFloatArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<float> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadFloatArray(length).CreateList()
                : (length == 0 ? new List<float>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<float> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadFloatArray(length).CreateList()
                : (length == 0 ? new List<float>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<float> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadFloatArray(length).CreateList() : new List<float>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<float> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadFloatArray(length).CreateList() : new List<float>();
        }

    }

    internal sealed class ListDoubleSerializer<TResolver> : Serializer<TResolver, List<double>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<double> value)
            => value is null ? 4 : value.Count * 8 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<double> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteDoubleArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<double> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteDoubleArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<double> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadDoubleArray(length).CreateList()
                : (length == 0 ? new List<double>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<double> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadDoubleArray(length).CreateList()
                : (length == 0 ? new List<double>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<double> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadDoubleArray(length).CreateList() : new List<double>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<double> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadDoubleArray(length).CreateList() : new List<double>();
        }

    }

    internal sealed class ListDecimalSerializer<TResolver> : Serializer<TResolver, List<decimal>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<decimal> value)
            => value is null ? 4 : value.Count * 16 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<decimal> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteDecimalArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<decimal> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteDecimalArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<decimal> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadDecimalArray(length).CreateList()
                : (length == 0 ? new List<decimal>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<decimal> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadDecimalArray(length).CreateList()
                : (length == 0 ? new List<decimal>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<decimal> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadDecimalArray(length).CreateList() : new List<decimal>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<decimal> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadDecimalArray(length).CreateList() : new List<decimal>();
        }

    }

    internal sealed class ListStringSerializer<TResolver> : Serializer<TResolver, List<string>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<string> value)
        {
            int capacity = 4;

            if (value is not null)
            {
                var array = value.GetArray();

                for (int i = 0; i < value.Count; i++)
                    capacity += StringEncoding.GetSize(array[i]);
            }

            return capacity;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<string> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
            {
                var array = value.GetArray();

                for (int i = 0; i < length; i++)
                    writer.WriteString(array[i]);
            }
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<string> value)
        {
            writer.WriteInt(value.Count);
            var array = value.GetArray();

            for (int i = 0; i < value.Count; i++)
                writer.WriteString(array[i]);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<string> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length > 0)
            {
                var value = new string[length];

                for (int i = 0; i < length; i++)
                    value[i] = reader.ReadString();

                return value.CreateList();
            }

            return length == 0 ? new List<string>() : null;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<string> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length > 0)
            {
                var value = new string[length];

                for (int i = 0; i < length; i++)
                    value[i] = reader.ReadString();

                return value.CreateList();
            }

            return length == 0 ? new List<string>() : null;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<string> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length == 0)
                return new List<string>();

            var value = new string[length];

            for (int i = 0; i < length; i++)
                value[i] = reader.ReadString();

            return value.CreateList();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<string> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length == 0)
                return new List<string>();

            var value = new string[length];

            for (int i = 0; i < length; i++)
                value[i] = reader.ReadString();

            return value.CreateList();
        }

    }

    internal sealed class ListGuidSerializer<TResolver> : Serializer<TResolver, List<Guid>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<Guid> value)
            => value is null ? 4 : value.Count * 16 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<Guid> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteGuidArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<Guid> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteGuidArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<Guid> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadGuidArray(length).CreateList()
                : (length == 0 ? new List<Guid>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<Guid> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadGuidArray(length).CreateList()
                : (length == 0 ? new List<Guid>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<Guid> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadGuidArray(length).CreateList() : new List<Guid>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<Guid> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadGuidArray(length).CreateList() : new List<Guid>();
        }

    }

    internal sealed class ListDateTimeSerializer<TResolver> : Serializer<TResolver, List<DateTime>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<DateTime> value)
            => value is null ? 4 : value.Count * 9 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<DateTime> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
            {
                var array = value.GetArray();

                for (int i = 0; i < length; i++)
                    writer.WriteDateTime(in array[i]);
            }
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<DateTime> value)
        {
            writer.WriteInt(value.Count);
            var array = value.GetArray();

            for (int i = 0; i < value.Count; i++)
                writer.WriteDateTime(in array[i]);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<DateTime> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length > 0)
            {
                var value = new DateTime[length];

                for (int i = 0; i < length; i++)
                    reader.AppendDateTime(ref value[i]);

                return value.CreateList();
            }

            return length == 0 ? new List<DateTime>() : null;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<DateTime> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length > 0)
            {
                var value = new DateTime[length];

                for (int i = 0; i < length; i++)
                    reader.AppendDateTimeInUTC(ref value[i]);

                return value.CreateList();
            }

            return length == 0 ? new List<DateTime>() : null;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<DateTime> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length == 0)
                return new List<DateTime>();

            var value = new DateTime[length];

            for (int i = 0; i < length; i++)
                reader.AppendDateTime(ref value[i]);

            return value.CreateList();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<DateTime> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length == 0)
                return new List<DateTime>();

            var value = new DateTime[length];

            for (int i = 0; i < length; i++)
                reader.AppendDateTimeInUTC(ref value[i]);

            return value.CreateList();
        }

    }

    internal sealed class ListDateTimeOffsetSerializer<TResolver> : Serializer<TResolver, List<DateTimeOffset>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<DateTimeOffset> value)
            => value is null ? 4 : value.Count * 10 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<DateTimeOffset> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
            {
                var array = value.GetArray();

                for (int i = 0; i < length; i++)
                {
                    var item = array[i];
                    writer.WriteDateTimeWithoutZone(new DateTime(item.Ticks, DateTimeKind.Utc));
                    writer.WriteShort((short)item.Offset.TotalMinutes);
                }
            }
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<DateTimeOffset> value)
        {
            writer.WriteInt(value.Count);
            var array = value.GetArray();

            for (int i = 0; i < value.Count; i++)
            {
                var item = array[i];
                writer.WriteDateTimeWithoutZone(new DateTime(item.Ticks, DateTimeKind.Utc));
                writer.WriteShort((short)item.Offset.TotalMinutes);
            }
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<DateTimeOffset> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length > 0)
            {
                var value = new DateTimeOffset[length];

                for (int i = 0; i < length; i++)
                    value[i] = new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort()));

                return value.CreateList();
            }

            return length == 0 ? new List<DateTimeOffset>() : null;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<DateTimeOffset> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length > 0)
            {
                var value = new DateTimeOffset[length];

                for (int i = 0; i < length; i++)
                    value[i] = new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort()));

                return value.CreateList();
            }

            return length == 0 ? new List<DateTimeOffset>() : null;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<DateTimeOffset> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length == 0)
                return new List<DateTimeOffset>();

            var value = new DateTimeOffset[length];

            for (int i = 0; i < length; i++)
                value[i] = new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort()));

            return value.CreateList();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<DateTimeOffset> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();

            if (length == 0)
                return new List<DateTimeOffset>();

            var value = new DateTimeOffset[length];

            for (int i = 0; i < length; i++)
                value[i] = new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort()));

            return value.CreateList();
        }

    }

    internal sealed class ListTimeSpanSerializer<TResolver> : Serializer<TResolver, List<TimeSpan>>
        where TResolver : Resolver
    {
        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(List<TimeSpan> value)
            => value is null ? 4 : value.Count * 8 + 4;

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, List<TimeSpan> value)
        {
            int length = value is null ? -1 : value.Count;
            writer.WriteInt(length);

            if (length > 0)
                writer.WriteTimeSpanArray(value.GetArray(), length);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, List<TimeSpan> value)
        {
            writer.WriteInt(value.Count);

            if (value.Count > 0)
                writer.WriteTimeSpanArray(value.GetArray(), value.Count);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<TimeSpan> Deserialize(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadTimeSpanArray(length).CreateList()
                : (length == 0 ? new List<TimeSpan>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<TimeSpan> DeserializeInUTC(ref Reader reader)
        {
            int length = reader.ReadInt();

            return length > 0
                ? reader.ReadTimeSpanArray(length).CreateList()
                : (length == 0 ? new List<TimeSpan>() : null);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<TimeSpan> DeserializeSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadTimeSpanArray(length).CreateList() : new List<TimeSpan>();
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed List<TimeSpan> DeserializeInUTCSpot(ref Reader reader)
        {
            int length = reader.ReadInt();
            return length > 0 ? reader.ReadTimeSpanArray(length).CreateList() : new List<TimeSpan>();
        }

    }

}
