using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public class AsyncEnumeratorSymbols
    {
        public IPropertySymbol Current { get; }
        public IMethodSymbol MoveNextAsync { get; }
        public IMethodSymbol? DisposeAsync { get; init; }
        public bool IsValueType { get; init; }    
        public bool IsAsyncEnumeratorInterface { get; init; }    

        public AsyncEnumeratorSymbols(IPropertySymbol current, IMethodSymbol moveNextAsync)
        {
            Current = current;
            MoveNextAsync = moveNextAsync;
        }
    }
}