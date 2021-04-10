using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFabric.CSharp.TestData
{
    public readonly struct AsyncEnumerableWithMissingGetAsyncEnumerator
    {
    }

    public readonly struct AsyncEnumerableWithMissingCurrent
    {
        public AsyncEnumeratorWithMissingCurrent GetAsyncEnumerator() 
            => new();
    }

    public readonly struct AsyncEnumerableWithMissingMoveNextAsync<T>
    {
        public AsyncEnumeratorWithMissingMoveNextAsync<T> GetAsyncEnumerator() 
            => new();
    }

    public readonly struct AsyncEnumerableWithValueTypeAsyncEnumerator<T>
    {
        readonly T[] source;

        public AsyncEnumerableWithValueTypeAsyncEnumerator(T[] source)
            => this.source = source; 
        
        public ValueTypeAsyncEnumerator<T> GetAsyncEnumerator() 
            => new(source);
    }

    public readonly struct AsyncEnumerableWithDisposableValueTypeAsyncEnumerator<T>
    {
        readonly T[] source;

        public AsyncEnumerableWithDisposableValueTypeAsyncEnumerator(T[] source)
            => this.source = source; 
        
        public DisposableValueTypeAsyncEnumerator<T> GetAsyncEnumerator() 
            => new(source);
    }

    public readonly struct CancellableAsyncEnumerableWithValueTypeAsyncEnumerator<T>
    {
        readonly T[] source;

        public CancellableAsyncEnumerableWithValueTypeAsyncEnumerator(T[] source)
            => this.source = source; 
        
        public ValueTypeAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken _ = default) 
            => new(source);
    }

    public readonly struct CancellableAsyncEnumerableWithDisposableValueTypeAsyncEnumerator<T>
    {
        readonly T[] source;

        public CancellableAsyncEnumerableWithDisposableValueTypeAsyncEnumerator(T[] source)
            => this.source = source; 
        
        public DisposableValueTypeAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken _ = default) 
            => new(source);
    }

    public readonly struct AsyncEnumerableWithReferenceTypeAsyncEnumerator<T>
    {
        readonly T[] source;

        public AsyncEnumerableWithReferenceTypeAsyncEnumerator(T[] source)
            => this.source = source; 
        
        public ReferenceTypeAsyncEnumerator<T> GetAsyncEnumerator() 
            => new(source);
    }

    public readonly struct AsyncEnumerableWithDisposableReferenceTypeAsyncEnumerator<T>
    {
        readonly T[] source;

        public AsyncEnumerableWithDisposableReferenceTypeAsyncEnumerator(T[] source)
            => this.source = source; 
        
        public DisposableReferenceTypeAsyncEnumerator<T> GetAsyncEnumerator() 
            => new(source);
    }

    public readonly struct CancellableAsyncEnumerableWithReferenceTypeAsyncEnumerator<T>
    {
        readonly T[] source;

        public CancellableAsyncEnumerableWithReferenceTypeAsyncEnumerator(T[] source)
            => this.source = source; 
        
        public ReferenceTypeAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken _ = default) 
            => new(source);
    }

    public readonly struct CancellableAsyncEnumerableWithDisposableReferenceTypeAsyncEnumerator<T>
    {
        readonly T[] source;

        public CancellableAsyncEnumerableWithDisposableReferenceTypeAsyncEnumerator(T[] source)
            => this.source = source; 
        
        public DisposableReferenceTypeAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken _ = default) 
            => new(source);
    }

    public readonly struct HybridAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        readonly T[] source;

        public HybridAsyncEnumerable(T[] source)
            => this.source = source; 
        
        public ValueTypeAsyncEnumerator<T> GetAsyncEnumerator() 
            => new(source);
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken _) 
            => new ExplicitAsyncEnumerator<T>(source);
    }

    public class ExplicitAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        readonly T[] source;

        public ExplicitAsyncEnumerable(T[] source)
            => this.source = source; 
        
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken _) 
            => new ExplicitAsyncEnumerator<T>(source);
    }

    static class ValidateAsyncEnumerables
    {
        public static async Task InvalidAsyncEnumerables()
        {
            // await foreach (var _ in new MissingGetEnumeratorEnumerable()) { }
            // await foreach (var _ in new MissingCurrentEnumerable()) { }
            // await foreach (var _ in new MissingMoveNextEnumerable<int>()) { }
        }

        public static async Task ValidAsyncEnumerables()
        {
            await foreach (var _ in new AsyncEnumerableWithValueTypeAsyncEnumerator<int>(Array.Empty<int>())) { }
            await foreach (var _ in new AsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>(Array.Empty<int>())) { }
            await foreach (var _ in new CancellableAsyncEnumerableWithValueTypeAsyncEnumerator<int>(Array.Empty<int>())) { }
            await foreach (var _ in new CancellableAsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>(Array.Empty<int>())) { }
            await foreach (var _ in new AsyncEnumerableWithReferenceTypeAsyncEnumerator<int>(Array.Empty<int>())) { }
            await foreach (var _ in new AsyncEnumerableWithDisposableReferenceTypeAsyncEnumerator<int>(Array.Empty<int>())) { }
            await foreach (var _ in new CancellableAsyncEnumerableWithReferenceTypeAsyncEnumerator<int>(Array.Empty<int>())) { }
            await foreach (var _ in new CancellableAsyncEnumerableWithDisposableReferenceTypeAsyncEnumerator<int>(Array.Empty<int>())) { }
            await foreach (var _ in new HybridAsyncEnumerable<int>(Array.Empty<int>())) { }
            await foreach (var _ in new ExplicitAsyncEnumerable<int>(Array.Empty<int>())) { }
            await foreach (var _ in new RangeAsyncEnumerable()) { }
        }
    }
}