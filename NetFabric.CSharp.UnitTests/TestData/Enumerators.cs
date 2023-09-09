using System;
using System.Collections;
using System.Collections.Generic;

namespace NetFabric.CSharp.TestData;

public readonly struct EnumeratorWithMissingCurrent
{
    public bool MoveNext() 
        => false;
}

public readonly struct EnumeratorWithMissingMoveNext<T>
{
    public readonly T Current 
        => default!;
}

public struct ValueTypeEnumerator<T>
{
    readonly T[] source;
    int index;

    public ValueTypeEnumerator(T[] source)
        => (this.source, index) = (source, -1);
    
    public readonly T Current 
        => source[index];

    public bool MoveNext() 
        => ++index < source.Length;
}

public struct DisposableValueTypeEnumerator<T> 
    : IDisposable
{
    readonly T[] source;
    int index;

    public DisposableValueTypeEnumerator(T[] source)
        => (this.source, index) = (source, -1);
    
    public readonly T Current 
        => source[index];

    public bool MoveNext() 
        => ++index < source.Length;

    public void Dispose() 
    { }
}

public ref struct ByRefLikeEnumerator<T>
{
    readonly ReadOnlySpan<T> source;
    int index;

    public ByRefLikeEnumerator(ReadOnlySpan<T> source)
    {
        this.source = source;
        index = -1;
    }
    
    public readonly T Current 
        => source[index];

    public bool MoveNext() 
        => ++index < source.Length;
}

public ref struct DisposableByRefLikeEnumerator<T>
{
    readonly ReadOnlySpan<T> source;
    int index;

    public DisposableByRefLikeEnumerator(ReadOnlySpan<T> source)
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

public class ReferenceTypeEnumerator<T>
{
    readonly T[] source;
    int index;

    public ReferenceTypeEnumerator(T[] source)
        => (this.source, index) = (source, -1);
    
    public T Current 
        => source[index];

    public bool MoveNext() 
        => ++index < source.Length;

    public void Dispose() // should not be called
        => throw new NotSupportedException();
}

public class DisposableReferenceTypeEnumerator<T> 
    : IDisposable
{
    readonly T[] source;
    int index;

    public DisposableReferenceTypeEnumerator(T[] source)
        => (this.source, index) = (source, -1);
    
    public T Current 
        => source[index];

    public bool MoveNext() 
        => ++index < source.Length;

    public void Dispose() 
    { }
}

public class ExplicitGenericEnumerator<T> 
    : IEnumerator<T>
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

public class ExplicitEnumerator<T> 
    : IEnumerator
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

    public void Dispose() // should not be called
        => throw new NotSupportedException();
}
