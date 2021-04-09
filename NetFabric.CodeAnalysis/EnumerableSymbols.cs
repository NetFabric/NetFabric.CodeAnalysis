using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public class EnumerableSymbols
    {
        public IMethodSymbol GetEnumerator { get; }
        public EnumeratorSymbols EnumeratorSymbols { get; }

        public EnumerableSymbols(IMethodSymbol getEnumerator, EnumeratorSymbols enumeratorSymbols)
        {
            GetEnumerator = getEnumerator;
            EnumeratorSymbols = enumeratorSymbols;
        }
    }
}