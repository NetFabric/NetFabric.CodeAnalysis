using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetFabric.VisualBasic.UnitTests.TestData;

namespace NetFabric.CSharp.TestData
{
    public readonly struct EnumerableWithMissingGetEnumerator
    {
    }

    public readonly struct EnumerableWithMissingCurrent
    {
        public EnumeratorWithMissingCurrent GetEnumerator() 
            => new();
    }

    public readonly struct EnumerableWithMissingMoveNext<T>
    {
        public EnumeratorWithMissingMoveNext<T> GetEnumerator() 
            => new();
    }

    public readonly struct EnumerableWithValueTypeEnumerator<T>
    {
        readonly T[] source;

        public EnumerableWithValueTypeEnumerator(T[] source)
            => this.source = source; 
        
        public ValueTypeEnumerator<T> GetEnumerator() 
            => new(source);
    }

    public readonly struct EnumerableWithDisposableValueTypeEnumerator<T>
    {
        readonly T[] source;

        public EnumerableWithDisposableValueTypeEnumerator(T[] source)
            => this.source = source; 
        
        public DisposableValueTypeEnumerator<T> GetEnumerator() 
            => new(source);
    }

    public readonly struct EnumerableWithByRefLikeEnumerator<T>
    {
        readonly T[] source;

        public EnumerableWithByRefLikeEnumerator(T[] source)
            => this.source = source; 
        
        public ByRefLikeEnumerator<T> GetEnumerator() 
            => new(source);
    }

    public readonly struct EnumerableWithDisposableByRefLikeEnumerator<T>
    {
        readonly T[] source;

        public EnumerableWithDisposableByRefLikeEnumerator(T[] source)
            => this.source = source; 
        
        public DisposableByRefLikeEnumerator<T> GetEnumerator() 
            => new(source);
    }

    public readonly struct EnumerableWithReferenceTypeEnumerator<T>
    {
        readonly T[] source;

        public EnumerableWithReferenceTypeEnumerator(T[] source)
            => this.source = source; 
        
        public ReferenceTypeEnumerator<T> GetEnumerator() 
            => new(source);
    }

    public readonly struct EnumerableWithDisposableReferenceTypeEnumerator<T>
    {
        readonly T[] source;

        public EnumerableWithDisposableReferenceTypeEnumerator(T[] source)
            => this.source = source; 
        
        public DisposableReferenceTypeEnumerator<T> GetEnumerator() 
            => new(source);
    }

    public class HybridEnumerable<T> : IEnumerable<T>
    {
        readonly T[] source;

        public HybridEnumerable(T[] source)
            => this.source = source; 
        
        public ValueTypeEnumerator<T> GetEnumerator() 
            => new(source);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => new ExplicitGenericEnumerator<T>(source);
        IEnumerator IEnumerable.GetEnumerator() 
            => new ExplicitEnumerator<T>(source);
    }

    public class DerivedHybridEnumerable<T>
        : HybridEnumerable<T>
    {
        public DerivedHybridEnumerable(T[] source)
            : base(source)
        {}
    }

    
    public readonly struct HybridEnumerableWithExplicitEnumerator<T>
        : IEnumerable<T>
    {
        readonly T[] source;

        public HybridEnumerableWithExplicitEnumerator(T[] source)
            => this.source = source; 
        
        public ExplicitGenericEnumerator<T> GetEnumerator() 
            => new(source);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => new ExplicitGenericEnumerator<T>(source);
        IEnumerator IEnumerable.GetEnumerator() 
            => new ExplicitEnumerator<T>(source);
    }
    
    public class ReadOnlyCollectionEnumerable<T>
        : IReadOnlyCollection<T>
    {
        readonly T[] source;

        public ReadOnlyCollectionEnumerable(T[] source)
            => this.source = source;

        public int Count
            => source.Length;
        
        public ValueTypeEnumerator<T> GetEnumerator() 
            => new(source);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => new ExplicitGenericEnumerator<T>(source);
        IEnumerator IEnumerable.GetEnumerator() 
            => new ExplicitEnumerator<T>(source);
    }

    public class ExplicitGenericEnumerable<T> : IEnumerable<T>
    {
        readonly T[] source;

        public ExplicitGenericEnumerable(T[] source)
            => this.source = source; 
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => new ExplicitGenericEnumerator<T>(source);
        IEnumerator IEnumerable.GetEnumerator() 
            => new ExplicitEnumerator<T>(source);
    }

    public class ExplicitEnumerable<T> : IEnumerable
    {
        readonly T[] source;

        public ExplicitEnumerable(T[] source)
            => this.source = source; 
        
        IEnumerator IEnumerable.GetEnumerator() 
            => new ExplicitEnumerator<T>(source);
    }

    static class ValidateEnumerables
    {
        public static void InvalidEnumerables()
        {
            // foreach (var _ in new MissingGetEnumeratorEnumerable()) { }
            // foreach (var _ in new MissingCurrentEnumerable()) { }
            // foreach (var _ in new MissingMoveNextEnumerable<int>()) { }
            // foreach (var _ in new HybridEnumerableWithExplicitEnumerator<int>(Array.Empty<int>())) { }
        }

        public static void ValidEnumerables()
        {
            foreach (var _ in new EnumerableWithValueTypeEnumerator<int>(Array.Empty<int>())) { }
            foreach (var _ in new EnumerableWithDisposableValueTypeEnumerator<int>(Array.Empty<int>())) { }
            foreach (var _ in new EnumerableWithByRefLikeEnumerator<int>(Array.Empty<int>())) { }
            foreach (var _ in new EnumerableWithDisposableByRefLikeEnumerator<int>(Array.Empty<int>())) { }
            foreach (var _ in new EnumerableWithReferenceTypeEnumerator<int>(Array.Empty<int>())) { }
            foreach (var _ in new EnumerableWithDisposableReferenceTypeEnumerator<int>(Array.Empty<int>())) { }
            foreach (var _ in new HybridEnumerable<int>(Array.Empty<int>())) { }
            foreach (var _ in new ExplicitEnumerable<int>(Array.Empty<int>())) { }
            foreach (var _ in new ExplicitGenericEnumerable<int>(Array.Empty<int>())) { }
            foreach (var _ in new RangeEnumerable()) { }
            foreach (var _ in new MappedEnumerable<int>()) { }
            foreach (var _ in new DerivedHybridEnumerable<int>(Array.Empty<int>())) { }
            foreach (var _ in new ReadOnlyCollectionEnumerable<int>(Array.Empty<int>())) { }
        }
    }
}