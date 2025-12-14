using System;
using IcyRain.Internal;

namespace IcyRain.Builders;

internal static class BaseCollectionBuilder
{
    public static Type GetType(Type type)
    {
        type = type.BaseType;

        while (type is not null)
        {
            if (type.IsGenericType)
            {
                var definitionType = type.GetGenericTypeDefinition();

                if (definitionType == Types.List || definitionType == Types.HashSet || definitionType == Types.Dictionary)
                    return type;
            }

            type = type.BaseType;
        }

        return type;
    }

}
