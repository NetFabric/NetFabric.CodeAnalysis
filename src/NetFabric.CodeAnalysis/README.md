# NetFabric.CodeAnalysis

This package extends the API provided by [Roslyn](https://github.com/dotnet/roslyn/blob/main/docs/wiki/Roslyn-Overview.md). It can be used in the development of [Roslyn Analyzers](https://learn.microsoft.com/en-us/visualstudio/code-quality/roslyn-analyzers-overview) and [Source Generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview).

## Enumerable type checking

To find if a type is enumerable, it's not enough to check if it implements `IEnumerable`, `IEnumerable<>` or `IAsyncEnumerable<>`. The `foreach` and `await foreach` statements support several other cases.

> NOTE: Check the article ["Efficient Data Processing: Leveraging C#'s foreach Loop"](https://www.linkedin.com/pulse/efficient-data-processing-leveraging-cs-foreach-loop-ant%C3%A3o-almada/) to understand all the possible cases supported by the `foreach` statement.

This package provides extension methods for the interface `ITypeSymbol` that can correctly validate if the type it represents can be used as the source in `foreach` or `await foreach` statements.

### IsEnumerable

```csharp
public static bool IsEnumerable(this ITypeSymbol typeSymbol,
    Compilation compilation,
    [NotNullWhen(true)] out EnumerableSymbols? enumerableSymbols,
    out IsEnumerableError error);
```

The method returns `true` if the type represented by `ITypeSymbol` can be used in a `foreach` statement; otherwise `false`. It supports all the cases including when `GetEnumerator()` is defined as an extension method.

If it returns `true`, the `enumerableSymbols` output parameter contains all the `IMethodInfo` and `IPropertySymbol` for the methods and properties that are going to be actually used by the `foreach` statement. The `GetEnumerator()` of the enumerable, the property `Current` and the method `MoveNext()` of the enumerator. It may also contain info for methods `Reset()` and `Dispose()` of the enumerator, if defined.

If it returns `false`, the `error` output parameter indicates why the type is not considered an enumerable. It can be `MissingGetEnumerator`, `MissingCurrent` or `MissingMoveNext`.

The output parameter also includes a `ForEachUsesIndexer` boolean property that indicates that, although the collection provides an enumerator, `foreach` will use the indexer instead. That's the case for arrays and spans.

You can use these info values to further validate the enumerable and its respective enumerator. For example, use the following to find if the `Current` property of the enumerator returns by reference:

```csharp
enumerableSymbols.EnumeratorSymbols.Current.ReturnsByRef;
```

### IsAsyncEnumerable

```csharp
public static bool IsAsyncEnumerable(this ITypeSymbol typeSymbol,
    Compilation compilation,
    [NotNullWhen(true)] out AsyncEnumerableSymbols? enumerableSymbols,
    out IsAsyncEnumerableError error);
```

The method returns `true` if the type represented by `ITypeSymbol` can be used in an `await foreach` statement; otherwise `false`. It supports all the cases including when `GetAsyncEnumerator()` is defined as an extension method.

If it returns `true`, the `enumerableSymbols` output parameter contains all the `IMethodInfo` and `IPropertySymbol` for the methods and properties that are going to be actually used by the `await foreach` statement. The `GetAsyncEnumerator()` of the enumerable, the property `Current` and the method `MoveNextAsync()` of the enumerator. It may also contain info for method `DisposeAsync()` of the enumerator, if defined.

If it returns `false`, the `error` output parameter indicates why the type is not considered an enumerable. It can be  `MissingGetAsyncEnumerator`, `MissingCurrent` or `MissingMoveNextAsync`.

You can use these info values to further validate the async enumerable or its respective enumerator.
