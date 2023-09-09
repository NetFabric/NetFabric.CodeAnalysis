using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetFabric.CSharp.TestData;

public readonly struct AsyncEnumeratorWithMissingCurrent
{
    public ValueTask<bool> MoveNextAsync() 
        => new(Task.FromResult(false));
}

public readonly struct AsyncEnumeratorWithMissingMoveNextAsync<T>
{
    public readonly T Current 
        => default!;
}

public struct ValueTypeAsyncEnumerator<T>
{
    readonly T[] source;
    int index;

    public ValueTypeAsyncEnumerator(T[] source)
        => (this.source, index) = (source, -1);
    
    public readonly T Current 
        => source[index];

    public ValueTask<bool> MoveNextAsync() 
        => new(++index < source.Length);
}

public struct DisposableValueTypeAsyncEnumerator<T>
    : IAsyncDisposable
{
    readonly T[] source;
    int index;

    public DisposableValueTypeAsyncEnumerator(T[] source)
        => (this.source, index) = (source, -1);
    
    public readonly T Current 
        => source[index];

    public ValueTask<bool> MoveNextAsync() 
        => new(++index < source.Length);

    public ValueTask DisposeAsync()
        => default;
}

public struct ReferenceTypeAsyncEnumerator<T>
{
    readonly T[] source;
    int index;

    public ReferenceTypeAsyncEnumerator(T[] source)
        => (this.source, index) = (source, -1);
    
    public readonly T Current 
        => source[index];

    public ValueTask<bool> MoveNextAsync() 
        => new(++index < source.Length);

    public ValueTask DisposeAsync() // should not be called
        => throw new NotSupportedException();
}

public struct DisposableReferenceTypeAsyncEnumerator<T>
    : IAsyncDisposable
{
    readonly T[] source;
    int index;

    public DisposableReferenceTypeAsyncEnumerator(T[] source)
        => (this.source, index) = (source, -1);
    
    public readonly T Current 
        => source[index];

    public ValueTask<bool> MoveNextAsync() 
        => new(++index < source.Length);

    public ValueTask DisposeAsync()
        => default;
}

public class ExplicitAsyncEnumerator<T> 
    : IAsyncEnumerator<T>
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
