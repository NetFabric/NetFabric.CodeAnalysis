using NetFabric.VisualBasic.UnitTests.TestData;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public static TheoryData<Type, Type, Type, Type, Type?, Type?, Type, bool, bool> Arrays =>
            new()
            {
                {
                    typeof(int[]),
                    typeof(IEnumerable<int>),
                    typeof(IEnumerator<int>),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    typeof(IDisposable),
                    typeof(int),
                    false,
                    false
                },
                {
                    typeof(Span<int>),
                    typeof(Span<int>),
                    typeof(Span<int>.Enumerator),
                    typeof(Span<int>.Enumerator),
                    null,
                    null,
                    typeof(int).MakeByRefType(),
                    true,
                    true
                },
                {
                    typeof(ReadOnlySpan<int>),
                    typeof(ReadOnlySpan<int>),
                    typeof(ReadOnlySpan<int>.Enumerator),
                    typeof(ReadOnlySpan<int>.Enumerator),
                    null,
                    null,
                    typeof(int).MakeByRefType(),
                    true,
                    true
                },
            };

        public static TheoryData<Type, Type, Type, Type, Type?, Type?, Type, bool, bool> Enumerables =>
            new()
            {
                {
                    typeof(EnumerableWithValueTypeEnumerator<int>),
                    typeof(EnumerableWithValueTypeEnumerator<int>),
                    typeof(ValueTypeEnumerator<int>),
                    typeof(ValueTypeEnumerator<int>),
                    null,
                    null,
                    typeof(int),
                    true,
                    false
                },
                {
                    typeof(EnumerableWithDisposableValueTypeEnumerator<int>),
                    typeof(EnumerableWithDisposableValueTypeEnumerator<int>),
                    typeof(DisposableValueTypeEnumerator<int>),
                    typeof(DisposableValueTypeEnumerator<int>),
                    null,
                    typeof(IDisposable),
                    typeof(int),
                    true,
                    false
                },
                {
                    typeof(EnumerableWithByRefLikeEnumerator<int>),
                    typeof(EnumerableWithByRefLikeEnumerator<int>),
                    typeof(ByRefLikeEnumerator<int>),
                    typeof(ByRefLikeEnumerator<int>),
                    null,
                    null,
                    typeof(int),
                    true,
                    true
                },
                {
                    typeof(EnumerableWithDisposableByRefLikeEnumerator<int>),
                    typeof(EnumerableWithDisposableByRefLikeEnumerator<int>),
                    typeof(DisposableByRefLikeEnumerator<int>),
                    typeof(DisposableByRefLikeEnumerator<int>),
                    null,
                    typeof(DisposableByRefLikeEnumerator<int>),
                    typeof(int),
                    true,
                    true
                },
                {
                    typeof(EnumerableWithReferenceTypeEnumerator<int>),
                    typeof(EnumerableWithReferenceTypeEnumerator<int>),
                    typeof(ReferenceTypeEnumerator<int>),
                    typeof(ReferenceTypeEnumerator<int>),
                    null,
                    null,
                    typeof(int),
                    false,
                    false
                },
                {
                    typeof(EnumerableWithDisposableReferenceTypeEnumerator<int>),
                    typeof(EnumerableWithDisposableReferenceTypeEnumerator<int>),
                    typeof(DisposableReferenceTypeEnumerator<int>),
                    typeof(DisposableReferenceTypeEnumerator<int>),
                    null,
                    typeof(IDisposable),
                    typeof(int),
                    false,
                    false
                },
                {
                    typeof(HybridEnumerable<int>),
                    typeof(HybridEnumerable<int>),
                    typeof(ValueTypeEnumerator<int>),
                    typeof(ValueTypeEnumerator<int>),
                    null,
                    null,
                    typeof(int),
                    true,
                    false
                },
                {
                    typeof(ExplicitEnumerable<int>),
                    typeof(IEnumerable),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    null,
                    typeof(object),
                    false,
                    false
                },
                {
                    typeof(ExplicitGenericEnumerable<int>),
                    typeof(IEnumerable<int>),
                    typeof(IEnumerator<int>),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    typeof(IDisposable),
                    typeof(int),
                    false,
                    false
                },
                {
                    typeof(RangeEnumerable),
                    typeof(RangeEnumerable),
                    typeof(RangeEnumerable.Enumerator),
                    typeof(RangeEnumerable.Enumerator),
                    null,
                    null,
                    typeof(int),
                    true,
                    false
                },
                {
                    typeof(DerivedHybridEnumerable<int>),
                    typeof(HybridEnumerable<int>),
                    typeof(ValueTypeEnumerator<int>),
                    typeof(ValueTypeEnumerator<int>),
                    null,
                    null,
                    typeof(int),
                    true,
                    false
                },
                {
                    typeof(ReadOnlyCollectionEnumerable<int>),
                    typeof(ReadOnlyCollectionEnumerable<int>),
                    typeof(ValueTypeEnumerator<int>),
                    typeof(ValueTypeEnumerator<int>),
                    null,
                    null,
                    typeof(int),
                    true,
                    false
                },
            };

        public static TheoryData<Type, Type, Type, Type, Type?, Type?, Type, bool, bool> VisualBasicEnumerables =>
            new()
            {
                {
                    typeof(MappedEnumerable<int>),
                    typeof(MappedEnumerable<int>),
                    typeof(IEnumerator<int>),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    typeof(IDisposable),
                    typeof(int),
                    false,
                    false
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
    }
}
