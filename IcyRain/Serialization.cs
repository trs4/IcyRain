using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Switchers;

namespace IcyRain;

/// <summary>Buffer fast serialization for concrete types</summary>
public static partial class Serialization
{
    #region Common

    static Serialization()
    {
        if (!BitConverter.IsLittleEndian)
            throw new NotSupportedException("Supports only Little-Endian environments");
    }

    /// <summary>Prepare serializator caches</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    [MethodImpl(Flags.HotPath)]
    public static void Prepare<T>()
        => PrepareSwitcher<T>.Instance.Prepare();

    #endregion
    #region Buffer

    /// <summary>Serialize via buffer</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="buffer">Buffer</param>
    /// <param name="value">Serializable object</param>
    /// <returns>Length of buffer</returns>
    [MethodImpl(Flags.HotPath)]
    public static int Serialize<T>(IBufferWriter<byte> buffer, T value)
        => BufferSwitcher<T>.Instance.Serialize(buffer, value);

    /// <summary>Serialize via buffer and compress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="buffer">Buffer</param>
    /// <param name="value">Serializable object</param>
    /// <returns>Length of buffer</returns>
    [MethodImpl(Flags.HotPath)]
    public static int SerializeWithLZ4<T>(IBufferWriter<byte> buffer, T value)
        => BufferSwitcher<T>.Instance.SerializeWithLZ4(buffer, value, out _);

    /// <summary>Serialize via buffer and compress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="buffer">Buffer</param>
    /// <param name="value">Serializable object</param>
    /// <param name="serializedLength">Length of buffer on serialization</param>
    /// <returns>Length of buffer</returns>
    [MethodImpl(Flags.HotPath)]
    public static int SerializeWithLZ4<T>(IBufferWriter<byte> buffer, T value, out int serializedLength)
        => BufferSwitcher<T>.Instance.SerializeWithLZ4(buffer, value, out serializedLength);


    /// <summary>Deserialize via buffer</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="sequence">Buffer</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T Deserialize<T>(in ReadOnlySequence<byte> sequence)
         => BufferSwitcher<T>.Instance.Deserialize(sequence);

    /// <summary>Deserialize via buffer in UTC</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="sequence">Buffer</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeInUTC<T>(in ReadOnlySequence<byte> sequence)
         => BufferSwitcher<T>.Instance.DeserializeInUTC(sequence);

    /// <summary>Deserialize via buffer and decompress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="sequence">Buffer</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeWithLZ4<T>(in ReadOnlySequence<byte> sequence)
         => BufferSwitcher<T>.Instance.DeserializeWithLZ4(sequence, (int)sequence.Length, out _);

    /// <summary>Deserialize via buffer and decompress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="sequence">Buffer</param>
    /// <param name="sequenceLength">Length of buffer</param>
    /// <param name="decodedLength">Decoded length of buffer</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeWithLZ4<T>(in ReadOnlySequence<byte> sequence, int sequenceLength, out int decodedLength)
         => BufferSwitcher<T>.Instance.DeserializeWithLZ4(sequence, sequenceLength, out decodedLength);

    /// <summary>Deserialize via buffer in UTC and decompress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="sequence">Buffer</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeInUTCWithLZ4<T>(in ReadOnlySequence<byte> sequence)
         => BufferSwitcher<T>.Instance.DeserializeInUTCWithLZ4(sequence, (int)sequence.Length, out _);

    /// <summary>Deserialize via buffer in UTC and decompress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="sequence">Buffer</param>
    /// <param name="sequenceLength">Length of buffer</param>
    /// <param name="decodedLength">Decoded length of buffer</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeInUTCWithLZ4<T>(in ReadOnlySequence<byte> sequence, int sequenceLength, out int decodedLength)
         => BufferSwitcher<T>.Instance.DeserializeInUTCWithLZ4(sequence, sequenceLength, out decodedLength);

    #endregion
    #region Bytes

    /// <summary>Serialize via byte array</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="value">Serializable object</param>
    /// <returns>Byte array</returns>
    [MethodImpl(Flags.HotPath)]
    public static byte[] Serialize<T>(T value)
        => BytesSwitcher<T>.Instance.Serialize(value);

    /// <summary>Serialize via byte array and compress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="value">Serializable object</param>
    /// <returns>Byte array</returns>
    [MethodImpl(Flags.HotPath)]
    public static byte[] SerializeWithLZ4<T>(T value)
        => BytesSwitcher<T>.Instance.SerializeWithLZ4(value, out _);

    /// <summary>Serialize via byte array and compress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="value">Serializable object</param>
    /// <param name="serializedLength">Length of buffer on serialization</param>
    /// <returns>Byte array</returns>
    [MethodImpl(Flags.HotPath)]
    public static byte[] SerializeWithLZ4<T>(T value, out int serializedLength)
        => BytesSwitcher<T>.Instance.SerializeWithLZ4(value, out serializedLength);


    /// <summary>Deserialize via byte array</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="bytes">Byte array</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T Deserialize<T>(byte[] bytes)
        => Deserialize<T>(bytes, 0, bytes?.Length ?? 0);

    /// <summary>Deserialize via byte array</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="bytes">Byte array</param>
    /// <param name="offset">Byte array offset</param>
    /// <param name="count">Byte array count</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T Deserialize<T>(byte[] bytes, int offset, int count)
        => BytesSwitcher<T>.Instance.Deserialize(bytes, offset, count);

    /// <summary>Deserialize via byte array in UTC</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="bytes">Byte array</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeInUTC<T>(byte[] bytes)
        => DeserializeInUTC<T>(bytes, 0, bytes?.Length ?? 0);

    /// <summary>Deserialize via byte array in UTC</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="bytes">Byte array</param>
    /// <param name="offset">Byte array offset</param>
    /// <param name="count">Byte array count</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeInUTC<T>(byte[] bytes, int offset, int count)
        => BytesSwitcher<T>.Instance.DeserializeInUTC(bytes, offset, count);

    /// <summary>Deserialize via byte array and decompress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="bytes">Byte array</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeWithLZ4<T>(byte[] bytes)
        => DeserializeWithLZ4<T>(bytes, 0, bytes?.Length ?? 0, out _);

    /// <summary>Deserialize via byte array and decompress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="bytes">Byte array</param>
    /// <param name="decodedLength">Decoded length of byte array</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeWithLZ4<T>(byte[] bytes, out int decodedLength)
        => DeserializeWithLZ4<T>(bytes, 0, bytes?.Length ?? 0, out decodedLength);

    /// <summary>Deserialize via byte array and decompress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="bytes">Byte array</param>
    /// <param name="offset">Byte array offset</param>
    /// <param name="count">Byte array count</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeWithLZ4<T>(byte[] bytes, int offset, int count)
        => BytesSwitcher<T>.Instance.DeserializeWithLZ4(bytes, offset, count, out _);

    /// <summary>Deserialize via byte array and decompress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="bytes">Byte array</param>
    /// <param name="offset">Byte array offset</param>
    /// <param name="count">Byte array count</param>
    /// <param name="decodedLength">Decoded length of byte array</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeWithLZ4<T>(byte[] bytes, int offset, int count, out int decodedLength)
        => BytesSwitcher<T>.Instance.DeserializeWithLZ4(bytes, offset, count, out decodedLength);

    /// <summary>Deserialize via byte array in UTC and decompress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="bytes">Byte array</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeInUTCWithLZ4<T>(byte[] bytes)
        => DeserializeInUTCWithLZ4<T>(bytes, 0, bytes?.Length ?? 0, out _);

    /// <summary>Deserialize via byte array in UTC and decompress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="bytes">Byte array</param>
    /// <param name="decodedLength">Decoded length of byte array</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeInUTCWithLZ4<T>(byte[] bytes, out int decodedLength)
        => DeserializeInUTCWithLZ4<T>(bytes, 0, bytes?.Length ?? 0, out decodedLength);

    /// <summary>Deserialize via byte array in UTC and decompress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="bytes">Byte array</param>
    /// <param name="offset">Byte array offset</param>
    /// <param name="count">Byte array count</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeInUTCWithLZ4<T>(byte[] bytes, int offset, int count)
        => BytesSwitcher<T>.Instance.DeserializeInUTCWithLZ4(bytes, offset, count, out _);

    /// <summary>Deserialize via byte array in UTC and decompress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="bytes">Byte array</param>
    /// <param name="offset">Byte array offset</param>
    /// <param name="count">Byte array count</param>
    /// <param name="decodedLength">Decoded length of byte array</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeInUTCWithLZ4<T>(byte[] bytes, int offset, int count, out int decodedLength)
        => BytesSwitcher<T>.Instance.DeserializeInUTCWithLZ4(bytes, offset, count, out decodedLength);

    #endregion
    #region Segment

    /// <summary>Serialize via byte array segment</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="value">Serializable object</param>
    /// <returns>Byte array segment</returns>
    [MethodImpl(Flags.HotPath)]
    public static ArraySegment<byte> SerializeSegment<T>(T value)
        => SegmentSwitcher<T>.Instance.Serialize(value);

    /// <summary>Serialize via byte array segment and compress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="value">Serializable object</param>
    /// <returns>Byte array segment</returns>
    [MethodImpl(Flags.HotPath)]
    public static ArraySegment<byte> SerializeSegmentWithLZ4<T>(T value)
        => SegmentSwitcher<T>.Instance.SerializeWithLZ4(value, out _);

    /// <summary>Serialize via byte array segment and compress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="value">Serializable object</param>
    /// <param name="serializedLength">Length of buffer on serialization</param>
    /// <returns>Byte array segment</returns>
    [MethodImpl(Flags.HotPath)]
    public static ArraySegment<byte> SerializeSegmentWithLZ4<T>(T value, out int serializedLength)
        => SegmentSwitcher<T>.Instance.SerializeWithLZ4(value, out serializedLength);


    /// <summary>Deserialize via byte array segment</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="segment">Byte array segment</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeSegment<T>(ArraySegment<byte> segment)
        => SegmentSwitcher<T>.Instance.Deserialize(segment);

    /// <summary>Deserialize via byte array segment in UTC</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="segment">Byte array segment</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeSegmentInUTC<T>(ArraySegment<byte> segment)
        => SegmentSwitcher<T>.Instance.DeserializeInUTC(segment);

    /// <summary>Deserialize via byte array segment and compress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="segment">Byte array segment</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeSegmentWithLZ4<T>(ArraySegment<byte> segment)
        => SegmentSwitcher<T>.Instance.DeserializeWithLZ4(segment, out _);

    /// <summary>Deserialize via byte array segment and compress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="segment">Byte array segment</param>
    /// <param name="decodedLength">Decoded length of byte array segment</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeSegmentWithLZ4<T>(ArraySegment<byte> segment, out int decodedLength)
        => SegmentSwitcher<T>.Instance.DeserializeWithLZ4(segment, out decodedLength);

    /// <summary>Deserialize via byte array segment in UTC and compress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="segment">Byte array segment</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeSegmentInUTCWithLZ4<T>(ArraySegment<byte> segment)
        => SegmentSwitcher<T>.Instance.DeserializeInUTCWithLZ4(segment, out _);

    /// <summary>Deserialize via byte array segment in UTC and compress via LZ4</summary>
    /// <typeparam name="T">Concrete type</typeparam>
    /// <param name="segment">Byte array segment</param>
    /// <param name="decodedLength">Decoded length of byte array segment</param>
    /// <returns>Deserialized object</returns>
    [MethodImpl(Flags.HotPath)]
    public static T DeserializeSegmentInUTCWithLZ4<T>(ArraySegment<byte> segment, out int decodedLength)
        => SegmentSwitcher<T>.Instance.DeserializeInUTCWithLZ4(segment, out decodedLength);

    #endregion
}
