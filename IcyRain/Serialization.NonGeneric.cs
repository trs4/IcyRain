using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using IcyRain.Internal;

namespace IcyRain;

/// <summary>Buffer fast serialization for concrete types</summary>
public static partial class Serialization
{
    public static partial class NonGeneric
    {
        #region Buffer

        /// <summary>Serialize via buffer</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="buffer">Buffer</param>
        /// <param name="value">Serializable object</param>
        [MethodImpl(Flags.HotPath)]
        public static void Serialize(Type type, IBufferWriter<byte> buffer, object value)
            => GetMethod(type).SerializeBuffer(buffer, value);

        /// <summary>Serialize via buffer and compress via LZ4</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="buffer">Buffer</param>
        /// <param name="value">Serializable object</param>
        [MethodImpl(Flags.HotPath)]
        public static void SerializeWithLZ4(Type type, IBufferWriter<byte> buffer, object value)
            => GetMethod(type).SerializeWithLZ4Buffer(buffer, value);


        /// <summary>Deserialize via buffer</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="sequence">Buffer</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object Deserialize(Type type, in ReadOnlySequence<byte> sequence)
            => GetMethod(type).DeserializeBuffer(sequence);

        /// <summary>Deserialize via buffer in UTC</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="sequence">Buffer</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeInUTC(Type type, in ReadOnlySequence<byte> sequence)
             => GetMethod(type).DeserializeInUTCBuffer(sequence);

        /// <summary>Deserialize via buffer and decompress via LZ4</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="sequence">Buffer</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeWithLZ4(Type type, in ReadOnlySequence<byte> sequence)
             => GetMethod(type).DeserializeWithLZ4Buffer(sequence);

        /// <summary>Deserialize via buffer in UTC and decompress via LZ4</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="sequence">Buffer</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeInUTCWithLZ4(Type type, in ReadOnlySequence<byte> sequence)
             => GetMethod(type).DeserializeInUTCWithLZ4Buffer(sequence);

        #endregion
        #region Bytes

        /// <summary>Serialize via byte array</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="value">Serializable object</param>
        /// <returns>Byte array</returns>
        [MethodImpl(Flags.HotPath)]
        public static byte[] Serialize(Type type, object value)
            => GetMethod(type).SerializeBytes(value);

        /// <summary>Serialize via byte array and compress via LZ4</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="value">Serializable object</param>
        /// <returns>Byte array</returns>
        [MethodImpl(Flags.HotPath)]
        public static byte[] SerializeWithLZ4(Type type, object value)
            => GetMethod(type).SerializeWithLZ4Bytes(value);


        /// <summary>Deserialize via byte array</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="bytes">Byte array</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object Deserialize(Type type, byte[] bytes)
            => Deserialize(type, bytes, 0, bytes?.Length ?? 0);

        /// <summary>Deserialize via byte array</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="bytes">Byte array</param>
        /// <param name="offset">Byte array offset</param>
        /// <param name="count">Byte array count</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object Deserialize(Type type, byte[] bytes, int offset, int count)
            => GetMethod(type).DeserializeBytes(bytes, offset, count);

        /// <summary>Deserialize via byte array in UTC</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="bytes">Byte array</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeInUTC(Type type, byte[] bytes)
            => DeserializeInUTC(type, bytes, 0, bytes?.Length ?? 0);

        /// <summary>Deserialize via byte array in UTC</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="bytes">Byte array</param>
        /// <param name="offset">Byte array offset</param>
        /// <param name="count">Byte array count</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeInUTC(Type type, byte[] bytes, int offset, int count)
            => GetMethod(type).DeserializeInUTCBytes(bytes, offset, count);

        /// <summary>Deserialize via byte array and decompress via LZ4</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="bytes">Byte array</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeWithLZ4(Type type, byte[] bytes)
            => DeserializeWithLZ4(type, bytes, 0, bytes?.Length ?? 0);

        /// <summary>Deserialize via byte array and decompress via LZ4</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="bytes">Byte array</param>
        /// <param name="offset">Byte array offset</param>
        /// <param name="count">Byte array count</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeWithLZ4(Type type, byte[] bytes, int offset, int count)
            => GetMethod(type).DeserializeWithLZ4Bytes(bytes, offset, count);

        /// <summary>Deserialize via byte array in UTC and decompress via LZ4</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="bytes">Byte array</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeInUTCWithLZ4(Type type, byte[] bytes)
            => DeserializeInUTCWithLZ4(type, bytes, 0, bytes?.Length ?? 0);

        /// <summary>Deserialize via byte array in UTC and decompress via LZ4</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="bytes">Byte array</param>
        /// <param name="offset">Byte array offset</param>
        /// <param name="count">Byte array count</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeInUTCWithLZ4(Type type, byte[] bytes, int offset, int count)
            => GetMethod(type).DeserializeInUTCWithLZ4Bytes(bytes, offset, count);

        #endregion
        #region Segment

        /// <summary>Serialize via byte array segment</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="value">Serializable object</param>
        /// <returns>Byte array segment</returns>
        [MethodImpl(Flags.HotPath)]
        public static ArraySegment<byte> SerializeSegment(Type type, object value)
            => GetMethod(type).SerializeSegment(value);

        /// <summary>Serialize via byte array segment and compress via LZ4</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="value">Serializable object</param>
        /// <returns>Byte array segment</returns>
        [MethodImpl(Flags.HotPath)]
        public static ArraySegment<byte> SerializeSegmentWithLZ4(Type type, object value)
            => GetMethod(type).SerializeWithLZ4Segment(value);


        /// <summary>Deserialize via byte array segment</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="segment">Byte array segment</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeSegment(Type type, ArraySegment<byte> segment)
            => GetMethod(type).DeserializeSegment(segment);

        /// <summary>Deserialize via byte array segment in UTC</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="segment">Byte array segment</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeSegmentInUTC(Type type, ArraySegment<byte> segment)
            => GetMethod(type).DeserializeInUTCSegment(segment);

        /// <summary>Deserialize via byte array segment and compress via LZ4</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="segment">Byte array segment</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeSegmentWithLZ4(Type type, ArraySegment<byte> segment)
            => GetMethod(type).DeserializeWithLZ4Segment(segment);

        /// <summary>Deserialize via byte array segment in UTC and compress via LZ4</summary>
        /// <param name="type">Concrete type</param>
        /// <param name="segment">Byte array segment</param>
        /// <returns>Deserialized object</returns>
        [MethodImpl(Flags.HotPath)]
        public static object DeserializeSegmentInUTCWithLZ4(Type type, ArraySegment<byte> segment)
            => GetMethod(type).DeserializeInUTCWithLZ4Segment(segment);

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
            public readonly Action<IBufferWriter<byte>, object> SerializeWithLZ4Buffer;

            public readonly Func<ReadOnlySequence<byte>, object> DeserializeBuffer;
            public readonly Func<ReadOnlySequence<byte>, object> DeserializeInUTCBuffer;
            public readonly Func<ReadOnlySequence<byte>, object> DeserializeWithLZ4Buffer;
            public readonly Func<ReadOnlySequence<byte>, object> DeserializeInUTCWithLZ4Buffer;


            public readonly Func<object, byte[]> SerializeBytes;
            public readonly Func<object, byte[]> SerializeWithLZ4Bytes;

            public readonly Func<byte[], int, int, object> DeserializeBytes;
            public readonly Func<byte[], int, int, object> DeserializeInUTCBytes;
            public readonly Func<byte[], int, int, object> DeserializeWithLZ4Bytes;
            public readonly Func<byte[], int, int, object> DeserializeInUTCWithLZ4Bytes;


            public readonly Func<object, ArraySegment<byte>> SerializeSegment;
            public readonly Func<object, ArraySegment<byte>> SerializeWithLZ4Segment;

            public readonly Func<ArraySegment<byte>, object> DeserializeSegment;
            public readonly Func<ArraySegment<byte>, object> DeserializeInUTCSegment;
            public readonly Func<ArraySegment<byte>, object> DeserializeWithLZ4Segment;
            public readonly Func<ArraySegment<byte>, object> DeserializeInUTCWithLZ4Segment;

            public CompiledMethods(Type type)
            {
                var methods = typeof(Serialization).GetMethods();

                #region Buffer

                {
                    // void Serialize(Type type, IBufferWriter<byte> buffer, object value)
                    // void Serialize<T>(IBufferWriter<byte> buffer, T value)
                    var method = methods.First(m => m.Name == nameof(Serialization.Serialize) && m.GetParameters().Length == 2)
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(IBufferWriter<byte>), "buffer");
                    var param2 = Expression.Parameter(typeof(object), "value");
                    var param2Boxed = type.IsValueType ? Expression.Unbox(param2, type) : Expression.Convert(param2, type);

                    var body = Expression.Call(method, param1, param2Boxed);

                    SerializeBuffer = Expression.Lambda<Action<IBufferWriter<byte>, object>>(
                        body, param1, param2).Compile();
                }
                {
                    // void SerializeWithLZ4(Type type, IBufferWriter<byte> buffer, object value)
                    // void SerializeWithLZ4<T>(IBufferWriter<byte> buffer, T value)
                    var method = methods.First(m => m.Name == nameof(Serialization.SerializeWithLZ4) && m.GetParameters().Length == 2)
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(IBufferWriter<byte>), "buffer");
                    var param2 = Expression.Parameter(typeof(object), "value");
                    var param2Boxed = type.IsValueType ? Expression.Unbox(param2, type) : Expression.Convert(param2, type);

                    var body = Expression.Call(method, param1, param2Boxed);

                    SerializeWithLZ4Buffer = Expression.Lambda<Action<IBufferWriter<byte>, object>>(
                        body, param1, param2).Compile();
                }

                {
                    // object Deserialize(Type type, in ReadOnlySequence<byte> sequence)
                    // T Deserialize<T>(in ReadOnlySequence<byte> sequence)
                    var method = methods.First(m => m.Name == nameof(Serialization.Deserialize) && m.GetParameters().Length == 1
                            && m.GetParameters()[0].Name == "sequence")
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(ReadOnlySequence<byte>), "sequence");

                    var body = Expression.Convert(Expression.Call(method, param1), typeof(object));

                    DeserializeBuffer = Expression.Lambda<Func<ReadOnlySequence<byte>, object>>(
                        body, param1).Compile();
                }
                {
                    // object DeserializeInUTC(Type type, in ReadOnlySequence<byte> sequence)
                    // T DeserializeInUTC<T>(in ReadOnlySequence<byte> sequence)
                    var method = methods.First(m => m.Name == nameof(Serialization.DeserializeInUTC) && m.GetParameters().Length == 1
                            && m.GetParameters()[0].Name == "sequence")
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(ReadOnlySequence<byte>), "sequence");

                    var body = Expression.Convert(Expression.Call(method, param1), typeof(object));

                    DeserializeInUTCBuffer = Expression.Lambda<Func<ReadOnlySequence<byte>, object>>(
                        body, param1).Compile();
                }
                {
                    // object DeserializeWithLZ4(Type type, in ReadOnlySequence<byte> sequence)
                    // T DeserializeWithLZ4<T>(in ReadOnlySequence<byte> sequence)
                    var method = methods.First(m => m.Name == nameof(Serialization.DeserializeWithLZ4) && m.GetParameters().Length == 1
                            && m.GetParameters()[0].Name == "sequence")
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(ReadOnlySequence<byte>), "sequence");

                    var body = Expression.Convert(Expression.Call(method, param1), typeof(object));

                    DeserializeWithLZ4Buffer = Expression.Lambda<Func<ReadOnlySequence<byte>, object>>(
                        body, param1).Compile();
                }
                {
                    // object DeserializeInUTCWithLZ4(Type type, in ReadOnlySequence<byte> sequence)
                    // T DeserializeInUTCWithLZ4<T>(in ReadOnlySequence<byte> sequence)
                    var method = methods.First(m => m.Name == nameof(Serialization.DeserializeInUTCWithLZ4) && m.GetParameters().Length == 1
                            && m.GetParameters()[0].Name == "sequence")
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(ReadOnlySequence<byte>), "sequence");

                    var body = Expression.Convert(Expression.Call(method, param1), typeof(object));

                    DeserializeInUTCWithLZ4Buffer = Expression.Lambda<Func<ReadOnlySequence<byte>, object>>(
                        body, param1).Compile();
                }

                #endregion
                #region Bytes

                {
                    // byte[] Serialize(Type type, object value)
                    // byte[] Serialize<T>(T value)
                    var method = methods.First(m => m.Name == nameof(Serialization.Serialize) && m.GetParameters().Length == 1)
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(object), "value");
                    var param1Boxed = type.IsValueType ? Expression.Unbox(param1, type) : Expression.Convert(param1, type);

                    var body = Expression.Call(method, param1Boxed);

                    SerializeBytes = Expression.Lambda<Func<object, byte[]>>(
                        body, param1).Compile();
                }
                {
                    // byte[] SerializeWithLZ4(Type type, object value)
                    // byte[] SerializeWithLZ4<T>(T value)
                    var method = methods.First(m => m.Name == nameof(Serialization.SerializeWithLZ4) && m.GetParameters().Length == 1)
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(object), "value");
                    var param1Boxed = type.IsValueType ? Expression.Unbox(param1, type) : Expression.Convert(param1, type);

                    var body = Expression.Call(method, param1Boxed);

                    SerializeWithLZ4Bytes = Expression.Lambda<Func<object, byte[]>>(
                        body, param1).Compile();
                }

                {
                    // object Deserialize(Type type, byte[] bytes, int offset, int count)
                    // T Deserialize<T>(byte[] bytes, int offset, int count)
                    var method = methods.First(m => m.Name == nameof(Serialization.Deserialize) && m.GetParameters().Length == 3)
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(byte[]), "bytes");
                    var param2 = Expression.Parameter(typeof(int), "offset");
                    var param3 = Expression.Parameter(typeof(int), "count");

                    var body = Expression.Convert(Expression.Call(method, param1, param2, param3), typeof(object));

                    DeserializeBytes = Expression.Lambda<Func<byte[], int, int, object>>(
                        body, param1, param2, param3).Compile();
                }
                {
                    // object DeserializeInUTC(Type type, byte[] bytes, int offset, int count)
                    // T DeserializeInUTC<T>(byte[] bytes, int offset, int count)
                    var method = methods.First(m => m.Name == nameof(Serialization.DeserializeInUTC) && m.GetParameters().Length == 3)
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(byte[]), "bytes");
                    var param2 = Expression.Parameter(typeof(int), "offset");
                    var param3 = Expression.Parameter(typeof(int), "count");

                    var body = Expression.Convert(Expression.Call(method, param1, param2, param3), typeof(object));

                    DeserializeInUTCBytes = Expression.Lambda<Func<byte[], int, int, object>>(
                        body, param1, param2, param3).Compile();
                }
                {
                    // object DeserializeWithLZ4(Type type, byte[] bytes, int offset, int count)
                    // T DeserializeWithLZ4<T>(byte[] bytes, int offset, int count)
                    var method = methods.First(m => m.Name == nameof(Serialization.DeserializeWithLZ4) && m.GetParameters().Length == 3
                            && m.GetParameters()[0].ParameterType == typeof(byte[]))
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(byte[]), "bytes");
                    var param2 = Expression.Parameter(typeof(int), "offset");
                    var param3 = Expression.Parameter(typeof(int), "count");

                    var body = Expression.Convert(Expression.Call(method, param1, param2, param3), typeof(object));

                    DeserializeWithLZ4Bytes = Expression.Lambda<Func<byte[], int, int, object>>(
                        body, param1, param2, param3).Compile();
                }
                {
                    // object DeserializeInUTCWithLZ4(Type type, byte[] bytes, int offset, int count)
                    // T DeserializeInUTCWithLZ4<T>(byte[] bytes, int offset, int count)
                    var method = methods.First(m => m.Name == nameof(Serialization.DeserializeInUTCWithLZ4) && m.GetParameters().Length == 3
                            && m.GetParameters()[0].ParameterType == typeof(byte[]))
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(byte[]), "bytes");
                    var param2 = Expression.Parameter(typeof(int), "offset");
                    var param3 = Expression.Parameter(typeof(int), "count");

                    var body = Expression.Convert(Expression.Call(method, param1, param2, param3), typeof(object));

                    DeserializeInUTCWithLZ4Bytes = Expression.Lambda<Func<byte[], int, int, object>>(
                        body, param1, param2, param3).Compile();
                }

                #endregion
                #region Segment

                {
                    // ArraySegment<byte> SerializeSegment(Type type, object value)
                    // ArraySegment<byte> SerializeSegment<T>(T value)
                    var method = methods.First(m => m.Name == nameof(Serialization.SerializeSegment))
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(object), "value");
                    var param1Boxed = type.IsValueType ? Expression.Unbox(param1, type) : Expression.Convert(param1, type);

                    var body = Expression.Call(method, param1Boxed);

                    SerializeSegment = Expression.Lambda<Func<object, ArraySegment<byte>>>(
                        body, param1).Compile();
                }
                {
                    // ArraySegment<byte> SerializeSegmentWithLZ4(Type type, object value)
                    // ArraySegment<byte> SerializeSegmentWithLZ4<T>(T value)
                    var method = methods.First(m => m.Name == nameof(Serialization.SerializeSegmentWithLZ4))
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(object), "value");
                    var param1Boxed = type.IsValueType ? Expression.Unbox(param1, type) : Expression.Convert(param1, type);

                    var body = Expression.Call(method, param1Boxed);

                    SerializeWithLZ4Segment = Expression.Lambda<Func<object, ArraySegment<byte>>>(
                        body, param1).Compile();
                }

                {
                    // object DeserializeSegment(Type type, ArraySegment<byte> segment)
                    // T DeserializeSegment<T>(ArraySegment<byte> segment)
                    var method = methods.First(m => m.Name == nameof(Serialization.DeserializeSegment))
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(ArraySegment<byte>), "segment");

                    var body = Expression.Convert(Expression.Call(method, param1), typeof(object));

                    DeserializeSegment = Expression.Lambda<Func<ArraySegment<byte>, object>>(
                        body, param1).Compile();
                }
                {
                    // object DeserializeSegmentInUTC(Type type, ArraySegment<byte> segment)
                    // T DeserializeSegmentInUTC<T>(ArraySegment<byte> segment)
                    var method = methods.First(m => m.Name == nameof(Serialization.DeserializeSegmentInUTC))
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(ArraySegment<byte>), "segment");

                    var body = Expression.Convert(Expression.Call(method, param1), typeof(object));

                    DeserializeInUTCSegment = Expression.Lambda<Func<ArraySegment<byte>, object>>(
                        body, param1).Compile();
                }
                {
                    // object DeserializeSegmentWithLZ4(Type type, ArraySegment<byte> segment)
                    // T DeserializeSegmentWithLZ4<T>(ArraySegment<byte> segment)
                    var method = methods.First(m => m.Name == nameof(Serialization.DeserializeSegmentWithLZ4))
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(ArraySegment<byte>), "segment");

                    var body = Expression.Convert(Expression.Call(method, param1), typeof(object));

                    DeserializeWithLZ4Segment = Expression.Lambda<Func<ArraySegment<byte>, object>>(
                        body, param1).Compile();
                }
                {
                    // object DeserializeSegmentInUTCWithLZ4(Type type, ArraySegment<byte> segment)
                    // T DeserializeSegmentInUTCWithLZ4<T>(ArraySegment<byte> segment)
                    var method = methods.First(m => m.Name == nameof(Serialization.DeserializeSegmentInUTCWithLZ4))
                        .MakeGenericMethod(type);

                    var param1 = Expression.Parameter(typeof(ArraySegment<byte>), "segment");

                    var body = Expression.Convert(Expression.Call(method, param1), typeof(object));

                    DeserializeInUTCWithLZ4Segment = Expression.Lambda<Func<ArraySegment<byte>, object>>(
                        body, param1).Compile();
                }

                #endregion
            }

        }

        #endregion
    }
}
