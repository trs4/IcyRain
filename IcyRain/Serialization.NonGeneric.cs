using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain
{
    /// <summary>Buffer fast serialization for concrete types</summary>
    public static partial class Serialization
    {
        public static class NonGeneric
        {
            #region Buffer

            /// <summary>Serialize via buffer</summary>
            /// <param name="type">Concrete type</param>
            /// <param name="buffer">Buffer</param>
            /// <param name="value">Serializable object</param>
            [MethodImpl(Flags.HotPath)]
            public static void Serialize(Type type, IBufferWriter<byte> buffer, object value)
                => GetMethod(type).SerializeBuffer(buffer, value);

            /// <summary>Deserialize via buffer</summary>
            /// <param name="type">Concrete type</param>
            /// <param name="sequence">Buffer</param>
            /// <param name="options">Deserialize options</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static object Deserialize(Type type, in ReadOnlySequence<byte> sequence, DeserializeOptions options = null)
                => GetMethod(type).DeserializeBuffer(sequence, options);

            #endregion
            #region Bytes

            /// <summary>Serialize via byte array</summary>
            /// <param name="type">Concrete type</param>
            /// <param name="value">Serializable object</param>
            /// <returns>Byte array</returns>
            [MethodImpl(Flags.HotPath)]
            public static byte[] Serialize(Type type, object value)
                => GetMethod(type).SerializeBytes(value);

            /// <summary>Deserialize via byte array</summary>
            /// <param name="type">Concrete type</param>
            /// <param name="bytes">Byte array</param>
            /// <param name="options">Deserialize options</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static object Deserialize(Type type, byte[] bytes, DeserializeOptions options = null)
                => GetMethod(type).DeserializeBytes(bytes, options);

            /// <summary>Deserialize via byte array</summary>
            /// <param name="type">Concrete type</param>
            /// <param name="bytes">Byte array</param>
            /// <param name="offset">Byte array offset</param>
            /// <param name="count">Byte array count</param>
            /// <param name="options">Deserialize options</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static object Deserialize(Type type, byte[] bytes, int offset, int count, DeserializeOptions options = null)
                => GetMethod(type).DeserializeBytesWithOffset(bytes, offset, count, options);

            #endregion
            #region Segment

            /// <summary>Serialize via byte array segment</summary>
            /// <param name="type">Concrete type</param>
            /// <param name="value">Serializable object</param>
            /// <returns>Byte array segment</returns>
            [MethodImpl(Flags.HotPath)]
            public static ArraySegment<byte> SerializeSegment(Type type, object value)
                => GetMethod(type).SerializeSegment(value);

            /// <summary>Deserialize via byte array segment</summary>
            /// <param name="type">Concrete type</param>
            /// <param name="segment">Byte array segment</param>
            /// <param name="options">Deserialize options</param>
            /// <returns>Deserialized object</returns>
            [MethodImpl(Flags.HotPath)]
            public static object DeserializeSegment(Type type, ArraySegment<byte> segment, DeserializeOptions options = null)
                => GetMethod(type).DeserializeSegment(segment, options);

            #endregion

            #region Internal

            private static readonly ConcurrentDictionary<Type, CompiledMethods> _methods = new ConcurrentDictionary<Type, CompiledMethods>();

            [MethodImpl(Flags.HotPath)]
            private static CompiledMethods GetMethod(Type type)
            {
                if (type is null)
                    throw new ArgumentNullException(nameof(type));

                return _methods.GetOrAdd(type, t => new CompiledMethods(t));
            }

            internal sealed class CompiledMethods
            {
                public readonly Action<IBufferWriter<byte>, object> SerializeBuffer;
                public readonly Func<ReadOnlySequence<byte>, DeserializeOptions, object> DeserializeBuffer;

                public readonly Func<object, byte[]> SerializeBytes;
                public readonly Func<byte[], DeserializeOptions, object> DeserializeBytes;
                public readonly Func<byte[], int, int, DeserializeOptions, object> DeserializeBytesWithOffset;

                public readonly Func<object, ArraySegment<byte>> SerializeSegment;
                public readonly Func<ArraySegment<byte>, DeserializeOptions, object> DeserializeSegment;

                public CompiledMethods(Type type)
                {
                    var methods = typeof(Serialization).GetMethods();

                    {
                        // void Serialize(Type type, IBufferWriter<byte> buffer, object value)
                        // void Serialize<T>(IBufferWriter<byte> buffer, T value)
                        var method = methods.First(m => m.Name == nameof(Serialization.Serialize) && m.GetParameters().Length == 2)
                            .MakeGenericMethod(type);

                        var param1 = Expression.Parameter(typeof(IBufferWriter<byte>), "buffer");
                        var param2 = Expression.Parameter(typeof(object), "value");
                        var param2Boxed = type.IsValueType ? Expression.Unbox(param2, type) : Expression.Convert(param2, type);

                        var body = Expression.Call(method, param1, param2Boxed);
                        SerializeBuffer = Expression.Lambda<Action<IBufferWriter<byte>, object>>(body, param1, param2).Compile();
                    }
                    {
                        // object Deserialize(Type type, in ReadOnlySequence<byte> sequence, DeserializeOptions options = null)
                        // T Deserialize<T>(in ReadOnlySequence<byte> sequence, DeserializeOptions options = null)
                        var method = methods.First(m => m.Name == nameof(Serialization.Deserialize) && m.GetParameters().Length == 2
                                && m.GetParameters()[0].Name == "sequence")
                            .MakeGenericMethod(type);

                        var param1 = Expression.Parameter(typeof(ReadOnlySequence<byte>), "sequence");
                        var param2 = Expression.Parameter(typeof(DeserializeOptions), "options");

                        var body = Expression.Convert(Expression.Call(method, param1, param2), typeof(object));
                        DeserializeBuffer = Expression.Lambda<Func<ReadOnlySequence<byte>, DeserializeOptions, object>>(body, param1, param2).Compile();
                    }

                    {
                        // byte[] Serialize(Type type, object value)
                        // byte[] Serialize<T>(T value)
                        var method = methods.First(m => m.Name == nameof(Serialization.Serialize) && m.GetParameters().Length == 1)
                            .MakeGenericMethod(type);

                        var param1 = Expression.Parameter(typeof(object), "value");
                        var param1Boxed = type.IsValueType ? Expression.Unbox(param1, type) : Expression.Convert(param1, type);

                        var body = Expression.Call(method, param1Boxed);
                        SerializeBytes = Expression.Lambda<Func<object, byte[]>>(body, param1).Compile();
                    }
                    {
                        // object Deserialize(Type type, byte[] bytes, DeserializeOptions options = null)
                        // T Deserialize<T>(byte[] bytes, DeserializeOptions options = null)
                        var method = methods.First(m => m.Name == nameof(Serialization.Deserialize) && m.GetParameters().Length == 2
                                && m.GetParameters()[0].ParameterType == typeof(byte[]))
                            .MakeGenericMethod(type);

                        var param1 = Expression.Parameter(typeof(byte[]), "bytes");
                        var param2 = Expression.Parameter(typeof(DeserializeOptions), "options");

                        var body = Expression.Convert(Expression.Call(method, param1, param2), typeof(object));
                        DeserializeBytes = Expression.Lambda<Func<byte[], DeserializeOptions, object>>(body, param1, param2).Compile();
                    }
                    {
                        // object Deserialize(Type type, byte[] bytes, int offset, int count, DeserializeOptions options = null)
                        // T Deserialize<T>(byte[] bytes, int offset, int count, DeserializeOptions options = null)
                        var method = methods.First(m => m.Name == nameof(Serialization.Deserialize) && m.GetParameters().Length == 4)
                            .MakeGenericMethod(type);

                        var param1 = Expression.Parameter(typeof(byte[]), "bytes");
                        var param2 = Expression.Parameter(typeof(int), "offset");
                        var param3 = Expression.Parameter(typeof(int), "count");
                        var param4 = Expression.Parameter(typeof(DeserializeOptions), "options");

                        var body = Expression.Convert(Expression.Call(method, param1, param2, param3, param4), typeof(object));

                        DeserializeBytesWithOffset = Expression.Lambda<Func<byte[], int, int, DeserializeOptions, object>>(
                            body, param1, param2, param3, param4).Compile();
                    }

                    {
                        // ArraySegment<byte> SerializeSegment(Type type, object value)
                        // ArraySegment<byte> SerializeSegment<T>(T value)
                        var method = methods.First(m => m.Name == nameof(Serialization.SerializeSegment))
                            .MakeGenericMethod(type);

                        var param1 = Expression.Parameter(typeof(object), "value");
                        var param1Boxed = type.IsValueType ? Expression.Unbox(param1, type) : Expression.Convert(param1, type);

                        var body = Expression.Call(method, param1Boxed);
                        SerializeSegment = Expression.Lambda<Func<object, ArraySegment<byte>>>(body, param1).Compile();
                    }
                    {
                        // object DeserializeSegment(Type type, ArraySegment<byte> segment, DeserializeOptions options = null)
                        // T DeserializeSegment<T>(ArraySegment<byte> segment, DeserializeOptions options = null)
                        var method = methods.First(m => m.Name == nameof(Serialization.DeserializeSegment))
                            .MakeGenericMethod(type);

                        var param1 = Expression.Parameter(typeof(ArraySegment<byte>), "segment");
                        var param2 = Expression.Parameter(typeof(DeserializeOptions), "options");

                        var body = Expression.Convert(Expression.Call(method, param1, param2), typeof(object));
                        DeserializeSegment = Expression.Lambda<Func<ArraySegment<byte>, DeserializeOptions, object>>(body, param1, param2).Compile();
                    }
                }

            }

            #endregion
        }
    }
}
