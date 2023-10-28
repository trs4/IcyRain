using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain;

/// <summary>Buffer fast serialization for concrete types</summary>
public static partial class Serialization
{
    public static partial class NonGeneric
    {
        public static partial class Tests
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
                Serialize(typeof(T), buffer, value);
                return (T)Deserialize(typeof(T), buffer.ToSequence());
            }

            /// <summary>Serialize and deserialize via buffer in UTC</summary>
            /// <typeparam name="T">Concrete type</typeparam>
            /// <param name="value">Serializable object</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static T DeepCloneInUTC<T>(T value)
            {
                using var buffer = new ArrayBufferWriter();
                Serialize(typeof(T), buffer, value);
                return (T)DeserializeInUTC(typeof(T), buffer.ToSequence());
            }

            /// <summary>Serialize and deserialize via buffer via LZ4</summary>
            /// <typeparam name="T">Concrete type</typeparam>
            /// <param name="value">Serializable object</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static T DeepCloneWithLZ4<T>(T value)
            {
                using var buffer = new ArrayBufferWriter();
                SerializeWithLZ4(typeof(T), buffer, value);
                return (T)DeserializeWithLZ4(typeof(T), buffer.ToSequence());
            }

            /// <summary>Serialize and deserialize via buffer in UTC and via LZ4</summary>
            /// <typeparam name="T">Concrete type</typeparam>
            /// <param name="value">Serializable object</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static T DeepCloneInUTCWithLZ4<T>(T value)
            {
                using var buffer = new ArrayBufferWriter();
                SerializeWithLZ4(typeof(T), buffer, value);
                return (T)DeserializeInUTCWithLZ4(typeof(T), buffer.ToSequence());
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
                byte[] bytes = Serialize(typeof(T), value);
                return (T)Deserialize(typeof(T), bytes);
            }

            /// <summary>Serialize and deserialize via byte array in UTC</summary>
            /// <typeparam name="T">Concrete type</typeparam>
            /// <param name="value">Serializable object</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static T DeepCloneBytesInUTC<T>(T value)
            {
                byte[] bytes = Serialize(typeof(T), value);
                return (T)DeserializeInUTC(typeof(T), bytes);
            }

            /// <summary>Serialize and deserialize via byte array via LZ4</summary>
            /// <typeparam name="T">Concrete type</typeparam>
            /// <param name="value">Serializable object</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static T DeepCloneBytesWithLZ4<T>(T value)
            {
                byte[] bytes = SerializeWithLZ4(typeof(T), value);
                return (T)DeserializeWithLZ4(typeof(T), bytes);
            }

            /// <summary>Serialize and deserialize via byte array in UTC and via LZ4</summary>
            /// <typeparam name="T">Concrete type</typeparam>
            /// <param name="value">Serializable object</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static T DeepCloneBytesInUTCWithLZ4<T>(T value)
            {
                byte[] bytes = SerializeWithLZ4(typeof(T), value);
                return (T)DeserializeInUTCWithLZ4(typeof(T), bytes);
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
                var segment = SerializeSegment(typeof(T), value);
                return (T)DeserializeSegment(typeof(T), segment);
            }

            /// <summary>Serialize and deserialize via byte array segment in UTC</summary>
            /// <typeparam name="T">Concrete type</typeparam>
            /// <param name="value">Serializable object</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static T DeepCloneSegmentInUTC<T>(T value)
            {
                var segment = SerializeSegment(typeof(T), value);
                return (T)DeserializeSegmentInUTC(typeof(T), segment);
            }

            /// <summary>Serialize and deserialize via byte array segment via LZ4</summary>
            /// <typeparam name="T">Concrete type</typeparam>
            /// <param name="value">Serializable object</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static T DeepCloneSegmentWithLZ4<T>(T value)
            {
                var segment = SerializeSegmentWithLZ4(typeof(T), value);
                return (T)DeserializeSegmentWithLZ4(typeof(T), segment);
            }

            /// <summary>Serialize and deserialize via byte array segment in UTC and via LZ4</summary>
            /// <typeparam name="T">Concrete type</typeparam>
            /// <param name="value">Serializable object</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static T DeepCloneSegmentInUTCWithLZ4<T>(T value)
            {
                var segment = SerializeSegmentWithLZ4(typeof(T), value);
                return (T)DeserializeSegmentInUTCWithLZ4(typeof(T), segment);
            }

            #endregion
        }
    }
}
