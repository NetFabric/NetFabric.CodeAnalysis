# NetFabric.CodeAnalysis

To find if a type is enumerable, it's not enough to check if it implements `IEnumerable`, `IEnumerable<>`, or `IAsyncEnumerable<>`. 
`foreach` and `await foreach` support several other cases. 
This repository contains extension methods that take into account all these cases.

# Usage

## IsEnumerable()

- Add [NetFabric.CodeAnalysis](https://www.nuget.org/packages/NetFabric.CodeAnalysis/) package to your project.
- Use the `IsEnumerable` method as follow:
``` csharp
using NetFabric.CodeAnalysis;

var isEnumerable = typeSymbol.IsEnumerable(compilation, out var enumerableSymbols);

var isAsyncEnumerable = typeSymbol.IsAsyncEnumerable(compilation, out var asyncEnumerableSymbols);
```

The methods return a boolean value indicating if it's a valid enumerable or enumerator. 

If `true`, the output parameter contains [`IMethodSymbol`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.imethodsymbol) for the method `GetEnumerator`/`GetAsynEnumerator` of the enumerable, the property `Current` and the method `MoveNext`/`MoveNextAsync` of the enumerator, following the precedences used by Roslyn for the `foreach` and `await foreach` keywords. It may also contain for methods `Reset` and `Dispose`/`DisposeAsync` if defined.

