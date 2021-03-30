using System;
using System.Reflection;
using System.Reflection.Emit;
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

        MethodInfo DeserializeInUTCSpot { get; }

        MethodInfo Add { get; }

        ISerializer Serializer { get; }

        void EmitDeserializeSpot(ILGenerator il);

        void EmitDeserializeInUTCSpot(ILGenerator il);
    }
}
