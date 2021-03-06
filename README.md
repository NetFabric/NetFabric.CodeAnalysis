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
- **[NetFabric.CodeAnalysis](https://www.nuget.org/packages/NetFabric.CodeAnalysis/)** - it can be used when parsing C# code, for example, in the development of for [Roslyn Analyzers](https://docs.microsoft.com/en-us/visualstudio/extensibility/getting-started-with-roslyn-analyzers) or [C# Code Generators](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/). Its used by the analyzer package [NetFabric.Hyperlinq.Analyzer](https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer) to implement rules for enumerables.
- **[NetFabric.Reflection](https://www.nuget.org/packages/NetFabric.Reflection/)** - it can be used in the runtime, for example, to optimize performance by using [Expression Trees](https://tyrrrz.me/blog/expression-trees). Its used by the package [NetFabric.Assertive](https://github.com/NetFabric/NetFabric.Assertive) to unit test any type of enumerable.

# Usage

## IsEnumerable()

- Add either [NetFabric.CodeAnalysis](https://www.nuget.org/packages/NetFabric.CodeAnalysis/) or [NetFabric.Reflection](https://www.nuget.org/packages/NetFabric.Reflection/) package to your project.
- Use the `IsEnumerable` method as follow:
``` csharp
using NetFabric.CodeAnalysis;

var isEnumerable = typeSymbol.IsEnumerable(compilation, out var enumerableSymbols);

var isAsyncEnumerable = typeSymbol.IsAsyncEnumerable(compilation, out var asyncEnumerableSymbols);
```

``` csharp
using NetFabric.Reflection;

var isEnumerable = type.IsEnumerable(out var enumerableInfo);

var isAsyncEnumerable = type.IsAsyncEnumerable(out var asyncEnumerableInfo);
```

The methods return a boolean value indicating if it's a valid enumerable or enumerator. 

If `true`, the output parameter contains [`MethodInfo`](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo) or [`IMethodSymbol`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.imethodsymbol) for the method `GetEnumerator`/`GetAsynEnumerator` of the enumerable, the property `Current` and the method `MoveNext`/`MoveNextAsync` of the enumerator, following the precedences used by Roslyn for the `foreach` and `await foreach` keywords. It may also contain for methods `Reset` and `Dispose`/`DisposeAsync` if defined.

## ExpressionEx

[NetFabric.Reflection](https://www.nuget.org/packages/NetFabric.Reflection/) contains high level `Expression` generators that makes it easier to handle enumerables in [Expression Trees](https://tyrrrz.me/blog/expression-trees). The code generated is as similar as possible to the one generated by Roslyn for the equivalent keywords.

To use these, add the [NetFabric.Reflection](https://www.nuget.org/packages/NetFabric.Reflection/) package to your project.

### ExpressionEx.ForEach

``` csharp
public static Expression ForEach(Expression enumerable, Func<Expression, Expression> body)
```

- `enumerable` - Defines an enumerable.
- `body` - Defines the body containing the code performed for each item. Pass a lambda expression that, given an `Expression` that defines an item, returns an `Expression` that uses it.

**WARNING:** Async enumerables are not supported.

The `Expression` generated depends on:

- Whether the enumerator is an `interface`, `class`, `struct`, or `ref struct`.
- Whether the enumerator is disposable or not.
- Whether the enumerable is an array. In this case, it uses the array indexer instead of `IEnumerable<>` to enumerate.

Throws an exception if the `Expression` in the first parameter does not define an enumerable. In case you don't want the exception to be thrown, use the other overload that takes an `EnumerableInfo` or `EnumerableSymbols` for the first parameter. Use `IsEnumerable` to get the required values.

Here's an example, using `ExpressionEx.ForEach`, that calculates the sum of the items in an enumerable:

``` csharp
using static NetFabric.Expressions.ExpressionEx;
using static System.Linq.Expressions.Expression;

int Sum<TEnumerable>(TEnumerable enumerable)
{
    var enumerableParameter = Parameter(typeof(TEnumerable), "enumerable");
    var sumVariable = Variable(typeof(int), "sum");
    var expression = Block(
        new[] {sumVariable},
        Assign(sumVariable, Constant(0)),
        ForEach(
            enumerableParameter,
            item => AddAssign(sumVariable, item)),
        sumVariable);
    var sum = Lambda<Func<TEnumerable, int>>(expression, enumerableParameter).Compile();

    return sum(enumerable);
}
```


### ExpressionEx.For

``` csharp
public static Expression For(Expression initialization, Expression condition, Expression iterator, Expression body) 
```

- `initialization` - Defines the initialization. Performed before starting the loop iteration.
- `condition` - Defines the condition. Performed before each loop iteration.
- `iterator` - Defines the iterator. Performed after each loop iteration.
- `body` - Defines the body. Performed in each loop iteration. 

`ExpressionEx.For` does not declare the iteration variable. You may have to declare it using an `Expression.Block`.

Here's an example, using `ExpressionEx.For`, that calculates the sum of the items in an array:

``` csharp
using static NetFabric.Expressions.ExpressionEx;
using static System.Linq.Expressions.Expression;

int Sum(int[] array, int start, int end)
{
    var arrayParameter = Parameter(typeof(int[]), "array");
    var startParameter = Parameter(typeof(int), "start");
    var endParameter = Parameter(typeof(int), "end");
    var indexVariable = Variable(typeof(int), "index");
    var sumVariable = Variable(typeof(int), "sum");
    var expression = Block(
        new[] { indexVariable, sumVariable },
        Assign(sumVariable, Constant(0)),
        For(
            Assign(indexVariable, startParameter), 
            LessThan(indexVariable, endParameter), 
            PostIncrementAssign(indexVariable),
            AddAssign(sumVariable, ArrayIndex(arrayParameter, indexVariable))),
        sumVariable);
    var sum = Lambda<Func<int[], int, int, int>>(expression, arrayParameter, startParameter, endParameter).Compile();

    return sum(array, start, end);
}
```

### ExpressionEx.While

``` csharp
public static LoopExpression While(Expression condition, Expression body) 
```

- `condition` - Defines the condition. Performed before each loop iteration.
- `body` - Defines the body. Performed in each loop iteration.

Here's an example, using `ExpressionEx.While`, that calculates the sum of the items in an array:

``` csharp
using static NetFabric.Expressions.ExpressionEx;
using static System.Linq.Expressions.Expression;

int Sum(int[] array, int start, int end)
{
    var valueParameter = Parameter(typeof(int[]), "value");
    var startParameter = Parameter(typeof(int), "start");
    var endParameter = Parameter(typeof(int), "end");
    var sumVariable = Variable(typeof(int), "sum");
    var indexVariable = Variable(typeof(int), "index");
    var expression = Block(
        new[] { indexVariable, sumVariable },
        Assign(sumVariable, Constant(0)),
        Assign(indexVariable, startParameter),
        While(
            LessThan(indexVariable, endParameter), 
            Block(
                AddAssign(sumVariable, ArrayIndex(valueParameter, indexVariable)),
                PostIncrementAssign(indexVariable)
            )
        ),
        sumVariable);
    var sum = Lambda<Func<int[], int, int, int>>(expression, valueParameter, startParameter, endParameter).Compile();

    return sum(array, start, end);
}
```

### ExpressionEx.Using

``` csharp
public static TryExpression Using(ParameterExpression instance, Expression body) 
```

- `instance` - Defines the variable to be disposed.
- `body` - Defines the body after which the variable is disposed.

Throws and exception if the variable is not disposable. To be considered disposable, if it's is a `class` or a `struct`, it has to implement the [`IDisposable`](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable) interface. If it's a `ref struct`, it only needs to have a public parameterless `Dispose`.

`ExpressionEx.Using` does not declare the iteration variable. You may have to declare it using an `Expression.Block`.

**WARNING:** `IAsyncDisposable` is not supported.

Here's an example, using `ExpressionEx.Using`, that calculates the sum of the items in an enumerable:

``` csharp
using static NetFabric.Expressions.ExpressionEx;
using static System.Linq.Expressions.Expression;

int Sum<TEnumerable>(TEnumerable enumerable)
{
    if (!typeof(TEnumerable).IsEnumerable(out var enumerableInfo))
        throw new Exception("Not an enumerable!");
    
    var enumerableParameter = Parameter(typeof(TEnumerable), "enumerable");
    var enumeratorVariable = Variable(enumerableInfo.GetEnumerator.ReturnType, "enumerator");
    var sumVariable = Variable(typeof(int), "sum");
    var expression = Block(
        new[] {enumeratorVariable, sumVariable},
        Assign(enumeratorVariable, Call(enumerableParameter, enumerableInfo.GetEnumerator)),
        Assign(sumVariable, Constant(0)),
        Using(
            enumeratorVariable,
            While(
                Call(enumeratorVariable, enumerableInfo.EnumeratorInfo.MoveNext),
                AddAssign(sumVariable, Call(enumeratorVariable, enumerableInfo.EnumeratorInfo.GetCurrent))
            )
        ),
        sumVariable);
    var sum = Lambda<Func<TEnumerable, int>>(expression, enumerableParameter).Compile();
    
    return sum(enumerable);
}
```

# Sync and Async Enumerables

The code implemented in this repository can handle any of the following enumerable implementations:

## No enumerable interfaces

A collection, to be enumerated by a `foreach` loop, does not have to implement any interface. It just needs to have a parameterless `GetEnumerator()` method that returns a type that has a property `Current` with a getter and a parameterless `MoveNext()` method that returns `bool`.

The same applies to [async streams](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/generate-consume-asynchronous-stream) that, to be enumerated by an `await foreach` loop, don't need to implement any interface. These only need to have a `GetAsyncEnumerator()` method, with no parameters or with a `CancellationToken`parameter, that returns a type that has a property `Current` with a getter and a parameterless `MoveNextAsync()` method that returns `ValueTask<bool>`.

Here's the minimal implementations for both types of enumerables:

``` csharp
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

``` csharp
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

``` csharp
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

``` csharp
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

The advantage here is performance. The enumerator is a value type so the method calls are not virtual. The use of interfaces would box the enumerator and turn method calls into virtual. The enumerator is not disposable, so the `foreach` does not generate a `try`/`finally` clause, making it inlinable. The example also uses the C# 8 'struct read-only members' feature to avoid defensive copies._

_NOTE: In this case, `GetAsyncEnumerator()` has a `CancellationToken` parameter, which is also supported._

## Enumerable interfaces

Collections can implement `IEnumerable` and/or `IAsyncEnumerable<>`, or any interface derived from these, but the public `GetEnumerator()` and `GetAsyncEnumerator()` take precedence when enumerating using `foreach`.

``` csharp
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

``` csharp
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

``` csharp
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

- [Enumeration in .NET](https://blog.usejournal.com/enumeration-in-net-d5674921512e) by Antão Almada

# Credits

The following open-source projects are used to build and test this project:

- [.NET](https://github.com/dotnet)
- [coveralls](https://coveralls.io)
- [coverlet](https://github.com/tonerdo/coverlet)
- [IsExternalInit](https://github.com/manuelroemer/IsExternalInit)
- [Nullable](https://github.com/manuelroemer/Nullable)
- [ReadableExpressions](https://github.com/agileobjects/ReadableExpressions)
- [ReflectionAnalyzers](https://github.com/DotNetAnalyzers/ReflectionAnalyzers)
- [xUnit.net](https://xunit.net/)

# License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info.

