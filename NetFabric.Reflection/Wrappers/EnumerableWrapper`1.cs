using System;
using System.Collections;

namespace NetFabric.Reflection
{
    public sealed class EnumerableWrapper<TEnumerable>
        : IEnumerable
    {
        public EnumerableWrapper(TEnumerable instance, EnumerableInfo info)
        {
            Instance = instance;
            Info = info;
        }

        public TEnumerable Instance { get; }
        public EnumerableInfo Info { get; }

        public IEnumerator GetEnumerator() 
            => new Enumerator(this);

        public sealed class Enumerator 
            : IEnumerator
        {
            readonly EnumeratorInfo info;
            readonly object instance;

            public Enumerator(EnumerableWrapper<TEnumerable> enumerable)
            {
                info = enumerable.Info.EnumeratorInfo;
                instance = enumerable.Info.InvokeGetEnumerator(enumerable.Instance);
            }

            public object Current 
                => info.GetValueCurrent(instance);

            public bool MoveNext() 
                => info.InvokeMoveNext(instance);

            public void Reset() 
            {
                if (info.Reset is object)
                    info.InvokeReset(instance);
            }

            public void Dispose() 
            {
                if (info.Dispose is object)
                    info.InvokeDispose(instance);
            }
        }
    }
}