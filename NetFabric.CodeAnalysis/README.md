# NetFabric.CodeAnalysis

This package extends the API provided by [Roslyn](https://github.com/dotnet/roslyn/blob/main/docs/wiki/Roslyn-Overview.md). It can be used in the development of [Roslyn Analyzers](https://learn.microsoft.com/en-us/visualstudio/code-quality/roslyn-analyzers-overview) and [Source Generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview).

## IsEnumerable() and IsAsyncEnumerable()

To find if a type is enumerable, it's not enough to check if it implements `IEnumerable`, `IEnumerable<>`, or `IAsyncEnumerable<>`. `foreach` and `await foreach` support several other cases. Use the methods `IsEnumerable` and `IsAsyncEnumerable` instead:

```csharp
using NetFabric.CodeAnalysis;

var isEnumerable = typeSymbol.IsEnumerable(compilation, out var enumerableSymbols, out var errors);

var isAsyncEnumerable = typeSymbol.IsAsyncEnumerable(compilation, out var asyncEnumerableSymbols, out var errors);
```

The methods return a boolean value indicating if it's an enumerable or enumerator accepted by `foreach`. It supports the cases where `GetEnumerator()` or `GetAsyncEnumerator()` are provided as extension methods.

If `true`, the first output parameter contains [`IMethodSymbol`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.imethodsymbol) for the method `GetEnumerator`/`GetAsynEnumerator` of the enumerable, the property `Current` and the method `MoveNext`/`MoveNextAsync` of the enumerator, following the precedences used by Roslyn for the `foreach` and `await foreach` keywords. It may also contain for methods `Reset` and `Dispose`/`DisposeAsync` if defined.

If `false`, the second output parameter indicates what error was found. It can be a missing `GetEnumerator()`, missing `Current`, or missing `MoveNext()`.
