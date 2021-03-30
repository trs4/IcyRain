using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Internal
{
    internal static class Types
    {
        #region Cache

        public static readonly Type Object = typeof(object);
        public static readonly Type Int = typeof(int);
        public static readonly Type String = typeof(string);
        public static readonly Type Nullable = typeof(Nullable<>);
        public static readonly Type Equatable = typeof(IEquatable<>);
        public static readonly Type NullableInt = typeof(int?);
        public static readonly Type Byte = typeof(byte);
        public static readonly Type Type = typeof(Type);
        public static readonly Type Bytes = typeof(byte[]);
        public static readonly Type BytesSegment = typeof(ArraySegment<byte>);
        public static readonly Type BytesMemory = typeof(Memory<byte>);
        public static readonly Type BytesReadOnlyMemory = typeof(ReadOnlyMemory<byte>);
        public static readonly Type BytesReadOnlySequence = typeof(ReadOnlySequence<byte>);
        public static readonly Type List = typeof(List<>);
        public static readonly Type HashSet = typeof(HashSet<>);
        public static readonly Type ReadOnlyCollection = typeof(ReadOnlyCollection<>);
        public static readonly Type Dictionary = typeof(Dictionary<,>);
        public static readonly Type ReadOnlyDictionary = typeof(ReadOnlyDictionary<,>);
        public static readonly Type KeyValuePair = typeof(KeyValuePair<,>);
        public static readonly Type IEnumerable = typeof(IEnumerable);
        public static readonly Type TEnumerable = typeof(IEnumerable<>);
        public static readonly Type ICollection = typeof(ICollection<>);
        public static readonly Type IList = typeof(IList<>);
        public static readonly Type ISet = typeof(ISet<>);
        public static readonly Type IReadOnlyList = typeof(IReadOnlyList<>);
        public static readonly Type IReadOnlyCollection = typeof(IReadOnlyCollection<>);
        public static readonly Type IDictionary = typeof(IDictionary<,>);
        public static readonly Type IReadOnlyDictionary = typeof(IReadOnlyDictionary<,>);
        public static readonly Type ILookup = typeof(ILookup<,>);

        public static readonly Type Serializer = typeof(Serializer<,>);
        public static readonly Type UnionByteMapSerializer = typeof(UnionByteMapSerializer<>);
        public static readonly Type UnionUShortMapSerializer = typeof(UnionUShortMapSerializer<>);
        public static readonly Type ListSerializer = typeof(ListSerializer<,>);
        public static readonly Type IListSerializer = typeof(IListSerializer<,>);
        public static readonly Type IReadOnlyListSerializer = typeof(IReadOnlyListSerializer<,>);
        public static readonly Type ReadOnlyCollectionSerializer = typeof(ReadOnlyCollectionSerializer<,>);
        public static readonly Type IReadOnlyCollectionSerializer = typeof(IReadOnlyCollectionSerializer<,>);
        public static readonly Type ICollectionSerializer = typeof(ICollectionSerializer<,>);
        public static readonly Type ObjectCollectionSerializer = typeof(ObjectCollectionSerializer<,,>);
        public static readonly Type IEnumerableSerializer = typeof(IEnumerableSerializer<,>);
        public static readonly Type HashSetSerializer = typeof(HashSetSerializer<,>);
        public static readonly Type ISetSerializer = typeof(ISetSerializer<,>);
        public static readonly Type DictionarySerializer = typeof(DictionarySerializer<,,>);
        public static readonly Type IDictionarySerializer = typeof(IDictionarySerializer<,,>);
        public static readonly Type ObjectDictionarySerializer = typeof(ObjectDictionarySerializer<,,,>);
        public static readonly Type ReadOnlyDictionarySerializer = typeof(ReadOnlyDictionarySerializer<,,>);
        public static readonly Type IReadOnlyDictionarySerializer = typeof(IReadOnlyDictionarySerializer<,,>);
        public static readonly Type KeyValuePairSerializer = typeof(KeyValuePairSerializer<,,>);
        public static readonly Type EnumSerializer = typeof(EnumSerializer<,>);
        public static readonly Type NullableSerializer = typeof(NullableSerializer<,>);
        public static readonly Type NullableEnumSerializer = typeof(NullableEnumSerializer<,>);
        public static readonly Type ArrayEnumSerializer = typeof(ArrayEnumSerializer<,>);
        public static readonly Type ListEnumSerializer = typeof(ListEnumSerializer<,>);

        public static readonly Type Writer = typeof(Writer);
        public static readonly Type WriterRef = Writer.MakeByRefType();
        public static readonly Type Reader = typeof(Reader);
        public static readonly Type ReaderRef = Reader.MakeByRefType();
        public static readonly Type[] Deserialize = new[] { ReaderRef };

        public static readonly Type Resolver = typeof(Resolver);
        public static readonly Type UnionResolver = typeof(UnionResolver);
        public static readonly Type UnionByteResolver = typeof(UnionByteResolver);
        public static readonly Type UnionUShortResolver = typeof(UnionUShortResolver);

        #endregion
        #region Constructors

        public static readonly ConstructorInfo NullableIntCtor = NullableInt.GetConstructors()[0];

        public static readonly ConstructorInfo InvalidOperationExceptionCtor
            = typeof(InvalidOperationException).GetConstructors()
                .First(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == String);

        #endregion
        #region Methods

        public static readonly MethodInfo WriteByte = Writer.GetMethod(nameof(Internal.Writer.WriteByte));
        public static readonly MethodInfo WriteUShort = Writer.GetMethod(nameof(Internal.Writer.WriteUShort));

        public static readonly MethodInfo ReadByte = Reader.GetMethod(nameof(Internal.Reader.ReadByte));
        public static readonly MethodInfo ReadUShort = Reader.GetMethod(nameof(Internal.Reader.ReadUShort));

        public static readonly MethodInfo GetTypeMethod
            = typeof(object).GetMethod(nameof(GetType));

        public static readonly MethodInfo TypeFullNameMethod
            = typeof(Type).GetMethod("get_FullName");

        public static readonly MethodInfo ByteToStringMethod
            = typeof(byte).GetMethods().First(m => m.Name == nameof(ToString) && m.GetParameters().Length == 0);

        public static readonly MethodInfo StringConcatMethod
            = String.GetMethods().First(x => x.Name == nameof(System.String.Concat) && x.GetParameters().Length == 2
                && x.GetParameters().All(y => y.ParameterType == String));

        public static readonly MethodInfo IsEmptyEqutableStructMethod
            = typeof(CompareExtensions).GetMethod(nameof(CompareExtensions.IsEmptyEqutableStruct));

        public static readonly MethodInfo IsEmptyStructMethod
            = typeof(CompareExtensions).GetMethod(nameof(CompareExtensions.IsEmptyStruct));

        public static readonly MethodInfo GetTypeFromHandleMethod
            = typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle));

        public static readonly MethodInfo GetUninitializedObjectMethod
            = typeof(FormatterServices).GetMethod(nameof(FormatterServices.GetUninitializedObject));

        public static readonly FieldInfo DecimalZeroField
            = typeof(decimal).GetField(nameof(decimal.Zero));

        public static readonly MethodInfo DecimalOpInequalityMethod
            = typeof(decimal).GetMethod("op_Inequality");

        public static readonly FieldInfo IntPtrZeroField
            = typeof(IntPtr).GetField(nameof(IntPtr.Zero));

        public static readonly MethodInfo IntPtrOpInequalityMethod
            = typeof(IntPtr).GetMethod("op_Inequality");

        public static readonly FieldInfo UIntPtrZeroField
            = typeof(UIntPtr).GetField(nameof(UIntPtr.Zero));

        public static readonly MethodInfo UIntPtrOpInequalityMethod
            = typeof(UIntPtr).GetMethod("op_Inequality");

        public static readonly MethodInfo BytesSegmentCountMethod
            = BytesSegment.GetMethod("get_Count");

        #endregion
        #region Extensions

        [MethodImpl(Flags.HotPath)]
        public static object CreateInstance(Type type, params Type[] argumentTypes)
            => Activator.CreateInstance(type.MakeGenericType(argumentTypes));

        [MethodImpl(Flags.HotPath)]
        public static object CreateInstance<TFirstType>(Type type, Type argumentType)
            => Activator.CreateInstance(type.MakeGenericType(typeof(TFirstType), argumentType));

        [MethodImpl(Flags.HotPath)]
        public static object CreateInstance<TFirstType>(Type type, Type[] argumentTypes)
            => Activator.CreateInstance(type.MakeGenericType(argumentTypes.WithFirst(typeof(TFirstType))));

        public static T[] WithFirst<T>(this T[] value, T first)
        {
            var result = new T[value.Length + 1];
            result[0] = first;
            Array.Copy(value, 0, result, 1, value.Length);
            return result;
        }

        public static string GetBaseTypeName(this Type type)
        {
            if (type.IsEnum)
                return null;

            return (Type.GetTypeCode(type)) switch
            {
                TypeCode.Boolean => "bool",
                TypeCode.Char => "char",
                TypeCode.SByte => "sbyte",
                TypeCode.Byte => "byte",
                TypeCode.Int16 => "short",
                TypeCode.UInt16 => "ushort",
                TypeCode.Int32 => "int",
                TypeCode.UInt32 => "uint",
                TypeCode.Int64 => "long",
                TypeCode.UInt64 => "ulong",
                TypeCode.Single => "float",
                TypeCode.Double => "double",
                TypeCode.Decimal => "decimal",
                TypeCode.String => "string",
                _ => null,
            };
        }

        public static bool TryGetIDictionatyArgumentTypes(Type type, out Type[] iDictionaryTypes)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == IDictionary)
                {
                    iDictionaryTypes = interfaceType.GetGenericArguments();
                    return true;
                }
            }

            iDictionaryTypes = null;
            return false;
        }

        public static bool TryGetICollectionArgumentType(Type type, out Type iCollectionType)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                var typeInfo = interfaceType.GetTypeInfo();

                if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == ICollection)
                {
                    iCollectionType = typeInfo.GetGenericArguments()[0];
                    return true;
                }
            }

            iCollectionType = null;
            return false;
        }

        #endregion
    }
}
