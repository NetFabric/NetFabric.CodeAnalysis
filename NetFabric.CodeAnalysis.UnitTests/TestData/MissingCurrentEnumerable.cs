using System.Threading.Tasks;

namespace NetFabric.CodeAnalysis.TestData
{
    public class MissingCurrentEnumerable<T>
    {
        public MissingCurrentEnumerable<T> GetEnumerator() => this;

        public bool MoveNextAsync() => false;
    }

    public class MissingCurrentAsyncEnumerable<T>
    {
        public MissingCurrentAsyncEnumerable<T> GetAsyncEnumerator() => this;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(Task.FromResult(false));
    }
}