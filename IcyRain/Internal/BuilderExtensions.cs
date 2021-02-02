using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace IcyRain.Internal
{
    internal static class BuilderExtensions
    {
        private static readonly CustomAttributeBuilder _aggressiveInliningBuilder
            = new CustomAttributeBuilder(
                typeof(MethodImplAttribute).GetConstructor(new[] { typeof(MethodImplOptions) }),
                new object[] { Flags.HotPath });

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
}
