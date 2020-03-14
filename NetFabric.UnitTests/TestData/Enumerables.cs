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
        public MissingCurrentEnumerator GetEnumerator() => new MissingCurrentEnumerator();
        public MissingCurrentEnumerator GetAsyncEnumerator() => new MissingCurrentEnumerator();
    }

    public readonly struct MissingMoveNextEnumerable<T>
    {
        public MissingMoveNextEnumerator<T> GetEnumerator() => new MissingMoveNextEnumerator<T>();
        public MissingMoveNextEnumerator<T> GetAsyncEnumerator() => new MissingMoveNextEnumerator<T>();
    }

    public readonly struct Enumerable<T>
    {
        public readonly Enumerator<T> GetEnumerator() => new Enumerator<T>();
    }

    public readonly struct AsyncEnumerable<T>
    {
        public readonly AsyncEnumerator<T> GetAsyncEnumerator() => new AsyncEnumerator<T>();
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
            foreach (var _ in new ExplicitEnumerable<int>()) { }
            foreach (var _ in new RangeEnumerable()) { }

            await foreach (var _ in new AsyncEnumerable<int>()) { }
            await foreach (var _ in new ExplicitAsyncEnumerable<int>()) { }
            await foreach (var _ in new RangeAsyncEnumerable()) { }
        }
    }
}