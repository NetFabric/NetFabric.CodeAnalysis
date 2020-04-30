using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public class EnumerableSymbols
    {
        public readonly IMethodSymbol GetEnumerator;
        public readonly EnumeratorSymbols EnumeratorSymbols;

        public EnumerableSymbols(IMethodSymbol getEnumerator, EnumeratorSymbols enumeratorSymbols)
        {
            GetEnumerator = getEnumerator;
            EnumeratorSymbols = enumeratorSymbols;
        }
    }
}