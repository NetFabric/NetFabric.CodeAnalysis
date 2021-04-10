using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    /// <summary>
    /// Contains information about methods and properties of an enumerator that ´foreach´ will use to enumerate a given type.
    /// </summary>
    public class EnumeratorSymbols
    {
        /// <summary>
        /// Information on the property that 'foreach' will use to get the value of current item.
        /// </summary>
        public IPropertySymbol Current { get; }
        
        /// <summary>
        /// Information on the method that 'foreach' will use to iterate to the next item.
        /// </summary>
        public IMethodSymbol MoveNext { get; }
        
        /// <summary>
        /// Information on the 'Reset' method, if defined. 
        /// </summary>
        public IMethodSymbol? Reset { get; init; }
        
        /// <summary>
        /// Information on the method that 'foreach' will use to dispose the enumerator, if it's disposable.
        /// </summary>
        /// <remarks>
        /// To be considered disposable, the enumerator has to implement <see cref="System.IDisposable"/>,
        /// except if it's a by reference like value type (ref struct) which only needs to have a public
        /// method named 'Dispose'.
        /// </remarks>
        public IMethodSymbol? Dispose { get; init; }
        
        /// <summary>
        /// Indicates if 'foreach' considers the enumerator to be a value type.
        /// </summary>
        public bool IsValueType { get; init; }
        
        /// <summary>
        /// Indicates if 'foreach' considers the enumerator to be a by reference like value type (ref struct).
        /// </summary>
        public bool IsRefLikeType { get; init; }
        
        /// <summary>
        /// Indicates if 'foreach' considers the enumerator to be of type <see cref="System.Collections.Generic.IEnumerator&lt;&gt;"/>.
        /// </summary>
        public bool IsGenericsEnumeratorInterface { get; init; }    
        
        /// <summary>
        /// Indicates if 'foreach' considers the enumerator to be of type <see cref="System.Collections.IEnumerator"/>.
        /// </summary>
        public bool IsEnumeratorInterface { get; init; }    

        internal EnumeratorSymbols(IPropertySymbol current, IMethodSymbol moveNext)
        {
            Current = current;
            MoveNext = moveNext;
        }
    }
}