using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public struct AsyncEnumerableSymbols
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