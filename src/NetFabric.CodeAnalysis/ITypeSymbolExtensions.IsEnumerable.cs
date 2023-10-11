using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NetFabric.CodeAnalysis;

public static partial class ITypeSymbolExtensions
{
    /// <summary>
    /// Checks if the provided <paramref name="typeSymbol"/> can be used as a source in a <c>foreach</c>
    /// statement, indicating whether it is enumerable.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> to be checked for enumerability.</param>
    /// <param name="compilation">The <see cref="Compilation"/> representing the current
    /// compilation context.</param>
    /// <returns>
    /// <c>true</c> if the <paramref name="typeSymbol"/> is enumerable; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method examines the provided <paramref name="typeSymbol"/> to determine if it can be
    /// used as a source in a <c>foreach</c> statement, indicating whether it is enumerable. To be
    /// considered enumerable, the type should support iteration, typically by implementing the
    /// <see cref="System.Collections.IEnumerable"/> or <see cref="System.Collections.Generic.IEnumerable{T}"/>
    /// interface, or by providing a suitable GetEnumerator method.
    /// </remarks>
    public static bool IsEnumerable(this ITypeSymbol typeSymbol, Compilation compilation)
        => IsEnumerable(typeSymbol, compilation, out _, out _);

    /// <summary>
    /// Checks if the provided <paramref name="typeSymbol"/> can be used as a source in a <c>foreach</c>
    /// statement, indicating whether it is enumerable, and if so, retrieves information about
    /// the enumerable symbols.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> to be checked for enumerability.</param>
    /// <param name="compilation">The <see cref="Compilation"/> representing the current
    /// compilation context.</param>
    /// <param name="enumerableSymbols">
    /// When the method returns <c>true</c>, this parameter will contain information about the
    /// enumerable symbols associated with the <paramref name="typeSymbol"/>. If the method returns
    /// <c>false</c>, this parameter will be <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <paramref name="typeSymbol"/> is enumerable; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method examines the provided <paramref name="typeSymbol"/> to determine if it can be
    /// used as a source in a <c>foreach</c> statement, indicating whether it is enumerable. To be
    /// considered enumerable, the type should support iteration, typically by implementing the
    /// <see cref="System.Collections.IEnumerable"/> or <see cref="System.Collections.Generic.IEnumerable{T}"/>
    /// interface, or by providing a suitable GetEnumerator method.
    /// 
    /// If the type is enumerable, the method provides information about the enumerable symbols
    /// associated with it, which can be useful for various code analysis tasks.
    /// </remarks>
    public static bool IsEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
        [NotNullWhen(true)] out EnumerableSymbols? enumerableSymbols)
        => IsEnumerable(typeSymbol, compilation, out enumerableSymbols, out _);

    /// <summary>
    /// Checks if the provided <paramref name="typeSymbol"/> can be used as a source in a <c>foreach</c>
    /// statement, indicating whether it is enumerable, and if so, retrieves information about
    /// the enumerable symbols and any potential error.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> to be checked for enumerability.</param>
    /// <param name="compilation">The <see cref="Compilation"/> representing the current
    /// compilation context.</param>
    /// <param name="enumerableSymbols">
    /// When the method returns <c>true</c>, this parameter will contain information about the
    /// enumerable symbols associated with the <paramref name="typeSymbol"/>. If the method returns
    /// <c>false</c>, this parameter will be <c>null</c>.
    /// </param>
    /// <param name="error">
    /// When the method returns <c>false</c>, this parameter will contain information about any
    /// error encountered while determining enumerability. If the method returns <c>true</c>, this
    /// parameter will be <see cref="IsEnumerableError.None"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <paramref name="typeSymbol"/> is enumerable; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method examines the provided <paramref name="typeSymbol"/> to determine if it can be
    /// used as a source in a <c>foreach</c> statement, indicating whether it is enumerable. To be
    /// considered enumerable, the type should support iteration, typically by implementing the
    /// <see cref="System.Collections.IEnumerable"/> or <see cref="System.Collections.Generic.IEnumerable{T}"/>
    /// interface, or by providing a suitable GetEnumerator method.
    /// 
    /// If the type is enumerable, the method provides information about the enumerable symbols
    /// associated with it, which can be useful for various code analysis tasks.
    /// </remarks>
    public static bool IsEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
        [NotNullWhen(true)] out EnumerableSymbols? enumerableSymbols,
        out IsEnumerableError error)
    {
        var forEachUsesIndexer = typeSymbol.TypeKind == TypeKind.Array || typeSymbol.IsSpanOrReadOnlySpanType();

        if (typeSymbol.TypeKind != TypeKind.Interface)
        {
            var getEnumerator = typeSymbol.GetPublicMethod(NameOf.GetEnumerator);
            if (getEnumerator is not null)
            {
                return HandleGetEnumerator(getEnumerator, compilation, out enumerableSymbols, out error);
            }

            getEnumerator = compilation.GetExtensionMethodsWithName(typeSymbol, NameOf.GetEnumerator)
                .FirstOrDefault(methodSymbol => methodSymbol.Parameters.Length == 1);
            if (getEnumerator is not null)
            {
                return HandleGetEnumerator(getEnumerator, compilation, out enumerableSymbols, out error);
            }
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
                forEachUsesIndexer,
                genericEnumerableType.GetPublicMethod(NameOf.GetEnumerator, Type.EmptyTypes)!,
                new EnumeratorSymbols(
                    genericEnumeratorType.GetPublicReadProperty(NameOf.Current)!,
                    enumeratorType.GetPublicMethod(NameOf.MoveNext, Type.EmptyTypes)!)
                    {
                        Reset = enumeratorType.GetPublicMethod(NameOf.Reset, Type.EmptyTypes),
                        Dispose = disposableType.GetPublicMethod(NameOf.Dispose, Type.EmptyTypes),
                        IsGenericsEnumeratorInterface = true,
                        IsEnumeratorInterface = true,
                    }
            );
            error = IsEnumerableError.None; 
            return true;
        }

        if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_IEnumerable, out _))
        {
            var enumerableType = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerable);
            var enumeratorType = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerator);

            enumerableSymbols = new EnumerableSymbols(
                forEachUsesIndexer,
                enumerableType.GetPublicMethod(NameOf.GetEnumerator, Type.EmptyTypes)!,
                new EnumeratorSymbols(
                    enumeratorType.GetPublicReadProperty(NameOf.Current)!,
                    enumeratorType.GetPublicMethod(NameOf.MoveNext, Type.EmptyTypes)!)
                {
                    Reset = enumeratorType.GetPublicMethod(NameOf.Reset, Type.EmptyTypes),
                    IsEnumeratorInterface = true,
                }                   
            );
            error = IsEnumerableError.None; 
            return true;
        }

        enumerableSymbols = default;
        error = IsEnumerableError.MissingGetEnumerator;
        return false;
    }

    static bool HandleGetEnumerator(IMethodSymbol getEnumerator, 
        Compilation compilation,
        [NotNullWhen(true)] out EnumerableSymbols? enumerableSymbols,
        out IsEnumerableError error)
    {
        var enumeratorType = getEnumerator.ReturnType;

        var current = enumeratorType.GetPublicReadProperty(NameOf.Current);
        if (current is null)
        {
            enumerableSymbols = default;
            error = IsEnumerableError.MissingCurrent;
            return false;
        }

        var moveNext = enumeratorType.GetPublicMethod(NameOf.MoveNext);
        if (moveNext is null || moveNext.ReturnType.ToDisplayString() != "bool")
        {   
            enumerableSymbols = default;
            error = IsEnumerableError.MissingMoveNext;
            return false;
        }

        var reset = enumeratorType.GetPublicMethod("Reset");
        _ = enumeratorType.IsDisposable(compilation, out var dispose);

        enumerableSymbols = new EnumerableSymbols(
            false,
            getEnumerator,
            new EnumeratorSymbols(current, moveNext)
            {
                Reset = reset,
                Dispose = dispose,
                IsValueType = enumeratorType.IsValueType,
                IsRefLikeType = enumeratorType.IsRefLikeType,
                IsGenericsEnumeratorInterface =
                    enumeratorType.TypeKind == TypeKind.Interface
                    && enumeratorType.ImplementsInterface(
                        SpecialType.System_Collections_Generic_IEnumerable_T, out _),
                IsEnumeratorInterface =
                    enumeratorType.TypeKind == TypeKind.Interface,
            }
        );
        error = IsEnumerableError.None;
        return true;
    }
}
