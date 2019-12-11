using System;
using System.Collections;
using System.Collections.Generic;

namespace NetFabric.Reflection
{
    public sealed class EnumerableWrapper<TEnumerable, TItem>
        : IEnumerable<TItem>
    {
        public EnumerableWrapper(TEnumerable instance, EnumerableInfo info)
        {
            Instance = instance;
            Info = info;
        }

        public TEnumerable Instance { get; }
        public EnumerableInfo Info { get; }

        public IEnumerator<TItem> GetEnumerator() 
            => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() 
            => new Enumerator(this);

        public sealed class Enumerator 
            : IEnumerator<TItem>
        {
            readonly EnumeratorInfo info;
            readonly object instance;

            public Enumerator(EnumerableWrapper<TEnumerable, TItem> enumerable)
            {
                info = enumerable.Info.EnumeratorInfo;
                instance = enumerable.Info.InvokeGetEnumerator(enumerable.Instance);
            }

            public TItem Current 
                => (TItem)info.GetValueCurrent(instance);
            object IEnumerator.Current 
                => info.GetValueCurrent(instance);

            public bool MoveNext() 
                => info.InvokeMoveNext(instance);

            public void Reset() 
                => info.InvokeReset(instance);

            public void Dispose() 
            {
                if (info.Dispose is object)
                    info.InvokeDispose(instance);
            }
        }
    }
}