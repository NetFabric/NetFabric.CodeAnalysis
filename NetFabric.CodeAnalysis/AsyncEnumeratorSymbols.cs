using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis;

/// <summary>
/// Contains information about methods and properties of an enumerator that ´await foreach´ will use to enumerate a given type.
/// </summary>
public class AsyncEnumeratorSymbols
{
    /// <summary>
    /// Information on the property that 'await foreach' will use to get the value of current item.
    /// </summary>
    public IPropertySymbol Current { get; }
    
    /// <summary>
    /// Information on the method that 'await foreach' will use to iterate to the next item.
    /// </summary>
    public IMethodSymbol MoveNextAsync { get; }
    
    /// <summary>
    /// Information on the method that 'await foreach' will use to dispose the enumerator, if it's disposable.
    /// </summary>
    /// <remarks>
    /// To be considered disposable, the enumerator has to implement <see cref="System.IAsyncDisposable"/>,
    /// except if it's a by reference like value type (ref struct) which only needs to have a public
    /// method named 'DisposeAsync'.
    /// </remarks>
    public IMethodSymbol? DisposeAsync { get; init; }
    
    /// <summary>
    /// Indicates if 'await foreach' considers the enumerator to be a value type.
    /// </summary>
    public bool IsValueType { get; init; }    
    
    /// <summary>
    /// Indicates if 'await foreach' considers the enumerator to be of type <see cref="System.Collections.Generic.IAsyncEnumerator&lt;&gt;"/>.
    /// </summary>
    public bool IsAsyncEnumeratorInterface { get; init; }    

    internal AsyncEnumeratorSymbols(IPropertySymbol current, IMethodSymbol moveNextAsync)
    {
        Current = current;
        MoveNextAsync = moveNextAsync;
    }
}