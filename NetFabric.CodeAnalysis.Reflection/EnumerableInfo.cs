using System;
using System.Reflection;

namespace NetFabric.CodeAnalysis
{
    public struct EnumerableInfo
    {
        public readonly MethodInfo GetEnumerator;
        public readonly PropertyInfo Current;
        public readonly MethodInfo MoveNext;
        public readonly MethodInfo Dispose;

        internal EnumerableInfo(MethodInfo getEnumerator, PropertyInfo current, MethodInfo moveNext, MethodInfo dispose)
        {
            GetEnumerator = getEnumerator;
            Current = current;
            MoveNext = moveNext;
            Dispose = dispose;
        }

        public Type ItemType
            => Current?.PropertyType;
    }
}