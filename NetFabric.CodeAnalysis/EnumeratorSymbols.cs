using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public class EnumeratorSymbols
    {
        public readonly IPropertySymbol Current;
        public readonly IMethodSymbol MoveNext;
        public readonly IMethodSymbol? Reset;
        public readonly IMethodSymbol? Dispose;

        public EnumeratorSymbols(IPropertySymbol current, IMethodSymbol moveNext, IMethodSymbol? reset, IMethodSymbol? dispose)
        {
            Current = current;
            MoveNext = moveNext;
            Reset = reset;
            Dispose = dispose;
        }
    }
}