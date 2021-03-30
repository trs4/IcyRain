using System;
using System.Reflection;
using System.Reflection.Emit;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Builders
{
    internal static class UnionBuilder
    {
        public static object Get(Type type, Type[] unionTypes, bool isAbstractBaseType)
        {
            var unionDataList = new IBuilderData[unionTypes.Length];

            for (int i = 0; i < unionTypes.Length; i++)
                unionDataList[i] = BuilderData<Resolver>.Get(unionTypes[i]);

            var serializerType = BuildSerializer(type, unionDataList, isAbstractBaseType);
            return Activator.CreateInstance(serializerType);
        }

        private static Type BuildSerializer(Type type, IBuilderData[] unionDataList, bool isAbstractBaseType)
        {
            var data = BuilderData<UnionResolver>.Get(type);
            string typeName = Naming.GetName(type, isUnion: true, forSerializer: true);
            var builder = DynamicAssembly.Module.DefineType(typeName, Flags.Type, data.SerializerType);

            MethodInfo serializeSpotMethod;
            int dataListCount = unionDataList.Length;

            if (!isAbstractBaseType)
                dataListCount--;

            // .ctor
            {
                var method = builder.DefineConstructor(Flags.Constructor, CallingConventions.Standard, Type.EmptyTypes);

                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, data.Constructor);
                il.Emit(OpCodes.Ret);
            }

            // int? GetSize()
            {
                var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.GetSize), Flags.PublicOverrideMethod,
                    Types.NullableInt, Type.EmptyTypes)
                    .WithAggressiveInlining();

                var il = method.GetILGenerator();
                il.DeclareLocal(Types.NullableInt);

                il.Emit(OpCodes.Ldloca_S, 0);
                il.Emit(OpCodes.Initobj, Types.NullableInt);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
            }

            // int GetCapacity(T value)
            {
                var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.GetCapacity), Flags.PublicOverrideMethod,
                    Types.Int, new[] { type })
                    .WithName(Naming.Value).WithAggressiveInlining();

                var il = method.GetILGenerator();
                var label = il.DefineLabel();

                // if (value is null) return 1;
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Brtrue_S, label);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Ret);
                il.MarkLabel(label);

                for (int i = 0; i < dataListCount; i++)
                {
                    var unionData = unionDataList[i];
                    var propertyLabel = il.DefineLabel();

                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Isinst, unionData.Type);
                    il.Emit(OpCodes.Brfalse_S, propertyLabel);

                    il.Emit(OpCodes.Call, unionData.GetInstance);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Castclass, unionData.Type);
                    il.Emit(OpCodes.Callvirt, unionData.GetCapacity);
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Add);
                    il.Emit(OpCodes.Ret);
                    il.MarkLabel(propertyLabel);
                }

                if (isAbstractBaseType)
                {
                    // throw new InvalidOperationException("Unknown type: " + value.GetType().FullName);
                    il.Emit(OpCodes.Ldstr, "Unknown type: ");
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Callvirt, Types.GetTypeMethod);
                    il.Emit(OpCodes.Callvirt, Types.TypeFullNameMethod);
                    il.Emit(OpCodes.Call, Types.StringConcatMethod);
                    il.Emit(OpCodes.Newobj, Types.InvalidOperationExceptionCtor);
                    il.Emit(OpCodes.Throw);
                }
                else
                {
                    var unionData = unionDataList[dataListCount];

                    il.Emit(OpCodes.Call, unionData.GetInstance);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Callvirt, unionData.GetCapacity);
                    il.Emit(OpCodes.Ldc_I4_1);
                    il.Emit(OpCodes.Add);
                    il.Emit(OpCodes.Ret);
                }
            }

            // void SerializeSpot(ref Writer writer, T value)
            {
                var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.SerializeSpot), Flags.PublicOverrideMethod,
                    null, new[] { Types.WriterRef, type })
                    .WithNames(Naming.Serialize);

                var il = method.GetILGenerator();

                for (int i = 0; i < dataListCount; i++)
                {
                    var unionData = unionDataList[i];
                    var propertyLabel = il.DefineLabel();

                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Isinst, unionData.Type);
                    il.Emit(OpCodes.Brfalse_S, propertyLabel);

                    il.Emit(OpCodes.Ldarg_1);
                    il.EmitLdc_I4(i + 1);
                    il.Emit(OpCodes.Call, Types.WriteByte);
                    il.Emit(OpCodes.Call, unionData.GetInstance);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Castclass, unionData.Type);
                    il.Emit(OpCodes.Callvirt, unionData.SerializeSpot);
                    il.Emit(OpCodes.Ret);
                    il.MarkLabel(propertyLabel);
                }

                if (isAbstractBaseType)
                {
                    // throw new InvalidOperationException("Unknown type: " + value.GetType().FullName);
                    il.Emit(OpCodes.Ldstr, "Unknown type: ");
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Callvirt, Types.GetTypeMethod);
                    il.Emit(OpCodes.Callvirt, Types.TypeFullNameMethod);
                    il.Emit(OpCodes.Call, Types.StringConcatMethod);
                    il.Emit(OpCodes.Newobj, Types.InvalidOperationExceptionCtor);
                    il.Emit(OpCodes.Throw);
                }
                else
                {
                    var unionData = unionDataList[dataListCount];

                    il.Emit(OpCodes.Ldarg_1);
                    il.EmitLdc_I4(unionDataList.Length);
                    il.Emit(OpCodes.Call, Types.WriteByte);
                    il.Emit(OpCodes.Call, unionData.GetInstance);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldarg_2);
                    il.Emit(OpCodes.Callvirt, unionData.SerializeSpot);
                    il.Emit(OpCodes.Ret);
                }

                serializeSpotMethod = method;
            }

            // void Serialize(ref Writer writer, T value)
            {
                var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.Serialize), Flags.PublicOverrideMethod,
                    null, new[] { Types.WriterRef, type })
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
                il.DeclareLocal(Types.Byte);

                // int index = reader.ReadByte();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, data.ReadMethod);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Brtrue_S, label);

                // return null;
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ret);
                il.MarkLabel(label);

                for (int i = 0; i < dataListCount; i++)
                {
                    var unionData = unionDataList[i];
                    var propertyLabel = il.DefineLabel();

                    il.Emit(OpCodes.Ldloc_0);
                    il.EmitLdc_I4(i + 1);
                    il.Emit(OpCodes.Bne_Un_S, propertyLabel);

                    unionData.EmitDeserializeSpot(il);
                    il.MarkLabel(propertyLabel);
                }

                if (isAbstractBaseType)
                    EmitException(il);
                else
                    unionDataList[dataListCount].EmitDeserializeSpot(il);
            }

            // T DeserializeInUTC(ref Reader reader)
            {
                var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.DeserializeInUTC), Flags.PublicOverrideMethod,
                    type, Types.Deserialize)
                    .WithNames(Naming.Deserialize);

                var il = method.GetILGenerator();
                var label = il.DefineLabel();
                il.DeclareLocal(Types.Byte);

                // int index = reader.ReadByte();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, data.ReadMethod);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Brtrue_S, label);

                // return null;
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ret);
                il.MarkLabel(label);

                for (int i = 0; i < dataListCount; i++)
                {
                    var unionData = unionDataList[i];
                    var propertyLabel = il.DefineLabel();

                    il.Emit(OpCodes.Ldloc_0);
                    il.EmitLdc_I4(i + 1);
                    il.Emit(OpCodes.Bne_Un_S, propertyLabel);

                    unionData.EmitDeserializeInUTCSpot(il);
                    il.MarkLabel(propertyLabel);
                }

                if (isAbstractBaseType)
                    EmitException(il);
                else
                    unionDataList[dataListCount].EmitDeserializeInUTCSpot(il);
            }

            // T DeserializeSpot(ref Reader reader)
            {
                var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.DeserializeSpot), Flags.PublicOverrideMethod,
                    type, Types.Deserialize)
                    .WithNames(Naming.Deserialize);

                var il = method.GetILGenerator();
                var label = il.DefineLabel();
                il.DeclareLocal(Types.Byte);

                // int index = reader.ReadByte();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, data.ReadMethod);
                il.Emit(OpCodes.Stloc_0);

                for (int i = 0; i < dataListCount; i++)
                {
                    var unionData = unionDataList[i];
                    var propertyLabel = il.DefineLabel();

                    il.Emit(OpCodes.Ldloc_0);
                    il.EmitLdc_I4(i + 1);
                    il.Emit(OpCodes.Bne_Un_S, propertyLabel);

                    unionData.EmitDeserializeSpot(il);
                    il.MarkLabel(propertyLabel);
                }

                if (isAbstractBaseType)
                    EmitException(il);
                else
                    unionDataList[dataListCount].EmitDeserializeSpot(il);
            }

            // T DeserializeInUTCSpot(ref Reader reader)
            {
                var method = builder.DefineMethod(nameof(Serializer<Resolver, object>.DeserializeInUTCSpot), Flags.PublicOverrideMethod,
                    type, Types.Deserialize)
                    .WithNames(Naming.Deserialize);

                var il = method.GetILGenerator();
                var label = il.DefineLabel();
                il.DeclareLocal(Types.Byte);

                // int index = reader.ReadByte();
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, data.ReadMethod);
                il.Emit(OpCodes.Stloc_0);

                for (int i = 0; i < dataListCount; i++)
                {
                    var unionData = unionDataList[i];
                    var propertyLabel = il.DefineLabel();

                    il.Emit(OpCodes.Ldloc_0);
                    il.EmitLdc_I4(i + 1);
                    il.Emit(OpCodes.Bne_Un_S, propertyLabel);

                    unionData.EmitDeserializeInUTCSpot(il);
                    il.MarkLabel(propertyLabel);
                }

                if (isAbstractBaseType)
                    EmitException(il);
                else
                    unionDataList[dataListCount].EmitDeserializeInUTCSpot(il);
            }

            return builder.CreateType();
        }

        private static void EmitException(ILGenerator il)
        {
            // throw new InvalidOperationException("Unknown index: " + index);
            il.Emit(OpCodes.Ldstr, "Unknown index: ");
            il.Emit(OpCodes.Ldloca_S, 0);
            il.Emit(OpCodes.Call, Types.ByteToStringMethod);
            il.Emit(OpCodes.Call, Types.StringConcatMethod);
            il.Emit(OpCodes.Newobj, Types.InvalidOperationExceptionCtor);
            il.Emit(OpCodes.Throw);
        }

    }
}
