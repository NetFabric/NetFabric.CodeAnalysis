using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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

        sealed class Enumerator 
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
            {
                if (info.Reset is not null)
                    info.InvokeReset(instance);
            }

            public void Dispose()
            {
                if (info.Dispose is not null)
                    info.InvokeDispose(instance);
            }
        }
    }
}