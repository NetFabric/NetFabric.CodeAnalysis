using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis;

/// <summary>
/// Contains information about methods and properties that ´await foreach´ will use to enumerate a given type.
/// </summary>
public class AsyncEnumerableSymbols
{
    /// <summary>
    /// Information on the method that 'await foreach' will use to get a new instance of the enumerator.
    /// </summary>
    public IMethodSymbol GetAsyncEnumerator { get; }

    /// <summary>
    /// Information on the enumerator methods that 'await foreach' will use.
    /// </summary>
    public AsyncEnumeratorSymbols EnumeratorSymbols { get; }

    internal AsyncEnumerableSymbols(IMethodSymbol getAsyncEnumerator, AsyncEnumeratorSymbols enumeratorSymbols)
    {
        GetAsyncEnumerator = getAsyncEnumerator;
        EnumeratorSymbols = enumeratorSymbols;
    }
}