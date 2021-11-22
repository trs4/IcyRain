using System;
using System.Reflection;
using System.Text;

namespace IcyRain.Internal
{
    internal static class Naming
    {
        [ThreadStatic]
        private static StringBuilder CacheBuilder;

        public const string FieldPreffix = "_s_";
        public const string TypeField = "_s_Type";
        public const string Value = "value";

        public static readonly string[] Serialize = new[] { "writer", "value" };
        public static readonly ParameterAttributes[] SerializeParams = new[] { ParameterAttributes.None, ParameterAttributes.In };
        public static readonly string[] Deserialize = new[] { "reader" };

        public static string GetName(Type type, bool isUnion = false, bool forSerializer = false)
        {
            var builder = CacheBuilder ??= new StringBuilder(256);
            CacheBuilder.Length = 0;

            builder.Append(DynamicAssembly.ModuleName).Append('.');

            if (isUnion)
                builder.Append("Union.");

            BuildName(builder, type, isUnion, true);

            if (forSerializer)
                builder.Append("$Serializer");

            return builder.ToString();
        }

        public static string GetSerializerNotFoundMessage<T>()
            => $"Serializer '{typeof(T).Name}' not found";

        public static string GetSerializerNotFoundMessage(Type type)
            => $"Serializer '{type.Name}' not found";

        private static void BuildName(StringBuilder builder, Type type, bool isUnion, bool isFirst)
        {
            if (type.IsArray)
            {
                BuildName(builder, type.GetElementType(), isUnion, isFirst);
                builder.Append("$A");
            }
            else if (type.IsGenericType)
            {
                var argumentTypes = type.GetGenericArguments();
                int index = type.Name.IndexOf('`');

                if (isFirst)
                    builder.Append(type.Namespace).Append('.');
                else
                    builder.Append(type.Namespace.Replace(".", "_")).Append('_');

                builder.Append(type.Name.Substring(0, index)).Append("$G").Append(argumentTypes.Length);

                foreach (var argumentType in argumentTypes)
                {
                    builder.Append('_');
                    BuildName(builder, argumentType, isUnion, false);
                }
            }
            else
            {
                string name = type.GetBaseTypeName();
                builder.Append(name ?? (isFirst ? type.FullName : type.FullName.Replace(".", "_")));
            }
        }

    }
}
