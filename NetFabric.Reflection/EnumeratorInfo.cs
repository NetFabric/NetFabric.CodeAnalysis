using System;
using System.Reflection;

namespace NetFabric.Reflection
{
    public struct EnumeratorInfo
    {
        public readonly PropertyInfo Current;
        public readonly MethodInfo MoveNext;
        public readonly MethodInfo Dispose;

        public EnumeratorInfo(PropertyInfo current, MethodInfo moveNext, MethodInfo dispose)
        {
            Current = current;
            MoveNext = moveNext;
            Dispose = dispose;
        }

        public Type EnumeratorType
            => Current?.DeclaringType;

        public Type ItemType
            => Current?.PropertyType;
    }
}