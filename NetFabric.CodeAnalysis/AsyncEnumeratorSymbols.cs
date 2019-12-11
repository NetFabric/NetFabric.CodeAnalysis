using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public struct AsyncEnumeratorSymbols
    {
        public readonly IPropertySymbol Current;
        public readonly IMethodSymbol MoveNextAsync;
        public readonly IMethodSymbol DisposeAsync;

        public AsyncEnumeratorSymbols(IPropertySymbol current, IMethodSymbol moveNextAsync, IMethodSymbol disposeAsync)
        {
            Current = current;
            MoveNextAsync = moveNextAsync;
            DisposeAsync = disposeAsync;
        }
    }
}