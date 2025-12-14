using System;
using System.Reflection;
using System.Reflection.Emit;
using IcyRain.Internal;

namespace IcyRain.Builders;

internal static class ObjectEmitBuilder
{
    public static void BuildConstructor(TypeBuilder builder, IBuilderData data, Type type, FieldInfo typeField, FieldData[] fields, BaseClassData baseClassData)
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

        if (baseClassData is not null)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldtoken, type);
            il.Emit(OpCodes.Call, Types.GetTypeFromHandleMethod);

            if (baseClassData.HasCapacityConstructor)
                il.Emit(OpCodes.Ldsfld, BuilderTypes.IntArrayField);
            else
                il.Emit(OpCodes.Call, Types.ArrayEmptyTypeMethod);

            il.Emit(OpCodes.Call, Types.TypeGetConstructorMethod);
            il.Emit(OpCodes.Stfld, baseClassData.ConstructorField);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, baseClassData.Data.GetInstance);
            il.Emit(OpCodes.Stfld, baseClassData.Field);
        }

        foreach (var field in fields)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, field.Data.GetInstance);
            il.Emit(OpCodes.Stfld, field.Field);
        }

        il.Emit(OpCodes.Ret);
    }

    public static void BuildCreateObject(ILGenerator il, Type type, FieldBuilder typeField, IBuilderData data, FieldData[] fields,
        BaseClassData baseClassData, Func<IBuilderData, MethodInfo> getDeserializeMethod, bool spot = false)
    {
        if (baseClassData is not null)
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
            il.Emit(OpCodes.Ldfld, baseClassData.ConstructorField);

            if (baseClassData.HasCapacityConstructor)
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
            il.Emit(OpCodes.Ldfld, baseClassData.Field);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldloc_2);
            il.Emit(OpCodes.Callvirt, getDeserializeMethod(baseClassData.Data).MakeGenericMethod(type));

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

    public static void ReadPropertyValues(ILGenerator il, IBuilderData data, FieldData[] fields, Func<IBuilderData, MethodInfo> getDeserializeMethod)
    {
        int maxIndex = fields.Length - 1;

        for (int i = 0; i <= maxIndex; i++)
        {
            var field = fields[i];
            var propertyLabel = il.DefineLabel();

            // if (num == 1)
            il.Emit(OpCodes.Ldloc_0);
            il.EmitLdc_I4(i + 1);
            il.Emit(OpCodes.Bne_Un_S, propertyLabel);

            // testData.Property = _s_PropertySerializer.DeserializeSpot(ref reader);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field.Field);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, getDeserializeMethod(field.Data));

            if (field.IsNullable)
                il.Emit(OpCodes.Newobj, field.RealPropertyTypeConstructor);

            field.EmitSetProperty(il);

            ReadByte(il, data, i < maxIndex);
            il.MarkLabel(propertyLabel);
        }

        il.Emit(OpCodes.Ldloc_1);
        il.Emit(OpCodes.Ret);
    }

    public static void DeclareLocalVariablesOnDeserialize(ILGenerator il, Type type, IBuilderData data, bool hasBaseCollectionType)
    {
        var label = il.DefineLabel();
        DeclareLocalVariablesOnDeserialize(il, type, hasBaseCollectionType);
        ReadByte(il, data);

        // if (num == 0)
        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Brtrue_S, label);

        // return null;
        il.Emit(OpCodes.Ldnull);
        il.Emit(OpCodes.Ret);
        il.MarkLabel(label);
    }

    public static void DeclareLocalVariablesOnDeserialize(ILGenerator il, Type type, bool hasBaseCollectionType)
    {
        il.DeclareLocal(Types.Int); // index
        il.DeclareLocal(type); // obj

        if (hasBaseCollectionType)
            il.DeclareLocal(Types.Int); // length
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
