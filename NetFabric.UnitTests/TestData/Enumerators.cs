using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetFabric.TestData
{
    public readonly struct MissingCurrentAndMoveNextEnumerator
    {
    }

    public readonly struct MissingCurrentEnumerator
    {
        public bool MoveNext() 
            => false;

        public ValueTask<bool> MoveNextAsync() 
            => new(Task.FromResult(false));
    }

    public readonly struct MissingMoveNextEnumerator<T>
    {
        public readonly T Current 
            => default!;
    }

    public struct Enumerator<T>
    {
        readonly T[] source;
        int index;

        public Enumerator(T[] source)
            => (this.source, index) = (source, -1);
        
        public readonly T Current 
            => source[index];

        public bool MoveNext() 
            => ++index < source.Length;

        public void Dispose() // should not be returned
        { } 
    }

    
    public struct DisposableEnumerator<T> : IDisposable
    {
        readonly T[] source;
        int index;

        public DisposableEnumerator(T[] source)
            => (this.source, index) = (source, -1);
        
        public readonly T Current 
            => source[index];

        public bool MoveNext() 
            => ++index < source.Length;

        public void Dispose() 
        { }
    }

    public ref struct RefEnumerator<T>
    {
        readonly ReadOnlySpan<T> source;
        int index;

        public RefEnumerator(ReadOnlySpan<T> source)
        {
            this.source = source;
            index = -1;
        }
        
        public readonly T Current 
            => source[index];

        public bool MoveNext() 
            => ++index < source.Length;
    }
    
    public ref struct DisposableRefEnumerator<T>
    {
        readonly ReadOnlySpan<T> source;
        int index;

        public DisposableRefEnumerator(ReadOnlySpan<T> source)
        {
            this.source = source;
            index = -1;
        }
        
        public readonly T Current 
            => source[index];

        public bool MoveNext() 
            => ++index < source.Length;
    
        public void Dispose() 
        { }
    }

    public struct AsyncEnumerator<T>
    {
        readonly T[] source;
        int index;

        public AsyncEnumerator(T[] source)
            => (this.source, index) = (source, -1);
        
        public readonly T Current 
            => source[index];

        public ValueTask<bool> MoveNextAsync() 
            => new(++index < source.Length);
    }

    public class ExplicitGenericEnumerator<T> : IEnumerator<T>
    {
        readonly T[] source;
        int index;

        public ExplicitGenericEnumerator(T[] source)
            => (this.source, index) = (source, -1);

        T IEnumerator<T>.Current 
            => source[index];
        object? IEnumerator.Current 
            => source[index];

        bool IEnumerator.MoveNext() 
            => ++index < source.Length;

        void IEnumerator.Reset() 
        { }

        void IDisposable.Dispose() 
        { }
    }

    public class ExplicitEnumerator<T> : IEnumerator
    {
        readonly T[] source;
        int index;

        public ExplicitEnumerator(T[] source)
            => (this.source, index) = (source, -1);

        object? IEnumerator.Current 
            => source[index];

        bool IEnumerator.MoveNext() 
            => ++index < source.Length;

        void IEnumerator.Reset() 
        { }
    }

    public class ExplicitAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        readonly T[] source;
        int index;

        public ExplicitAsyncEnumerator(T[] source)
            => (this.source, index) = (source, -1);

        T IAsyncEnumerator<T>.Current 
            => source[index];

        ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync() 
            => new(++index < source.Length);

        ValueTask IAsyncDisposable.DisposeAsync() 
            => new();
    }
}
