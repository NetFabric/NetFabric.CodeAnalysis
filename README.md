# NetFabric.CodeAnalysis
Extension methods for Roslyn and reflection.

## Enumerables

This project implements methods that detect if a type is an enumerable, both for Roslyn and reflection.

It can handle any of the following enumerable implementations:

### No enumerable interfaces

A collection, to be enumerated by a `foreach` loop, does not have to implement any interface. It just needs to have a parameterless `GetEnumerator()` method that returns a type that has a property `Current` with a getter and a parameterless `MoveNext()` method that returns `bool`.

The same applies to [async streams](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/generate-consume-asynchronous-stream) that, to be enumerated by an `await foreach` loop, they also don't have to implement any interface. They just needs to have a parameterless `GetAsyncEnumerator()` method that returns a type that has a property `Current` with a getter and a parameterless `MoveNextAsync()` method that returns `ValueTask<bool>`.

Here's the minimal implementations for both types of enumerables:

``` csharp
public class EmptyEnumerable<T>
{
    public EmptyEnumerable<T> GetEnumerator() 
        => this;

    public T Current 
        => default;

    public bool MoveNext() 
        => false;
}
```

``` csharp
public class EmptyAsyncEnumerable<T>
{
    public EmptyAsyncEnumerable<T> GetAsyncEnumerator() 
        => this;

    public T Current 
        => default;

    public ValueTask<bool> MoveNextAsync() 
        => new ValueTask<bool>(Task.FromResult(false));
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

readonly struct RangeAsyncEnumerable
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

            return new ValueTask<bool>(Task.FromResult(++current < count));
        }
    }
}
```

_NOTE: The advantage here is performance. The enumerator is a value type so the method calls are not virtual. The use of interfaces would box the enumerator and turn method calls into virtual. The enumerator is not disposable, so the `foreach` does not generate a `try`/`finally` clause, making it inlinable. The example also uses the C# 8 'struct read-only members' feature to avoid defensive copies._

In this case, `GetAsyncEnumerator()` has a `CancellationToken` parameters which is also supported.

### Enumerable interfaces

Collections can have multiple forms of enumeration. For example; a collection that implements `IReadOnlyList<T>` can be enumerated using the indexer, using `IEnumerable<T>.GetEnumerator()`, using `IEnumerable.GetEnumerator()` and using a public `GetEnumerator()` that is not an override of any of these interfaces. There's no guarantee that they all are correctly implemented. The `Count` property can also return the wrong value.

Here's an example of a collection with multiple possible enumerations and enumerator implementations:

``` csharp
public readonly struct MyRange : IReadOnlyList<int>
{    
    public MyRange(int count)
    {
        Count = count;
    }
    
    public readonly int Count { get; }
    
    public int this[int index]
    {
    	get
        {
            if (index < 0 || index >= Count)
                ThrowIndexOutOfRangeException();

            return index;

            static void ThrowIndexOutOfRangeException() => throw new IndexOutOfRangeException();
        }
    }
    
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
        
        internal DisposableEnumerator(int count)
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

_NOTE: The indexer uses a local function so that the accessor does not throw an exception, making it inlinable. The local function does not add dependencies to the example... ;)_

This example has two enumerators so that it can take advantage of the performance features described above and also; be casted to an interface and take advantage of extension methods for collections (like LINQ). It can also be enumerated using the indexer.

### Custom enumerable interfaces

Custom enumerable interfaces can add even more possible enumerations, that would increase the number of tests required.

Here's an interface that restricts the enumerator to a value type and adds a `GetEnumerator()` that explicitly returns the enumerator type. This allows the use of an enumerable interface without boxing the enumerator:

``` csharp
public interface IValueEnumerable<T, TEnumerator>
    : IEnumerable<T>
    where TEnumerator : struct, IEnumerator<T>
{
    new TEnumerator GetEnumerator();
}
```

Here's a possible implementation:

``` csharp
public readonly struct MyRange 
    : IValueEnumerable<int, MyRange.DisposableEnumerator>
{    
    readonly int count;
    
    public MyRange(int count)
    {
        this.count = count;
    }
        
    public readonly Enumerator GetEnumerator() => new Enumerator(count);
    readonly DisposableEnumerator IValueEnumerable<int, DisposableEnumerator>.GetEnumerator() => new DisposableEnumerator(count);
    readonly IEnumerator<int> IEnumerable<int>.GetEnumerator() => new DisposableEnumerator(count);
    readonly IEnumerator IEnumerable.GetEnumerator() => new DisposableEnumerator(count);
    
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
    
    public struct DisposableEnumerator : IEnumerator<int>
    {
        readonly int count;
        int current;
        
        internal DisposableEnumerator(int count)
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

### Explicit interface implementations

Enumerables usually implement interfaces by overriding them using public implementations, or using a mix but, an enumerable can be implemented using only explicit interface implementations:

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

### By reference return

The `Current` property can return the item by reference. 

_NOTE: In this case, you should also be careful to declare the enumeration variable as `foreach (ref var item in source)` or `foreach (ref readonly var item in source)`. If you use `foreach (var item in source)`, no warning is shown and a copy of the item is made on each iteraton. You can use [NetFabric.Hyperlinq.Analyzer](https://www.nuget.org/packages/NetFabric.Hyperlinq.Analyzer/) to warn you of this case._

Here's possible implementation of a sync enumerable:

``` csharp
readonly struct WhereEnumerable<T>
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

- [Enumeration in .NET](https://blog.usejournal.com/enumeration-in-net-d5674921512e) by Ant�o Almada

## Credits

The following open-source projects are used to build and test this project:

- [.NET](https://github.com/dotnet)
- [NetFabric.Assertive](https://github.com/NetFabric/NetFabric.Assertive)
- [xUnit.net](https://xunit.net/)

## License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info.

