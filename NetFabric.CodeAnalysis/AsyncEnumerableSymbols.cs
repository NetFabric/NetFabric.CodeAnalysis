using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public class AsyncEnumerableSymbols
    {
        public IMethodSymbol GetAsyncEnumerator { get; }
        public AsyncEnumeratorSymbols EnumeratorSymbols { get; }

        public AsyncEnumerableSymbols(IMethodSymbol getAsyncEnumerator, AsyncEnumeratorSymbols enumeratorSymbols)
        {
            GetAsyncEnumerator = getAsyncEnumerator;
            EnumeratorSymbols = enumeratorSymbols;
        }
    }
}