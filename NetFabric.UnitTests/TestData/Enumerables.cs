using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFabric.TestData
{
    public readonly struct MissingGetEnumeratorEnumerable
    {
    }

    public readonly struct MissingCurrentEnumerable
    {
        public MissingCurrentEnumerator GetEnumerator() 
            => new();
        public MissingCurrentEnumerator GetAsyncEnumerator() 
            => new();
    }

    public readonly struct MissingMoveNextEnumerable<T>
    {
        public MissingMoveNextEnumerator<T> GetEnumerator() 
            => new();
        public MissingMoveNextEnumerator<T> GetAsyncEnumerator() 
            => new();
    }

    public readonly struct Enumerable<T>
    {
        readonly T[] source;

        public Enumerable(T[] source)
            => this.source = source; 
        
        public Enumerator<T> GetEnumerator() 
            => new(source);
    }

    public readonly struct EnumerableRefEnumerator<T>
    {
        readonly T[] source;

        public EnumerableRefEnumerator(T[] source)
            => this.source = source; 
        
        public RefEnumerator<T> GetEnumerator() 
            => new(source);
    }

    public readonly struct EnumerableDisposableRefEnumerator<T>
    {
        readonly T[] source;

        public EnumerableDisposableRefEnumerator(T[] source)
            => this.source = source; 
        
        public DisposableRefEnumerator<T> GetEnumerator() 
            => new(source);
    }

    public readonly struct AsyncEnumerable<T>
    {
        readonly T[] source;

        public AsyncEnumerable(T[] source)
            => this.source = source; 
        
        public AsyncEnumerator<T> GetAsyncEnumerator() 
            => new(source);
    }

    public readonly struct CancellableAsyncEnumerable<T>
    {
        readonly T[] source;

        public CancellableAsyncEnumerable(T[] source)
            => this.source = source; 
        
        public AsyncEnumerator<T> GetAsyncEnumerator(CancellationToken _ = default) 
            => new(source);
    }

    public readonly struct HybridEnumerable<T> : IEnumerable<T>
    {
        readonly T[] source;

        public HybridEnumerable(T[] source)
            => this.source = source; 
        
        public Enumerator<T> GetEnumerator() 
            => new(source);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => new ExplicitGenericEnumerator<T>(source);
        IEnumerator IEnumerable.GetEnumerator() 
            => new ExplicitEnumerator<T>(source);
    }

    public readonly struct HybridAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        readonly T[] source;

        public HybridAsyncEnumerable(T[] source)
            => this.source = source; 
        
        public AsyncEnumerator<T> GetAsyncEnumerator() 
            => new(source);
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken _) 
            => new ExplicitAsyncEnumerator<T>(source);
    }

    public class ExplicitGenericEnumerable<T> : IEnumerable<T>
    {
        readonly T[] source;

        public ExplicitGenericEnumerable(T[] source)
            => this.source = source; 
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => new ExplicitGenericEnumerator<T>(source);
        IEnumerator IEnumerable.GetEnumerator() 
            => new ExplicitEnumerator<T>(source);
    }

    public class ExplicitEnumerable<T> : IEnumerable
    {
        readonly T[] source;

        public ExplicitEnumerable(T[] source)
            => this.source = source; 
        
        IEnumerator IEnumerable.GetEnumerator() 
            => new ExplicitEnumerator<T>(source);
    }

    public class ExplicitAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        readonly T[] source;

        public ExplicitAsyncEnumerable(T[] source)
            => this.source = source; 
        
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken _) 
            => new ExplicitAsyncEnumerator<T>(source);
    }

    static class ValidateEnumerables
    {
        //public static async Task InvalidEnumerables()
        //{
        //    foreach (var _ in new MissingGetEnumeratorEnumerable()) { }
        //    foreach (var _ in new MissingCurrentEnumerable()) { }
        //    foreach (var _ in new MissingMoveNextEnumerable<int>()) { }

        //    await foreach (var _ in new MissingGetEnumeratorEnumerable()) { }
        //    await foreach (var _ in new MissingCurrentEnumerable()) { }
        //    await foreach (var _ in new MissingMoveNextEnumerable<int>()) { }
        //}

        public static async Task ValidEnumerables()
        {
            foreach (var _ in new Enumerable<int>(Array.Empty<int>())) { }
            foreach (var _ in new HybridEnumerable<int>(Array.Empty<int>())) { }
            foreach (var _ in new ExplicitEnumerable<int>(Array.Empty<int>())) { }
            foreach (var _ in new RangeEnumerable()) { }

            await foreach (var _ in new AsyncEnumerable<int>(Array.Empty<int>())) { }
            await foreach (var _ in new CancellableAsyncEnumerable<int>(Array.Empty<int>())) { }
            await foreach (var _ in new HybridAsyncEnumerable<int>(Array.Empty<int>())) { }
            await foreach (var _ in new ExplicitAsyncEnumerable<int>(Array.Empty<int>())) { }
            await foreach (var _ in new RangeAsyncEnumerable()) { }
        }
    }
}