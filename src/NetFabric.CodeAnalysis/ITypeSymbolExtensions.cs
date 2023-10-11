using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
// ReSharper disable InvertIf

namespace NetFabric.CodeAnalysis;

public static partial class ITypeSymbolExtensions
{
    
    /// <summary>
    /// Gets a value indicating whether <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> implements the given interface type.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="System.Type"/> to test.</param>
    /// <param name="interfaceType">The interface <see cref="System.Type"/> to test.</param>
    /// <param name="genericArguments">If methods returns <c>true</c> and interface type is generic, contains the generic arguments of the implemented interface.</param>
    /// <returns><c>true</c> if <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> implements interface type; otherwise, <c>false</c>.</returns>
    public static bool ImplementsInterface(this ITypeSymbol typeSymbol, SpecialType interfaceType, out ImmutableArray<ITypeSymbol> genericArguments)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol 
            && namedTypeSymbol.OriginalDefinition.SpecialType == interfaceType)
        {
            genericArguments = namedTypeSymbol.TypeArguments;
            return true;
        }
        
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var @interface in typeSymbol.AllInterfaces)
        {
            if (@interface.OriginalDefinition.SpecialType == interfaceType)
            {
                genericArguments = @interface.TypeArguments;
                return true;
            }
        }

        genericArguments = default;
        return false;
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> implements the given interface type.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="System.Type"/> to test.</param>
    /// <param name="interfaceType">The interface <see cref="System.Type"/> to test.</param>
    /// <param name="genericArguments">If methods returns <c>true</c> and interface type is generic, contains the generic arguments of the implemented interface.</param>
    /// <returns><c>true</c> if <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> implements interface type; otherwise, <c>false</c>.</returns>
    public static bool ImplementsInterface(this ITypeSymbol typeSymbol, INamedTypeSymbol interfaceType, out ImmutableArray<ITypeSymbol> genericArguments)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol 
            && SymbolEqualityComparer.Default.Equals(namedTypeSymbol.OriginalDefinition, interfaceType))
        {
            genericArguments = namedTypeSymbol.TypeArguments;
            return true;
        }
        
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var @interface in typeSymbol.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(@interface.OriginalDefinition, interfaceType))
            {
                genericArguments = @interface.TypeArguments;
                return true;
            }
        }

        genericArguments = default;
        return false;
    }

    /// <summary>
    /// Gets a public readable indexer property with the specified parameter types from the given type symbol.
    /// </summary>
    /// <param name="typeSymbol">The type symbol from which to retrieve the indexer property.</param>
    /// <param name="parameterTypes">An array of parameter types, represented by <see cref="ITypeSymbol"/>, that the indexer should accept (empty array for parameterless).</param>
    /// <returns>
    /// An <see cref="IPropertySymbol"/> representing the public readable indexer property if found; otherwise, returns <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method searches for a public readable indexer property within the given type symbol with the specified parameter types.
    /// If a matching indexer property is found, and it has a getter, it returns an <see cref="IPropertySymbol"/> representing the indexer;
    /// otherwise, it returns <c>null</c>.
    /// </remarks>
    public static IPropertySymbol? GetPublicReadIndexer(this ITypeSymbol typeSymbol, params ITypeSymbol[] parameterTypes)
    {
        foreach (var member in typeSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (member.IsIndexer && 
                !member.IsStatic && 
                member.DeclaredAccessibility == Accessibility.Public && 
                member.GetMethod is not null && 
                SequenceEqual(member.Parameters, parameterTypes))
            {
                return member;
            }
        }

        if (typeSymbol.TypeKind == TypeKind.Interface)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var @interface in typeSymbol.AllInterfaces)
            {
                var property = @interface.GetPublicReadIndexer(parameterTypes);
                if (property is not null)
                    return property;
            }
        }
        else
        {
            var baseType = typeSymbol.BaseType;
            if (baseType is not null)
                return baseType.GetPublicReadIndexer(parameterTypes);
        }

        return null;
    }

    /// <summary>
    /// Gets a public readable indexer property with the specified parameter types from the given type symbol.
    /// </summary>
    /// <param name="typeSymbol">The type symbol from which to retrieve the indexer property.</param>
    /// <param name="parameterTypes">An array of parameter types that the indexer should accept (empty array for parameterless).</param>
    /// <returns>
    /// An <see cref="IPropertySymbol"/> representing the public readable indexer property if found; otherwise, returns <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method searches for a public readable indexer property within the given type symbol with the specified parameter types.
    /// If a matching indexer property is found, and it has a getter, it returns an <see cref="IPropertySymbol"/> representing the indexer;
    /// otherwise, it returns <c>null</c>.
    /// </remarks>
    public static IPropertySymbol? GetPublicReadIndexer(this ITypeSymbol typeSymbol, params Type[] parameterTypes)
    {
        foreach (var member in typeSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (member.IsIndexer && 
                !member.IsStatic && 
                member.DeclaredAccessibility == Accessibility.Public && 
                member.GetMethod is not null && 
                SequenceEqual(member.Parameters, parameterTypes))
            {
                return member;
            }
        }

        if (typeSymbol.TypeKind == TypeKind.Interface)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var @interface in typeSymbol.AllInterfaces)
            {
                var property = @interface.GetPublicReadIndexer(parameterTypes);
                if (property is not null)
                    return property;
            }
        }
        else
        {
            var baseType = typeSymbol.BaseType;
            if (baseType is not null)
                return baseType.GetPublicReadIndexer(parameterTypes);
        }

        return null;
    }

    /// <summary>
    /// Gets a public readable property with the specified name from the given type symbol.
    /// </summary>
    /// <param name="typeSymbol">The type symbol from which to retrieve the property.</param>
    /// <param name="name">The name of the property to search for.</param>
    /// <returns>
    /// An <see cref="IPropertySymbol"/> representing the public readable property if found; otherwise, returns <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method searches for a public readable property with the specified name within the given type symbol.
    /// If a matching property is found and it has a getter, it returns an <see cref="IPropertySymbol"/> representing the property;
    /// otherwise, it returns <c>null</c>.
    /// </remarks>
    public static IPropertySymbol? GetPublicReadProperty(this ITypeSymbol typeSymbol, string name)
    {
        foreach (var member in typeSymbol.GetMembers(name).OfType<IPropertySymbol>())
        {
            if (!member.IsStatic && 
                member.DeclaredAccessibility == Accessibility.Public && 
                member.GetMethod is not null)
                return member;
        }

        if (typeSymbol.TypeKind == TypeKind.Interface)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var @interface in typeSymbol.AllInterfaces)
            {
                var property = @interface.GetPublicReadProperty(name);
                if (property is not null)
                    return property;
            }
        }
        else
        {
            var baseType = typeSymbol.BaseType;
            if (baseType is not null)
                return baseType.GetPublicReadProperty(name);
        }

        return null;
    }

    /// <summary>
    /// Gets a public method with the specified name and parameter types from the given type symbol.
    /// </summary>
    /// <param name="typeSymbol">The type symbol from which to retrieve the method.</param>
    /// <param name="name">The name of the method to search for.</param>
    /// <param name="parameters">An array of parameter types that the method should accept (empty array for parameterless).</param>
    /// <returns>
    /// An <see cref="IMethodSymbol"/> representing the public method if found; otherwise, returns <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method searches for a public method with the specified name and parameter types within the given type symbol.
    /// If a matching method is found, it returns an <see cref="IMethodSymbol"/> that represents the method;
    /// otherwise, it returns <c>null</c>.
    /// </remarks>
    public static IMethodSymbol? GetPublicMethod(this ITypeSymbol typeSymbol, string name, params Type[] parameters)
    {
        foreach (var member in typeSymbol.GetMembers(name).OfType<IMethodSymbol>())
        {
            if (!member.IsStatic &&
                member.DeclaredAccessibility == Accessibility.Public &&
                SequenceEqual(member.Parameters, parameters))
            {
                return member;
            }
        }

        if (typeSymbol.TypeKind == TypeKind.Interface)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var @interface in typeSymbol.AllInterfaces)
            {
                var method = @interface.GetPublicMethod(name, parameters);
                if (method is not null)
                    return method;
            }
        }
        else
        {
            var baseType = typeSymbol.BaseType;
            if (baseType is not null)
                return baseType.GetPublicMethod(name, parameters);
        }

        return null;
    }

    static bool SequenceEqual(ImmutableArray<IParameterSymbol> parameters, Type[] types)
    {
        if (parameters.Length != types.Length)
            return false;

        // ReSharper disable once LoopCanBeConvertedToQuery
        for (var index = 0; index < parameters.Length; index++)
        {
            if (parameters[index].Type.MetadataName != types[index].Name)
                return false;
        }

        return true;
    }

    static bool SequenceEqual(ImmutableArray<IParameterSymbol> parameters, ITypeSymbol[] types)
    {
        if (parameters.Length != types.Length)
            return false;

        // ReSharper disable once LoopCanBeConvertedToQuery
        for (var index = 0; index < parameters.Length; index++)
        {
            if (parameters[index].Type.MetadataName != types[index].MetadataName)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether the given type symbol represents a Span or ReadOnlySpan type.
    /// </summary>
    /// <param name="typeSymbol">The type symbol to be checked.</param>
    /// <returns>
    /// <c>true</c> if the type symbol represents a Span or ReadOnlySpan; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method checks whether the provided <paramref name="typeSymbol"/> corresponds to a type that is compatible with
    /// the Span and ReadOnlySpan types, which are used for efficient and safe data access in .NET.
    /// </remarks>
    public static bool IsSpanOrReadOnlySpanType(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.MetadataName == "System.Span" || typeSymbol.MetadataName == "System.ReadOnlySpan")
        {
            return typeSymbol is INamedTypeSymbol namedType && 
                namedType.TypeArguments.Length == 1;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified <see cref="ITypeSymbol"/> represents an integer type within the given <see cref="Compilation"/>.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> to check for an integer type.</param>
    /// <param name="compilation">The <see cref="Compilation"/> containing the semantic information about the code.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="ITypeSymbol"/> represents an integer type; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method checks if the <see cref="ITypeSymbol"/> represents an integer type within the context of the provided <see cref="Compilation"/>.
    /// Starting from .NET 7, it also checks if the <see cref="ITypeSymbol"/> implements the <see cref="System.Numerics.IBinaryInteger{T}"/> interface,
    /// which indicates support for binary integer operations.
    /// </remarks>
    public static bool IsIntegerType(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        if (typeSymbol.MetadataName == "System.SByte" || 
            typeSymbol.MetadataName == "System.Byte" || 
            typeSymbol.MetadataName == "System.Int16" || 
            typeSymbol.MetadataName == "System.UInt16" || 
            typeSymbol.MetadataName == "System.Int32" || 
            typeSymbol.MetadataName == "System.UInt32" || 
            typeSymbol.MetadataName == "System.Int64" || 
            typeSymbol.MetadataName == "System.UInt64")
        {
            return true;
        }

        // supported starting from .NET 7
        var binaryIntegerType = compilation.GetTypeByMetadataName("System.Numerics.IBinaryInteger`1")!;
        if (binaryIntegerType is not null &&
            typeSymbol.ImplementsInterface(binaryIntegerType, out var arguments) && 
            arguments.Length == 1 &&
            arguments[0].MetadataName == typeSymbol.MetadataName)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified <see cref="ITypeSymbol"/> represents a floating-point numeric type within the context of the provided <see cref="Compilation"/>.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> to check for a floating-point numeric type.</param>
    /// <param name="compilation">The <see cref="Compilation"/> containing the semantic information about the code.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="ITypeSymbol"/> represents a floating-point numeric type; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Starting from .NET 7, this method checks if the <see cref="ITypeSymbol"/> represents a floating-point numeric type within the context of the provided <see cref="Compilation"/>.
    /// It also compares the <see cref="ITypeSymbol"/> to the <see cref="System.Numerics.IFloatingPoint{T}"/> interface,
    /// which indicates support for floating-point numeric operations.
    /// </remarks>
    public static bool IsFloatingPointType(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        if (typeSymbol.MetadataName == "System.Half" || 
            typeSymbol.MetadataName == "System.Float" || 
            typeSymbol.MetadataName == "System.Double" || 
            typeSymbol.MetadataName == "System.Decimal")
        {
            return true;
        }

        // supported starting from .NET 7
        var floatingPointType = compilation.GetTypeByMetadataName("System.Numerics.IFloatingPoint`1")!;
        if (floatingPointType is not null &&
            typeSymbol.ImplementsInterface(floatingPointType, out var arguments) && 
            arguments.Length == 1 &&
            arguments[0].MetadataName == typeSymbol.MetadataName)
        {
            return true;
        }

        return false;
    }

}
