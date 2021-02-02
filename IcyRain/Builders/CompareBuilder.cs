using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using IcyRain.Internal;

namespace IcyRain.Builders
{
    internal static class CompareBuilder
    {
        private static readonly Dictionary<Type, EmitDelegate> _map = new()
        {
            { typeof(bool), EmitBool },
            { typeof(char), EmitBool },
            { typeof(sbyte), EmitBool },
            { typeof(byte), EmitBool },
            { typeof(short), EmitBool },
            { typeof(ushort), EmitBool },
            { typeof(int), EmitBool },
            { typeof(uint), EmitBool },
            { typeof(long), EmitBool },
            { typeof(ulong), EmitBool },
            { typeof(float), EmitFloat },
            { typeof(double), EmitDouble },
            { typeof(decimal), EmitDecimal },
            { typeof(IntPtr), EmitIntPtr },
            { typeof(UIntPtr), EmitUIntPtr },
            { typeof(ArraySegment<byte>), EmitByteSegment },
        };

        private delegate void EmitDelegate(ILGenerator il, Label label, FieldData data);

        public static void Emit(ILGenerator il, Label label, FieldData data)
        {
            if (data.PropertyType.IsClass)
            {
                EmitBool(il, label, data);
            }
            else if (_map.TryGetValue(data.PropertyType, out var emitDelegate))
            {
                emitDelegate(il, label, data);
            }
            else if (data.PropertyType.IsNullable())
            {
                il.Emit(OpCodes.Ldarg_2);
                data.EmitGetProperty(il);
                il.Emit(OpCodes.Stloc, data.VariableIndex);
                il.Emit(OpCodes.Ldloca_S, data.VariableIndex);
                il.Emit(OpCodes.Call, data.RealPropertyType.GetMethod("get_HasValue"));
                il.Emit(OpCodes.Brfalse_S, label);
            }
            else
            {
                var method = (data.PropertyType.HasEquatableInterface() ? Types.IsEmptyEqutableStructMethod : Types.IsEmptyStructMethod)
                    .MakeGenericMethod(data.PropertyType);

                il.Emit(OpCodes.Ldarg_2);
                data.EmitGetProperty(il);
                il.Emit(OpCodes.Call, method);
                il.Emit(OpCodes.Brtrue_S, label);
            }
        }

        private static void EmitBool(ILGenerator il, Label label, FieldData data)
        {
            il.Emit(OpCodes.Ldarg_2);
            data.EmitGetProperty(il);
            il.Emit(OpCodes.Brfalse_S, label);
        }

        private static void EmitFloat(ILGenerator il, Label label, FieldData data)
        {
            il.Emit(OpCodes.Ldarg_2);
            data.EmitGetProperty(il);
            il.Emit(OpCodes.Ldc_R4, 0.0f);
            il.Emit(OpCodes.Beq_S, label);
        }

        private static void EmitDouble(ILGenerator il, Label label, FieldData data)
        {
            il.Emit(OpCodes.Ldarg_2);
            data.EmitGetProperty(il);
            il.Emit(OpCodes.Ldc_R8, 0.0);
            il.Emit(OpCodes.Beq_S, label);
        }

        private static void EmitDecimal(ILGenerator il, Label label, FieldData data)
        {
            il.Emit(OpCodes.Ldarg_2);
            data.EmitGetProperty(il);
            il.Emit(OpCodes.Ldsfld, Types.DecimalZeroField);
            il.Emit(OpCodes.Call, Types.DecimalOpInequalityMethod);
            il.Emit(OpCodes.Brfalse_S, label);
        }

        private static void EmitIntPtr(ILGenerator il, Label label, FieldData data)
        {
            il.Emit(OpCodes.Ldarg_2);
            data.EmitGetProperty(il);
            il.Emit(OpCodes.Ldsfld, Types.IntPtrZeroField);
            il.Emit(OpCodes.Call, Types.IntPtrOpInequalityMethod);
            il.Emit(OpCodes.Brfalse_S, label);
        }

        private static void EmitUIntPtr(ILGenerator il, Label label, FieldData data)
        {
            il.Emit(OpCodes.Ldarg_2);
            data.EmitGetProperty(il);
            il.Emit(OpCodes.Ldsfld, Types.UIntPtrZeroField);
            il.Emit(OpCodes.Call, Types.UIntPtrOpInequalityMethod);
            il.Emit(OpCodes.Brfalse_S, label);
        }

        private static void EmitByteSegment(ILGenerator il, Label label, FieldData data)
        {
            il.Emit(OpCodes.Ldarg_2);
            data.EmitGetProperty(il);
            il.Emit(OpCodes.Stloc, data.VariableIndex);
            il.Emit(OpCodes.Ldloca_S, data.VariableIndex);
            il.Emit(OpCodes.Call, Types.BytesSegmentCountMethod);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ble_S, label);
        }

    }
}
