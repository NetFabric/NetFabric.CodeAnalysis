using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NetFabric.CodeAnalysis
{
    public static partial class ITypeSymbolExtensions
    {

        /// <summary>
        /// Gets a value indicating whether 'await foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be enumerable.
        /// </summary>
        /// <param name="typeSymbol">The <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to test.</param>
        /// <param name="compilation">The <see cref="Microsoft.CodeAnalysis.Compilation"/> context.</param>
        /// <param name="enumerableSymbols">If methods returns <c>true</c>, contains information on the methods 'await foreach' will use to enumerate.</param>
        /// <returns><c>true</c> if 'await foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be enumerable; otherwise, <c>false</c>.</returns>
        public static bool IsAsyncEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out AsyncEnumerableSymbols? enumerableSymbols)
            => IsAsyncEnumerable(typeSymbol, compilation, out enumerableSymbols, out _);

        /// <summary>
        /// Gets a value indicating whether 'await foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be enumerable.
        /// </summary>
        /// <param name="typeSymbol">The <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to test.</param>
        /// <param name="compilation">The <see cref="Microsoft.CodeAnalysis.Compilation"/> context.</param>
        /// <param name="enumerableSymbols">If methods returns <c>true</c>, contains information on the methods 'await foreach' will use to enumerate.</param>
        /// <param name="errors">Gets information on what error caused the method to return <c>false</c>.</param>
        /// <returns><c>true</c> if 'await foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be enumerable; otherwise, <c>false</c>.</returns>
        public static bool IsAsyncEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out AsyncEnumerableSymbols? enumerableSymbols,
            out Errors errors)
        {
            if (typeSymbol.TypeKind != TypeKind.Interface)
            {
                var getEnumerator =
                    typeSymbol.GetPublicMethod(NameOf.GetAsyncEnumerator, typeof(CancellationToken))
                    ?? typeSymbol.GetPublicMethod(NameOf.GetAsyncEnumerator);

                if (getEnumerator is not null)
                {
                    var enumeratorType = getEnumerator.ReturnType;

                    var current = enumeratorType.GetPublicReadProperty(NameOf.Current);
                    if (current is null)
                    {
                        enumerableSymbols = default;
                        errors = Errors.MissingCurrent;
                        return false;
                    }

                    var moveNext = enumeratorType.GetPublicMethod(NameOf.MoveNextAsync);
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
            }

            var asyncEnumerableType = compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1")!;
            if (typeSymbol.ImplementsInterface(asyncEnumerableType, out var genericArguments))
            {
                var asyncEnumeratorType = compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerator`1")!.Construct(genericArguments[0]);
                var asyncDisposableType = compilation.GetTypeByMetadataName("System.IAsyncDisposable")!;

                enumerableSymbols = new AsyncEnumerableSymbols(
                    asyncEnumerableType.GetPublicMethod(NameOf.GetAsyncEnumerator, typeof(CancellationToken))!,
                    new AsyncEnumeratorSymbols(
                        asyncEnumeratorType.GetPublicReadProperty(NameOf.Current)!,
                        asyncEnumeratorType.GetPublicMethod(NameOf.MoveNextAsync, Type.EmptyTypes)!)
                    {
                        DisposeAsync = asyncDisposableType.GetPublicMethod(NameOf.DisposeAsync, Type.EmptyTypes),
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
    }
}
