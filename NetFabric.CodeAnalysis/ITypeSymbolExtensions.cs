using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Threading;

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

            var isEnumerator = getEnumerator.ReturnType.IsEnumeratorType(compilation, out var current, out var moveNext, out var reset, out var dispose);
            enumerableSymbols = new EnumerableSymbols(getEnumerator, new EnumeratorSymbols(current, moveNext, reset, dispose));
            return isEnumerator;
        }

        public static bool IsAsyncEnumerable(this ITypeSymbol typeSymbol, Compilation compilation, out AsyncEnumerableSymbols enumerableSymbols)
        {
            if (typeSymbol is null)
                throw new ArgumentNullException(nameof(typeSymbol));
            if (compilation is null)
                throw new ArgumentNullException(nameof(compilation));

            if (!typeSymbol.IsAsyncEnumerableType(compilation, out var getEnumerator))
            {
                enumerableSymbols = default;
                return false;
            }

            var isAsyncEnumerator = getEnumerator.ReturnType.IsAsyncEnumeratorType(compilation, out var current, out var moveNext, out var dispose);
            enumerableSymbols = new AsyncEnumerableSymbols(getEnumerator, new AsyncEnumeratorSymbols(current, moveNext, dispose));
            return isAsyncEnumerator;
        }

        public static bool IsEnumerator(this ITypeSymbol typeSymbol, Compilation compilation, out EnumeratorSymbols enumeratorSymbols)
        {
            if (typeSymbol is null)
                throw new ArgumentNullException(nameof(typeSymbol));
            if (compilation is null)
                throw new ArgumentNullException(nameof(compilation));

            var isEnumerator = typeSymbol.IsEnumeratorType(compilation, out var current, out var moveNext, out var reset, out var dispose);
            enumeratorSymbols = new EnumeratorSymbols(current, moveNext, reset, dispose);
            return isEnumerator;
        }

        public static bool IsAsyncEnumerator(this ITypeSymbol typeSymbol, Compilation compilation, out AsyncEnumeratorSymbols enumeratorSymbols)
        {
            if (typeSymbol is null)
                throw new ArgumentNullException(nameof(typeSymbol));
            if (compilation is null)
                throw new ArgumentNullException(nameof(compilation));

            var isAsyncEnumerator = typeSymbol.IsAsyncEnumeratorType(compilation, out var current, out var moveNext, out var dispose);
            enumeratorSymbols = new AsyncEnumeratorSymbols(current, moveNext, dispose);
            return isAsyncEnumerator;
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

        static bool IsAsyncEnumerableType(this ITypeSymbol typeSymbol, Compilation compilation, out IMethodSymbol getAsyncEnumerator)
        {
            getAsyncEnumerator = typeSymbol.GetPublicMethod("GetAsyncEnumerator", typeof(CancellationToken));
            if (getAsyncEnumerator is object)
                return true;

            getAsyncEnumerator = typeSymbol.GetPublicMethod("GetAsyncEnumerator");
            if (getAsyncEnumerator is object)
                return true;

            var asyncEnumerableType = compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1");
            if (typeSymbol.ImplementsInterface(asyncEnumerableType, out var genericArguments))
            {
                getAsyncEnumerator = asyncEnumerableType
                    .Construct(genericArguments[0])
                    .GetPublicMethod("GetAsyncEnumerator", typeof(CancellationToken));
                return true;
            }

            return false;
        }

        static bool IsEnumeratorType(this ITypeSymbol typeSymbol, Compilation compilation, out IPropertySymbol current, out IMethodSymbol moveNext, out IMethodSymbol reset, out IMethodSymbol dispose)
        {
            if (typeSymbol.ImplementsInterface(SpecialType.System_IDisposable, out _))
                dispose = compilation.GetSpecialType(SpecialType.System_IDisposable).GetPublicMethod("Dispose");
            else
                dispose = null;

            current = typeSymbol.GetPublicProperty("Current");
            moveNext = typeSymbol.GetPublicMethod("MoveNext");
            reset = typeSymbol.GetPublicMethod("Reset");
            if (current is object && moveNext is object)
                return true;

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_Generic_IEnumerator_T, out var genericArguments))
            {
                var enumeratorOfT = compilation
                    .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerator_T)
                    .Construct(genericArguments[0]);
                var enumerator = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerator);

                current = enumeratorOfT.GetPublicProperty("Current");
                moveNext = enumerator.GetPublicMethod("MoveNext");
                reset = enumerator.GetPublicMethod("Reset");

                return true;
            }

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_IEnumerator, out _))
            {
                var enumerator = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerator);

                current = enumerator.GetPublicProperty("Current");
                moveNext = enumerator.GetPublicMethod("MoveNext");
                reset = enumerator.GetPublicMethod("Reset");

                return true;
            }

            return false;
        }

        static bool IsAsyncEnumeratorType(this ITypeSymbol typeSymbol, Compilation compilation, out IPropertySymbol current, out IMethodSymbol moveNextAsync, out IMethodSymbol disposeAsync)
        {
            var asyncDisposableType = compilation.GetTypeByMetadataName("System.IAsyncDisposable");
            if (typeSymbol.ImplementsInterface(asyncDisposableType, out _))
                disposeAsync = asyncDisposableType.GetPublicMethod("DisposeAsync");
            else
                disposeAsync = null;

            current = typeSymbol.GetPublicProperty("Current");
            moveNextAsync = typeSymbol.GetPublicMethod("MoveNextAsync");
            if (current is object && moveNextAsync is object)
                return true;

            var asyncEnumeratorType = compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerator`1");
            if (typeSymbol.ImplementsInterface(asyncEnumeratorType, out var genericArguments))
            {
                var interfaceType = asyncEnumeratorType
                    .Construct(genericArguments[0]);
                current = interfaceType.GetPublicProperty("Current");
                moveNextAsync = interfaceType.GetPublicMethod("MoveNextAsync");
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

        public static bool ImplementsInterface(this ITypeSymbol typeSymbol, INamedTypeSymbol interfaceType, out ImmutableArray<ITypeSymbol> genericArguments)
        {
            if (typeSymbol is null)
                throw new ArgumentNullException(nameof(typeSymbol));
            if (interfaceType is null)
                throw new ArgumentNullException(nameof(interfaceType));

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


        static bool SequenceEqual(ImmutableArray<IParameterSymbol> parameters, Type[] types)
        {
            if (parameters.Length != types.Length)
                return false;

            for (var index = 0; index < parameters.Length; index++)
            {
                if (parameters[index].Type.MetadataName != types[index].Name)
                    return false;
            }

            return true;
        }
    }
}
