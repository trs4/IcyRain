using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using IcyRain.Builders;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers
{
    internal static class Builder<TResolver>
        where TResolver : Resolver
    {
        public static object Get<T>()
        {
            var type = typeof(T);

            if (_primitiveMap.TryGetValue(type, out var serializer))
                return serializer();
            else if (type.IsEnum)
                return Types.CreateInstance<TResolver>(Types.EnumSerializer, type);
            else if (type.IsArray)
            {
                var elementType = type.GetElementType();

                if (elementType.IsEnum)
                    return Types.CreateInstance<TResolver>(Types.ArrayEnumSerializer, type);

                return new ArraySerializer<TResolver, T>();
            }
            else if (type.IsGenericType)
            {
                var typeDefinition = type.GetGenericTypeDefinition();
                var argumentTypes = type.GetGenericArguments();
                var argumentType = argumentTypes[0];

                if (argumentType.IsEnum && argumentTypes.Length == 1)
                {
                    if (typeDefinition == Types.Nullable)
                        return Types.CreateInstance<TResolver>(Types.NullableEnumSerializer, argumentType);
                    else if (typeDefinition == Types.List)
                        return Types.CreateInstance<TResolver>(Types.ListEnumSerializer, type);
                }

                if (_genericMap.TryGetValue(typeDefinition, out var serializerType))
                    return Types.CreateInstance<TResolver>(serializerType, argumentTypes);

                if (type.FullName.StartsWith("System.Tuple"))
                    return GetTuple<T>(argumentTypes);
                else if (type.FullName.StartsWith("System.ValueTuple"))
                    return GetValueTuple<T>(argumentTypes);
                else if (Types.TryGetIDictionatyArgumentTypes(type, out var iDictionaryTypes))
                    return Types.CreateInstance(Types.ObjectDictionarySerializer, typeof(TResolver), iDictionaryTypes[0], iDictionaryTypes[0], type);
                else if (Types.TryGetICollectionArgumentType(type, out var iCollectionType))
                    return Types.CreateInstance(Types.ObjectCollectionSerializer, typeof(TResolver), iCollectionType, type);
            }

            if (type.IsValueType)
                return null; // %%TODO

            if (type.HasDataContractAttribute())
            {
                if (typeof(TResolver) == Types.UnionResolver && TryGetUnionTypes(type, out var unionTypes, out bool isAbstractBaseType))
                {
                    return unionTypes.Length <= UnionHashBuilder.TypesCount
                        ? UnionBuilder.Get(type, unionTypes, isAbstractBaseType)
                        : UnionHashBuilder.Get(type, unionTypes);
                }

                return ObjectBuilder.Get(type);
            }
            else if (type.HasCollectionDataContractAttribute())
            {
                if (Types.TryGetIDictionatyArgumentTypes(type, out var iDictionaryTypes))
                    return Types.CreateInstance(Types.ObjectDictionarySerializer, typeof(TResolver), iDictionaryTypes[0], iDictionaryTypes[0], type);
                else if (Types.TryGetICollectionArgumentType(type, out var iCollectionType))
                    return Types.CreateInstance(Types.ObjectCollectionSerializer, typeof(TResolver), iCollectionType, type);
            }

            return null;
        }

        private static object GetTuple<T>(Type[] argumentTypes)
            => argumentTypes.Length switch
            {
                1 => Types.CreateInstance<TResolver>(typeof(TupleSerializer<,>), argumentTypes[0]),
                2 => Types.CreateInstance<TResolver>(typeof(TupleSerializer<,,>), argumentTypes),
                3 => Types.CreateInstance<TResolver>(typeof(TupleSerializer<,,,>), argumentTypes),
                4 => Types.CreateInstance<TResolver>(typeof(TupleSerializer<,,,,>), argumentTypes),
                5 => Types.CreateInstance<TResolver>(typeof(TupleSerializer<,,,,,>), argumentTypes),
                6 => Types.CreateInstance<TResolver>(typeof(TupleSerializer<,,,,,,>), argumentTypes),
                7 => Types.CreateInstance<TResolver>(typeof(TupleSerializer<,,,,,,,>), argumentTypes),
                8 => Types.CreateInstance<TResolver>(typeof(TupleSerializer<,,,,,,,,>), argumentTypes),
                _ => null,
            };

        private static object GetValueTuple<T>(Type[] argumentTypes)
            => argumentTypes.Length switch
            {
                1 => Types.CreateInstance<TResolver>(typeof(ValueTupleSerializer<,>), argumentTypes[0]),
                2 => Types.CreateInstance<TResolver>(typeof(ValueTupleSerializer<,,>), argumentTypes),
                3 => Types.CreateInstance<TResolver>(typeof(ValueTupleSerializer<,,,>), argumentTypes),
                4 => Types.CreateInstance<TResolver>(typeof(ValueTupleSerializer<,,,,>), argumentTypes),
                5 => Types.CreateInstance<TResolver>(typeof(ValueTupleSerializer<,,,,,>), argumentTypes),
                6 => Types.CreateInstance<TResolver>(typeof(ValueTupleSerializer<,,,,,,>), argumentTypes),
                7 => Types.CreateInstance<TResolver>(typeof(ValueTupleSerializer<,,,,,,,>), argumentTypes),
                8 => Types.CreateInstance<TResolver>(typeof(ValueTupleSerializer<,,,,,,,,>), argumentTypes),
                _ => null,
            };

        private static bool TryGetUnionTypes(Type type, out Type[] unionTypes, out bool isAbstractBaseType)
        {
            var knownTypes = type.GetKnownTypes();

            if (knownTypes is null) // Типы с зацикливанием
            {
                var baseType = type;

                while (baseType.BaseType != Types.Object)
                    baseType = baseType.BaseType;

                if (!baseType.HasKnownTypes())
                    knownTypes = Array.Empty<Type>();
            }

            if (knownTypes is null)
            {
                unionTypes = null;
                isAbstractBaseType = false;
                return false;
            }

            isAbstractBaseType = type.IsAbstract;
            int count = knownTypes.Length;

            if (!isAbstractBaseType)
                count++;

            unionTypes = new Type[count];

            for (int i = 0; i < knownTypes.Length; i++)
                unionTypes[i] = knownTypes[i];

            if (!isAbstractBaseType)
                unionTypes[count - 1] = type;

            return true;
        }

        #region Maps

        private static readonly Dictionary<Type, Func<object>> _primitiveMap = new Dictionary<Type, Func<object>>
        {
            // Simple
            { typeof(bool), () => new BoolSerializer<TResolver>() },
            { typeof(char), () => new CharSerializer<TResolver>() },
            { typeof(sbyte), () => new SByteSerializer<TResolver>() },
            { typeof(byte), () => new ByteSerializer<TResolver>() },
            { typeof(short), () => new ShortSerializer<TResolver>() },
            { typeof(ushort), () => new UShortSerializer<TResolver>() },
            { typeof(int), () => new IntSerializer<TResolver>() },
            { typeof(uint), () => new UIntSerializer<TResolver>() },
            { typeof(long), () => new LongSerializer<TResolver>() },
            { typeof(ulong), () => new ULongSerializer<TResolver>() },
            { typeof(float), () => new FloatSerializer<TResolver>() },
            { typeof(double), () => new DoubleSerializer<TResolver>() },
            { typeof(decimal), () => new DecimalSerializer<TResolver>() },
            { typeof(string), () => new StringSerializer<TResolver>() },
            { typeof(Guid), () => new GuidSerializer<TResolver>() },
            { typeof(DateTime), () => new DateTimeSerializer<TResolver>() },
            { typeof(DateTimeOffset), () => new DateTimeOffsetSerializer<TResolver>() },
            { typeof(TimeSpan), () => new TimeSpanSerializer<TResolver>() },
            { typeof(IntPtr), () => new IntPtrSerializer<TResolver>() },
            { typeof(UIntPtr), () => new UIntPtrSerializer<TResolver>() },
            { typeof(ArraySegment<byte>), () => new ArraySegmentByteSerializer<TResolver>() },
            { typeof(Memory<byte>), () => new MemorySerializer<TResolver>() },
            { typeof(ReadOnlyMemory<byte>), () => new ReadOnlyMemorySerializer<TResolver>() },
            { typeof(ReadOnlySequence<byte>), () => new ReadOnlySequenceByteSerializer<TResolver>() },
            
            // Extended
            { typeof(Version), () => new VersionSerializer<TResolver>() },
            { typeof(IPAddress), () => new IPAddressSerializer<TResolver>() },
            { typeof(IPEndPoint), () => new IPEndPointSerializer<TResolver>() },

            // Nullable
            { typeof(bool?), () => new NullableBoolSerializer<TResolver>() },
            { typeof(char?), () => new NullableCharSerializer<TResolver>() },
            { typeof(sbyte?), () => new NullableSByteSerializer<TResolver>() },
            { typeof(byte?), () => new NullableByteSerializer<TResolver>() },
            { typeof(short?), () => new NullableShortSerializer<TResolver>() },
            { typeof(ushort?), () => new NullableUShortSerializer<TResolver>() },
            { typeof(int?), () => new NullableIntSerializer<TResolver>() },
            { typeof(uint?), () => new NullableUIntSerializer<TResolver>() },
            { typeof(long?), () => new NullableLongSerializer<TResolver>() },
            { typeof(ulong?), () => new NullableULongSerializer<TResolver>() },
            { typeof(float?), () => new NullableFloatSerializer<TResolver>() },
            { typeof(double?), () => new NullableDoubleSerializer<TResolver>() },
            { typeof(decimal?), () => new NullableDecimalSerializer<TResolver>() },
            { typeof(Guid?), () => new NullableGuidSerializer<TResolver>() },
            { typeof(DateTime?), () => new NullableDateTimeSerializer<TResolver>() },
            { typeof(DateTimeOffset?), () => new NullableDateTimeOffsetSerializer<TResolver>() },
            { typeof(TimeSpan?), () => new NullableTimeSpanSerializer<TResolver>() },
            { typeof(IntPtr?), () => new NullableIntPtrSerializer<TResolver>() },
            { typeof(UIntPtr?), () => new NullableUIntPtrSerializer<TResolver>() },

            // Array
            { typeof(bool[]), () => new ArrayBoolSerializer<TResolver>() },
            { typeof(char[]), () => new ArrayCharSerializer<TResolver>() },
            { typeof(sbyte[]), () => new ArraySByteSerializer<TResolver>() },
            { typeof(byte[]), () => new ArrayByteSerializer<TResolver>() },
            { typeof(short[]), () => new ArrayShortSerializer<TResolver>() },
            { typeof(ushort[]), () => new ArrayUShortSerializer<TResolver>() },
            { typeof(int[]), () => new ArrayIntSerializer<TResolver>() },
            { typeof(uint[]), () => new ArrayUIntSerializer<TResolver>() },
            { typeof(long[]), () => new ArrayLongSerializer<TResolver>() },
            { typeof(ulong[]), () => new ArrayULongSerializer<TResolver>() },
            { typeof(float[]), () => new ArrayFloatSerializer<TResolver>() },
            { typeof(double[]), () => new ArrayDoubleSerializer<TResolver>() },
            { typeof(decimal[]), () => new ArrayDecimalSerializer<TResolver>() },
            { typeof(string[]), () => new ArrayStringSerializer<TResolver>() },
            { typeof(Guid[]), () => new ArrayGuidSerializer<TResolver>() },
            { typeof(DateTime[]), () => new ArrayDateTimeSerializer<TResolver>() },
            { typeof(DateTimeOffset[]), () => new ArrayDateTimeOffsetSerializer<TResolver>() },
            { typeof(TimeSpan[]), () => new ArrayTimeSpanSerializer<TResolver>() },

            // List
            { typeof(List<bool>), () => new ListBoolSerializer<TResolver>() },
            { typeof(List<char>), () => new ListCharSerializer<TResolver>() },
            { typeof(List<sbyte>), () => new ListSByteSerializer<TResolver>() },
            { typeof(List<byte>), () => new ListByteSerializer<TResolver>() },
            { typeof(List<short>), () => new ListShortSerializer<TResolver>() },
            { typeof(List<ushort>), () => new ListUShortSerializer<TResolver>() },
            { typeof(List<int>), () => new ListIntSerializer<TResolver>() },
            { typeof(List<uint>), () => new ListUIntSerializer<TResolver>() },
            { typeof(List<long>), () => new ListLongSerializer<TResolver>() },
            { typeof(List<ulong>), () => new ListULongSerializer<TResolver>() },
            { typeof(List<float>), () => new ListFloatSerializer<TResolver>() },
            { typeof(List<double>), () => new ListDoubleSerializer<TResolver>() },
            { typeof(List<decimal>), () => new ListDecimalSerializer<TResolver>() },
            { typeof(List<string>), () => new ListStringSerializer<TResolver>() },
            { typeof(List<Guid>), () => new ListGuidSerializer<TResolver>() },
            { typeof(List<DateTime>), () => new ListDateTimeSerializer<TResolver>() },
            { typeof(List<DateTimeOffset>), () => new ListDateTimeOffsetSerializer<TResolver>() },
            { typeof(List<TimeSpan>), () => new ListTimeSpanSerializer<TResolver>() },
        };

        private static readonly Dictionary<Type, Type> _genericMap = new Dictionary<Type, Type>
        {
            { Types.Nullable, Types.NullableSerializer },
            { Types.List, Types.ListSerializer },
            { Types.IList, Types.IListSerializer },
            { Types.IReadOnlyList, Types.IReadOnlyListSerializer },
            { Types.ReadOnlyCollection, Types.ReadOnlyCollectionSerializer },
            { Types.IReadOnlyCollection, Types.IReadOnlyCollectionSerializer },
            { Types.ICollection, Types.ICollectionSerializer },
            { Types.IEnumerable, Types.IEnumerableSerializer },
            { Types.KeyValuePair, Types.KeyValuePairSerializer },
            { Types.Dictionary, Types.DictionarySerializer },
            { Types.IDictionary, Types.IDictionarySerializer },
            { Types.ReadOnlyDictionary, Types.ReadOnlyDictionarySerializer },
            { Types.IReadOnlyDictionary, Types.IReadOnlyDictionarySerializer },
            { Types.HashSet, Types.HashSetSerializer },
            { Types.ISet, Types.ISetSerializer },
        };

        #endregion
    }
}
