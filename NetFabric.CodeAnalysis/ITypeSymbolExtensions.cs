using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NetFabric.CodeAnalysis
{
    public static class ITypeSymbolExtensions
    {
        public static bool IsEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out EnumerableSymbols? enumerableSymbols)
            => IsEnumerable(typeSymbol, compilation, out enumerableSymbols, out var _);

        public static bool IsEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out EnumerableSymbols? enumerableSymbols,
            out Errors errors)
        {
            if (!typeSymbol.IsEnumerableType(compilation, out var getEnumerator))
            {
                enumerableSymbols = default;
                errors = Errors.MissingGetEnumerable;
                return false;
            }

            if (!getEnumerator.ReturnType.IsEnumerator(compilation, out var enumeratorSymbols, out errors))
            {
                enumerableSymbols = default;
                return false;
            }

            enumerableSymbols = new EnumerableSymbols(getEnumerator, enumeratorSymbols);
            errors = Errors.None;
            return true;
        }

        public static bool IsAsyncEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out AsyncEnumerableSymbols? enumerableSymbols)
            => IsAsyncEnumerable(typeSymbol, compilation, out enumerableSymbols, out var _);

        public static bool IsAsyncEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out AsyncEnumerableSymbols? enumerableSymbols,
            out Errors errors)
        {
            if (!typeSymbol.IsAsyncEnumerableType(compilation, out var getEnumerator))
            {
                enumerableSymbols = default;
                errors = Errors.MissingGetEnumerable;
                return false;
            }

            if (!getEnumerator.ReturnType.IsAsyncEnumerator(compilation, out var enumeratorSymbols, out errors))
            {
                enumerableSymbols = default;
                return false;
            }

            enumerableSymbols = new AsyncEnumerableSymbols(getEnumerator, enumeratorSymbols);
            return true;
        }

        public static bool IsEnumerator(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out EnumeratorSymbols? enumeratorSymbols)
            => IsEnumerator(typeSymbol, compilation, out enumeratorSymbols, out var _);

        public static bool IsEnumerator(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out EnumeratorSymbols? enumeratorSymbols,
            out Errors errors)
        {
            if (typeSymbol.IsEnumeratorType(compilation, out var current, out var moveNext, out var reset, out var dispose))
            {
                enumeratorSymbols = new EnumeratorSymbols(current, moveNext, reset, dispose);
                errors = Errors.None;
                return true;
            }

            enumeratorSymbols = default;
            errors = Errors.None;
            if (current is null) errors |= Errors.MissingCurrent;
            if (moveNext is null) errors |= Errors.MissingMoveNext;
            return false;
        }

        public static bool IsAsyncEnumerator(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out AsyncEnumeratorSymbols? enumeratorSymbols)
            => IsAsyncEnumerator(typeSymbol, compilation, out enumeratorSymbols, out var _);

        public static bool IsAsyncEnumerator(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out AsyncEnumeratorSymbols? enumeratorSymbols,
            out Errors errors)
        {
            if (typeSymbol.IsAsyncEnumeratorType(compilation, out var current, out var moveNext, out var dispose))
            {
                enumeratorSymbols = new AsyncEnumeratorSymbols(current, moveNext, dispose);
                errors = Errors.None;
                return true;
            }

            enumeratorSymbols = default;
            errors = Errors.None;
            if (current is null) errors |= Errors.MissingCurrent;
            if (moveNext is null) errors |= Errors.MissingMoveNext;
            return false;
        }

        static bool IsEnumerableType(this ITypeSymbol typeSymbol, Compilation compilation, 
            [NotNullWhen(true)] out IMethodSymbol? getEnumerator)
        {
            getEnumerator = typeSymbol.GetPublicMethod("GetEnumerator");
            if (getEnumerator is not null)
                return true;

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_Generic_IEnumerable_T, out var genericArguments))
            {
                var interfaceType = compilation
                    .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T)
                    .Construct(genericArguments[0]);
                getEnumerator = interfaceType.GetPublicMethod("GetEnumerator");
                return getEnumerator is not null;
            }

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_IEnumerable, out _))
            {
                var interfaceType = compilation
                    .GetSpecialType(SpecialType.System_Collections_IEnumerable);
                getEnumerator = interfaceType.GetPublicMethod("GetEnumerator");
                return getEnumerator is not null;
            }

            return false;
        }

        static bool IsAsyncEnumerableType(this ITypeSymbol typeSymbol, Compilation compilation, 
            [NotNullWhen(true)] out IMethodSymbol? getAsyncEnumerator)
        {
            getAsyncEnumerator = typeSymbol.GetPublicMethod("GetAsyncEnumerator", typeof(CancellationToken));
            if (getAsyncEnumerator is not null)
                return true;

            getAsyncEnumerator = typeSymbol.GetPublicMethod("GetAsyncEnumerator");
            if (getAsyncEnumerator is not null)
                return true;

            var asyncEnumerableType = compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1");
            if (typeSymbol.ImplementsInterface(asyncEnumerableType, out var genericArguments))
            {
                getAsyncEnumerator = asyncEnumerableType
                    .Construct(genericArguments[0])
                    .GetPublicMethod("GetAsyncEnumerator", typeof(CancellationToken));
                return getAsyncEnumerator is not null;
            }

            return false;
        }

        static bool IsEnumeratorType(this ITypeSymbol typeSymbol, Compilation compilation, 
            [NotNullWhen(true)] out IPropertySymbol? current, 
            [NotNullWhen(true)] out IMethodSymbol? moveNext, 
            out IMethodSymbol? reset, 
            out IMethodSymbol? dispose)
        {
            if (typeSymbol.ImplementsInterface(SpecialType.System_IDisposable, out _))
                dispose = compilation.GetSpecialType(SpecialType.System_IDisposable).GetPublicMethod(nameof(IDisposable.Dispose));
            else
                dispose = default;

            current = typeSymbol.GetPublicProperty(nameof(IEnumerator.Current));
            moveNext = typeSymbol.GetPublicMethod(nameof(IEnumerator.MoveNext));
            reset = typeSymbol.GetPublicMethod(nameof(IEnumerator.Reset));
            if (current is not null && moveNext is not null)
                return true;

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_Generic_IEnumerator_T, out var genericArguments))
            {
                var enumeratorOfT = compilation
                    .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerator_T)
                    .Construct(genericArguments[0]);
                var enumerator = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerator);

                current = enumeratorOfT.GetPublicProperty(nameof(IEnumerator.Current));
                moveNext = enumerator.GetPublicMethod(nameof(IEnumerator.MoveNext));
                reset = enumerator.GetPublicMethod(nameof(IEnumerator.Reset));

                return current is not null && moveNext is not null;
            }

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_IEnumerator, out _))
            {
                var enumerator = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerator);

                current = enumerator.GetPublicProperty(nameof(IEnumerator.Current));
                moveNext = enumerator.GetPublicMethod(nameof(IEnumerator.MoveNext));
                reset = enumerator.GetPublicMethod(nameof(IEnumerator.Reset));

                return current is not null && moveNext is not null;
            }

            return false;
        }

        static bool IsAsyncEnumeratorType(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out IPropertySymbol? current,
            [NotNullWhen(true)] out IMethodSymbol? moveNextAsync, 
            out IMethodSymbol? disposeAsync)
        {
            var asyncDisposableType = compilation.GetTypeByMetadataName("System.IAsyncDisposable");
            if (typeSymbol.ImplementsInterface(asyncDisposableType, out _))
                disposeAsync = asyncDisposableType.GetPublicMethod("DisposeAsync");
            else
                disposeAsync = default;

            current = typeSymbol.GetPublicProperty("Current");
            moveNextAsync = typeSymbol.GetPublicMethod("MoveNextAsync");
            if (current is not null && moveNextAsync is not null)
                return true;

            var asyncEnumeratorType = compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerator`1");
            if (typeSymbol.ImplementsInterface(asyncEnumeratorType, out var genericArguments))
            {
                var interfaceType = asyncEnumeratorType
                    .Construct(genericArguments[0]);
                current = interfaceType.GetPublicProperty("Current");
                moveNextAsync = interfaceType.GetPublicMethod("MoveNextAsync");
                return current is not null && moveNextAsync is not null;
            }

            return false;
        }

        public static IPropertySymbol? GetPublicProperty(this ITypeSymbol typeSymbol, string name)
        {
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
                    if (property is not null)
                        return property;
                }
            }
            else
            {
                var baseType = typeSymbol.BaseType;
                if (baseType is not null)
                    return baseType.GetPublicProperty(name);
            }

            return null;
        }

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

        public static bool ImplementsInterface(this ITypeSymbol typeSymbol, SpecialType interfaceType, out ImmutableArray<ITypeSymbol> genericArguments)
        {
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
