﻿using System;
using System.Collections.Generic;
using NetFabric.Reflection;
using Xunit;

namespace NetFabric.CSharp.TestData;

public static class AsyncEnumerableDataSets
{

    public class AsyncEnumerableTestData 
    {
        public Type GetAsyncEnumeratorDeclaringType;
        public int GetAsyncEnumeratorParametersCount;
        public AsyncEnumeratorTestData AsyncEnumeratorTestData;
    }

    public class AsyncEnumeratorTestData 
    {
        public Type CurrentDeclaringType;
        public Type MoveNextAsyncDeclaringType;
        public Type? DisposeAsyncDeclaringType;
        public Type ItemType;
        public bool IsValueType;
    }

    public static TheoryData<Type, AsyncEnumerableTestData> AsyncEnumerables =>
        new()
        {
            {
                typeof(IAsyncEnumerable<int>),
                new AsyncEnumerableTestData 
                {
                    GetAsyncEnumeratorDeclaringType = typeof(IAsyncEnumerable<int>),
                    GetAsyncEnumeratorParametersCount = 1,
                    AsyncEnumeratorTestData = new AsyncEnumeratorTestData 
                    {
                        CurrentDeclaringType = typeof(IAsyncEnumerator<int>),
                        MoveNextAsyncDeclaringType = typeof(IAsyncEnumerator<int>),
                        DisposeAsyncDeclaringType = typeof(IAsyncDisposable),
                        ItemType = typeof(int),
                        IsValueType = false,
                    }
                }
            },
            {
                typeof(AsyncEnumerableWithValueTypeAsyncEnumerator<int>),
                new AsyncEnumerableTestData 
                {
                    GetAsyncEnumeratorDeclaringType = typeof(AsyncEnumerableWithValueTypeAsyncEnumerator<int>),
                    GetAsyncEnumeratorParametersCount = 0,
                    AsyncEnumeratorTestData = new AsyncEnumeratorTestData 
                    {
                        CurrentDeclaringType = typeof(ValueTypeAsyncEnumerator<int>),
                        MoveNextAsyncDeclaringType = typeof(ValueTypeAsyncEnumerator<int>),
                        DisposeAsyncDeclaringType = null,
                        ItemType = typeof(int),
                        IsValueType = true,
                    }
                }
            },
            {
                typeof(AsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>),
                new AsyncEnumerableTestData 
                {
                    GetAsyncEnumeratorDeclaringType = typeof(AsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>),
                    GetAsyncEnumeratorParametersCount = 0,
                    AsyncEnumeratorTestData = new AsyncEnumeratorTestData 
                    {
                        CurrentDeclaringType = typeof(DisposableValueTypeAsyncEnumerator<int>),
                        MoveNextAsyncDeclaringType = typeof(DisposableValueTypeAsyncEnumerator<int>),
                        DisposeAsyncDeclaringType = typeof(IAsyncDisposable),
                        ItemType = typeof(int),
                        IsValueType = true,
                    }
                }
            },
            {
                typeof(CancellableAsyncEnumerableWithValueTypeAsyncEnumerator<int>),
                new AsyncEnumerableTestData 
                {
                    GetAsyncEnumeratorDeclaringType = typeof(CancellableAsyncEnumerableWithValueTypeAsyncEnumerator<int>),
                    GetAsyncEnumeratorParametersCount = 1,
                    AsyncEnumeratorTestData = new AsyncEnumeratorTestData 
                    {
                        CurrentDeclaringType = typeof(ValueTypeAsyncEnumerator<int>),
                        MoveNextAsyncDeclaringType = typeof(ValueTypeAsyncEnumerator<int>),
                        DisposeAsyncDeclaringType = null,
                        ItemType = typeof(int),
                        IsValueType = true,
                    }
                }
            },
            {
                typeof(CancellableAsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>),
                new AsyncEnumerableTestData 
                {
                    GetAsyncEnumeratorDeclaringType = typeof(CancellableAsyncEnumerableWithDisposableValueTypeAsyncEnumerator<int>),
                    GetAsyncEnumeratorParametersCount = 1,
                    AsyncEnumeratorTestData = new AsyncEnumeratorTestData 
                    {
                        CurrentDeclaringType = typeof(DisposableValueTypeAsyncEnumerator<int>),
                        MoveNextAsyncDeclaringType = typeof(DisposableValueTypeAsyncEnumerator<int>),
                        DisposeAsyncDeclaringType = typeof(IAsyncDisposable),
                        ItemType = typeof(int),
                        IsValueType = true,
                    }
                }
            },
            {
                typeof(HybridAsyncEnumerable<int>),
                new AsyncEnumerableTestData 
                {
                    GetAsyncEnumeratorDeclaringType = typeof(HybridAsyncEnumerable<int>),
                    GetAsyncEnumeratorParametersCount = 0,
                    AsyncEnumeratorTestData = new AsyncEnumeratorTestData 
                    {
                        CurrentDeclaringType = typeof(ValueTypeAsyncEnumerator<int>),
                        MoveNextAsyncDeclaringType = typeof(ValueTypeAsyncEnumerator<int>),
                        DisposeAsyncDeclaringType = null,
                        ItemType = typeof(int),
                        IsValueType = true,
                    }
                }
            },
            {
                typeof(ExplicitAsyncEnumerable<int>),
                new AsyncEnumerableTestData 
                {
                    GetAsyncEnumeratorDeclaringType = typeof(IAsyncEnumerable<int>),
                    GetAsyncEnumeratorParametersCount = 1,
                    AsyncEnumeratorTestData = new AsyncEnumeratorTestData 
                    {
                        CurrentDeclaringType = typeof(IAsyncEnumerator<int>),
                        MoveNextAsyncDeclaringType = typeof(IAsyncEnumerator<int>),
                        DisposeAsyncDeclaringType = typeof(IAsyncDisposable),
                        ItemType = typeof(int),
                        IsValueType = false,
                    }
                }
            },
            {
                typeof(RangeAsyncEnumerable),
                new AsyncEnumerableTestData 
                {
                    GetAsyncEnumeratorDeclaringType = typeof(RangeAsyncEnumerable),
                    GetAsyncEnumeratorParametersCount = 0,
                    AsyncEnumeratorTestData = new AsyncEnumeratorTestData 
                    {
                        CurrentDeclaringType = typeof(RangeAsyncEnumerable.AsyncEnumerator),
                        MoveNextAsyncDeclaringType = typeof(RangeAsyncEnumerable.AsyncEnumerator),
                        DisposeAsyncDeclaringType = null,
                        ItemType = typeof(int),
                        IsValueType = true,
                    }
                }
            },
            {
                typeof(AsyncEnumerableWrapper<int, int>),
                new AsyncEnumerableTestData 
                {
                    GetAsyncEnumeratorDeclaringType = typeof(AsyncEnumerableWrapper<int, int>),
                    GetAsyncEnumeratorParametersCount = 1,
                    AsyncEnumeratorTestData = new AsyncEnumeratorTestData 
                    {
                        CurrentDeclaringType = typeof(IAsyncEnumerator<int>),
                        MoveNextAsyncDeclaringType = typeof(IAsyncEnumerator<int>),
                        DisposeAsyncDeclaringType = typeof(IAsyncDisposable),
                        ItemType = typeof(int),
                        IsValueType = false,
                    }
                }
            },
        };

    public static TheoryData<Type, AsyncEnumerableTestData> CodeAnalysisAsyncEnumerables =>
        new()
        {
            {
                typeof(AsyncEnumerableWithExtensionMethod<int>),
                new AsyncEnumerableTestData 
                {
                    GetAsyncEnumeratorDeclaringType = typeof(MyExtensions),
                    GetAsyncEnumeratorParametersCount = 0,
                    AsyncEnumeratorTestData = new AsyncEnumeratorTestData 
                    {
                        CurrentDeclaringType = typeof(ValueTypeAsyncEnumerator<int>),
                        MoveNextAsyncDeclaringType = typeof(ValueTypeAsyncEnumerator<int>),
                        DisposeAsyncDeclaringType = null,
                        ItemType = typeof(int),
                        IsValueType = true,
                    }
                }
            },
            {
                typeof(CancellableAsyncEnumerableWithExtensionMethod<int>),
                new AsyncEnumerableTestData 
                {
                    GetAsyncEnumeratorDeclaringType = typeof(MyExtensions),
                    GetAsyncEnumeratorParametersCount = 1,
                    AsyncEnumeratorTestData = new AsyncEnumeratorTestData 
                    {
                        CurrentDeclaringType = typeof(ValueTypeAsyncEnumerator<int>),
                        MoveNextAsyncDeclaringType = typeof(ValueTypeAsyncEnumerator<int>),
                        DisposeAsyncDeclaringType = null,
                        ItemType = typeof(int),
                        IsValueType = true,
                    }
                }
            },
        };

    public static TheoryData<Type, IsAsyncEnumerableError> InvalidAsyncEnumerables =>
        new()
        {
            {
                typeof(AsyncEnumerableWithMissingGetAsyncEnumerator),
                IsAsyncEnumerableError.MissingGetAsyncEnumerator
            },
            {
                typeof(AsyncEnumerableWithMissingCurrent),
                IsAsyncEnumerableError.MissingCurrent
            },
            {
                typeof(AsyncEnumerableWithMissingMoveNextAsync<int>),
                IsAsyncEnumerableError.MissingMoveNextAsync
            },
            {
                typeof(AsyncEnumerableWithMoveNextAsyncWithWrongReturnType<int>),
                IsAsyncEnumerableError.MissingMoveNextAsync
            },
        };
}
