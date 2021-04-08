﻿using NetFabric.VisualBasic.UnitTests.TestData;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.CSharp.TestData
{
    public static partial class DataSets
    {

        public static TheoryData<Type, Type, int, Type, Type, Type?, Type> AsyncEnumerables =>
            new()
            {
                {
                    typeof(AsyncEnumerableWithValueTypeAsyncEnumerator<int>),
                    typeof(AsyncEnumerableWithValueTypeAsyncEnumerator<int>),
                    0,
                    typeof(ValueTypeAsyncEnumerator<int>),
                    typeof(ValueTypeAsyncEnumerator<int>),
                    null,
                    typeof(int)
                },
                {
                    typeof(AsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>),
                    typeof(AsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>),
                    0,
                    typeof(DisposableValueTypeAsyncEnumerator<int>),
                    typeof(DisposableValueTypeAsyncEnumerator<int>),
                    typeof(IAsyncDisposable),
                    typeof(int)
                },
                
                {
                    typeof(CancellableAsyncEnumerableWithValueTypeAsyncEnumerator<int>),
                    typeof(CancellableAsyncEnumerableWithValueTypeAsyncEnumerator<int>),
                    1,
                    typeof(ValueTypeAsyncEnumerator<int>),
                    typeof(ValueTypeAsyncEnumerator<int>),
                    null,
                    typeof(int)
                },
                {
                    typeof(CancellableAsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>),
                    typeof(CancellableAsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>),
                    1,
                    typeof(DisposableValueTypeAsyncEnumerator<int>),
                    typeof(DisposableValueTypeAsyncEnumerator<int>),
                    typeof(IAsyncDisposable),
                    typeof(int)
                },

                {
                    typeof(HybridAsyncEnumerable<int>),
                    typeof(HybridAsyncEnumerable<int>),
                    0,
                    typeof(ValueTypeAsyncEnumerator<int>),
                    typeof(ValueTypeAsyncEnumerator<int>),
                    null,
                    typeof(int)
                },
                {
                    typeof(ExplicitAsyncEnumerable<int>),
                    typeof(IAsyncEnumerable<int>),
                    1,
                    typeof(IAsyncEnumerator<int>),
                    typeof(IAsyncEnumerator<int>),
                    typeof(IAsyncDisposable),
                    typeof(int)
                },
                {
                    typeof(RangeAsyncEnumerable),
                    typeof(RangeAsyncEnumerable),
                    0,
                    typeof(RangeAsyncEnumerable.AsyncEnumerator),
                    typeof(RangeAsyncEnumerable.AsyncEnumerator),
                    null,
                    typeof(int)
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
    }
}
