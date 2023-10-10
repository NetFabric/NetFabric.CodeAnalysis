[![GitHub last commit (main)](https://img.shields.io/github/last-commit/NetFabric/NetFabric.CodeAnalysis/main.svg?style=flat-square&logo=github)](https://github.com/NetFabric/NetFabric.CodeAnalysis/commits/main)
[![Build (main)](https://img.shields.io/github/actions/workflow/status/NetFabric/NetFabric.CodeAnalysis/dotnetcore.yml?style=flat-square&logo=github)](https://github.com/NetFabric/NetFabric.CodeAnalysis/actions)
[![Coverage](https://img.shields.io/coveralls/github/NetFabric/NetFabric.CodeAnalysis/main?style=flat-square&logo=coveralls)](https://coveralls.io/github/NetFabric/NetFabric.CodeAnalysis)

[![CodeAnalysis NuGet Version](https://img.shields.io/nuget/v/NetFabric.CodeAnalysis.svg?style=flat-square&label=CodeAnalysis%20nuget&logo=nuget)](https://www.nuget.org/packages/NetFabric.CodeAnalysis/)
[![CodeAnalysis NuGet Downloads](https://img.shields.io/nuget/dt/NetFabric.CodeAnalysis?style=flat-square&label=CodeAnalysis%20downloads&logo=nuget)](https://www.nuget.org/packages/NetFabric.CodeAnalysis/)

[![Reflection NuGet Version](https://img.shields.io/nuget/v/NetFabric.Reflection.svg?style=flat-square&label=Reflection%20nuget&logo=nuget)](https://www.nuget.org/packages/NetFabric.Reflection/)
[![Reflection NuGet Downloads](https://img.shields.io/nuget/dt/NetFabric.Reflection.svg?style=flat-square&label=Reflection%20downloads&logo=nuget)](https://www.nuget.org/packages/NetFabric.Reflection/)

# NetFabric.CodeAnalysis and NetFabric.Reflection Documentation

This documentation covers both the NetFabric.CodeAnalysis and NetFabric.Reflection packages. These packages offer overlapping functionality designed for different phases of code execution.

Having both packages within the same repository facilitates the sharing of common data sets among unit tests.

## NetFabric.CodeAnalysis

The NetFabric.CodeAnalysis package serves the purpose of code analysis during compile time. It finds application in Roslyn analyzers' implementation and code generation tasks.

## NetFabric.Reflection

The NetFabric.Reflection package, on the other hand, is geared towards runtime code analysis. It proves valuable in scenarios such as unit testing or any other runtime code execution.

Moreover, it offers expression trees that facilitate dynamic code generation during runtime.

# Enumerable Analysis

Both packages boast extensive capabilities for analyzing enumerables. In C#, the 'foreach' statement seamlessly handles enumerables, with the compiler generating code to iterate over them. This involves invoking the 'GetEnumerator' method on the enumerable, repeatedly calling 'MoveNext' on the enumerator until it returns false, and utilizing the 'Current' property to access the current value. The compiler also takes care of invoking the 'Dispose' method on the enumerator upon 'foreach' statement completion.

Hence, merely checking if a type implements `IEnumerable` or `IEnumerable<T>` is insufficient for identifying its enumerability. These packages offer a rich array of functionality to ascertain whether a type is enumerable and to gather comprehensive information about it.

# Documentation

Documentation is available [here](https://netfabric.github.io/NetFabric.CodeAnalysis/).

# References

-   [Efficient Data Processing: Leveraging C#'s foreach Loop](https://www.linkedin.com/pulse/efficient-data-processing-leveraging-cs-foreach-loop-ant%C3%A3o-almada) by Antão Almada
-   [Handling enumerables in Roslyn Analyzers and Code Generators](https://www.linkedin.com/pulse/handling-enumerables-roslyn-analyzers-code-generators-ant%C3%A3o-almada) by Antão Almada
-   [Handling enumerables when using reflection](https://www.linkedin.com/pulse/handling-enumerables-when-using-reflection-ant%C3%A3o-almada) by Antão Almada

# Credits

The following open-source projects are used to build and test this project:

-   [.NET](https://github.com/dotnet)
-   [coveralls](https://coveralls.io)
-   [coverlet](https://github.com/tonerdo/coverlet)
-   [IsExternalInit](https://github.com/manuelroemer/IsExternalInit)
-   [Nullable](https://github.com/manuelroemer/Nullable)
-   [ReadableExpressions](https://github.com/agileobjects/ReadableExpressions)
-   [ReflectionAnalyzers](https://github.com/DotNetAnalyzers/ReflectionAnalyzers)
-   [xUnit.net](https://xunit.net/)

# License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info.
