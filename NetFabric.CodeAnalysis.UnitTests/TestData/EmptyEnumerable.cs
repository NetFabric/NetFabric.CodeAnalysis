using System.Threading.Tasks;

namespace NetFabric.CodeAnalysis.TestData
{
    public readonly struct EmptyEnumerable<T>
    {
        public readonly EmptyEnumerable<T> GetEnumerator() => this;

        public readonly T Current => default;

        public bool MoveNext() => false;
    }

    public readonly struct EmptyAsyncEnumerable<T>
    {
        public readonly EmptyAsyncEnumerable<T> GetAsyncEnumerator() => this;

        public readonly T Current => default;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(Task.FromResult(false));
    }}