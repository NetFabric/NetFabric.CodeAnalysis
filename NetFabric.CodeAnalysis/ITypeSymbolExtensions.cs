using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;

namespace NetFabric.CodeAnalysis
{
    public static class ITypeSymbolExtensions
    {
        public static bool IsEnumerable(this ITypeSymbol typeSymbol, Compilation compilation, out EnumerableSymbols enumerableSymbols)
        {
            if (typeSymbol is null)
                throw new ArgumentNullException(nameof(typeSymbol));
            if (compilation is null)
                throw new ArgumentNullException(nameof(compilation));

            if (!typeSymbol.IsEnumerableType(compilation, out var getEnumerator))
            {
                enumerableSymbols = default;
                return false;
            }

            var isEnumerator = getEnumerator.ReturnType.IsEnumeratorType(compilation, out var current, out var moveNext, out var dispose);
            enumerableSymbols = new EnumerableSymbols(getEnumerator, current, moveNext, dispose);
            return isEnumerator;
        }

        public static bool IsEnumerator(this ITypeSymbol typeSymbol, Compilation compilation, out EnumeratorSymbols enumeratorSymbols)
        {
            if (typeSymbol is null)
                throw new ArgumentNullException(nameof(typeSymbol));
            if (compilation is null)
                throw new ArgumentNullException(nameof(compilation));

            var isEnumerator = typeSymbol.IsEnumeratorType(compilation, out var current, out var moveNext, out var dispose);
            enumeratorSymbols = new EnumeratorSymbols(current, moveNext, dispose);
            return isEnumerator;
        }

        static bool IsEnumerableType(this ITypeSymbol typeSymbol, Compilation compilation, out IMethodSymbol getEnumerator)
        {
            getEnumerator = typeSymbol.GetPublicMethod("GetEnumerator");
            if (getEnumerator is object)
                return true;

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_Generic_IEnumerable_T, out var genericArguments))
            {
                var interfaceType = compilation
                    .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T)
                    .Construct(genericArguments[0]);
                getEnumerator = interfaceType.GetPublicMethod("GetEnumerator");
                return true;
            }

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_IEnumerable, out _))
            {
                var interfaceType = compilation
                    .GetSpecialType(SpecialType.System_Collections_IEnumerable);
                getEnumerator = interfaceType.GetPublicMethod("GetEnumerator");
                return true;
            }

            return false;
        }

        static bool IsEnumeratorType(this ITypeSymbol typeSymbol, Compilation compilation, out IPropertySymbol current, out IMethodSymbol moveNext, out IMethodSymbol dispose)
        {
            if (typeSymbol.ImplementsInterface(SpecialType.System_IDisposable, out _))
                dispose = compilation.GetSpecialType(SpecialType.System_IDisposable).GetPublicMethod("Dispose");
            else
                dispose = null;

            current = typeSymbol.GetPublicProperty("Current");
            moveNext = typeSymbol.GetPublicMethod("MoveNext");
            if (current is object && moveNext is object)
                return true;

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_Generic_IEnumerator_T, out var genericArguments))
            {
                var interfaceType = compilation
                    .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerator_T)
                    .Construct(genericArguments[0]);
                current = interfaceType.GetPublicProperty("Current");
                moveNext = interfaceType.GetPublicMethod("MoveNext");
                return true;
            }

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_IEnumerator, out _))
            {
                var interfaceType = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerator);
                current = interfaceType.GetPublicProperty("Current");
                moveNext = interfaceType.GetPublicMethod("MoveNext");
                return true;
            }

            return false;
        }

        public static IPropertySymbol GetPublicProperty(this ITypeSymbol typeSymbol, string name)
        {
            if (typeSymbol is null)
                throw new ArgumentNullException(nameof(typeSymbol));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            foreach (var member in typeSymbol.GetMembers(name).OfType<IPropertySymbol>())
            {
                if (!member.IsStatic && member.DeclaredAccessibility == Accessibility.Public)
                    return member;
            }

            if (typeSymbol.TypeKind == TypeKind.Interface)
            {
                foreach (var @interface in typeSymbol.AllInterfaces)
                {
                    var property = @interface.GetPublicProperty(name);
                    if (property is object)
                        return property;
                }
            }
            else
            {
                var baseType = typeSymbol.BaseType;
                if (baseType is object)
                    return baseType.GetPublicProperty(name);
            }

            return null;
        }

        public static IMethodSymbol GetPublicMethod(this ITypeSymbol typeSymbol, string name, params Type[] parameters)
        {
            if (typeSymbol is null)
                throw new ArgumentNullException(nameof(typeSymbol));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

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
                foreach (var @interface in typeSymbol.AllInterfaces)
                {
                    var method = @interface.GetPublicMethod(name, parameters);
                    if (method is object)
                        return method;
                }
            }
            else
            {
                var baseType = typeSymbol.BaseType;
                if (baseType is object)
                    return baseType.GetPublicMethod(name, parameters);
            }

            return null;
        }

        public static bool ImplementsInterface(this ITypeSymbol typeSymbol, SpecialType interfaceType, out ImmutableArray<ITypeSymbol> genericArguments)
        {
            if (typeSymbol is null)
                throw new ArgumentNullException(nameof(typeSymbol));

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


        static bool SequenceEqual(ImmutableArray<IParameterSymbol> parameters, Type[] types)
        {
            if (parameters.Length != types.Length)
                return false;

            for (var index = 0; index < parameters.Length; index++)
            {
                if (parameters[index].Type.Name != types[index].Name)
                    return false;
            }

            return true;
        }
    }
}
