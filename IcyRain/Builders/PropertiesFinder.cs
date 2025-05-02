using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Builders;

internal static class PropertiesFinder
{
    public static List<PropertyInfo> Get(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var map = new Dictionary<int, PropertyInfo>(properties.Length);

        foreach (var property in properties)
        {
            if (TryGetDataMember(type, property, out var dataMember))
                map.Add(dataMember.Order, property);
        }

        return [.. map.OrderBy(p => p.Key).Select(p => p.Value)];
    }

    public static void Test(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var map = new Dictionary<int, PropertyInfo>(properties.Length);

        foreach (var property in properties)
        {
            if (!TryGetDataMember(type, property, out var dataMember))
                continue;

            int index = dataMember.Order;
            var getMethod = property.GetGetMethod(true);
            var setMethod = property.GetSetMethod(true);

            if (getMethod is null || setMethod is null || getMethod.IsPrivate || setMethod.IsPrivate)
                throw new InvalidOperationException($"Property {type.Name}.{property.Name} must needs public get and set accessor");

            if (map.ContainsKey(index))
                throw new InvalidOperationException("IndexAttribute is not allow duplicate number " + type.Name + "." + property.Name + ", Index:" + index);

            var serializer = BuilderData<Resolver>.Get(property.PropertyType).Serializer;

            if (serializer is null)
                throw new InvalidOperationException(Naming.GetSerializerNotFoundMessage(type));
            else if (serializer is IErrorSerializer errorSerializer)
                errorSerializer.Throw();

            map.Add(index, property);
        }
    }

    private static bool TryGetDataMember(Type type, PropertyInfo property, out DataMemberAttribute dataMember)
    {
        if (property.HasAttribute<IgnoreDataMemberAttribute>())
        {
            dataMember = null;
            return false;
        }

        dataMember = property.GetAttribute<DataMemberAttribute>();

        if (dataMember is not null)
            return true;

        var baseType = type.BaseType;

        while (baseType is not null)
        {
            var baseProperty = baseType.GetProperty(property.Name);

            if (baseProperty is not null)
            {
                if (baseProperty.HasAttribute<IgnoreDataMemberAttribute>())
                    return false;

                dataMember = baseProperty.GetAttribute<DataMemberAttribute>();

                if (dataMember is not null)
                    return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

}
