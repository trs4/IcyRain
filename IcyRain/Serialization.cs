using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Switchers;

namespace IcyRain
{
    /// <summary>Buffer fast serialization for concrete types</summary>
    public static class Serialization
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
        [MethodImpl(Flags.HotPath)]
        public static void Serialize<T>(IBufferWriter<byte> buffer, T value)
            => BufferSwitcher<T>.Instance.Serialize(buffer, value);

        /// <summary>Deserialize via buffer</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="sequence">Buffer</param>
        /// <param name="options">Deserialize options</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T Deserialize<T>(in ReadOnlySequence<byte> sequence, DeserializeOptions options = null)
             => BufferSwitcher<T>.Instance.Deserialize(sequence, options);

        #endregion
        #region Bytes

        /// <summary>Serialize via byte array</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Byte array</returns>
        [MethodImpl(Flags.HotPath)]
        public static byte[] Serialize<T>(T value)
            => BytesSwitcher<T>.Instance.Serialize(value);

        /// <summary>Deserialize via byte array</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="bytes">Byte array</param>
        /// <param name="options">Deserialize options</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T Deserialize<T>(byte[] bytes, DeserializeOptions options = null)
            => BytesSwitcher<T>.Instance.Deserialize(bytes, options);

        #endregion
        #region Segment

        /// <summary>Serialize via byte array segment</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Byte array segment</returns>
        [MethodImpl(Flags.HotPath)]
        public static ArraySegment<byte> SerializeSegment<T>(T value)
            => SegmentSwitcher<T>.Instance.Serialize(value);

        /// <summary>Deserialize via byte array segment</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="segment">Byte array segment</param>
        /// <param name="options">Deserialize options</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeserializeSegment<T>(ArraySegment<byte> segment, DeserializeOptions options = null)
            => SegmentSwitcher<T>.Instance.Deserialize(segment, options);

        #endregion
        #region Tests

        /// <summary>Serialize and deserialize via buffer</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeepClone<T>(T value)
        {
            using var buffer = new ArrayBufferWriter();
            Serialize(buffer, value);
            return Deserialize<T>(buffer.ToSequence());
        }

        #endregion
    }
}
