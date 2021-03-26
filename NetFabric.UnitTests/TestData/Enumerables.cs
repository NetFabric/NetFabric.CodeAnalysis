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
        public MissingCurrentEnumerator GetEnumerator() => new();
        public MissingCurrentEnumerator GetAsyncEnumerator() => new();
    }

    public readonly struct MissingMoveNextEnumerable<T>
    {
        public MissingMoveNextEnumerator<T> GetEnumerator() => new();
        public MissingMoveNextEnumerator<T> GetAsyncEnumerator() => new();
    }

    public readonly struct Enumerable<T>
    {
        public readonly Enumerator<T> GetEnumerator() => new();
    }

    public readonly struct AsyncEnumerable<T>
    {
        public readonly AsyncEnumerator<T> GetAsyncEnumerator() => new();
    }

    public readonly struct CancellableAsyncEnumerable<T>
    {
        public readonly AsyncEnumerator<T> GetAsyncEnumerator(CancellationToken _ = default) => new();
    }

    public readonly struct HybridEnumerable<T> : IEnumerable<T>
    {
        public readonly Enumerator<T> GetEnumerator() => new();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new ExplicitEnumerator<T>();
        IEnumerator IEnumerable.GetEnumerator() => new ExplicitEnumerator<T>();
    }

    public readonly struct HybridAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        public readonly AsyncEnumerator<T> GetAsyncEnumerator() => new();
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken _) => new ExplicitAsyncEnumerator<T>();
    }

    public class ExplicitEnumerable<T> : IEnumerable<T>
    {
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new ExplicitEnumerator<T>();
        IEnumerator IEnumerable.GetEnumerator() => new ExplicitEnumerator<T>();
    }

    public class ExplicitEnumerable : IEnumerable
    {
        IEnumerator IEnumerable.GetEnumerator() => new ExplicitEnumerator();
    }

    public class ExplicitAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken _) => new ExplicitAsyncEnumerator<T>();
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
            foreach (var _ in new Enumerable<int>()) { }
            foreach (var _ in new HybridEnumerable<int>()) { }
            foreach (var _ in new ExplicitEnumerable<int>()) { }
            foreach (var _ in new RangeEnumerable()) { }

            await foreach (var _ in new AsyncEnumerable<int>()) { }
            await foreach (var _ in new CancellableAsyncEnumerable<int>()) { }
            await foreach (var _ in new HybridAsyncEnumerable<int>()) { }
            await foreach (var _ in new ExplicitAsyncEnumerable<int>()) { }
            await foreach (var _ in new RangeAsyncEnumerable()) { }
        }
    }
}