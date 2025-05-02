using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace IcyRain.Internal;

internal static class Commons
{
    #region Attributes

    public static IEnumerable<T> GetAttributes<T>(this MemberInfo member)
        where T : Attribute
        => member.GetCustomAttributes(false).OfType<T>();

    public static T GetAttribute<T>(this MemberInfo member)
        where T : Attribute
        => member.GetAttributes<T>().FirstOrDefault();

    public static bool HasAttribute<T>(this MemberInfo member)
        where T : Attribute
        => member.GetAttributes<T>().Any();

    #endregion
    #region Types

    public static bool IsSystemType(this Type type)
        => type.Namespace?.StartsWith("System") ?? false;

    public static bool IsNullable(this Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == Types.Nullable;

    public static bool HasEquatableInterface(this Type type)
        => type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == Types.Equatable);

    #endregion
    #region Inherit

    public static bool IsInherit(this Type type, Type baseType)
        => type is not null && (type == baseType || type.IsSubclassOf(baseType));

    public static bool IsInherit<T>(this Type type)
        => type.IsInherit(typeof(T));

    public static Type GetGenericArgumentType(this Type type)
    {
        while (true)
        {
            if (type is null)
                return null;

            if (type.IsGenericType)
            {
                var genericTypes = type.GetGenericArguments();
                return genericTypes.Length == 1 && type.GetInterfaces().Contains(Types.IEnumerable) ? genericTypes[0] : null;
            }
            else if (type.IsArray)
                return type.GetElementType();

            type = type.BaseType;
        }
    }

    public static Type GetGenericArgumentValueType(this Type type)
    {
        while (true)
        {
            if (type is null)
                return null;

            if (type.IsGenericType)
            {
                Type dictionaryType = null;
                Type enumerableType = null;

                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (!interfaceType.IsGenericType)
                        continue;

                    var definitionType = interfaceType.GetGenericTypeDefinition();

                    if (definitionType == Types.IDictionary)
                        dictionaryType = interfaceType;
                    else if (definitionType == Types.TEnumerable)
                        enumerableType = interfaceType;
                }

                if (dictionaryType is not null)
                    return dictionaryType.GetGenericArguments()[1];
                else if (enumerableType is not null)
                    return enumerableType.GetGenericArguments()[0];
                else
                    return null;
            }
            else if (type.IsArray)
                return type.GetElementType();

            type = type.BaseType;
        }
    }

    public static Type[] GetGenericArgumentTypes(this Type type)
    {
        while (true)
        {
            if (type is null)
                return null;

            if (type.IsGenericType)
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (!interfaceType.IsGenericType)
                        continue;

                    var definitionType = interfaceType.GetGenericTypeDefinition();

                    if (definitionType == Types.IDictionary || definitionType == Types.TEnumerable)
                        return interfaceType.GetGenericArguments();
                }
            }
            else if (type.IsArray)
                return [type.GetElementType()];

            type = type.BaseType;
        }
    }

    #endregion
    #region Contract

    private static readonly ConcurrentDictionary<Type, ContractData> _contractAttributes
        = new ConcurrentDictionary<Type, ContractData>();

    private static readonly ConcurrentDictionary<Type, (Type[] Types, Type BaseType)> _knownTypes
        = new ConcurrentDictionary<Type, (Type[] Types, Type BaseType)>();

    public static bool HasDataContractAttribute(this Type type)
        => _contractAttributes.GetOrAdd(type, t => new ContractData(t)).HasContract;

    public static bool HasCollectionDataContractAttribute(this Type type)
        => _contractAttributes.GetOrAdd(type, t => new ContractData(t)).HasCollectionContract;

    public static KnownTypeAttribute[] GetKnownTypeAttributes(this Type type)
        => _contractAttributes.GetOrAdd(type, t => new ContractData(t)).KnownTypes;

    public static Type[] GetKnownTypes(this Type type)
        => _knownTypes.GetOrAdd(type, GetKnownTypesCore).Types;

    public static Type[] GetKnownTypes(this Type type, out Type baseType)
    {
        var result = _knownTypes.GetOrAdd(type, GetKnownTypesCore);
        baseType = result.BaseType;
        return result.Types;
    }

    public static bool HasKnownTypes(this Type type)
        => _knownTypes.GetOrAdd(type, GetKnownTypesCore).Types is not null;

    private static (Type[] Types, Type BaseType) GetKnownTypesCore(Type type)
    {
        if (type == Types.Object)
            return (null, null);

        HashSet<Type> subTypes = null;
        var baseType = type;

        while (baseType.HasDataContractAttribute())
        {
            foreach (var knownTypeAttribute in baseType.GetKnownTypeAttributes())
            {
                subTypes ??= [];

                if (knownTypeAttribute.Type is not null)
                    subTypes.Add(knownTypeAttribute.Type);
                else if (knownTypeAttribute.MethodName is not null)
                {
                    var methodTypes = baseType.GetMethod(knownTypeAttribute.MethodName, Flags.StaticPublicBindingFlags)
                        ?.Invoke(null, null) as IEnumerable<Type>
                        ?? throw new InvalidOperationException($"Empty types is KnownTypeAttribute {baseType.FullName} ");

                    foreach (var methodType in methodTypes)
                        subTypes.Add(methodType);
                }
            }

            if (baseType.BaseType == Types.Object)
                break;

            baseType = baseType.BaseType;
        }

        if (subTypes is null)
            return (null, baseType);

        subTypes.Remove(type); // Проверка на всякий случай

        foreach (var subType in subTypes.ToArray())
        {
            if (subType.IsAbstract || !subType.IsSubclassOf(type))
                subTypes.Remove(subType);
        }

        var types = subTypes.Count == 0 ? null : subTypes.OrderByDescending(GetDepth).ThenBy(t => t.Name).ToArray();
        return (types, baseType);
    }

    private static int GetDepth(Type type)
    {
        int count = 0;

        while (type is not null)
        {
            count++;
            type = type.BaseType;
        }

        return count;
    }

    #endregion
}
