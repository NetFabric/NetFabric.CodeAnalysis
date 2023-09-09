using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetFabric.Hyperlinq;

namespace NetFabric.CSharp.TestData;

public readonly struct RangeAsyncEnumerable
    : IAsyncValueEnumerable<int, RangeAsyncEnumerable.DisposableAsyncEnumerator>
{    
    readonly int count;
    
    public RangeAsyncEnumerable(int count)
        => this.count = count;
        
    public readonly AsyncEnumerator GetAsyncEnumerator() 
        => new(count);
    readonly DisposableAsyncEnumerator IAsyncValueEnumerable<int, DisposableAsyncEnumerator>.GetAsyncEnumerator(CancellationToken cancellationToken) 
        => new(count);
    readonly IAsyncEnumerator<int> IAsyncEnumerable<int>.GetAsyncEnumerator(CancellationToken cancellationToken) 
        => new DisposableAsyncEnumerator(count);
    
    public struct AsyncEnumerator
    {
        readonly int count;
        int current;
        
        internal AsyncEnumerator(int count)
        {
            this.count = count;
            current = -1;
        }
        
        public readonly int Current => current;
        
        public ValueTask<bool> MoveNextAsync()
            => new(Task.FromResult(++current < count));
    }
    
    public struct DisposableAsyncEnumerator : IAsyncEnumerator<int>
    {
        readonly int count;
        int current;
        
        internal DisposableAsyncEnumerator(int count)
        {
            this.count = count;
            current = -1;
        }
        
        public int Current => current;
        
        public ValueTask<bool> MoveNextAsync()
            => new(Task.FromResult(++current < count));
        
        ValueTask IAsyncDisposable.DisposeAsync() => new();
    }
}