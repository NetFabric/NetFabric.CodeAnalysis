using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public class AsyncEnumerableSymbols
    {
        public readonly IMethodSymbol GetAsyncEnumerator;
        public readonly AsyncEnumeratorSymbols EnumeratorSymbols;

        public AsyncEnumerableSymbols(IMethodSymbol getAsyncEnumerator, AsyncEnumeratorSymbols enumeratorSymbols)
        {
            GetAsyncEnumerator = getAsyncEnumerator;
            EnumeratorSymbols = enumeratorSymbols;
        }
    }
}