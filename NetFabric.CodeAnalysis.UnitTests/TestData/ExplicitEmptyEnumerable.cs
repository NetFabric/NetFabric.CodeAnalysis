using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFabric.CodeAnalysis.TestData
{
    public readonly struct ExplicitEmptyEnumerable<T>
        : IEnumerable<T>
        , IEnumerator<T>
    {
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;
        readonly IEnumerator IEnumerable.GetEnumerator() => this;

        readonly T IEnumerator<T>.Current => default;
        readonly object IEnumerator.Current => default;

        bool IEnumerator.MoveNext() => false;
        void IEnumerator.Reset() {}
        void IDisposable.Dispose() {}
    }

    public readonly struct ExplicitEmptyAsyncEnumerable<T>
        : IAsyncEnumerable<T>
        , IAsyncEnumerator<T>
    {
        readonly IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken = default) => this;

        readonly T IAsyncEnumerator<T>.Current => default;

        ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync() => new ValueTask<bool>(Task.FromResult(false));

        ValueTask IAsyncDisposable.DisposeAsync() => new ValueTask();
    }}