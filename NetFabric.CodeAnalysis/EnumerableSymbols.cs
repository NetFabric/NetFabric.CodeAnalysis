using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public struct EnumerableSymbols
    {
        public readonly IMethodSymbol GetEnumerator;
        public readonly IPropertySymbol Current;
        public readonly IMethodSymbol MoveNext;
        public readonly IMethodSymbol Dispose;

        public EnumerableSymbols(IMethodSymbol getEnumerator, IPropertySymbol current, IMethodSymbol moveNext, IMethodSymbol dispose)
        {
            GetEnumerator = getEnumerator;
            Current = current;
            MoveNext = moveNext;
            Dispose = dispose;
        }

        public ITypeSymbol EnumerableType
            => GetEnumerator?.ContainingSymbol as ITypeSymbol;

        public ITypeSymbol EnumeratorType
            => GetEnumerator?.ReturnType;

        public ITypeSymbol ItemType
            => Current?.Type;
    }
}