using System;
using System.Reflection;
using IcyRain.Serializers;

namespace IcyRain.Builders
{
    internal interface IBuilderData
    {
        Type Type { get; }

        PropertyInfo[] Properties { get; }

        Type SerializerType { get; }

        bool IsBytePropertyIndexes { get; }

        int PropertyIndexSize { get; }

        int EndMessageCode { get; }

        MethodInfo WriteMethod { get; }

        MethodInfo ReadMethod { get; }

        ConstructorInfo Constructor { get; }

        MethodInfo GetInstance { get; }

        MethodInfo GetSize { get; }

        MethodInfo GetCapacity { get; }

        MethodInfo SerializeSpot { get; }

        MethodInfo DeserializeSpot { get; }

        MethodInfo Add { get; }

        ISerializer Serializer { get; }
    }
}
