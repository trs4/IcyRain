using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain;

/// <summary>Buffer fast serialization for concrete types</summary>
public static partial class Serialization
{
    public static class Tests
    {
        #region Buffer

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

        /// <summary>Serialize and deserialize via buffer in UTC</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeepCloneInUTC<T>(T value)
        {
            using var buffer = new ArrayBufferWriter();
            Serialize(buffer, value);
            return DeserializeInUTC<T>(buffer.ToSequence());
        }

        /// <summary>Serialize and deserialize via buffer via LZ4</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeepCloneWithLZ4<T>(T value)
        {
            using var buffer = new ArrayBufferWriter();
            SerializeWithLZ4(buffer, value, out _);
            return DeserializeWithLZ4<T>(buffer.ToSequence(), buffer.Count, out _);
        }

        /// <summary>Serialize and deserialize via buffer in UTC and via LZ4</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeepCloneInUTCWithLZ4<T>(T value)
        {
            using var buffer = new ArrayBufferWriter();
            SerializeWithLZ4(buffer, value, out _);
            return DeserializeInUTCWithLZ4<T>(buffer.ToSequence(), buffer.Count, out _);
        }

        #endregion
        #region Bytes

        /// <summary>Serialize and deserialize via byte array</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeepCloneBytes<T>(T value)
        {
            byte[] bytes = Serialize(value);

            try
            {
                return Deserialize<T>(bytes);
            }
            finally
            {
                Buffers.ReturnWithCheck(bytes);
            }
        }

        /// <summary>Serialize and deserialize via byte array in UTC</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeepCloneBytesInUTC<T>(T value)
        {
            byte[] bytes = Serialize(value);

            try
            {
                return DeserializeInUTC<T>(bytes);
            }
            finally
            {
                Buffers.ReturnWithCheck(bytes);
            }
        }

        /// <summary>Serialize and deserialize via byte array via LZ4</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeepCloneBytesWithLZ4<T>(T value)
        {
            byte[] bytes = SerializeWithLZ4(value, out _);

            try
            {
                return DeserializeWithLZ4<T>(bytes, out _);
            }
            finally
            {
                Buffers.ReturnWithCheck(bytes);
            }
        }

        /// <summary>Serialize and deserialize via byte array in UTC and via LZ4</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeepCloneBytesInUTCWithLZ4<T>(T value)
        {
            byte[] bytes = SerializeWithLZ4(value, out _);

            try
            {
                return DeserializeInUTCWithLZ4<T>(bytes, out _);
            }
            finally
            {
                Buffers.ReturnWithCheck(bytes);
            }
        }

        #endregion
        #region Segment

        /// <summary>Serialize and deserialize via byte array segment</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeepCloneSegment<T>(T value)
        {
            var segment = SerializeSegment(value);

            try
            {
                return DeserializeSegment<T>(segment);
            }
            finally
            {
                Buffers.ReturnWithCheck(segment.Array);
            }
        }

        /// <summary>Serialize and deserialize via byte array segment in UTC</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeepCloneSegmentInUTC<T>(T value)
        {
            var segment = SerializeSegment(value);

            try
            {
                return DeserializeSegmentInUTC<T>(segment);
            }
            finally
            {
                Buffers.ReturnWithCheck(segment.Array);
            }
        }

        /// <summary>Serialize and deserialize via byte array segment via LZ4</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeepCloneSegmentWithLZ4<T>(T value)
        {
            var segment = SerializeSegmentWithLZ4(value, out _);

            try
            {
                return DeserializeSegmentWithLZ4<T>(segment, out _);
            }
            finally
            {
                Buffers.ReturnWithCheck(segment.Array);
            }
        }

        /// <summary>Serialize and deserialize via byte array segment in UTC and via LZ4</summary>
        /// <typeparam name="T">Concrete type</typeparam>
        /// <param name="value">Serializable object</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static T DeepCloneSegmentInUTCWithLZ4<T>(T value)
        {
            var segment = SerializeSegmentWithLZ4(value, out _);

            try
            {
                return DeserializeSegmentInUTCWithLZ4<T>(segment, out _);
            }
            finally
            {
                Buffers.ReturnWithCheck(segment.Array);
            }
        }

        #endregion
    }
}
