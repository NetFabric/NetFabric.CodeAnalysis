using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis;

/// <summary>
/// Contains information about methods and properties that will be used to enumerate a given type using the indexer.
/// </summary>
public class IndexableSymbols
{
    /// <summary>
    /// Information on the indexer with a single parameter of type <see cref="int"/>.
    /// </summary>
    /// <remarks><c>null</c> if indexable is an array.</remarks>
    public IPropertySymbol? Indexer { get; }

    /// <summary>
    /// Information on the property that returns the number of items.
    /// </summary>
    public IPropertySymbol CountOrLength { get; }

    internal IndexableSymbols(IPropertySymbol indexer, IPropertySymbol countOrLength)
    {
        Indexer = indexer;
        CountOrLength = countOrLength;
    }
}