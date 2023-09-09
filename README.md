[![GitHub last commit (master)](https://img.shields.io/github/last-commit/NetFabric/NetFabric.CodeAnalysis/master.svg?style=flat-square&logo=github)](https://github.com/NetFabric/NetFabric.CodeAnalysis/commits/master)
[![Build (master)](https://img.shields.io/github/workflow/status/NetFabric/NetFabric.CodeAnalysis/.NET%20Core/master.svg?style=flat-square&logo=github)](https://github.com/NetFabric/NetFabric.CodeAnalysis/actions)
[![Coverage](https://img.shields.io/coveralls/github/NetFabric/NetFabric.CodeAnalysis/master?style=flat-square&logo=coveralls)](https://coveralls.io/github/NetFabric/NetFabric.CodeAnalysis)

[![CodeAnalysis NuGet Version](https://img.shields.io/nuget/v/NetFabric.CodeAnalysis.svg?style=flat-square&label=CodeAnalysis%20nuget&logo=nuget)](https://www.nuget.org/packages/NetFabric.CodeAnalysis/)
[![CodeAnalysis NuGet Downloads](https://img.shields.io/nuget/dt/NetFabric.CodeAnalysis?style=flat-square&label=CodeAnalysis%20downloads&logo=nuget)](https://www.nuget.org/packages/NetFabric.CodeAnalysis/)

[![Reflection NuGet Version](https://img.shields.io/nuget/v/NetFabric.Reflection.svg?style=flat-square&label=Reflection%20nuget&logo=nuget)](https://www.nuget.org/packages/NetFabric.Reflection/)
[![Reflection NuGet Downloads](https://img.shields.io/nuget/dt/NetFabric.Reflection.svg?style=flat-square&label=Reflection%20downloads&logo=nuget)](https://www.nuget.org/packages/NetFabric.Reflection/)

# NetFabric.CodeAnalysis and NetFabric.Reflection

To find if a type is enumerable, it's not enough to check if it implements `IEnumerable`, `IEnumerable<>`, or `IAsyncEnumerable<>`. `foreach` and `await foreach` support several other cases as described [below](#sync-and-async-enumerables). This repository contains extension methods that take into account all these cases.

This repository is distributed as two separate NuGet packages:

-   **[NetFabric.CodeAnalysis](https://www.nuget.org/packages/NetFabric.CodeAnalysis/)** - it can be used when parsing C# code, for example, in the development of for [Roslyn Analyzers](https://docs.microsoft.com/en-us/visualstudio/extensibility/getting-started-with-roslyn-analyzers) or [C# Code Generators](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/). Its used by the analyzer package [NetFabric.Hyperlinq.Analyzer](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer) to implement rules for enumerables.

-   **[NetFabric.Reflection](https://www.nuget.org/packages/NetFabric.Reflection/)** - it can be used in the runtime, for example, to optimize performance by using [Expression Trees](https://tyrrrz.me/blog/expression-trees). Its used by the package [NetFabric.Assertive](https://github.com/NetFabric/NetFabric.Assertive) to unit test any type of enumerable.

# Sync and Async Enumerables

The code implemented in this repository can handle any of the following enumerable implementations:

## No enumerable interfaces

A collection, to be enumerated by a `foreach` loop, does not have to implement any interface. It just needs to have a parameterless `GetEnumerator()` method that returns a type that has a property `Current` with a getter and a parameterless `MoveNext()` method that returns `bool`.

The same applies to [async streams](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/generate-consume-asynchronous-stream) that, to be enumerated by an `await foreach` loop, don't need to implement any interface. These only need to have a `GetAsyncEnumerator()` method, with no parameters or with a `CancellationToken`parameter, that returns a type that has a property `Current` with a getter and a parameterless `MoveNextAsync()` method that returns `ValueTask<bool>`.

Here's the minimal implementations for both types of enumerables:

```csharp
public class Enumerable<T>
{
    public Enumerable<T> GetEnumerator()
        => this;

    public T Current
        => default;

    public bool MoveNext()
        => default;
}
```

```csharp
public class AsyncEnumerable<T>
{
    public AsyncEnumerable<T> GetAsyncEnumerator()
        => this;

    public T Current
        => default;

    public ValueTask<bool> MoveNextAsync()
        => default;
}
```

You can see in [SharpLab](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwAEAmARgFgAoagM2hgEMwALACgDdGoACAfR4BLPDzwwA7jwCieHAFsYURsAA2MADzCMAPjYBKPdR7GeRMgE42AIkA8G4BB9q3oDc1akQDMpktNkKlqjQAVbWoAbyMTDx95RWU1dWCeAHEYDBkYpQxofR4IkxMAXm0eDBZBAGcXGip8qMCeAGEcKFg8DFya/MLigBMYWkYcFQwqvNNPYAgIFR4AWQgOGAA5GAQMHLH8op4+gaGR6gBfIA) that `foreach` considers them as valid enumerables.

The previous implementations are equivalent to `Enumerable.Empty()`.

Here's the implementation that returns values. Implements a range collection, for sync and async enumerables, returning values from 0 to `count`:

```csharp
public readonly struct RangeEnumerable
{
    readonly int count;

    public RangeEnumerable(int count)
        => this.count = count;

    public Enumerator GetEnumerator()
        => new Enumerator(count);

    public struct Enumerator
    {
        readonly int count;
        int current;

        internal Enumerator(int count)
            => (this.count, current) = (count, -1);

        public readonly int Current
            => current;

        public bool MoveNext()
            => ++current < count;
    }
}
```

```csharp
public readonly struct RangeAsyncEnumerable
{
    readonly int count;

    public RangeAsyncEnumerable(int count)
        => this.count = count;

    public Enumerator GetAsyncEnumerator(CancellationToken token = default)
        => new Enumerator(count, token);

    public struct Enumerator
    {
        readonly int count;
        readonly CancellationToken token;
        int current;

        internal Enumerator(int count, CancellationToken token)
            => (this.count, this.token, current) = (count, token, -1);

        public readonly int Current => current;

        public ValueTask<bool> MoveNextAsync()
        {
            token.ThrowIfCancellationRequested();

            return new ValueTask<bool>(++current < count);
        }
    }
}
```

You can see in [SharpLab](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwAEAmARgFgAoagM2hgEMwALACgDdGoACASwwwAtvzw88MAO48ASozwBzGAFE8OITCiNgAGxhsSABgCUx6jws8iZAJxsBw4wG5q1IgGYesRgBMIeHQBPHgBnDFwwDFl5JVV1TW09agBvc0tvPwDgvjwoyHwMFypLHjSLD2jFFTUNLV19HLyIArNikssAXgA+HgwWPhCAOnzcng6eEcKy0rbyzzjaxgxoHgBxGAwFhOWoNmMZ9s6eiWktrR22Sedp6YqwiKizpehp1Nn2jP8g0SaCosOLI0JjgoLBcv8AdMSo1NHhGDoeE8LkCrlCAd0eGw+gNhs1cmhgaCYLl9uNLniMAS4GRru8Smi5l4mJlvkCAMIgsFRBntDFgTnEqZ0yw8irACAQBEAWQgHBgADkYAgMHsDgCShiANSa/lE0YAHgmFIhFgAvtRTUA) that `foreach` considers them as valid enumerables.

The advantage here is performance. The enumerator is a value type so the method calls are not virtual. The use of interfaces would box the enumerator and turn method calls into virtual. The enumerator is not disposable, so the `foreach` does not generate a `try`/`finally` clause, making it inlinable. The example also uses the C# 8 'struct read-only members' feature to avoid defensive copies.\_

_NOTE: In this case, `GetAsyncEnumerator()` has a `CancellationToken` parameter, which is also supported._

## Enumerable interfaces

Collections can implement `IEnumerable` and/or `IAsyncEnumerable<>`, or any interface derived from these, but the public `GetEnumerator()` and `GetAsyncEnumerator()` take precedence when enumerating using `foreach`.

```csharp
public readonly struct RangeEnumerable
    : IReadOnlyCollection<int>
{
    public RangeEnumerable(int count)
        => Count = count;

    public readonly int Count { get; }

    public readonly Enumerator GetEnumerator()
        => new(Count);
    readonly IEnumerator<int> IEnumerable<int>.GetEnumerator()
        => new DisposableEnumerator(Count);
    readonly IEnumerator IEnumerable.GetEnumerator()
        => new DisposableEnumerator(Count);

    public struct Enumerator
    {
        readonly int count;
        int current;

        internal Enumerator(int count)
            => (this.count, current) = (count, -1);

        public readonly int Current
            => current;

        public bool MoveNext()
            => ++current < count;
    }

    class DisposableEnumerator
        : IEnumerator<int>
    {
        readonly int count;
        int current;

        public DisposableEnumerator(int count)
            => (this.count, current) = (count, -1);

        public int Current => current;
        object IEnumerator.Current => current;

        public bool MoveNext()
            => ++current < count;

        public void Reset()
            => current = -1;

        public void Dispose()
        {}
    }
}
```

You can see in [SharpLab](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwAEAmARgFgAoIgBgAIiyA6AYQgBsOYwMBLCHgDOAbmp1GZFGKrUAZtBgBDMAAsAFADclUenwwwAtnrz08MAO70ASkrwBzGAFE8OQzChLg3dSVoBKf2p6EMkATnV9I38ZcQBmelglABNBDgBPeiEMXF4bO0cXNw8vbnpg0JB6AElrZWSAeTwM9i4efkEAHj48DAA+agBvUPKqEaIE2wdnV3dPbxhI3vpIfAwgsZHQgF4++nY1+m2ViDXYzZCJxPq0zJ6MfdPl4ccMEXoAXwqQ78YEpNSzUyRTmSgw0HoAHEYBgQSVwVB1P5RlsdntzBZ1AdejFfgDbjU4Z4Ed1ensmHFSf1mNDYbN4dAkSjUUd0ZZ6AARPhCAAOECEpRmxWJjOx6xkI3xQMJ9JFugpNJhRLBjORvxGuzM7K5vP5guVCKxT3Fv1+V2yuQeBugv0G6tCUoyJgeq16EpZ9xWOCgsDd9p+Fy29w8eCUHHo1sRntd639W016gwqm5zBjaC9PpgOKO9HUafocDIuMDIzjV0dd2WrG9voecY1ezANazbzjZYSwAgnHoAFkIJoYAA5GAIDBM+to+gAainTczy06JzOvy+gbNJE53L5AoWkeZqKq1UjVIGgbtJYdN2l0eN7tR0ebfovAZZf03up33EjSxdxo2r4bXMkxTfM51rZFjjzY100LYtX3bZ19kfB5NTAls7y2CBgAAK3aGVhRVKA2GQ1kM1rDDS2fN9O27PsB2HUdxyowCZzQhclyfeCqKuIgUBsGAhBhJiAMnNiUILMgKNCBDePfbdFjVKjBlXEZV1XIA=) that `foreach` uses the more efficient public `GetEnumerator()` and the value-type non-disposable `Enumerator`.

When casted to `IEnumerable<>`, the `IEnumerable<>.GetEnumerator()` and the reference-type `DisposableEnumerator` will be used. That's the case for all operations in `System.Linq` namespace.

An enumerable can contain only explicit interface implementations but these have to derive from `IEnumerable`:

```csharp
public class MyRange : IEnumerable<int>
{
    readonly int count;

    public MyRange(int count)
    {
        this.count = count;
    }

    IEnumerator<int> IEnumerable<int>.GetEnumerator() => new Enumerator(count);
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(count);

    class Enumerator : IEnumerator<int>
    {
        readonly int count;
        int current;

        internal Enumerator(int count)
        {
            this.count = count;
            current = -1;
        }

        int IEnumerator<int>.Current => current;
        object IEnumerator.Current => current;

        bool IEnumerator.MoveNext() => ++current < count;

        void IEnumerator.Reset() => current = -1;

        void IDisposable.Dispose() {}
    }
}
```

`foreach` only accepts interfaces derived from `IEnumerable` or `IAsyncEnumerable<>`. Using another interface results in an error, even if it implements the same contract. You can check the result in [SharpLab](https://sharplab.io/#v2:EYLgxg9gTgpgtADwGwBYA0AXEUCuA7AHwAEAmABgFgAoUgRmqLIAIjaA6AYQgBtuYwMASwh4AzgG4GzViklVqAM2gwAhmAAWACgBuKqE0EYYAWwN4meGAHcmAJRV4A5jACieHMZhQVwPptpkAJSB1ExhLLQAnJqGJoFyDADMZkZQCmowTACyAJ6siQA8EDgYTAAqAHxM1ADeoeEAkm4eXioY0AWVTADiMBjNnt7tUJrx1AC+1EksJHYOzgOtvpn1YSDZebSFgngYFbXh1VSHsCoAJiLcOSlMkPgYcoerLMn2Tq7ugz5+O6V3uyFjodwgBeKoYdSCURsf6lEG3Yq7BJAsJNT6tYYFX5VXL5LG7CpsXr9dFDaCjI7A0FVSw2RZkkawsYoynhUhMeltaCsqnrNEtBn4vbPOosk6qC54K43WGPKkynBQWBI55PMXhX5ePAqbgc0lcka/BH3QHy4FgpiaCFQmGIjBoW6K5UYQJMeGaWEOuC0Zlm1Ua3ZMflfTHYzhOmCB/1Ui1gCMq9VhCDAABW/FKwYx0HDSsjpWj5qqcdzCb9iaYwAgPCDnOGbCyEG0MAAcjAEBgKQXDhaANQ94vOpgFY2l+UFogoGv6uu2GCiPqd8vdovxuFMb1yqnjycNAAiUIADhBRN8YGx96Ij/PF2aapMWffxkA===).

## Disposable enumerator

The enumerator can be disposable and it's disposed at the end of the enumeration.

If the enumerator is a `class` or a `struct`, it has to implement the [`IDisposable`](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable) interface. If it's a `ref struct`, it only needs to have a public parameterless `Dispose` method.

## By-reference item return

The `Current` property can return the item by reference.

_NOTE: In this case, you should also be careful to declare the enumeration variable as `foreach (ref var item in source)` or `foreach (ref readonly var item in source)`. If you use `foreach (var item in source)`, no warning is shown and a copy of the item is made on each iteraton. You can use [NetFabric.CodeAnalysis.Analyzer](https://www.nuget.org/packages/NetFabric.CodeAnalysis.Analyzer/) to warn you of this case._

Here's a possible implementation of a sync enumerable:

```csharp
public readonly struct WhereEnumerable<T>
{
    readonly T[] source;
    readonly Func<T, bool> predicate;

    public WhereEnumerable(T[] source, Func<T, bool> predicate)
    {
        this.source = source;
        this.predicate = predicate;
    }

    public Enumerator GetEnumerator()
        => new Enumerator(this);

    public struct Enumerator
    {
        readonly T[] source;
        readonly Func<T, bool> predicate;
        int index;

        internal Enumerator(in WhereEnumerable<T> enumerable)
        {
            source = enumerable.source;
            predicate = enumerable.predicate;
            index = -1;
        }

        public readonly ref readonly T Current
            => ref source[index];

        public bool MoveNext()
        {
            while (++index < source.Length)
            {
                if (predicate(source[index]))
                    return true;
            }
            return false;
        }

    }
}
```

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
