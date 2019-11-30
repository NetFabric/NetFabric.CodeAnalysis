using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFabric.CodeAnalysis.TestData
{
    public readonly struct MissingGetEnumeratorEnumerable
    {
    }

    public readonly struct Enumerable<T>
    {
        public readonly Enumerable<T> GetEnumerator() => this;

        public readonly T Current => default;

        public bool MoveNext() => false;
    }

    public readonly struct AsyncEnumerable<T>
    {
        public readonly AsyncEnumerable<T> GetAsyncEnumerator() => this;

        public readonly T Current => default;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(Task.FromResult(false));
    }

    public class ExplicitEnumerable<T> : IEnumerable<T>, IEnumerator<T>
    {
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        T IEnumerator<T>.Current => default;
        object IEnumerator.Current => default;

        bool IEnumerator.MoveNext() => false;

        void IEnumerator.Reset() { }

        void IDisposable.Dispose() { }
    }

    public class ExplicitEnumerable : IEnumerable, IEnumerator
    {
        IEnumerator IEnumerable.GetEnumerator() => this;

        object IEnumerator.Current => default;

        bool IEnumerator.MoveNext() => false;

        void IEnumerator.Reset() { }
    }

    public class ExplicitAsyncEnumerable<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
    {
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken _) => this;

        T IAsyncEnumerator<T>.Current => default;

        ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync() => new ValueTask<bool>(Task.FromResult(false));

        ValueTask IAsyncDisposable.DisposeAsync() => new ValueTask();
    }
}