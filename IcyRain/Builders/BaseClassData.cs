using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Builders;

internal sealed class BaseClassData
{
    private BaseClassData(FieldBuilder constructorField, bool hasCapacityConstructor, IBuilderData data, FieldBuilder field)
    {
        ConstructorField = constructorField;
        HasCapacityConstructor = hasCapacityConstructor;
        Data = data;
        Field = field;
    }

    public static BaseClassData Create(Type type, TypeBuilder builder)
    {
        var baseCollectionType = BaseCollectionBuilder.GetType(type);

        if (baseCollectionType is null)
            return null;

        var constructorField = builder.DefineField(Naming.BaseConstructorField, typeof(ConstructorInfo), Flags.PrivateReadOnlyField);
        bool hasCapacityConstructor = type.GetConstructors().Any(c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == Types.Int);

        var data = ResolverHelper.GetBuilderData(baseCollectionType);
        string baseName = Naming.BaseFieldPrefix + nameof(Serializer<,>);
        var field = builder.DefineField(baseName, data.SerializerType, Flags.PrivateReadOnlyField);
        return new BaseClassData(constructorField, hasCapacityConstructor, data, field);
    }

    public FieldBuilder ConstructorField { get; }

    public bool HasCapacityConstructor { get; }

    public IBuilderData Data { get; }

    public FieldBuilder Field { get; }
}
