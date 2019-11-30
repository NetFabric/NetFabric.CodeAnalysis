using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetFabric.CodeAnalysis.TestData
{
    public class MissingCurrentAndMoveNextEnumerator
    {
    }

    public class MissingCurrentEnumerator
    {
        public bool MoveNext() => false;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(Task.FromResult(false));
    }

    public class MissingMoveNextEnumerator<T>
    {
        public T Current => default;
    }

    public class Enumerator<T>
    {
        public T Current => default;

        public bool MoveNext() => false;
    }

    public class AsyncEnumerator<T>
    {
        public T Current => default;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(Task.FromResult(false));
    }

    public class ExplicitEnumerator<T> : IEnumerator<T>
    {
        T IEnumerator<T>.Current => default;
        object IEnumerator.Current => default;

        bool IEnumerator.MoveNext() => false;

        void IEnumerator.Reset() { }

        void IDisposable.Dispose() { }
    }

    public class ExplicitAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        T IAsyncEnumerator<T>.Current => default;

        ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync() => new ValueTask<bool>(Task.FromResult(false));

        ValueTask IAsyncDisposable.DisposeAsync() => new ValueTask();
    }
}
