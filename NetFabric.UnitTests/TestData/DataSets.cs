using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.TestData
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

        public static TheoryData<Type, Type, Type, Type, Type?, Type?, Type> Arrays =>
            new()
            {
                {
                    typeof(int[]),
                    typeof(Array),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    null,
                    typeof(object)
                },
                {
                    typeof(Span<int>),
                    typeof(Span<int>),
                    typeof(Span<int>.Enumerator),
                    typeof(Span<int>.Enumerator),
                    null,
                    null,
                    typeof(int).MakeByRefType()
                },
                {
                    typeof(ReadOnlySpan<int>),
                    typeof(ReadOnlySpan<int>),
                    typeof(ReadOnlySpan<int>.Enumerator),
                    typeof(ReadOnlySpan<int>.Enumerator),
                    null,
                    null,
                    typeof(int).MakeByRefType()
                },
            };

        public static TheoryData<Type, Type, Type, Type, Type?, Type?, Type> Enumerables =>
            new()
            {
                {
                    typeof(Enumerable<int>),
                    typeof(Enumerable<int>),
                    typeof(Enumerator<int>),
                    typeof(Enumerator<int>),
                    null,
                    null,
                    typeof(int)
                },
                {
                    typeof(HybridEnumerable<int>),
                    typeof(HybridEnumerable<int>),
                    typeof(Enumerator<int>),
                    typeof(Enumerator<int>),
                    null,
                    null,
                    typeof(int)
                },
                {
                    typeof(ExplicitEnumerable<int>),
                    typeof(IEnumerable),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    null,
                    typeof(object)
                },
                {
                    typeof(ExplicitGenericEnumerable<int>),
                    typeof(IEnumerable<int>),
                    typeof(IEnumerator<int>),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    typeof(IDisposable),
                    typeof(int)
                },
                {
                    typeof(RangeEnumerable),
                    typeof(RangeEnumerable),
                    typeof(RangeEnumerable.Enumerator),
                    typeof(RangeEnumerable.Enumerator),
                    null,
                    null,
                    typeof(int)
                },
            };

        public static TheoryData<Type, Type?, Type?, Type?, Type?> InvalidEnumerables =>
            new()
            {
                {
                    typeof(MissingGetEnumeratorEnumerable),
                    null,
                    null,
                    null,
                    null
                },
                {
                    typeof(MissingCurrentEnumerable),
                    typeof(MissingCurrentEnumerable),
                    null,
                    typeof(MissingCurrentEnumerator),
                    null
                },
                {
                    typeof(MissingMoveNextEnumerable<int>),
                    typeof(MissingMoveNextEnumerable<int>),
                    typeof(MissingMoveNextEnumerator<int>),
                    null,
                    typeof(int)
                },
            };

        public static TheoryData<Type, Type, int, Type, Type, Type?, Type> AsyncEnumerables =>
            new()
            {
                {
                    typeof(AsyncEnumerable<int>),
                    typeof(AsyncEnumerable<int>),
                    0,
                    typeof(AsyncEnumerator<int>),
                    typeof(AsyncEnumerator<int>),
                    null,
                    typeof(int)
                },
                {
                    typeof(CancellableAsyncEnumerable<int>),
                    typeof(CancellableAsyncEnumerable<int>),
                    1,
                    typeof(AsyncEnumerator<int>),
                    typeof(AsyncEnumerator<int>),
                    null,
                    typeof(int)
                },
                {
                    typeof(HybridAsyncEnumerable<int>),
                    typeof(HybridAsyncEnumerable<int>),
                    0,
                    typeof(AsyncEnumerator<int>),
                    typeof(AsyncEnumerator<int>),
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
                    typeof(MissingGetEnumeratorEnumerable),
                    null,
                    0,
                    null,
                    null,
                    null
                },
                {
                    typeof(MissingCurrentEnumerable),
                    typeof(MissingCurrentEnumerable),
                    0,
                    null,
                    typeof(MissingCurrentEnumerator),
                    null
                },
                {
                    typeof(MissingMoveNextEnumerable<int>),
                    typeof(MissingMoveNextEnumerable<int>),
                    0,
                    typeof(MissingMoveNextEnumerator<int>),
                    null,
                    typeof(int)
                },
            };

        public static TheoryData<Type, Type, Type, Type?, Type?, Type, bool> Enumerators =>
            new()
            {
                {
                    typeof(Enumerator<int>),
                    typeof(Enumerator<int>),
                    typeof(Enumerator<int>),
                    null,
                    null,
                    typeof(int),
                    false
                },
                {
                    typeof(DisposableEnumerator<int>),
                    typeof(DisposableEnumerator<int>),
                    typeof(DisposableEnumerator<int>),
                    null,
                    typeof(IDisposable),
                    typeof(int),
                    false
                },
                {
                    typeof(ExplicitEnumerator<int>),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    null,
                    typeof(object),
                    false
                },
                {
                    typeof(ExplicitGenericEnumerator<int>),
                    typeof(IEnumerator<int>),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    typeof(IDisposable),
                    typeof(int),
                    false
                },
                {
                    typeof(RefEnumerator<int>),
                    typeof(RefEnumerator<int>),
                    typeof(RefEnumerator<int>),
                    null,
                    null,
                    typeof(int),
                    true
                },
                {
                    typeof(DisposableRefEnumerator<int>),
                    typeof(DisposableRefEnumerator<int>),
                    typeof(DisposableRefEnumerator<int>),
                    null,
                    typeof(DisposableRefEnumerator<int>),
                    typeof(int),
                    true
                },
            };

        public static TheoryData<Type, Type?, Type?, Type?> InvalidEnumerators =>
            new()
            {
                {
                    typeof(MissingCurrentAndMoveNextEnumerator),
                    null,
                    null,
                    null
                },
                {
                    typeof(MissingCurrentEnumerator),
                    null,
                    typeof(MissingCurrentEnumerator),
                    null
                },
                {
                    typeof(MissingMoveNextEnumerator<int>),
                    typeof(MissingMoveNextEnumerator<int>),
                    null,
                    typeof(int)
                },
            };

        public static TheoryData<Type, Type, Type, Type?, Type> AsyncEnumerators =>
            new()
            {
                {
                    typeof(AsyncEnumerator<int>),
                    typeof(AsyncEnumerator<int>),
                    typeof(AsyncEnumerator<int>),
                    null,
                    typeof(int)
                },
                {
                    typeof(ExplicitAsyncEnumerator<int>),
                    typeof(IAsyncEnumerator<int>),
                    typeof(IAsyncEnumerator<int>),
                    typeof(IAsyncDisposable),
                    typeof(int)
                },
            };

        public static TheoryData<Type, Type?, Type?, Type?> InvalidAsyncEnumerators =>
            new()
            {
                {
                    typeof(MissingCurrentAndMoveNextEnumerator),
                    null,
                    null,
                    null
                },
                {
                    typeof(MissingCurrentEnumerator),
                    null,
                    typeof(MissingCurrentEnumerator),
                    null
                },
                {
                    typeof(MissingMoveNextEnumerator<int>),
                    typeof(MissingMoveNextEnumerator<int>),
                    null,
                    typeof(int)
                },
            };
    }
}
