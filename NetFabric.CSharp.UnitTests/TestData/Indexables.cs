namespace NetFabric.CSharp.TestData;

public readonly struct IndexableWithMissingIndexer<T>
{
    readonly T[] source;

    public IndexableWithMissingIndexer(T[] source)
        => this.source = source;

    public int Count
        => source.Length;
}

public readonly struct IndexableWithMissingCountOrLength<T>
{
    readonly T[] source;

    public IndexableWithMissingCountOrLength(T[] source)
        => this.source = source;

    public T this[int index]
        => source[index];
}

public readonly struct IndexableWithDifferentIndexingTypes<T>
{
    readonly T[] source;

    public IndexableWithDifferentIndexingTypes(T[] source)
        => this.source = source;

    public T this[int index]
        => source[index];

    public long Count 
        => source.Length;
}

public readonly struct IndexableWithCount<T>
{
    readonly T[] source;

    public IndexableWithCount(T[] source)
        => this.source = source;

    public T this[int index]
        => source[index];

    public int Count 
        => source.Length;
}

public readonly struct IndexableWithLength<T>
{
    readonly T[] source;

    public IndexableWithLength(T[] source)
        => this.source = source;

    public T this[int index]
        => source[index];

    public int Length
        => source.Length;
}

public interface IIndexable<T>
{
    T this[int index] { get; }

    int Length { get; }
}

public interface IDerivedIndexable<T>
    : IIndexable<T>
{
    
}
