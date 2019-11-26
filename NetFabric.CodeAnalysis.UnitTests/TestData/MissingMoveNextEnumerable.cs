namespace NetFabric.CodeAnalysis.TestData
{
    public class MissingMoveNextEnumerable<T>
    {
        public MissingMoveNextEnumerable<T> GetEnumerator() => this;

        public T Current => default;
    }

    public class MissingMoveNextAsyncAsyncEnumerable<T>
    {
        public MissingMoveNextAsyncAsyncEnumerable<T> GetAsyncEnumerator() => this;

        public T Current => default;
    }
}