using NetFabric.VisualBasic.UnitTests.TestData;
using System;
using System.Collections;
using System.Collections.Generic;
using NetFabric.Reflection;
using Xunit;

namespace NetFabric.CSharp.TestData
{
    public static partial class DataSets
    {
        public static TheoryData<string, Type> InstanceProperties =>
            new()
            {
                { "Property", typeof(int) },
                { "InheritedProperty", typeof(int) },
            };

        public static TheoryData<string> ExplicitInstanceProperties =>
            new()
            {
                "ExplicitProperty",
                "StaticProperty",
            };

        public static TheoryData<string, Type[]> InstanceMethods =>
            new()
            {
                { "Method", Array.Empty<Type>() },
                { "Method", new[] { typeof(int), typeof(string) } },
                { "InheritedMethod", Array.Empty<Type>() },
                { "InheritedMethod", new[] { typeof(int), typeof(string) } },
            };

        public static TheoryData<string, Type[]> ExplicitInstanceMethods =>
            new()
            {
                { "ExplicitMethod", Array.Empty<Type>() },
                { "ExplicitMethod", new[] { typeof(int), typeof(string) } },
                { "StaticMethod", Array.Empty<Type>() },
                { "StaticMethod", new[] { typeof(int), typeof(string) } },
            };

        public class EnumerableTestData 
        {
            public bool ForEachUsesIndexer;
            public Type GetEnumeratorDeclaringType;
            public EnumeratorTestData EnumeratorTestData;
        }

        public class EnumeratorTestData 
        {
            public Type CurrentDeclaringType;
            public Type MoveNextDeclaringType;
            public Type? ResetDeclaringType;
            public Type? DisposeDeclaringType;
            public Type ItemType;
            public bool IsValueType;
            public bool IsByRefLikeType;
        }

        public static TheoryData<Type, EnumerableTestData> Arrays =>
            new()
            {
                {
                    typeof(int[]),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = true,
                        GetEnumeratorDeclaringType = typeof(Array),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(IEnumerator),
                            MoveNextDeclaringType = typeof(IEnumerator),
                            ResetDeclaringType = typeof(IEnumerator),
                            DisposeDeclaringType = null,
                            ItemType = typeof(object),
                            IsValueType = false,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(Span<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = true,
                        GetEnumeratorDeclaringType = typeof(Span<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(Span<int>.Enumerator),
                            MoveNextDeclaringType = typeof(Span<int>.Enumerator),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = null,
                            ItemType = typeof(int).MakeByRefType(),
                            IsValueType = true,
                            IsByRefLikeType = true
                        }
                    }
                },
                {
                    typeof(ReadOnlySpan<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = true,
                        GetEnumeratorDeclaringType = typeof(ReadOnlySpan<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(ReadOnlySpan<int>.Enumerator),
                            MoveNextDeclaringType = typeof(ReadOnlySpan<int>.Enumerator),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = null,
                            ItemType = typeof(int).MakeByRefType(),
                            IsValueType = true,
                            IsByRefLikeType = true
                        }
                    }
                },
            };

        public static TheoryData<Type, EnumerableTestData> Enumerables =>
            new()
            {
                {
                    typeof(IEnumerable),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(IEnumerable),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(IEnumerator),
                            MoveNextDeclaringType = typeof(IEnumerator),
                            ResetDeclaringType = typeof(IEnumerator),
                            DisposeDeclaringType = null,
                            ItemType = typeof(object),
                            IsValueType = false,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(IEnumerable<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(IEnumerable<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(IEnumerator<int>),
                            MoveNextDeclaringType = typeof(IEnumerator),
                            ResetDeclaringType = typeof(IEnumerator),
                            DisposeDeclaringType = typeof(IDisposable),
                            ItemType = typeof(int),
                            IsValueType = false,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(IReadOnlyCollection<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(IEnumerable<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(IEnumerator<int>),
                            MoveNextDeclaringType = typeof(IEnumerator),
                            ResetDeclaringType = typeof(IEnumerator),
                            DisposeDeclaringType = typeof(IDisposable),
                            ItemType = typeof(int),
                            IsValueType = false,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(EnumerableWithValueTypeEnumerator<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(EnumerableWithValueTypeEnumerator<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(ValueTypeEnumerator<int>),
                            MoveNextDeclaringType = typeof(ValueTypeEnumerator<int>),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = null,
                            ItemType = typeof(int),
                            IsValueType = true,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(EnumerableWithDisposableValueTypeEnumerator<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(EnumerableWithDisposableValueTypeEnumerator<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(DisposableValueTypeEnumerator<int>),
                            MoveNextDeclaringType = typeof(DisposableValueTypeEnumerator<int>),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = typeof(IDisposable),
                            ItemType = typeof(int),
                            IsValueType = true,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(EnumerableWithByRefLikeEnumerator<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(EnumerableWithByRefLikeEnumerator<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(ByRefLikeEnumerator<int>),
                            MoveNextDeclaringType = typeof(ByRefLikeEnumerator<int>),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = null,
                            ItemType = typeof(int),
                            IsValueType = true,
                            IsByRefLikeType = true
                        }
                    }
                },
                {
                    typeof(EnumerableWithDisposableByRefLikeEnumerator<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(EnumerableWithDisposableByRefLikeEnumerator<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(DisposableByRefLikeEnumerator<int>),
                            MoveNextDeclaringType = typeof(DisposableByRefLikeEnumerator<int>),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = typeof(DisposableByRefLikeEnumerator<int>),
                            ItemType = typeof(int),
                            IsValueType = true,
                            IsByRefLikeType = true
                        }
                    }
                },
                {
                    typeof(EnumerableWithReferenceTypeEnumerator<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(EnumerableWithReferenceTypeEnumerator<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(ReferenceTypeEnumerator<int>),
                            MoveNextDeclaringType = typeof(ReferenceTypeEnumerator<int>),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = null,
                            ItemType = typeof(int),
                            IsValueType = false,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(EnumerableWithDisposableReferenceTypeEnumerator<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(EnumerableWithDisposableReferenceTypeEnumerator<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(DisposableReferenceTypeEnumerator<int>),
                            MoveNextDeclaringType = typeof(DisposableReferenceTypeEnumerator<int>),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = typeof(IDisposable),
                            ItemType = typeof(int),
                            IsValueType = false,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(HybridEnumerable<int>),
                    new EnumerableTestData {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(HybridEnumerable<int>),
                        EnumeratorTestData = new EnumeratorTestData {
                            CurrentDeclaringType = typeof(ValueTypeEnumerator<int>),
                            MoveNextDeclaringType = typeof(ValueTypeEnumerator<int>),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = null,
                            ItemType = typeof(int),
                            IsValueType = true,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(ExplicitEnumerable<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(IEnumerable),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(IEnumerator),
                            MoveNextDeclaringType = typeof(IEnumerator),
                            ResetDeclaringType = typeof(IEnumerator),
                            DisposeDeclaringType = null,
                            ItemType = typeof(object),
                            IsValueType = false,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(ExplicitGenericEnumerable<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(IEnumerable<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(IEnumerator<int>),
                            MoveNextDeclaringType = typeof(IEnumerator),
                            ResetDeclaringType = typeof(IEnumerator),
                            DisposeDeclaringType = typeof(IDisposable),
                            ItemType = typeof(int),
                            IsValueType = false,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(RangeEnumerable),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(RangeEnumerable),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(RangeEnumerable.Enumerator),
                            MoveNextDeclaringType = typeof(RangeEnumerable.Enumerator),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = null,
                            ItemType = typeof(int),
                            IsValueType = true,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(DerivedHybridEnumerable<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(HybridEnumerable<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(ValueTypeEnumerator<int>),
                            MoveNextDeclaringType = typeof(ValueTypeEnumerator<int>),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = null,
                            ItemType = typeof(int),
                            IsValueType = true,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(ReadOnlyCollectionEnumerable<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(ReadOnlyCollectionEnumerable<int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(ValueTypeEnumerator<int>),
                            MoveNextDeclaringType = typeof(ValueTypeEnumerator<int>),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = null,
                            ItemType = typeof(int),
                            IsValueType = true,
                            IsByRefLikeType = false
                        }
                    }
                },
                {
                    typeof(EnumerableWrapper<int, int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(EnumerableWrapper<int, int>),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(IEnumerator<int>),
                            MoveNextDeclaringType = typeof(IEnumerator),
                            ResetDeclaringType = typeof(IEnumerator),
                            DisposeDeclaringType = typeof(IDisposable),
                            ItemType = typeof(int),
                            IsValueType = false,
                            IsByRefLikeType = false
                        }
                    }
                },
            };

        public static TheoryData<Type, EnumerableTestData> CodeAnalysisEnumerables =>
            new()
            {
                {
                    typeof(EnumerableWithExtensionMethod<int>),
                    new EnumerableTestData 
                    {
                        ForEachUsesIndexer = false,
                        GetEnumeratorDeclaringType = typeof(MyExtensions),
                        EnumeratorTestData = new EnumeratorTestData 
                        {
                            CurrentDeclaringType = typeof(ValueTypeEnumerator<int>),
                            MoveNextDeclaringType = typeof(ValueTypeEnumerator<int>),
                            ResetDeclaringType = null,
                            DisposeDeclaringType = null,
                            ItemType = typeof(int),
                            IsValueType = true,
                            IsByRefLikeType = false
                        }
                    }
                },
            };

        public static TheoryData<Type, Type?, Type?, Type?, Type?> InvalidEnumerables =>
            new()
            {
                {
                    typeof(EnumerableWithMissingGetEnumerator),
                    null,
                    null,
                    null,
                    null
                },
                {
                    typeof(EnumerableWithMissingCurrent),
                    typeof(EnumerableWithMissingCurrent),
                    null,
                    typeof(EnumeratorWithMissingCurrent),
                    null
                },
                {
                    typeof(EnumerableWithMissingMoveNext<int>),
                    typeof(EnumerableWithMissingMoveNext<int>),
                    typeof(EnumeratorWithMissingMoveNext<int>),
                    null,
                    typeof(int)
                },
                {
                    typeof(HybridEnumerableWithExplicitEnumerator<int>),
                    typeof(HybridEnumerableWithExplicitEnumerator<int>),
                    null,
                    typeof(HybridEnumerableWithExplicitEnumerator<int>),
                    typeof(int)
                },
            };

        public static TheoryData<Type> Enumerators =>
            new()
            {
                typeof(IEnumerator),
                typeof(IEnumerator<int>),
                typeof(ValueTypeEnumerator<int>),
                typeof(DisposableValueTypeEnumerator<int>),
                typeof(ByRefLikeEnumerator<int>),
                typeof(DisposableByRefLikeEnumerator<int>),
                typeof(ReferenceTypeEnumerator<int>),
                typeof(DisposableReferenceTypeEnumerator<int>),
                typeof(ExplicitGenericEnumerator<int>),
                typeof(ExplicitEnumerator<int>),
            };

        public static TheoryData<Type, bool, bool> InvalidEnumerators =>
            new()
            {
                {
                    typeof(EnumeratorWithMissingCurrent),
                    true,
                    false
                },
                {
                    typeof(EnumeratorWithMissingMoveNext<int>),
                    false,
                    true
                },
            };
    }
}
