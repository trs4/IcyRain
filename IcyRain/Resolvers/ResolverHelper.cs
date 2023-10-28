using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using IcyRain.Builders;
using IcyRain.Internal;

namespace IcyRain.Resolvers;

internal static class ResolverHelper
{
    private static readonly ConcurrentDictionary<Type, bool> _unionMap = new();

    public static IBuilderData GetBuilderData(Type type)
        => IsUnionResolver(type) ? BuilderData<UnionResolver>.Get(type) : BuilderData<Resolver>.Get(type);

    [MethodImpl(Flags.HotPath)]
    public static bool IsUnionResolver<T>() => IsUnionResolver(typeof(T));

    public static bool IsUnionResolver(Type type)
        => _unionMap.GetOrAdd(type, t =>
        {
            if (t.IsEnum)
                return false;

            while (t.HasCollectionDataContractAttribute())
                t = t.BaseType;

            while (true)
            {
                if (t.IsArray)
                    t = t.GetElementType();
                else if (t.IsSystemType())
                {
                    if (t.IsGenericType)
                    {
                        t = t.GetGenericArgumentValueType();
                        return t is not null && IsUnion(t);
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (t.IsClass && t.BaseType.IsSystemType())
                    return IsUnion(t.BaseType.GetGenericArgumentValueType() ?? t);
                else if (t.IsGenericType)
                    t = t.GetGenericArguments()[0];
                else
                    break;
            }

            return t?.HasKnownTypes() ?? false;
        });

    private static bool IsUnion(Type type)
    {
        if (type.HasKnownTypes())
            return true;
        else if (type.IsSystemType())
            return false;

        return IsUnion(type, type, new HashSet<Type>());
    }

    private static bool IsUnion(Type resolverType, Type type, HashSet<Type> checkedTypes)
    {
        if (type.IsSystemType() || !checkedTypes.Add(type))
            return false;

        var propertyTypes = GetPropertyTypes(type);

        if (propertyTypes.Any(t => t == resolverType))
            return true;

        foreach (var propertyType in propertyTypes)
        {
            if (IsUnion(resolverType, propertyType, checkedTypes))
                return true;
        }

        return false;
    }

    private static HashSet<Type> GetPropertyTypes(Type type)
    {
        var properties = type.GetProperties();
        var propertyTypes = new HashSet<Type>();

        foreach (var property in properties)
        {
            if (!property.HasAttribute<DataMemberAttribute>())
                continue;

            var t = property.PropertyType;

            while (true)
            {
                if (t.IsArray)
                {
                    t = t.GetElementType();
                }
                else if (t.IsSystemType())
                {
                    if (t.IsGenericType)
                        t = t.GetGenericArgumentValueType();

                    break;
                }
                else if (t.IsClass && t.BaseType.IsSystemType())
                {
                    var baseElementType = t.BaseType.GetGenericArgumentValueType();

                    if (baseElementType != null && baseElementType != Types.Object)
                        t = baseElementType;

                    break;
                }
                else if (t.IsGenericType)
                {
                    t = t.GetGenericArguments()[0];
                }
                else
                {
                    break;
                }
            }

            if (t != null && !t.IsSystemType())
                propertyTypes.Add(t);
        }

        return propertyTypes;
    }

}
