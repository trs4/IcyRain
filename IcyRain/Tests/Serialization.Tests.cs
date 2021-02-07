using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain
{
    /// <summary>Buffer fast serialization for concrete types</summary>
    public static partial class Serialization
    {
        public static class Tests
        {
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

            /// <summary>Serialize and deserialize via byte array</summary>
            /// <typeparam name="T">Concrete type</typeparam>
            /// <param name="value">Serializable object</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static T DeepCloneBytes<T>(T value)
            {
                byte[] bytes = Serialize(value);
                return Deserialize<T>(bytes);
            }

            /// <summary>Serialize and deserialize via byte array segment</summary>
            /// <typeparam name="T">Concrete type</typeparam>
            /// <param name="value">Serializable object</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static T DeepCloneSegment<T>(T value)
            {
                var segment = SerializeSegment(value);
                return DeserializeSegment<T>(segment);
            }

        }
    }
}
