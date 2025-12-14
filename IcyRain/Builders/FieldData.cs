using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IcyRain.Builders;

internal sealed class FieldData
{
    private readonly bool _isVirtualGetPropertyMethod;
    private readonly MethodInfo _getPropertyMethod;

    private readonly MethodInfo _setPropertyMethod;
    private readonly bool _isVirtualSetPropertyMethod;

    private ConstructorInfo _realPropertyTypeConstructor;

    public FieldData(PropertyInfo property, FieldInfo field, Type propertyType, bool isNullable, IBuilderData data)
    {
        _getPropertyMethod = property.GetGetMethod();
        _isVirtualGetPropertyMethod = _getPropertyMethod.IsVirtual;

        _setPropertyMethod = property.GetSetMethod();
        _isVirtualSetPropertyMethod = _setPropertyMethod.IsVirtual;

        Field = field;
        RealPropertyType = isNullable ? property.PropertyType : propertyType;
        PropertyType = propertyType;
        IsNullable = isNullable;
        Data = data;
    }

    public FieldInfo Field { get; }

    public Type RealPropertyType { get; }

    public ConstructorInfo RealPropertyTypeConstructor
        => _realPropertyTypeConstructor ??= RealPropertyType.GetConstructors()[0];

    public Type PropertyType { get; }

    public bool IsNullable { get; }

    public int VariableIndex { get; set; } = -1;

    public IBuilderData Data { get; }

    public void EmitGetProperty(ILGenerator il)
        => il.Emit(_isVirtualGetPropertyMethod ? OpCodes.Callvirt : OpCodes.Call, _getPropertyMethod);

    public void EmitSetProperty(ILGenerator il)
        => il.Emit(_isVirtualSetPropertyMethod ? OpCodes.Callvirt : OpCodes.Call, _setPropertyMethod);
}
