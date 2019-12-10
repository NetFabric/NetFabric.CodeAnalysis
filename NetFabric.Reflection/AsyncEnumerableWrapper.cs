using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFabric.Reflection
{
    public sealed class AsyncEnumerableWrapper<TEnumerable, TItem>
        : IAsyncEnumerable<TItem>
    {
        public AsyncEnumerableWrapper(TEnumerable instance, AsyncEnumerableInfo info)
        {
            Instance = instance;
            Info = info;
        }

        public TEnumerable Instance { get; }
        public AsyncEnumerableInfo Info { get; }

        public IAsyncEnumerator<TItem> GetAsyncEnumerator(CancellationToken token) 
            => new Enumerator(this, token);

        public sealed class Enumerator 
            : IAsyncEnumerator<TItem>
        {
            readonly AsyncEnumeratorInfo info;
            readonly object instance;

            public Enumerator(AsyncEnumerableWrapper<TEnumerable, TItem> enumerable, CancellationToken token)
            {
                info = enumerable.Info.EnumeratorInfo;
                instance = enumerable.Info.InvokeGetAsyncEnumerator(enumerable.Instance, token);
            }

            public TItem Current 
                => (TItem)info.GetValueCurrent(instance);

            public ValueTask<bool> MoveNextAsync() 
                => info.InvokeMoveNextAsync(instance);

            public ValueTask DisposeAsync() 
            {
                if (info.DisposeAsync is object)
                    return info.InvokeDisposeAsync(instance);

                return default;
            }
        }
    }
}