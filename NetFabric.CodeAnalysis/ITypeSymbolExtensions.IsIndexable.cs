using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetFabric.CodeAnalysis;

public static partial class ITypeSymbolExtensions
{
    /// <summary>
    /// Checks if the provided <paramref name="typeSymbol"/> is indexable.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> to be checked for indexability.</param>
    /// <returns>
    /// <c>true</c> if the <paramref name="typeSymbol"/> is indexable; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method examines the provided <paramref name="typeSymbol"/> to determine if it can be
    /// indexed. To be considered indexable, the type must have an indexer with a single dimension
    /// of type <see cref="int"/> and a readable property named <c>Count</c> or <c>Length</c> of
    /// type <see cref="int"/>.
    /// </remarks>
    public static bool IsIndexable(this ITypeSymbol typeSymbol)
        => IsIndexable(typeSymbol, out _, out _);

    /// <summary>
    /// Checks if the provided <paramref name="typeSymbol"/> is indexable, and if so, retrieves
    /// information about the indexable symbols.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> to be checked for indexability.</param>
    /// <param name="indexableSymbols">
    /// When the method returns <c>true</c>, this parameter will contain information about the
    /// indexable symbols associated with the <paramref name="typeSymbol"/>. If the method returns
    /// <c>false</c>, this parameter will be <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <paramref name="typeSymbol"/> is indexable; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method examines the provided <paramref name="typeSymbol"/> to determine if it can be
    /// indexed. To be considered indexable, the type must have an indexer with a single dimension
    /// of type <see cref="int"/> and a readable property named <c>Count</c> or <c>Length</c> of
    /// type <see cref="int"/>.
    /// 
    /// If the type is indexable, the method provides information about the indexable symbols
    /// associated with it, which can be useful for various code analysis tasks.
    /// </remarks>
    public static bool IsIndexable(this ITypeSymbol typeSymbol,
        [NotNullWhen(true)] out IndexableSymbols? indexableSymbols)
        => IsIndexable(typeSymbol, out indexableSymbols, out _);

    /// <summary>
    /// Checks if the provided <paramref name="typeSymbol"/> is indexable, and if so, retrieves
    /// information about the indexable symbols and any potential error.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> to be checked for indexability.</param>
    /// <param name="indexableSymbols">
    /// When the method returns <c>true</c>, this parameter will contain information about the
    /// indexable symbols associated with the <paramref name="typeSymbol"/>. If the method returns
    /// <c>false</c>, this parameter will be <c>null</c>.
    /// </param>
    /// <param name="error">
    /// When the method returns <c>false</c>, this parameter will contain information about any
    /// error encountered while determining indexability. If the method returns <c>true</c>, this
    /// parameter will be <see cref="IsIndexableError.None"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <paramref name="typeSymbol"/> is indexable; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method examines the provided <paramref name="typeSymbol"/> to determine if it can be
    /// indexed. To be considered indexable, the type must have an indexer with a single dimension
    /// of type <see cref="int"/> and a readable property named <c>Count</c> or <c>Length</c> of
    /// type <see cref="int"/>.
    /// 
    /// If the type is indexable, the method provides information about the indexable symbols
    /// associated with it, which can be useful for various code analysis tasks.
    /// </remarks>
    public static bool IsIndexable(this ITypeSymbol typeSymbol,
        [NotNullWhen(true)] out IndexableSymbols? indexableSymbols,
        out IsIndexableError error)
    {
        var countOrLength = typeSymbol.GetPublicReadProperty(NameOf.Count);
        countOrLength ??= typeSymbol.GetPublicReadProperty(NameOf.Length);
        if (countOrLength is null)
        {
            indexableSymbols = default;
            error = IsIndexableError.MissingCountOrLength;
            return false;
        }

        var indexer = default(IPropertySymbol);
        if (typeSymbol.TypeKind != TypeKind.Array)
        {
            indexer = typeSymbol.GetPublicReadIndexer(countOrLength.GetMethod.ReturnType);
            if (indexer is null)
            {
                indexableSymbols = default;
                error = IsIndexableError.MissingIndexer;
                return false;
            }
        }

        indexableSymbols = new IndexableSymbols(indexer, countOrLength);
        error = IsIndexableError.None;
        return true;
    }
}
