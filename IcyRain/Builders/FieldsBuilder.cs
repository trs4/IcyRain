using System;
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
            string name = Naming.FieldPreffix + property.Name + nameof(Serializer<Resolver, object>);
            var field = builder.DefineField(name, data.SerializerType, Flags.PrivateReadOnlyField);
            fields[i] = new FieldData(property, field, propertyType, isNullable, data);
        }

        return fields;
    }

    public static void BuildConstructor(TypeBuilder builder, IBuilderData data, Type type, FieldInfo typeField, FieldData[] fields)
    {
        var method = builder.DefineConstructor(Flags.Constructor, CallingConventions.Standard, Type.EmptyTypes);

        var il = method.GetILGenerator();

        // _s_Type = typeof(TestData);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldtoken, type);
        il.Emit(OpCodes.Call, Types.GetTypeFromHandleMethod);
        il.Emit(OpCodes.Stfld, typeField);

        // base..ctor();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Call, data.Constructor);

        foreach (var field in fields)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, field.Data.GetInstance);
            il.Emit(OpCodes.Stfld, field.Field);
        }

        il.Emit(OpCodes.Ret);
    }

}
