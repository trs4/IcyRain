using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Builders;

internal static class FieldsBuilder
{
    public static FieldData[] Build(TypeBuilder builder, List<PropertyInfo> properties)
    {
        var fields = new FieldData[properties.Count];

        for (int i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            var propertyType = property.PropertyType;
            bool isNullable = propertyType.IsNullable();

            if (isNullable)
                propertyType = propertyType.GetGenericArguments()[0];

            var data = ResolverHelper.GetBuilderData(propertyType);
            string name = Naming.FieldPrefix + property.Name + nameof(Serializer<,>);
            var field = builder.DefineField(name, data.SerializerType, Flags.PrivateReadOnlyField);
            fields[i] = new FieldData(property, field, propertyType, isNullable, data);
        }

        return fields;
    }

}
