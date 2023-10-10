using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using Microsoft.VisualBasic;

// ReSharper disable LoopCanBeConvertedToQuery

namespace NetFabric.Reflection;

public static partial class TypeExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="Type"/> implements a specified interface and, if so, provides information about generic type arguments.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check for interface implementation.</param>
    /// <param name="interfaceType">The <see cref="Type"/> of the interface to check for implementation.</param>
    /// <param name="genericArguments">
    /// When the method returns <c>true</c>, this parameter contains an array of <see cref="Type"/> objects representing the generic type arguments
    /// that make the implementation of the specified interface. If the type does not implement the interface or if the interface is non-generic,
    /// this parameter is set to <c>null</c>.
    /// </param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="Type"/> implements the specified interface; otherwise, <c>false</c>.
    /// </returns>
    public static bool ImplementsInterface(this Type type, Type interfaceType, [NotNullWhen(true)] out Type[]? genericArguments)
    {
        if (!interfaceType.IsGenericType)
        {
            genericArguments = Type.EmptyTypes;
            return interfaceType.IsAssignableFrom(type);
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == interfaceType.GetGenericTypeDefinition())
        {
            genericArguments = type.GetGenericArguments();
            return true;
        }

        foreach (var @interface in type.GetInterfaces())
        {
            if (ImplementsInterface(@interface, interfaceType, out genericArguments))
                return true;
        }

        genericArguments = default;
        return false;
    }

    const BindingFlags PublicInstance =
        BindingFlags.Public | BindingFlags.Instance;

    internal static PropertyInfo? GetPublicReadIndexer(this Type type, params Type[] parameterTypes)
    {
        foreach (var property in type.GetProperties(PublicInstance))
        {
            if (property.GetGetMethod() is not null && 
                SequenceEqual(property.GetIndexParameters(), parameterTypes))
            {
                return property;
            }
        }

        if (type.IsInterface)
        {
            foreach(var @interface in type.GetInterfaces())
            {
                var indexer = GetPublicReadIndexer(@interface, parameterTypes);
                if (indexer is not null)
                    return indexer;
            }
        }

        return default;
    }

    internal static PropertyInfo? GetPublicReadProperty(this Type type, string name)
    {
        foreach (var property in type.GetProperties(PublicInstance))
        {
            if (property.Name == name && 
            property.GetGetMethod() is not null)
            {
                return property;
            }
        }

        if (type.IsInterface)
        {
            foreach(var @interface in type.GetInterfaces())
            {
                var indexer = GetPublicReadProperty(@interface, name);
                if (indexer is not null)
                    return indexer;
            }
        }

        return default;
    }

    //internal static PropertyInfo? GetExplicitReadProperty(this Type type, Type interfaceType, string name)
    //{
    //    var interfaceMap = type.GetInterfaceMap(interfaceType);

    //    var getMethodName = $"get_{name}";
    //    foreach (var methodInfo in interfaceMap.InterfaceMethods)
    //    {
    //        if (methodInfo.Name == getMethodName)
    //        {
    //            return interfaceType.GetProperty(name);
    //        }
    //    }  

    //    return default;
    //}

    internal static MethodInfo? GetPublicMethod(this Type type, string name, params Type[] types)
    {
        var method = type.GetMethod(name, PublicInstance, types);
        if (method is not null)
            return method;

        if (type.IsInterface)
        {
            foreach(var @interface in type.GetInterfaces())
            {
                method = GetPublicMethod(@interface, name);
                if (method is not null)
                    return method;
            }
        }            

        return default;
    }

    //internal static MethodInfo? GetExplicitMethod(this Type type, Type interfaceType, string name, params Type[] types)
    //{
    //    var interfaceMap = type.GetInterfaceMap(interfaceType);
    //    var interfaceMethods = interfaceMap.InterfaceMethods;
    //    for (int index = 0; index < interfaceMethods.Length; index++)
    //    {
    //        if (interfaceMethods[index].Name == name)
    //        {
    //            var methodInfo = interfaceMap.TargetMethods[index];
    //            if(SequenceEqual(methodInfo.GetParameters(), types))
    //                return methodInfo;
    //        }
    //    }

    //    return default;
    //}

    static bool SequenceEqual(ParameterInfo[] parameters, Type[] types)
    {
        if (parameters.Length != types.Length)
            return false;

        // ReSharper disable once LoopCanBeConvertedToQuery
        for (var index = 0; index < parameters.Length; index++)
        {
            if (parameters[index].ParameterType != types[index])
                return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether the specified <see cref="Type"/> represents a <see cref="Span{T}"/> or <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check for <see cref="Span{T}"/> or <see cref="ReadOnlySpan{T}"/>.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="Type"/> represents a <see cref="Span{T}"/> or <see cref="ReadOnlySpan{T}"/>;
    ///   otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSpanOrReadOnlySpan(this Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            return genericTypeDefinition == typeof(Span<>) || genericTypeDefinition == typeof(ReadOnlySpan<>);
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified <see cref="Type"/> represents an integer type.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check for integer type.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="Type"/> represents an integer type; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Starting from .NET 7, this method checks if the <see cref="Type"/> implements the <see cref="System.Numerics.IBinaryInteger{T}"/> interface,
    /// which indicates support for binary integer operations.
    /// </remarks>
    public static bool IsIntegerType(this Type type)
    {
        if (type == typeof(SByte) || 
            type == typeof(Byte) || 
            type == typeof(Int16) || 
            type == typeof(UInt16) || 
            type == typeof(Int32) || 
            type == typeof(UInt32) || 
            type == typeof(Int64) || 
            type == typeof(UInt64)) 
        {
            return true;
        }

#if NET7_0_OR_GREATER
        if (type.ImplementsInterface(typeof(IBinaryInteger<>), out var arguments) && 
            arguments.Length == 1 &&
            arguments[0] == type)
        {
            return true;
        }
#endif

        return false;
    }

    /// <summary>
    /// Determines whether the specified <see cref="Type"/> represents a floating-point numeric type.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check for a floating-point numeric type.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="Type"/> represents a floating-point numeric type; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Starting from .NET 7, this method compares the <see cref="Type"/> to the <see cref="System.Numerics.IFloatingPoint{T}"/> interface,
    /// which indicates support for floating-point numeric operations.
    /// </remarks>
    public static bool IsFloatingPointType(this Type type)
    {
        if (type == typeof(Half) || 
            type == typeof(float) || 
            type == typeof(double) || 
            type == typeof(decimal))
        {
            return true;
        }

#if NET7_0_OR_GREATER
        if (type.ImplementsInterface(typeof(IFloatingPoint<>), out var arguments) && 
            arguments.Length == 1 &&
            arguments[0] == type)
        {
            return true;
        }
#endif

        return false;
    }

}