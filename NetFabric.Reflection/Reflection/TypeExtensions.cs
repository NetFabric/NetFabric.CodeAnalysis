using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

// ReSharper disable LoopCanBeConvertedToQuery

namespace NetFabric.Reflection;

public static partial class TypeExtensions
{

    /// <summary>
    /// Gets a value indicating whether <see cref="System.Type"/> implements the given interface <see cref="System.Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> to test.</param>
    /// <param name="interfaceType">The interface <see cref="System.Type"/> to test.</param>
    /// <param name="genericArguments">If methods returns <c>true</c> and interface <see cref="System.Type"/> is generic, contains the generic arguments of the implemented interface.</param>
    /// <returns><c>true</c> if <see cref="System.Type"/> implements interface <see cref="System.Type"/>; otherwise, <c>false</c>.</returns>
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

        foreach (var @interface in type.GetAllInterfaces())
        {
            if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == interfaceType.GetGenericTypeDefinition())
            {
                genericArguments = @interface.GetGenericArguments();
                return true;
            }
        }

        genericArguments = default;
        return false;
    }

    static IEnumerable<Type> GetAllInterfaces(this Type type)
    {
        foreach (var @interface in type.GetInterfaces())
        {
            yield return @interface;

            foreach (var baseInterface in @interface.GetAllInterfaces())
                yield return baseInterface;
        }
    }

    const BindingFlags PublicInstance =
        BindingFlags.Public | BindingFlags.Instance;

    internal static PropertyInfo? GetPublicInstanceReadProperty(this Type type, string name)
    {
        foreach (var property in type.GetProperties(PublicInstance))
        {
            if (property.Name == name && property.GetGetMethod() is not null)
                return property;
        }

        return null;
    }

    internal static MethodInfo? GetPublicInstanceMethod(this Type type, string name, params Type[] types)
        => type.GetMethod(name, PublicInstance, null, types, null);

    const BindingFlags PublicInstanceDeclaredOnly =
        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
    
    internal static PropertyInfo? GetPublicInstanceDeclaredOnlyReadProperty(this Type type, string name)
        => type.GetProperties(PublicInstanceDeclaredOnly)
            .FirstOrDefault(property => property.Name == name && property.GetGetMethod() is not null);
    
    internal static MethodInfo? GetPublicInstanceDeclaredOnlyMethod(this Type type, string name, params Type[] types)
        => type.GetMethod(name, PublicInstanceDeclaredOnly, null, types, null);

    static bool IsByRefLike(this Type type)
        => type.IsByRefLike;

    public static bool IsSpanOrReadOnlySpan(this Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            return genericTypeDefinition == typeof(Span<>) || genericTypeDefinition == typeof(ReadOnlySpan<>);
        }

        return false;
    }
}