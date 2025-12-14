using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Builders;

internal sealed class BuilderData<TResolver> : IBuilderData
    where TResolver : Resolver
{
    private static readonly ConcurrentDictionary<Type, IBuilderData> _types = new();
    private static readonly ResolverType _resolverType;

    private readonly TypeInfo _serializerTypeInfo;
    private ConstructorInfo _constructor;
    private MethodInfo _getInstance;
    private MethodInfo _getSize;
    private MethodInfo _getCapacity;
    private MethodInfo _serializeSpot;
    private MethodInfo _deserializeSpot;
    private MethodInfo _deserializeInUTCSpot;
    private MethodInfo _baseDeserializeSpot;
    private MethodInfo _baseDeserializeInUTCSpot;
    private MethodInfo _add;
    private ISerializer _serializer;

    static BuilderData()
    {
        var type = typeof(TResolver);

        if (type == Types.Resolver)
            _resolverType = ResolverType.Default;
        else if (type == Types.UnionResolver)
            _resolverType = ResolverType.Union;
        else if (type == Types.UnionByteResolver)
            _resolverType = ResolverType.UnionByte;
        else if (type == Types.UnionUShortResolver)
            _resolverType = ResolverType.UnionUShort;
        else
            throw new InvalidOperationException();
    }

    private BuilderData(Type type)
    {
        Type = type;
        Properties = PropertiesFinder.Get(type);

        SerializerType = _resolverType switch
        {
            ResolverType.Default => Types.Serializer.MakeGenericType(Types.Resolver, type),
            ResolverType.Union => Types.Serializer.MakeGenericType(Types.UnionResolver, type),
            ResolverType.UnionByte => Types.UnionByteMapSerializer.MakeGenericType(type),
            ResolverType.UnionUShort => Types.UnionUShortMapSerializer.MakeGenericType(type),
            _ => throw new InvalidOperationException(),
        };

        _serializerTypeInfo = SerializerType.GetTypeInfo();
        IsBytePropertyIndexes = GetIsBytePropertyIndexes(Properties.Count);
        PropertyIndexSize = IsBytePropertyIndexes ? 1 : 2;
        WriteMethod = IsBytePropertyIndexes ? Types.WriteByte : Types.WriteUShort;
        ReadMethod = IsBytePropertyIndexes ? Types.ReadByte : Types.ReadUShort;
        EndMessageCode = IsBytePropertyIndexes ? byte.MaxValue : ushort.MaxValue;
    }

    public Type Type { get; }

    public List<PropertyInfo> Properties { get; }

    public Type SerializerType { get; }

    public bool IsBytePropertyIndexes { get; }

    public int PropertyIndexSize { get; }

    public int EndMessageCode { get; }

    public MethodInfo WriteMethod { get; }

    public MethodInfo ReadMethod { get; }

    public ConstructorInfo Constructor => _constructor ??= _serializerTypeInfo.GetConstructors(BindingFlags.Instance ^ BindingFlags.NonPublic).First();

    public MethodInfo GetInstance => _getInstance ??= _serializerTypeInfo.GetMethod("get_Instance");

    public MethodInfo GetSize => _getSize ??= _serializerTypeInfo.GetMethod(nameof(Serializer<,>.GetSize));

    public MethodInfo GetCapacity => _getCapacity ??= _serializerTypeInfo.GetMethod(nameof(Serializer<,>.GetCapacity));

    public MethodInfo SerializeSpot => _serializeSpot ??= _serializerTypeInfo.GetMethod(nameof(Serializer<,>.SerializeSpot));

    public MethodInfo DeserializeSpot => _deserializeSpot ??= _serializerTypeInfo.GetMethod(nameof(Serializer<,>.DeserializeSpot));

    public MethodInfo DeserializeInUTCSpot => _deserializeInUTCSpot ??= _serializerTypeInfo.GetMethod(nameof(Serializer<,>.DeserializeInUTCSpot));

    public MethodInfo BaseDeserializeSpot => _baseDeserializeSpot ??= _serializerTypeInfo.GetMethod(nameof(Serializer<,>.BaseDeserializeSpot));

    public MethodInfo BaseDeserializeInUTCSpot => _baseDeserializeInUTCSpot ??= _serializerTypeInfo.GetMethod(nameof(Serializer<,>.BaseDeserializeInUTCSpot));

    public MethodInfo Add => _add ??= _serializerTypeInfo.GetMethod(nameof(UnionByteMapSerializer<>.Add));

    public ISerializer Serializer => _serializer ??= GetInstance.Invoke(null, null) as ISerializer;

    public static IBuilderData Get(Type type)
        => _types.GetOrAdd(type, t => new BuilderData<TResolver>(t));

    private static bool GetIsBytePropertyIndexes(int propertiesCount)
    {
        const int reservedIndexes = 2; // null, base collection type

        if (propertiesCount <= byte.MaxValue - reservedIndexes) // 1..253 (255-2)
            return true;
        else if (propertiesCount <= ushort.MaxValue - reservedIndexes) // 1..65533 (65535-2)
            return false;

        throw new NotSupportedException($"Properties count is {propertiesCount}. Max available count is 65534");
    }

    public void EmitDeserializeSpot(ILGenerator il)
    {
        il.Emit(OpCodes.Call, GetInstance);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Callvirt, DeserializeSpot);
        il.Emit(OpCodes.Ret);
    }

    public void EmitDeserializeInUTCSpot(ILGenerator il)
    {
        il.Emit(OpCodes.Call, GetInstance);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Callvirt, DeserializeInUTCSpot);
        il.Emit(OpCodes.Ret);
    }

}
