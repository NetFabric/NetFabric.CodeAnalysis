using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace NetFabric.CodeAnalysis;

public static partial class ITypeSymbolExtensions
{
    /// <summary>
    /// Checks if the provided <paramref name="typeSymbol"/> can be used as a source in an <c>await foreach</c>
    /// statement, indicating whether it is an async enumerable.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> to be checked for async enumerability.</param>
    /// <param name="compilation">The <see cref="Compilation"/> representing the current
    /// compilation context.</param>
    /// <returns>
    /// <c>true</c> if the <paramref name="typeSymbol"/> is an async enumerable; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method examines the provided <paramref name="typeSymbol"/> to determine if it can be
    /// used as a source in an <c>await foreach</c> statement, indicating whether it is an async enumerable.
    /// To be considered an async enumerable, the type should support asynchronous iteration,
    /// typically by implementing the <see cref="IAsyncEnumerable{T}"/> interface or providing
    /// a suitable asynchronous <c>GetAsyncEnumerator</c> method.
    /// 
    /// If the type is an async enumerable, it is considered an async source for an <c>await foreach</c> statement.
    /// </remarks>
    public static bool IsAsyncEnumerable(this ITypeSymbol typeSymbol, Compilation compilation)
        => IsAsyncEnumerable(typeSymbol, compilation, out _, out _);

    /// <summary>
    /// Checks if the provided <paramref name="typeSymbol"/> can be used as a source in an <c>await foreach</c>
    /// statement, indicating whether it is an async enumerable, and if so, retrieves information about
    /// the async enumerable symbols.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> to be checked for async enumerability.</param>
    /// <param name="compilation">The <see cref="Compilation"/> representing the current
    /// compilation context.</param>
    /// <param name="enumerableSymbols">
    /// When the method returns <c>true</c>, this parameter will contain information about the
    /// async enumerable symbols associated with the <paramref name="typeSymbol"/>. If the method returns
    /// <c>false</c>, this parameter will be <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <paramref name="typeSymbol"/> is an async enumerable; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method examines the provided <paramref name="typeSymbol"/> to determine if it can be
    /// used as a source in an <c>await foreach</c> statement, indicating whether it is an async enumerable.
    /// To be considered an async enumerable, the type should support asynchronous iteration,
    /// typically by implementing the <see cref="IAsyncEnumerable{T}"/> interface or providing
    /// a suitable asynchronous <c>GetAsyncEnumerator</c> method.
    /// 
    /// If the type is an async enumerable, the method provides information about the async
    /// enumerable symbols associated with it, which can be useful for various code analysis tasks.
    /// </remarks>
    public static bool IsAsyncEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
        [NotNullWhen(true)] out AsyncEnumerableSymbols? enumerableSymbols)
        => IsAsyncEnumerable(typeSymbol, compilation, out enumerableSymbols, out _);

    /// <summary>
    /// Checks if the provided <paramref name="typeSymbol"/> can be used as a source in an <c>await foreach</c>
    /// statement, indicating whether it is an async enumerable, and if so, retrieves information about
    /// the async enumerable symbols.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> to be checked for async enumerability.</param>
    /// <param name="compilation">The <see cref="Compilation"/> representing the current
    /// compilation context.</param>
    /// <param name="enumerableSymbols">
    /// When the method returns <c>true</c>, this parameter will contain information about the
    /// async enumerable symbols associated with the <paramref name="typeSymbol"/>. If the method returns
    /// <c>false</c>, this parameter will be <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <paramref name="typeSymbol"/> is an async enumerable; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method examines the provided <paramref name="typeSymbol"/> to determine if it can be
    /// used as a source in an <c>await foreach</c> statement, indicating whether it is an async enumerable.
    /// To be considered an async enumerable, the type should support asynchronous iteration,
    /// typically by implementing the <see cref="IAsyncEnumerable{T}"/> interface or providing
    /// a suitable asynchronous <c>GetAsyncEnumerator</c> method.
    /// 
    /// If the type is an async enumerable, the method provides information about the async
    /// enumerable symbols associated with it, which can be useful for various code analysis tasks.
    /// </remarks>    
    public static bool IsAsyncEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
    [NotNullWhen(true)] out AsyncEnumerableSymbols? enumerableSymbols,
        out IsAsyncEnumerableError error)
    {
        if (typeSymbol.TypeKind != TypeKind.Interface)
        {
            var getEnumerator =
                typeSymbol.GetPublicMethod(NameOf.GetAsyncEnumerator, typeof(CancellationToken))
                ?? typeSymbol.GetPublicMethod(NameOf.GetAsyncEnumerator);
            if (getEnumerator is not null)
                return HandleGetAsyncEnumerator(getEnumerator, compilation, out enumerableSymbols, out error);

            getEnumerator = compilation.GetExtensionMethodsWithName(typeSymbol, NameOf.GetAsyncEnumerator)
                .FirstOrDefault(methodSymbol => methodSymbol.Parameters.Length == 1 || methodSymbol.Parameters.Length == 2);
            if (getEnumerator is not null)
            {
                return HandleGetAsyncEnumerator(getEnumerator, compilation, out enumerableSymbols, out error);
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
            error = IsAsyncEnumerableError.None; 
            return true;
        }
        
        enumerableSymbols = default;
        error = IsAsyncEnumerableError.MissingGetAsyncEnumerator;
        return false;
    }

    static bool HandleGetAsyncEnumerator(IMethodSymbol getEnumerator, 
        Compilation compilation,
        [NotNullWhen(true)] out AsyncEnumerableSymbols? enumerableSymbols,
        out IsAsyncEnumerableError error)
    {
        var enumeratorType = getEnumerator.ReturnType;

        var current = enumeratorType.GetPublicReadProperty(NameOf.Current);
        if (current is null)
        {
            enumerableSymbols = default;
            error = IsAsyncEnumerableError.MissingCurrent;
            return false;
        }

        var moveNext = enumeratorType.GetPublicMethod(NameOf.MoveNextAsync);
        if (moveNext is null || 
            moveNext.ReturnType is not INamedTypeSymbol namedReturnType || 
            namedReturnType.ToDisplayString() != "System.Threading.Tasks.ValueTask<bool>")
        {
            enumerableSymbols = default;
            error = IsAsyncEnumerableError.MissingMoveNextAsync;
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
        error = IsAsyncEnumerableError.None;
        return true;
    }
}
