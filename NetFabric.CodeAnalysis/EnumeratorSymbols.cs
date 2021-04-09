using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public class EnumeratorSymbols
    {
        public IPropertySymbol Current { get; }
        public IMethodSymbol MoveNext { get; }
        public IMethodSymbol? Reset { get; init; }
        public IMethodSymbol? Dispose { get; init; }
        public bool IsValueType { get; init; }
        public bool IsRefLikeType { get; init; }
        public bool IsGenericsEnumeratorInterface { get; init; }    
        public bool IsEnumeratorInterface { get; init; }    

        public EnumeratorSymbols(IPropertySymbol current, IMethodSymbol moveNext)
        {
            Current = current;
            MoveNext = moveNext;
        }
    }
}