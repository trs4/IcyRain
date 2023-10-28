using System;
using System.Reflection;
using System.Reflection.Emit;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Builders;

internal static class UnionHashBuilder
{
    public const int TypesCount = 4;

    public static object Get(Type type, Type[] unionTypes)
    {
        var serializerType = BuildSerializer(type, unionTypes);
        return Activator.CreateInstance(serializerType);
    }

    private static Type BuildSerializer(Type type, Type[] unionTypes)
    {
        bool isByteResolver = unionTypes.Length <= byte.MaxValue;

        var data = isByteResolver
            ? BuilderData<UnionByteResolver>.Get(type)
            : BuilderData<UnionUShortResolver>.Get(type);

        string typeName = Naming.GetName(type, isUnion: true, forSerializer: true);
        var builder = DynamicAssembly.Module.DefineType(typeName, Flags.Type, data.SerializerType);

        var addMethod = data.Add;

        // .ctor
        {
            var method = builder.DefineConstructor(Flags.Constructor, CallingConventions.Standard, Type.EmptyTypes);

            var il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.EmitLdc_I4(unionTypes.Length);
            il.Emit(OpCodes.Call, data.Constructor);

            for (int i = 0; i < unionTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.EmitLdc_I4(i + 1);
                il.Emit(OpCodes.Call, addMethod.MakeGenericMethod(unionTypes[i]));
            }

            il.Emit(OpCodes.Ret);
        }

        return builder.CreateType();
    }

}
