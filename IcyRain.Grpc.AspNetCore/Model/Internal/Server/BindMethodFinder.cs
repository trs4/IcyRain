using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Grpc.Core;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal static class BindMethodFinder
{
    private const BindingFlags BindMethodBindingFlags = BindingFlags.Public | BindingFlags.Static;

    internal static MethodInfo? GetBindMethod(Type serviceType)
        => GetBindMethodUsingAttribute(serviceType) ?? GetBindMethodFallback(serviceType);
    
    internal static MethodInfo? GetBindMethodUsingAttribute(Type serviceType)
    {
        Type? currentServiceType = serviceType;
        BindServiceMethodAttribute? bindServiceMethod;

        do
        {
            // Search through base types for bind service attribute
            // We need to know the base service type because it is used with GetMethod below
            bindServiceMethod = currentServiceType.GetCustomAttribute<BindServiceMethodAttribute>();

            if (bindServiceMethod != null)
            {
                // Bind method will be public and static
                // Two parameters: ServiceBinderBase and the service type
                return bindServiceMethod.BindType.GetMethod(
                    bindServiceMethod.BindMethodName,
                    BindMethodBindingFlags,
                    binder: null,
                    [typeof(ServiceBinderBase), currentServiceType],
                    []);
            }
        } while ((currentServiceType = currentServiceType.BaseType) != null);

        return null;
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075:UnrecognizedReflectionPattern",
        Justification = "Fallback doesn't have BindServiceMethodAttribute so can't be verified.")]
    internal static MethodInfo? GetBindMethodFallback(Type serviceType)
    {
        // Search for the generated service base class
        var baseType = GetServiceBaseType(serviceType);

        if (baseType is null)
            return null;

        // We need to call Foo.BindService from the declaring type.
        var declaringType = baseType.DeclaringType;

        // The method we want to call is public static void BindService(ServiceBinderBase, BaseType)
        return declaringType?.GetMethod(
            "BindService",
            BindMethodBindingFlags,
            binder: null,
            [typeof(ServiceBinderBase), baseType],
            []);
    }

    private static Type? GetServiceBaseType(Type serviceImplementation)
    {
        // TService is an implementation of the gRPC service. It ultimately derives from Foo.TServiceBase base class.
        // We need to access the static BindService method on Foo which implicitly derives from Object.
        var baseType = serviceImplementation.BaseType;

        // Handle services that have multiple levels of inheritence
        while (baseType?.BaseType?.BaseType != null)
        {
            baseType = baseType.BaseType;
        }

        return baseType;
    }
}
