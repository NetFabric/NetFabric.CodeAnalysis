[![GitHub last commit (master)](https://img.shields.io/github/last-commit/NetFabric/NetFabric.CodeAnalysis/master.svg?style=flat-square&logo=github)](https://github.com/NetFabric/NetFabric.CodeAnalysis/commits/master)
[![Build (master)](https://img.shields.io/github/workflow/status/NetFabric/NetFabric.CodeAnalysis/.NET%20Core/master.svg?style=flat-square&logo=github)](https://github.com/NetFabric/NetFabric.CodeAnalysis/actions)
[![Coverage](https://img.shields.io/coveralls/github/NetFabric/NetFabric.CodeAnalysis/master?style=flat-square&logo=coveralls)](https://coveralls.io/github/NetFabric/NetFabric.CodeAnalysis)
[![Gitter](https://img.shields.io/gitter/room/netfabric/netfabric.hyperlinq?style=flat-square&logo=gitter)](https://gitter.im/NetFabric/NetFabric.CodeAnalysis)

[![CodeAnalysis NuGet Version](https://img.shields.io/nuget/v/NetFabric.CodeAnalysis.svg?style=flat-square&label=CodeAnalysis%20nuget&logo=nuget)](https://www.nuget.org/packages/NetFabric.CodeAnalysis/)
[![CodeAnalysis NuGet Downloads](https://img.shields.io/nuget/dt/NetFabric.CodeAnalysis?style=flat-square&label=CodeAnalysis%20downloads&logo=nuget)](https://www.nuget.org/packages/NetFabric.CodeAnalysis/) 

[![Reflection NuGet Version](https://img.shields.io/nuget/v/NetFabric.Reflection.svg?style=flat-square&label=Reflection%20nuget&logo=nuget)](https://www.nuget.org/packages/NetFabric.Reflection/)
[![Reflection NuGet Downloads](https://img.shields.io/nuget/dt/NetFabric.Reflection.svg?style=flat-square&label=Reflection%20downloads&logo=nuget)](https://www.nuget.org/packages/NetFabric.Reflection/) 

# NetFabric.CodeAnalysis

Extensions to `System.CodeAnalysis` (Roslyn) and `System.Reflection`.

## Sync and Async Enumerables

This project implements methods that validate if a type is an enumerable or async enumerable, both for `CodeAnalysis` and `Reflection`.

It can handle any of the following enumerable implementations:

### No enumerable interfaces

A collection, to be enumerated by a `foreach` loop, does not have to implement any interface. It just needs to have a parameterless `GetEnumerator()` method that returns a type that has a property `Current` with a getter and a parameterless `MoveNext()` method that returns `bool`.

The same applies to [async streams](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/generate-consume-asynchronous-stream) that, to be enumerated by an `await foreach` loop, also don't have to implement any interface. They just need to have a parameterless `GetAsyncEnumerator()` method that returns a type that has a property `Current` with a getter and a parameterless `MoveNextAsync()` method that returns `ValueTask<bool>`.

Here's the minimal implementations for both types of enumerables:

``` csharp
public class Enumerable<T>
{
    public Enumerable<T> GetEnumerator() 
        => this;

    public T Current 
        => default;

    public bool MoveNext() 
        => false;
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
        => new ValueTask<bool>(false);
}
```

Here's the implementation of a range collection, for sync and async enumerables, returning values from 0 to `count`:

``` csharp
public readonly struct RangeEnumerable
{
    readonly int count;
    
    public RangeEnumerable(int count)
    {
        this.count = count;
    }
    
    public readonly Enumerator GetEnumerator() 
        => new Enumerator(count);
    
    public struct Enumerator
    {
        readonly int count;
        int current;
        
        internal Enumerator(int count)
        {
            this.count = count;
            current = -1;
        }
        
        public readonly int Current => current;
        
        public bool MoveNext() => ++current < count;
    }
}
```

``` csharp
public readonly struct RangeAsyncEnumerable
{
    readonly int count;

    public RangeAsyncEnumerable(int count)
    {
        this.count = count;
    }

    public Enumerator GetAsyncEnumerator(CancellationToken token = default)
        => new Enumerator(count, token);

    public struct Enumerator 
    {
        readonly int count;
        readonly CancellationToken token;
        int current;

        internal Enumerator(int count, CancellationToken token)
        {
            this.count = count;
            this.token = token;
            current = -1;
        }

        public readonly int Current => current;

        public ValueTask<bool> MoveNextAsync()
        {
            token.ThrowIfCancellationRequested();

            return new ValueTask<bool>(++current < count);
        }
    }
}
```

_NOTE: The advantage here is performance. The enumerator is a value type so the method calls are not virtual. The use of interfaces would box the enumerator and turn method calls into virtual. The enumerator is not disposable, so the `foreach` does not generate a `try`/`finally` clause, making it inlinable. The example also uses the C# 8 'struct read-only members' feature to avoid defensive copies._

_NOTE: In this case, `GetAsyncEnumerator()` has a `CancellationToken` parameter, which is also supported._

### Enumerable interfaces

Collections can implement `IEnumerable` and/or `IAsyncEnumerable<>`, or any interface derived from these, but the public `GetEnumerator()` and `GetAsyncEnumerator()` will take precedence.

``` csharp
public readonly struct MyRange : IReadOnlyCollection<int>
{    
    public MyRange(int count)
    {
        Count = count;
    }
    
    public readonly int Count { get; }
    
    public readonly Enumerator GetEnumerator() => new Enumerator(Count);
    readonly IEnumerator<int> IEnumerable<int>.GetEnumerator() => new DisposableEnumerator(Count);
    readonly IEnumerator IEnumerable.GetEnumerator() => new DisposableEnumerator(Count);
    
    public struct Enumerator
    {
        readonly int count;
        int current;
        
        internal Enumerator(int count)
        {
            this.count = count;
            current = -1;
        }
        
        public readonly int Current => current;
        
        public bool MoveNext() => ++current < count;
    }
    
    class DisposableEnumerator : IEnumerator<int>
    {
        readonly int count;
        int current;
        
        public DisposableEnumerator(int count)
        {
            this.count = count;
            current = -1;
        }
        
        public int Current => current;
        object IEnumerator.Current => current;
        
        public bool MoveNext() => ++current < count;
        
        public void Reset() => current = -1;
        
        public void Dispose() {}
    }
}
```

A `foreach` loop will use the more efficient public `GetEnumerator()` and the value-type non-disposable `Enumerator`. But, when casted to `IEnumerable<>`, the `IEnumerable<>.GetEnumerator()` and the reference-type `DisposableEnumerator` will be used.

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

Using the following enumerable on a `foreach` will result in the error: `error CS1579: foreach statement cannot operate on variables of type 'MyRange' because 'MyRange' does not contain a public instance definition for 'GetEnumerator'`.

``` csharp
public interface MyIEnumerable<out T> 
{
    IEnumerator<T> GetEnumerator();
}

public class MyRange : MyIEnumerable<int>
{    
    readonly int count;
    
    public MyRange(int count)
    {
        this.count = count;
    }

    IEnumerator<int> MyIEnumerable<int>.GetEnumerator() => new Enumerator(count);
    
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

### By-reference item return

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
    
    public Enumerator GetEnumerator() => new Enumerator(this);
    
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
        
        public readonly ref readonly T Current => ref source[index];
        
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

## References

- [Enumeration in .NET](https://blog.usejournal.com/enumeration-in-net-d5674921512e) by Antão Almada

## Credits

The following open-source projects are used to build and test this project:

- [.NET](https://github.com/dotnet)
- [xUnit.net](https://xunit.net/)

## License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info.

