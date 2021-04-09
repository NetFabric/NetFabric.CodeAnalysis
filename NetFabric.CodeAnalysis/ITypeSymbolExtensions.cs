using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NetFabric.CodeAnalysis
{
    public static class ITypeSymbolExtensions
    {
        public static bool IsEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out EnumerableSymbols? enumerableSymbols)
            => IsEnumerable(typeSymbol, compilation, out enumerableSymbols, out _);

        public static bool IsEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out EnumerableSymbols? enumerableSymbols,
            out Errors errors)
        {
            var getEnumerator = typeSymbol.GetPublicMethod("GetEnumerator");
            if (getEnumerator is not null)
            {
                var enumeratorType = getEnumerator.ReturnType;
                
                var current = enumeratorType.GetPublicReadProperty(nameof(IEnumerator.Current));
                if (current is null)
                {
                    enumerableSymbols = default;
                    errors = Errors.MissingCurrent;
                    return false;
                }
                
                var moveNext = enumeratorType.GetPublicMethod(nameof(IEnumerator.MoveNext));
                if (moveNext is null)
                {
                    enumerableSymbols = default;
                    errors = Errors.MissingMoveNext;
                    return false;
                }

                var reset = enumeratorType.GetPublicMethod(nameof(IEnumerator.Reset));
                _ = enumeratorType.IsDisposable(compilation, out var dispose, out var isRefLike);
                
                enumerableSymbols = new EnumerableSymbols(
                    getEnumerator,
                    new EnumeratorSymbols(current, moveNext)
                    {
                        Reset = reset,
                        Dispose = dispose,
                        IsValueType = getEnumerator.ReturnType.IsValueType,
                        IsRefLikeType = isRefLike,
                        IsGenericsEnumeratorInterface = 
                            enumeratorType.TypeKind == TypeKind.Interface 
                            && enumeratorType.ImplementsInterface(SpecialType.System_Collections_Generic_IEnumerable_T, out _),
                        IsEnumeratorInterface = 
                            enumeratorType.TypeKind == TypeKind.Interface,
                    }
                );
                errors = Errors.None; 
                return true;
            }

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_Generic_IEnumerable_T, out var genericArguments))
            {
                var genericEnumerableType = compilation
                    .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T)
                    .Construct(genericArguments[0]);
                var genericEnumeratorType = compilation
                    .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerator_T)
                    .Construct(genericArguments[0]);
                var enumeratorType = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerator);
                var disposableType = compilation.GetSpecialType(SpecialType.System_IDisposable);

                enumerableSymbols = new EnumerableSymbols(
                    genericEnumerableType.GetPublicMethod(nameof(IEnumerable<int>.GetEnumerator), Type.EmptyTypes)!,
                    new EnumeratorSymbols(
                        genericEnumeratorType.GetPublicReadProperty(nameof(IEnumerator<int>.Current))!,
                        enumeratorType.GetPublicMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes)!)
                        {
                            Reset = enumeratorType.GetPublicMethod(nameof(IEnumerator.Reset), Type.EmptyTypes),
                            Dispose = disposableType.GetPublicMethod(nameof(IDisposable.Dispose), Type.EmptyTypes),
                            IsGenericsEnumeratorInterface = true,
                            IsEnumeratorInterface = true,
                        }
                );
                errors = Errors.None; 
                return true;
            }

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_IEnumerable, out _))
            {
                var enumerableType = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerable);
                var enumeratorType = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerator);

                enumerableSymbols = new EnumerableSymbols(
                    enumerableType.GetPublicMethod(nameof(IEnumerable.GetEnumerator), Type.EmptyTypes)!,
                    new EnumeratorSymbols(
                        enumeratorType.GetPublicReadProperty(nameof(IEnumerator.Current))!,
                        enumeratorType.GetPublicMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes)!)
                    {
                        Reset = enumeratorType.GetPublicMethod(nameof(IEnumerator.Reset), Type.EmptyTypes),
                        IsEnumeratorInterface = true,
                    }                   
                );
                errors = Errors.None; 
                return true;
            }

            enumerableSymbols = default;
            errors = Errors.MissingGetEnumerator;
            return false;
        }

        public static bool IsAsyncEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out AsyncEnumerableSymbols? enumerableSymbols)
            => IsAsyncEnumerable(typeSymbol, compilation, out enumerableSymbols, out _);

        public static bool IsAsyncEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out AsyncEnumerableSymbols? enumerableSymbols,
            out Errors errors)
        {
            var getEnumerator =
                typeSymbol.GetPublicMethod("GetAsyncEnumerator", typeof(CancellationToken))
                ?? typeSymbol.GetPublicMethod("GetAsyncEnumerator");

            if (getEnumerator is not null)
            {
                var enumeratorType = getEnumerator.ReturnType;
                
                var current = enumeratorType.GetPublicReadProperty("Current");
                if (current is null)
                {
                    enumerableSymbols = default;
                    errors = Errors.MissingCurrent;
                    return false;
                }
                
                var moveNext = enumeratorType.GetPublicMethod("MoveNextAsync");
                if (moveNext is null)
                {
                    enumerableSymbols = default;
                    errors = Errors.MissingMoveNext;
                    return false;
                }

                _ = enumeratorType.IsAsyncDisposable(compilation, out var dispose);
                
                enumerableSymbols = new AsyncEnumerableSymbols(
                    getEnumerator,
                    new AsyncEnumeratorSymbols(current, moveNext)
                    {
                        DisposeAsync = dispose,
                        IsValueType = getEnumerator.ReturnType.IsValueType,
                        IsAsyncEnumeratorInterface = enumeratorType.TypeKind == TypeKind.Interface,
                    }
                );
                errors = Errors.None; 
                return true;
            }

            var asyncEnumerableType = compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1")!;
            if (typeSymbol.ImplementsInterface(asyncEnumerableType, out var genericArguments))
            {
                var asyncEnumeratorType = compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerator`1")!.Construct(genericArguments[0]);
                var asyncDisposableType = compilation.GetTypeByMetadataName("System.IAsyncDisposable")!;

                enumerableSymbols = new AsyncEnumerableSymbols(
                    asyncEnumerableType.GetPublicMethod("GetAsyncEnumerator", typeof(CancellationToken))!,
                    new AsyncEnumeratorSymbols(
                        asyncEnumeratorType.GetPublicReadProperty("Current")!,
                        asyncEnumeratorType.GetPublicMethod("MoveNextAsync", Type.EmptyTypes)!)
                    {
                        DisposeAsync = asyncDisposableType.GetPublicMethod("DisposeAsync", Type.EmptyTypes),
                        IsAsyncEnumeratorInterface = true,
                    }
                );
                errors = Errors.None; 
                return true;
            }

            enumerableSymbols = default;
            errors = Errors.MissingGetEnumerator;
            return false;
        }

        public static bool IsDisposable(this ITypeSymbol typeSymbol, Compilation compilation, 
            [NotNullWhen(true)] out IMethodSymbol? dispose,
            out bool isRefLike)
        {
            isRefLike = typeSymbol.IsRefLikeType;
            if (isRefLike)
                dispose = typeSymbol.GetPublicMethod(nameof(IDisposable.Dispose));
            else if (typeSymbol.ImplementsInterface(SpecialType.System_IDisposable, out _))
                dispose = compilation.GetSpecialType(SpecialType.System_IDisposable).GetPublicMethod(nameof(IDisposable.Dispose));
            else
                dispose = default;

            return dispose is not null;
        }

        public static bool IsAsyncDisposable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out IMethodSymbol? disposeAsync)
        {
            var asyncDisposableType = compilation.GetTypeByMetadataName("System.IAsyncDisposable")!;
            if (typeSymbol.ImplementsInterface(asyncDisposableType, out _))
                disposeAsync = asyncDisposableType.GetPublicMethod("DisposeAsync");
            else
                disposeAsync = default;

            return disposeAsync is not null;
        }

        public static IPropertySymbol? GetPublicReadProperty(this ITypeSymbol typeSymbol, string name)
        {
            foreach (var member in typeSymbol.GetMembers(name).OfType<IPropertySymbol>())
            {
                if (!member.IsStatic && member.DeclaredAccessibility == Accessibility.Public && member.GetMethod is not null)
                    return member;
            }

            if (typeSymbol.TypeKind == TypeKind.Interface)
            {
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
