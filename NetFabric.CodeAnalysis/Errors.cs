namespace NetFabric.CodeAnalysis;

/// <summary>
/// Specifies possible errors that may occur when checking if a type is enumerable.
/// </summary>
public enum IsEnumerableError
{
    /// <summary>
    /// Indicates that no error occurred while checking if the type is enumerable.
    /// </summary>
    None,

    /// <summary>
    /// Indicates that the type is missing a suitable <c>GetEnumerator</c> method, which is
    /// required for enumeration.
    /// </summary>
    MissingGetEnumerator,

    /// <summary>
    /// Indicates that the type is missing a suitable <c>Current</c> property, which is
    /// required to access elements during enumeration.
    /// </summary>
    MissingCurrent,

    /// <summary>
    /// Indicates that the type is missing a suitable <c>MoveNext</c> method, which is
    /// required to advance the enumeration to the next element.
    /// </summary>
    MissingMoveNext,
}

/// <summary>
/// Specifies possible errors that may occur when checking if a type is an async enumerable.
/// </summary>
public enum IsAsyncEnumerableError
{
    /// <summary>
    /// Indicates that no error occurred while checking if the type is an async enumerable.
    /// </summary>
    None,

    /// <summary>
    /// Indicates that the type is missing a suitable <c>GetAsyncEnumerator</c> method, which is
    /// required for asynchronous enumeration.
    /// </summary>
    MissingGetAsyncEnumerator,

    /// <summary>
    /// Indicates that the type is missing a suitable <c>Current</c> property, which is
    /// required to access elements during asynchronous enumeration.
    /// </summary>
    MissingCurrent,

    /// <summary>
    /// Indicates that the type is missing a suitable <c>MoveNextAsync</c> method, which is
    /// required to asynchronously advance the enumeration to the next element.
    /// </summary>
    MissingMoveNextAsync,
}

/// <summary>
/// Specifies possible errors that may occur when checking if a type is indexable.
/// </summary>
public enum IsIndexableError
{
    /// <summary>
    /// Indicates that no error occurred while checking if the type is indexable.
    /// </summary>
    None,

    /// <summary>
    /// Indicates that the type is missing a suitable indexer, which is required for indexing.
    /// </summary>
    MissingIndexer,

    /// <summary>
    /// Indicates that the type is missing a suitable <c>Count</c> or <c>Length</c> property, which is
    /// required for determining the number of elements in the indexable collection.
    /// </summary>
    MissingCountOrLength,
}

