namespace NetFabric.CodeAnalysis.TestData
{
    public class PrivateGetEnumeratorEnumerable<T>
    {
        PrivateGetEnumeratorEnumerable<T> GetEnumerator() => null;
    }

    public class StaticGetEnumeratorEnumerable<T>
    {
        public static StaticGetEnumeratorEnumerable<T> GetEnumerator() => null;
    }

    public class PrivateGetAsyncEnumeratorAsyncEnumerable<T>
    {
        PrivateGetAsyncEnumeratorAsyncEnumerable<T> GetAsyncEnumerator() => null;
    }

    public class StaticGetAsyncEnumeratorAsyncEnumerable<T>
    {
        public static StaticGetAsyncEnumeratorAsyncEnumerable<T> GetAsyncEnumerator() => null;
    }
}