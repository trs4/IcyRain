using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers;

internal sealed class ArrayBoolSerializer<TResolver> : Serializer<TResolver, bool[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(bool[] value)
        => value is null ? 4 : value.Length + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, bool[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteBoolArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, bool[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteBoolArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed bool[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadBoolArray(length)
            : (length == 0 ? Array.Empty<bool>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed bool[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadBoolArray(length)
            : (length == 0 ? Array.Empty<bool>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed bool[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadBoolArray(length) : Array.Empty<bool>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed bool[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadBoolArray(length) : Array.Empty<bool>();
    }

}

internal sealed class ArrayCharSerializer<TResolver> : Serializer<TResolver, char[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(char[] value)
        => value is null ? 4 : value.Length * 2 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, char[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteCharArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, char[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteCharArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed char[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadCharArray(length)
            : (length == 0 ? Array.Empty<char>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed char[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadCharArray(length)
            : (length == 0 ? Array.Empty<char>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed char[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadCharArray(length) : Array.Empty<char>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed char[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadCharArray(length) : Array.Empty<char>();
    }

}

internal sealed class ArraySByteSerializer<TResolver> : Serializer<TResolver, sbyte[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(sbyte[] value)
        => value is null ? 4 : value.Length + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, sbyte[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteSByteArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, sbyte[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteSByteArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed sbyte[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadSByteArray(length)
            : (length == 0 ? Array.Empty<sbyte>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed sbyte[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadSByteArray(length)
            : (length == 0 ? Array.Empty<sbyte>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed sbyte[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadSByteArray(length) : Array.Empty<sbyte>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed sbyte[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadSByteArray(length) : Array.Empty<sbyte>();
    }

}

internal sealed class ArrayByteSerializer<TResolver> : Serializer<TResolver, byte[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(byte[] value)
        => value is null ? 4 : value.Length + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, byte[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteByteArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, byte[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteByteArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed byte[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadByteArray(length)
            : (length == 0 ? Array.Empty<byte>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed byte[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadByteArray(length)
            : (length == 0 ? Array.Empty<byte>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed byte[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadByteArray(length) : Array.Empty<byte>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed byte[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadByteArray(length) : Array.Empty<byte>();
    }

}

internal sealed class ArrayShortSerializer<TResolver> : Serializer<TResolver, short[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(short[] value)
        => value is null ? 4 : value.Length * 2 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, short[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteShortArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, short[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteShortArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed short[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadShortArray(length)
            : (length == 0 ? Array.Empty<short>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed short[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadShortArray(length)
            : (length == 0 ? Array.Empty<short>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed short[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadShortArray(length) : Array.Empty<short>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed short[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadShortArray(length) : Array.Empty<short>();
    }

}

internal sealed class ArrayUShortSerializer<TResolver> : Serializer<TResolver, ushort[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(ushort[] value)
        => value is null ? 4 : value.Length * 2 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, ushort[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteUShortArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, ushort[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteUShortArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ushort[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadUShortArray(length)
            : (length == 0 ? Array.Empty<ushort>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ushort[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadUShortArray(length)
            : (length == 0 ? Array.Empty<ushort>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ushort[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadUShortArray(length) : Array.Empty<ushort>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ushort[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadUShortArray(length) : Array.Empty<ushort>();
    }

}

internal sealed class ArrayIntSerializer<TResolver> : Serializer<TResolver, int[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(int[] value)
        => value is null ? 4 : value.Length * 4 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, int[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteIntArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, int[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteIntArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadIntArray(length)
            : (length == 0 ? Array.Empty<int>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadIntArray(length)
            : (length == 0 ? Array.Empty<int>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadIntArray(length) : Array.Empty<int>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed int[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadIntArray(length) : Array.Empty<int>();
    }

}

internal sealed class ArrayUIntSerializer<TResolver> : Serializer<TResolver, uint[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(uint[] value)
        => value is null ? 4 : value.Length * 4 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, uint[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteUIntArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, uint[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteUIntArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed uint[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadUIntArray(length)
            : (length == 0 ? Array.Empty<uint>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed uint[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadUIntArray(length)
            : (length == 0 ? Array.Empty<uint>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed uint[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadUIntArray(length) : Array.Empty<uint>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed uint[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadUIntArray(length) : Array.Empty<uint>();
    }

}

internal sealed class ArrayLongSerializer<TResolver> : Serializer<TResolver, long[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(long[] value)
        => value is null ? 4 : value.Length * 8 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, long[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteLongArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, long[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteLongArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed long[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadLongArray(length)
            : (length == 0 ? Array.Empty<long>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed long[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadLongArray(length)
            : (length == 0 ? Array.Empty<long>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed long[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadLongArray(length) : Array.Empty<long>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed long[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadLongArray(length) : Array.Empty<long>();
    }

}

internal sealed class ArrayULongSerializer<TResolver> : Serializer<TResolver, ulong[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(ulong[] value)
        => value is null ? 4 : value.Length * 8 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, ulong[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteULongArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, ulong[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteULongArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ulong[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadULongArray(length)
            : (length == 0 ? Array.Empty<ulong>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ulong[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadULongArray(length)
            : (length == 0 ? Array.Empty<ulong>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ulong[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadULongArray(length) : Array.Empty<ulong>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed ulong[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadULongArray(length) : Array.Empty<ulong>();
    }

}

internal sealed class ArrayFloatSerializer<TResolver> : Serializer<TResolver, float[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(float[] value)
        => value is null ? 4 : value.Length * 4 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, float[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteFloatArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, float[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteFloatArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed float[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadFloatArray(length)
            : (length == 0 ? Array.Empty<float>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed float[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadFloatArray(length)
            : (length == 0 ? Array.Empty<float>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed float[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadFloatArray(length) : Array.Empty<float>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed float[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadFloatArray(length) : Array.Empty<float>();
    }

}

internal sealed class ArrayDoubleSerializer<TResolver> : Serializer<TResolver, double[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(double[] value)
        => value is null ? 4 : value.Length * 8 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, double[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteDoubleArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, double[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteDoubleArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed double[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadDoubleArray(length)
            : (length == 0 ? Array.Empty<double>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed double[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadDoubleArray(length)
            : (length == 0 ? Array.Empty<double>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed double[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadDoubleArray(length) : Array.Empty<double>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed double[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadDoubleArray(length) : Array.Empty<double>();
    }

}

internal sealed class ArrayDecimalSerializer<TResolver> : Serializer<TResolver, decimal[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(decimal[] value)
        => value is null ? 4 : value.Length * 16 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, decimal[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteDecimalArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, decimal[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteDecimalArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed decimal[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadDecimalArray(length)
            : (length == 0 ? Array.Empty<decimal>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed decimal[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadDecimalArray(length)
            : (length == 0 ? Array.Empty<decimal>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed decimal[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadDecimalArray(length) : Array.Empty<decimal>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed decimal[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadDecimalArray(length) : Array.Empty<decimal>();
    }

}

internal sealed class ArrayStringSerializer<TResolver> : Serializer<TResolver, string[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(string[] value)
    {
        int capacity = 4;

        if (value is not null)
        {
            for (int i = 0; i < value.Length; i++)
                capacity += StringEncoding.GetSize(value[i]);
        }

        return capacity;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, string[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
        {
            for (int i = 0; i < value.Length; i++)
                writer.WriteString(value[i]);
        }
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, string[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
        {
            for (int i = 0; i < value.Length; i++)
                writer.WriteString(value[i]);
        }
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed string[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new string[length];

            for (int i = 0; i < length; i++)
                value[i] = reader.ReadString();

            return value;
        }

        return length == 0 ? Array.Empty<string>() : null;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed string[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new string[length];

            for (int i = 0; i < length; i++)
                value[i] = reader.ReadString();

            return value;
        }

        return length == 0 ? Array.Empty<string>() : null;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed string[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return Array.Empty<string>();

        var value = new string[length];

        for (int i = 0; i < length; i++)
            value[i] = reader.ReadString();

        return value;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed string[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return Array.Empty<string>();

        var value = new string[length];

        for (int i = 0; i < length; i++)
            value[i] = reader.ReadString();

        return value;
    }

}

internal sealed class ArrayGuidSerializer<TResolver> : Serializer<TResolver, Guid[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(Guid[] value)
        => value is null ? 4 : value.Length * 16 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, Guid[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteGuidArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, Guid[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteGuidArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed Guid[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadGuidArray(length)
            : (length == 0 ? Array.Empty<Guid>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed Guid[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadGuidArray(length)
            : (length == 0 ? Array.Empty<Guid>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed Guid[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadGuidArray(length) : Array.Empty<Guid>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed Guid[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadGuidArray(length) : Array.Empty<Guid>();
    }

}

internal sealed class ArrayDateTimeSerializer<TResolver> : Serializer<TResolver, DateTime[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(DateTime[] value)
        => value is null ? 4 : value.Length * 9 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, DateTime[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
        {
            for (int i = 0; i < value.Length; i++)
                writer.WriteDateTime(in value[i]);
        }
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, DateTime[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
        {
            for (int i = 0; i < value.Length; i++)
                writer.WriteDateTime(in value[i]);
        }
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTime[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new DateTime[length];

            for (int i = 0; i < length; i++)
                reader.AppendDateTime(ref value[i]);

            return value;
        }

        return length == 0 ? Array.Empty<DateTime>() : null;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTime[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new DateTime[length];

            for (int i = 0; i < length; i++)
                reader.AppendDateTimeInUTC(ref value[i]);

            return value;
        }

        return length == 0 ? Array.Empty<DateTime>() : null;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTime[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return Array.Empty<DateTime>();

        var value = new DateTime[length];

        for (int i = 0; i < length; i++)
            reader.AppendDateTime(ref value[i]);

        return value;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTime[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return Array.Empty<DateTime>();

        var value = new DateTime[length];

        for (int i = 0; i < length; i++)
            reader.AppendDateTimeInUTC(ref value[i]);

        return value;
    }

}

internal sealed class ArrayDateTimeOffsetSerializer<TResolver> : Serializer<TResolver, DateTimeOffset[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(DateTimeOffset[] value)
        => value is null ? 4 : value.Length * 10 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, DateTimeOffset[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
        {
            for (int i = 0; i < value.Length; i++)
            {
                ref var item = ref value[i];
                writer.WriteDateTimeWithoutZone(new DateTime(item.Ticks, DateTimeKind.Utc));
                writer.WriteShort((short)item.Offset.TotalMinutes);
            }
        }
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, DateTimeOffset[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
        {
            for (int i = 0; i < value.Length; i++)
            {
                ref var item = ref value[i];
                writer.WriteDateTimeWithoutZone(new DateTime(item.Ticks, DateTimeKind.Utc));
                writer.WriteShort((short)item.Offset.TotalMinutes);
            }
        }
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTimeOffset[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new DateTimeOffset[length];

            for (int i = 0; i < length; i++)
                value[i] = new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort()));

            return value;
        }

        return length == 0 ? Array.Empty<DateTimeOffset>() : null;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTimeOffset[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length > 0)
        {
            var value = new DateTimeOffset[length];

            for (int i = 0; i < length; i++)
                value[i] = new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort()));

            return value;
        }

        return length == 0 ? Array.Empty<DateTimeOffset>() : null;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTimeOffset[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return Array.Empty<DateTimeOffset>();

        var value = new DateTimeOffset[length];

        for (int i = 0; i < length; i++)
            value[i] = new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort()));

        return value;
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed DateTimeOffset[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();

        if (length == 0)
            return Array.Empty<DateTimeOffset>();

        var value = new DateTimeOffset[length];

        for (int i = 0; i < length; i++)
            value[i] = new DateTimeOffset(reader.ReadDateTimeWithoutZone().Ticks, TimeSpan.FromMinutes(reader.ReadShort()));

        return value;
    }

}

internal sealed class ArrayTimeSpanSerializer<TResolver> : Serializer<TResolver, TimeSpan[]>
    where TResolver : Resolver
{
    [MethodImpl(Flags.HotPath)]
    public override sealed int? GetSize() => null;

    [MethodImpl(Flags.HotPath)]
    public override sealed int GetCapacity(TimeSpan[] value)
        => value is null ? 4 : value.Length * 8 + 4;

    [MethodImpl(Flags.HotPath)]
    public override sealed void Serialize(ref Writer writer, TimeSpan[] value)
    {
        int length = value is null ? -1 : value.Length;
        writer.WriteInt(length);

        if (length > 0)
            writer.WriteTimeSpanArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed void SerializeSpot(ref Writer writer, TimeSpan[] value)
    {
        writer.WriteInt(value.Length);

        if (value.Length > 0)
            writer.WriteTimeSpanArray(value);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed TimeSpan[] Deserialize(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadTimeSpanArray(length)
            : (length == 0 ? Array.Empty<TimeSpan>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed TimeSpan[] DeserializeInUTC(ref Reader reader)
    {
        int length = reader.ReadInt();

        return length > 0
            ? reader.ReadTimeSpanArray(length)
            : (length == 0 ? Array.Empty<TimeSpan>() : null);
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed TimeSpan[] DeserializeSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadTimeSpanArray(length) : Array.Empty<TimeSpan>();
    }

    [MethodImpl(Flags.HotPath)]
    public override sealed TimeSpan[] DeserializeInUTCSpot(ref Reader reader)
    {
        int length = reader.ReadInt();
        return length > 0 ? reader.ReadTimeSpanArray(length) : Array.Empty<TimeSpan>();
    }

}
