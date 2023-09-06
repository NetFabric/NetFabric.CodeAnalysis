using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    /// <summary>
    /// Contains information about methods and properties that ´foreach´ will use to enumerate a given type.
    /// </summary>
    public class EnumerableSymbols
    {
        /// <summary>
        /// Indicates if 'foreach' will use the indexer instead of the enumerator.
        /// </summary>
        public bool ForEachUsesIndexer { get; }

        /// <summary>
        /// Information on the method that 'foreach' will use to get a new instance of the enumerator.
        /// </summary>
        public IMethodSymbol GetEnumerator { get; }
        
        /// <summary>
        /// Information on the enumerator methods that 'foreach' will use.
        /// </summary>
        public EnumeratorSymbols EnumeratorSymbols { get; }

        internal EnumerableSymbols(bool forEachUsesIndexer, IMethodSymbol getEnumerator, EnumeratorSymbols enumeratorSymbols)
        {
            ForEachUsesIndexer = forEachUsesIndexer;
            GetEnumerator = getEnumerator;
            EnumeratorSymbols = enumeratorSymbols;
        }
    }
}