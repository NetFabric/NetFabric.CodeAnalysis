using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public struct EnumeratorSymbols
    {
        public readonly IPropertySymbol Current;
        public readonly IMethodSymbol MoveNext;
        public readonly IMethodSymbol Dispose;

        public EnumeratorSymbols(IPropertySymbol current, IMethodSymbol moveNext, IMethodSymbol dispose)
        {
            Current = current;
            MoveNext = moveNext;
            Dispose = dispose;
        }

        public ITypeSymbol EnumeratorType
            => Current?.ContainingSymbol as ITypeSymbol;

        public ITypeSymbol ItemType
            => Current?.Type;
    }
}