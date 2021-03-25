﻿using System;
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
        public bool MoveNext() => false;

        public ValueTask<bool> MoveNextAsync() => new(Task.FromResult(false));
    }

    public readonly struct MissingMoveNextEnumerator<T>
    {
        public readonly T Current => default!;
    }

    public readonly struct Enumerator<T>
    {
        public readonly T Current => default!;

        public bool MoveNext() => false;

        public void Dispose() { } // should not be returned
    }

    
    public readonly struct DisposableEnumerator<T> : IDisposable
    {
        public readonly T Current => default!;

        public bool MoveNext() => false;

        public void Dispose() { }
    }

    public readonly ref struct RefEnumerator<T>
    {
        public readonly T Current => default!;

        public bool MoveNext() => false;
    }
    
    public readonly ref struct DisposableRefEnumerator<T>
    {
        public readonly T Current => default!;

        public bool MoveNext() => false;

        public void Dispose() { }
    }

    public readonly struct AsyncEnumerator<T>
    {
        public readonly T Current => default!;

        public ValueTask<bool> MoveNextAsync() => new(Task.FromResult(false));
    }

    public class ExplicitEnumerator<T> : IEnumerator<T>
    {
        T IEnumerator<T>.Current => default!;
        object? IEnumerator.Current => default;

        bool IEnumerator.MoveNext() => false;

        void IEnumerator.Reset() { }

        void IDisposable.Dispose() { }
    }

    public class ExplicitEnumerator : IEnumerator
    {
        object? IEnumerator.Current => default;

        bool IEnumerator.MoveNext() => false;

        void IEnumerator.Reset() { }
    }

    public class ExplicitAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        T IAsyncEnumerator<T>.Current => default!;

        ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync() => new(Task.FromResult(false));

        ValueTask IAsyncDisposable.DisposeAsync() => new();
    }
}
