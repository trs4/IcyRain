using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace IcyRain.Internal;

internal static class BuilderExtensions
{
    private static readonly CustomAttributeBuilder _aggressiveInliningBuilder
        = new CustomAttributeBuilder(typeof(MethodImplAttribute).GetConstructor([typeof(MethodImplOptions)]), [Flags.HotPath]);

    private static readonly CustomAttributeBuilder _isReadOnlyBuilder
        = new CustomAttributeBuilder(typeof(IsReadOnlyAttribute).GetConstructor(Type.EmptyTypes), []);

    public static ConstructorBuilder WithName(this ConstructorBuilder constructor, string name)
    {
        constructor.DefineParameter(1, ParameterAttributes.None, name);
        return constructor;
    }

    public static ConstructorBuilder WithNames(this ConstructorBuilder constructor, params string[] names)
    {
        for (int i = 0; i < names.Length; i++)
            constructor.DefineParameter(i + 1, ParameterAttributes.None, names[i]);

        return constructor;
    }

    public static MethodBuilder WithName(this MethodBuilder method, string name)
    {
        method.DefineParameter(1, ParameterAttributes.None, name);
        return method;
    }

    public static MethodBuilder WithName(this MethodBuilder method, ParameterAttributes attribute, string name)
    {
        var builder = method.DefineParameter(1, attribute, name);

        if (attribute == ParameterAttributes.In)
            builder.SetCustomAttribute(_isReadOnlyBuilder);

        return method;
    }

    public static MethodBuilder WithNames(this MethodBuilder method, ParameterAttributes[] attributes, params string[] names)
    {
        for (int i = 0; i < names.Length; i++)
        {
            var attribute = attributes[i];
            var builder = method.DefineParameter(i + 1, attribute, names[i]);

            if (attribute == ParameterAttributes.In)
                builder.SetCustomAttribute(_isReadOnlyBuilder);
        }

        return method;
    }

    public static MethodBuilder WithNames(this MethodBuilder method, params string[] names)
    {
        for (int i = 0; i < names.Length; i++)
            method.DefineParameter(i + 1, ParameterAttributes.None, names[i]);

        return method;
    }

    public static MethodBuilder WithAggressiveInlining(this MethodBuilder method)
    {
        method.SetCustomAttribute(_aggressiveInliningBuilder);
        return method;
    }

}
