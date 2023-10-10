# NetFabric.CodeAnalysis and NetFabric.Reflection

This is documentation for both the NetFabric.CodeAnalysis and NetFabric.Reflection packages. These packages share a lot of the same functionality to be executed at different times.

## NetFabric.CodeAnalysis

The NetFabric.CodeAnalysis package is used to analyze code at compile time. It can be used in the implementation of a Roslyn analyzer, or in a code generator.

## NetFabric.Reflection

The NetFabric.Reflection package is used to analyze code at runtime. It can be used to analyze code in a unit test, or any other executable code.

It also provides expression trees that can be used to generate code at runtime.

# Enumerables

Both packages contain a lot of functionality to analyze enumerables. The 'foreach' statement in C# supports enumerables, and the compiler will generate code to iterate over the enumerable. The compiler will generate code to call the 'GetEnumerator' method on the enumerable, and then call the 'MoveNext' method on the enumerator until it returns false. The 'Current' property will be used to get the current value. The compiler will also generate code to call the 'Dispose' method on the enumerator when the 'foreach' statement is done.

This means that it's not enough to check if a  type implements `IEnumerable` or `IEnumerable<T>` to find out if it's enumerable. These two packages provide a lot of functionality to find out if a type is enumerable, and to get information about the enumerable.

