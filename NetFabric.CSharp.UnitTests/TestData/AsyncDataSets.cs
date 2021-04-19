using System;
using System.Collections.Generic;
using NetFabric.Reflection;
using Xunit;

namespace NetFabric.CSharp.TestData
{
    public static partial class DataSets
    {

        public static TheoryData<Type, Type, int, Type, Type, Type?, Type, bool> AsyncEnumerables =>
            new()
            {
                {
                    typeof(IAsyncEnumerable<int>),
                    typeof(IAsyncEnumerable<int>),
                    1,
                    typeof(IAsyncEnumerator<int>),
                    typeof(IAsyncEnumerator<int>),
                    typeof(IAsyncDisposable),
                    typeof(int),
                    false
                },
                {
                    typeof(AsyncEnumerableWithValueTypeAsyncEnumerator<int>),
                    typeof(AsyncEnumerableWithValueTypeAsyncEnumerator<int>),
                    0,
                    typeof(ValueTypeAsyncEnumerator<int>),
                    typeof(ValueTypeAsyncEnumerator<int>),
                    null,
                    typeof(int),
                    true
                },
                {
                    typeof(AsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>),
                    typeof(AsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>),
                    0,
                    typeof(DisposableValueTypeAsyncEnumerator<int>),
                    typeof(DisposableValueTypeAsyncEnumerator<int>),
                    typeof(IAsyncDisposable),
                    typeof(int),
                    true
                },
                
                {
                    typeof(CancellableAsyncEnumerableWithValueTypeAsyncEnumerator<int>),
                    typeof(CancellableAsyncEnumerableWithValueTypeAsyncEnumerator<int>),
                    1,
                    typeof(ValueTypeAsyncEnumerator<int>),
                    typeof(ValueTypeAsyncEnumerator<int>),
                    null,
                    typeof(int),
                    true
                },
                {
                    typeof(CancellableAsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>),
                    typeof(CancellableAsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>),
                    1,
                    typeof(DisposableValueTypeAsyncEnumerator<int>),
                    typeof(DisposableValueTypeAsyncEnumerator<int>),
                    typeof(IAsyncDisposable),
                    typeof(int),
                    true
                },

                {
                    typeof(HybridAsyncEnumerable<int>),
                    typeof(HybridAsyncEnumerable<int>),
                    0,
                    typeof(ValueTypeAsyncEnumerator<int>),
                    typeof(ValueTypeAsyncEnumerator<int>),
                    null,
                    typeof(int),
                    true
                },
                {
                    typeof(ExplicitAsyncEnumerable<int>),
                    typeof(IAsyncEnumerable<int>),
                    1,
                    typeof(IAsyncEnumerator<int>),
                    typeof(IAsyncEnumerator<int>),
                    typeof(IAsyncDisposable),
                    typeof(int),
                    false
                },
                {
                    typeof(RangeAsyncEnumerable),
                    typeof(RangeAsyncEnumerable),
                    0,
                    typeof(RangeAsyncEnumerable.AsyncEnumerator),
                    typeof(RangeAsyncEnumerable.AsyncEnumerator),
                    null,
                    typeof(int),
                    true
                },
                {
                    typeof(AsyncEnumerableWrapper<int, int>),
                    typeof(AsyncEnumerableWrapper<int, int>),
                    1,
                    typeof(IAsyncEnumerator<int>),
                    typeof(IAsyncEnumerator<int>),
                    typeof(IAsyncDisposable),
                    typeof(int),
                    false
                },
            };

        public static TheoryData<Type, Type?, int, Type?, Type?, Type?> InvalidAsyncEnumerables =>
            new()
            {
                {
                    typeof(AsyncEnumerableWithMissingGetAsyncEnumerator),
                    null,
                    0,
                    null,
                    null,
                    null
                },
                {
                    typeof(AsyncEnumerableWithMissingCurrent),
                    typeof(AsyncEnumerableWithMissingCurrent),
                    0,
                    null,
                    typeof(AsyncEnumeratorWithMissingCurrent),
                    null
                },
                {
                    typeof(AsyncEnumerableWithMissingMoveNextAsync<int>),
                    typeof(AsyncEnumerableWithMissingMoveNextAsync<int>),
                    0,
                    typeof(AsyncEnumeratorWithMissingMoveNextAsync<int>),
                    null,
                    typeof(int)
                },
            };

        public static TheoryData<Type> AsyncEnumerators =>
            new()
            {
                typeof(IAsyncEnumerator<int>),
                typeof(ValueTypeAsyncEnumerator<int>),
                typeof(DisposableValueTypeAsyncEnumerator<int>),
                typeof(ReferenceTypeAsyncEnumerator<int>),
                typeof(DisposableReferenceTypeAsyncEnumerator<int>),
                typeof(ExplicitAsyncEnumerator<int>),
            };

        public static TheoryData<Type, bool, bool> InvalidAsyncEnumerators =>
            new()
            {
                {
                    typeof(AsyncEnumeratorWithMissingCurrent),
                    true,
                    false
                },
                {
                    typeof(AsyncEnumeratorWithMissingMoveNextAsync<int>),
                    false,
                    true
                },
            };
    }
}
