using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
// ReSharper disable InvertIf

namespace NetFabric.CodeAnalysis
{
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

        internal static IPropertySymbol? GetPublicReadProperty(this ITypeSymbol typeSymbol, string name)
        {
            foreach (var member in typeSymbol.GetMembers(name).OfType<IPropertySymbol>())
            {
                if (!member.IsStatic && member.DeclaredAccessibility == Accessibility.Public && member.GetMethod is not null)
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

        internal static IMethodSymbol? GetPublicMethod(this ITypeSymbol typeSymbol, string name, params Type[] parameters)
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
    }
}
