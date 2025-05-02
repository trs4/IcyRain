﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Builders;

internal static class ObjectBuilder
{
    public static object Get(Type type)
    {
        var serializerType = BuildSerializer(type);
        return Activator.CreateInstance(serializerType);
    }

    private static Type BuildSerializer(Type type)
    {
        var data = BuilderData<Resolver>.Get(type);
        string typeName = Naming.GetName(type, forSerializer: true);
        var builder = DynamicAssembly.Module.DefineType(typeName, Flags.Type, data.SerializerType);

        var typeField = builder.DefineField(Naming.TypeField, Types.Type, Flags.PrivateReadOnlyField);
        MethodInfo serializeSpotMethod;

        var fields = FieldsBuilder.Build(builder, data.Properties);
        var calculatedFields = new List<FieldData>(fields.Length);
        int size = data.PropertyIndexSize * (fields.Length + 1);

        foreach (var field in fields)
        {
            int? fieldSize = field.Data.Serializer.GetSize();

            if (fieldSize.HasValue)
                size += fieldSize.Value;
            else
                calculatedFields.Add(field);
        }

        // .ctor
        FieldsBuilder.BuildConstructor(builder, data, type, typeField, fields);

        // int? GetSize()
        {
            var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.GetSize), Flags.PublicOverrideMethod,
                Types.NullableInt, Type.EmptyTypes)
                .WithAggressiveInlining();

            var il = method.GetILGenerator();

            if (calculatedFields.Count == 0)
            {
                il.EmitLdc_I4(size);
                il.Emit(OpCodes.Newobj, Types.NullableIntCtor);
            }
            else
            {
                il.DeclareLocal(Types.NullableInt);

                il.Emit(OpCodes.Ldloca_S, 0);
                il.Emit(OpCodes.Initobj, Types.NullableInt);
                il.Emit(OpCodes.Ldloc_0);
            }

            il.Emit(OpCodes.Ret);
        }

        // int GetCapacity(T value)
        {
            var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.GetCapacity), Flags.PublicOverrideMethod,
                Types.Int, [type])
                .WithName(Naming.Value).WithAggressiveInlining();

            var il = method.GetILGenerator();
            var label = il.DefineLabel();

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Brfalse_S, label);
            il.EmitLdc_I4(size);

            foreach (var field in calculatedFields)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, field.Field);
                il.Emit(OpCodes.Ldarg_1);
                field.EmitGetProperty(il);
                il.Emit(OpCodes.Callvirt, field.Data.GetCapacity);
                il.Emit(OpCodes.Add);
            }

            il.Emit(OpCodes.Ret);
            il.MarkLabel(label);

            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Ret);
        }

        // void SerializeSpot(ref Writer writer, T value)
        {
            var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.SerializeSpot), Flags.PublicOverrideMethod,
                null, [Types.WriterRef, type])
                .WithNames(Naming.Serialize);

            var il = method.GetILGenerator();

            int variableIndex = -1;

            foreach (var field in fields)
            {
                if (field.PropertyType == Types.BytesSegment)
                {
                    field.VariableIndex = ++variableIndex;
                    il.DeclareLocal(field.PropertyType);
                }
                else if (field.IsNullable)
                {
                    field.VariableIndex = ++variableIndex;
                    il.DeclareLocal(field.RealPropertyType);
                }
            }

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var propertyLabel = il.DefineLabel();

                // if (value.Property != default)
                CompareBuilder.Emit(il, propertyLabel, field);

                // writer.WriteByte(number);
                il.Emit(OpCodes.Ldarg_1);
                il.EmitLdc_I4(i + 1);
                il.Emit(OpCodes.Call, data.WriteMethod);

                // _s_PropertySerializer.Serialize(ref writer, value.Property);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, field.Field);
                il.Emit(OpCodes.Ldarg_1);

                if (field.IsNullable)
                {
                    il.Emit(OpCodes.Ldloca_S, field.VariableIndex);
                    il.Emit(OpCodes.Call, field.RealPropertyType.GetMethod("get_Value"));
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_2);
                    field.EmitGetProperty(il);
                }

                il.Emit(OpCodes.Callvirt, field.Data.SerializeSpot);
                il.MarkLabel(propertyLabel);
            }

            // writer.WriteByte(255);
            il.Emit(OpCodes.Ldarg_1);
            il.EmitLdc_I4(data.EndMessageCode);
            il.Emit(OpCodes.Call, data.WriteMethod);
            il.Emit(OpCodes.Ret);

            serializeSpotMethod = method;
        }

        // void Serialize(ref Writer writer, T value)
        {
            var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.Serialize), Flags.PublicOverrideMethod,
                null, [Types.WriterRef, type])
                .WithNames(Naming.Serialize);

            var il = method.GetILGenerator();
            var label = il.DefineLabel();

            // if (value is null)
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Brtrue_S, label);

            // writer.WriteByte(0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Call, data.WriteMethod);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(label);

            // SerializeSpot(ref writer, value);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, serializeSpotMethod);
            il.Emit(OpCodes.Ret);
        }

        // T Deserialize(ref Reader reader)
        {
            var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.Deserialize), Flags.PublicOverrideMethod,
                type, Types.Deserialize)
                .WithNames(Naming.Deserialize);

            var il = method.GetILGenerator();
            var label = il.DefineLabel();
            il.DeclareLocal(Types.Int);
            il.DeclareLocal(type);

            // int num = reader.ReadByte();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, data.ReadMethod);
            il.Emit(OpCodes.Stloc_0);

            // if (num == 0)
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Brtrue_S, label);

            // return null;
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(label);

            // TestData testData = (TestData)FormatterServices.GetUninitializedObject(_s_Type);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, typeField);
            il.Emit(OpCodes.Call, Types.GetUninitializedObjectMethod);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc_1);

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
                il.Emit(OpCodes.Callvirt, field.Data.DeserializeSpot);
                field.EmitSetProperty(il);

                // num = reader.ReadByte();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, data.ReadMethod);

                if (i < maxIndex)
                    il.Emit(OpCodes.Stloc_0);
                else
                    il.Emit(OpCodes.Pop);

                il.MarkLabel(propertyLabel);
            }

            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
        }

        // T DeserializeInUTC(ref Reader reader)
        {
            var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.DeserializeInUTC), Flags.PublicOverrideMethod,
                type, Types.Deserialize)
                .WithNames(Naming.Deserialize);

            var il = method.GetILGenerator();
            var label = il.DefineLabel();
            il.DeclareLocal(Types.Int);
            il.DeclareLocal(type);

            // int num = reader.ReadByte();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, data.ReadMethod);
            il.Emit(OpCodes.Stloc_0);

            // if (num == 0)
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Brtrue_S, label);

            // return null;
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(label);

            // TestData testData = (TestData)FormatterServices.GetUninitializedObject(_s_Type);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, typeField);
            il.Emit(OpCodes.Call, Types.GetUninitializedObjectMethod);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc_1);

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
                il.Emit(OpCodes.Callvirt, field.Data.DeserializeInUTCSpot);
                field.EmitSetProperty(il);

                // num = reader.ReadByte();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, data.ReadMethod);

                if (i < maxIndex)
                    il.Emit(OpCodes.Stloc_0);
                else
                    il.Emit(OpCodes.Pop);

                il.MarkLabel(propertyLabel);
            }

            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
        }

        // T DeserializeSpot(ref Reader reader)
        {
            var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.DeserializeSpot), Flags.PublicOverrideMethod,
                type, Types.Deserialize)
                .WithNames(Naming.Deserialize);

            var il = method.GetILGenerator();
            var label = il.DefineLabel();
            il.DeclareLocal(Types.Int);
            il.DeclareLocal(type);

            // TestData testData = (TestData)FormatterServices.GetUninitializedObject(_s_Type);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, typeField);
            il.Emit(OpCodes.Call, Types.GetUninitializedObjectMethod);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc_1);

            // int num = reader.ReadByte();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, data.ReadMethod);
            il.Emit(OpCodes.Stloc_0);

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
                il.Emit(OpCodes.Callvirt, field.Data.DeserializeSpot);
                field.EmitSetProperty(il);

                // num = reader.ReadByte();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, data.ReadMethod);

                if (i < maxIndex)
                    il.Emit(OpCodes.Stloc_0);
                else
                    il.Emit(OpCodes.Pop);

                il.MarkLabel(propertyLabel);
            }

            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
        }

        // T DeserializeInUTCSpot(ref Reader reader)
        {
            var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.DeserializeInUTCSpot), Flags.PublicOverrideMethod,
                type, Types.Deserialize)
                .WithNames(Naming.Deserialize);

            var il = method.GetILGenerator();
            var label = il.DefineLabel();
            il.DeclareLocal(Types.Int);
            il.DeclareLocal(type);

            // TestData testData = (TestData)FormatterServices.GetUninitializedObject(_s_Type);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, typeField);
            il.Emit(OpCodes.Call, Types.GetUninitializedObjectMethod);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc_1);

            // int num = reader.ReadByte();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, data.ReadMethod);
            il.Emit(OpCodes.Stloc_0);

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
                il.Emit(OpCodes.Callvirt, field.Data.DeserializeInUTCSpot);
                field.EmitSetProperty(il);

                // num = reader.ReadByte();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, data.ReadMethod);

                if (i < maxIndex)
                    il.Emit(OpCodes.Stloc_0);
                else
                    il.Emit(OpCodes.Pop);

                il.MarkLabel(propertyLabel);
            }

            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
        }

        return builder.CreateType();
    }

}
