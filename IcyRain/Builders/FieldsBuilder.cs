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
            string name = Naming.FieldPrefix + property.Name + nameof(Serializer<,>);
            var field = builder.DefineField(name, data.SerializerType, Flags.PrivateReadOnlyField);
            fields[i] = new FieldData(property, field, propertyType, isNullable, data);
        }

        return fields;
    }

    public static void BuildConstructor(TypeBuilder builder, IBuilderData data, Type type, FieldInfo typeField, FieldData[] fields,
        IBuilderData baseData, FieldBuilder constructorField, FieldBuilder baseField, bool hasCapacityConstructor)
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

        if (constructorField is not null)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldtoken, type);
            il.Emit(OpCodes.Call, Types.GetTypeFromHandleMethod);

            if (hasCapacityConstructor)
                il.Emit(OpCodes.Ldsfld, BuilderTypes.IntArrayField);
            else
                il.Emit(OpCodes.Call, Types.ArrayEmptyTypeMethod);

            il.Emit(OpCodes.Call, Types.TypeGetConstructorMethod);
            il.Emit(OpCodes.Stfld, constructorField);
        }

        if (baseData is not null && baseField is not null)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, baseData.GetInstance);
            il.Emit(OpCodes.Stfld, baseField);
        }

        foreach (var field in fields)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, field.Data.GetInstance);
            il.Emit(OpCodes.Stfld, field.Field);
        }

        il.Emit(OpCodes.Ret);
    }

    public static void BuildCreateObject(ILGenerator il, Type type, FieldBuilder typeField, bool hasBaseCollectionType,
        IBuilderData data, FieldData[] fields,
        FieldBuilder baseField, MethodInfo baseDeserializeSpot, FieldBuilder constructorField, bool hasCapacityConstructor, bool spot = false)
    {
        if (hasBaseCollectionType)
        {
            if (spot)
                ReadByte(il, data, false);

            // int length = reader.ReadInt();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, Types.ReadInt);
            il.Emit(OpCodes.Stloc_2);

            // DataTable dataTable = (DataTable)_constructor.Invoke(new object[1] { length });
            // DataTable dataTable = (DataTable)_constructor.Invoke(Array.Empty<object>());
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, constructorField);

            if (hasCapacityConstructor)
            {
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Newarr, Types.Object);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ldloc_2);
                il.Emit(OpCodes.Box, Types.Int);
                il.Emit(OpCodes.Stelem_Ref);
            }
            else
                il.Emit(OpCodes.Call, Types.ArrayEmptyObjectMethod);

            il.Emit(OpCodes.Callvirt, Types.ConstructorInfoInvokeMethod);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc_1);

            // _s_Base_Serializer.BaseDeserializeSpot<DataTable>(ref reader, dataTable, length);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, baseField);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Callvirt, baseDeserializeSpot.MakeGenericMethod(type));

            ReadByte(il, data, spot || fields.Length > 0);
        }
        else
        {
            // TestData testData = (TestData)FormatterServices.GetUninitializedObject(_s_Type);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, typeField);
            il.Emit(OpCodes.Call, Types.GetUninitializedObjectMethod);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc_1);

            if (spot)
                ReadByte(il, data);
        }
    }

    public static void ReadByte(ILGenerator il, IBuilderData data, bool set = true)
    {
        // num = reader.ReadByte();
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Call, data.ReadMethod);

        if (set)
            il.Emit(OpCodes.Stloc_0);
        else
            il.Emit(OpCodes.Pop);
    }

}
