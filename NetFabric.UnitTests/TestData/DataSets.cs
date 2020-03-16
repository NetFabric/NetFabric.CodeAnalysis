using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.TestData
{
    public static partial class DataSets
    {
        public static TheoryData<string, Type> InstanceProperties =>
            new TheoryData<string, Type>
            {
                { "Property", typeof(int) },
                { "InheritedProperty", typeof(int) },
            };

        public static TheoryData<string> ExplicitInstanceProperties =>
            new TheoryData<string>
            {
                "ExplicitProperty",
                "StaticProperty",
            };

        public static TheoryData<string, Type[]> InstanceMethods =>
            new TheoryData<string, Type[]>
            {
                { "Method", new Type[] { } },
                { "Method", new Type[] { typeof(int), typeof(string) } },
                { "InheritedMethod", new Type[] { } },
                { "InheritedMethod", new Type[] { typeof(int), typeof(string) } },
            };

        public static TheoryData<string, Type[]> ExplicitInstanceMethods =>
            new TheoryData<string, Type[]>
            {
                { "ExplicitMethod", new Type[] { } },
                { "ExplicitMethod", new Type[] { typeof(int), typeof(string) } },
                { "StaticMethod", new Type[] { } },
                { "StaticMethod", new Type[] { typeof(int), typeof(string) } },
            };

        public static TheoryData<Type, Type, Type, Type, Type, Type, Type> Enumerables =>
            new TheoryData<Type, Type, Type, Type, Type, Type, Type>
            {
                {
                    typeof(Enumerable<>).MakeGenericType(typeof(int)),
                    typeof(Enumerable<>).MakeGenericType(typeof(int)),
                    typeof(Enumerator<>).MakeGenericType(typeof(int)),
                    typeof(Enumerator<>).MakeGenericType(typeof(int)),
                    null,
                    null,
                    typeof(int)
                },
                {
                    typeof(HybridEnumerable<>).MakeGenericType(typeof(int)),
                    typeof(HybridEnumerable<>).MakeGenericType(typeof(int)),
                    typeof(Enumerator<>).MakeGenericType(typeof(int)),
                    typeof(Enumerator<>).MakeGenericType(typeof(int)),
                    null,
                    null,
                    typeof(int)
                },
                {
                    typeof(ExplicitEnumerable),
                    typeof(IEnumerable),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    null,
                    typeof(object)
                },
                {
                    typeof(ExplicitEnumerable<>).MakeGenericType(typeof(int)),
                    typeof(IEnumerable<>).MakeGenericType(typeof(int)),
                    typeof(IEnumerator<>).MakeGenericType(typeof(int)),
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

        public static TheoryData<Type, Type, Type, Type, Type, Type, Type> InvalidEnumerables =>
            new TheoryData<Type, Type, Type, Type, Type, Type, Type>
            {
                {
                    typeof(MissingGetEnumeratorEnumerable),
                    null,
                    null,
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
                    null,
                    null,
                    null
                },
                {
                    typeof(MissingMoveNextEnumerable<int>),
                    typeof(MissingMoveNextEnumerable<int>),
                    typeof(MissingMoveNextEnumerator<int>),
                    null,
                    null,
                    null,
                    typeof(int)
                },
            };

        public static TheoryData<Type, Type, int, Type, Type, Type, Type> AsyncEnumerables =>
            new TheoryData<Type, Type, int, Type, Type, Type, Type>
            {
                {
                    typeof(AsyncEnumerable<>).MakeGenericType(typeof(int)),
                    typeof(AsyncEnumerable<>).MakeGenericType(typeof(int)),
                    0,
                    typeof(AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    null,
                    typeof(int)
                },
                {
                    typeof(CancellableAsyncEnumerable<>).MakeGenericType(typeof(int)),
                    typeof(CancellableAsyncEnumerable<>).MakeGenericType(typeof(int)),
                    1,
                    typeof(AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    null,
                    typeof(int)
                },
                {
                    typeof(HybridAsyncEnumerable<>).MakeGenericType(typeof(int)),
                    typeof(HybridAsyncEnumerable<>).MakeGenericType(typeof(int)),
                    0,
                    typeof(AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    null,
                    typeof(int)
                },
                {
                    typeof(ExplicitAsyncEnumerable<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncEnumerable<>).MakeGenericType(typeof(int)),
                    1,
                    typeof(IAsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncEnumerator<>).MakeGenericType(typeof(int)),
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

        public static TheoryData<Type, Type, int, Type, Type, Type, Type> InvalidAsyncEnumerables =>
            new TheoryData<Type, Type, int, Type, Type, Type, Type>
            {
                {
                    typeof(MissingGetEnumeratorEnumerable),
                    null,
                    0,
                    null,
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
                    null,
                    null
                },
                {
                    typeof(MissingMoveNextEnumerable<int>),
                    typeof(MissingMoveNextEnumerable<int>),
                    0,
                    typeof(MissingMoveNextEnumerator<int>),
                    null,
                    null,
                    typeof(int)
                },
            };

        public static TheoryData<Type, Type, Type, Type, Type, Type> Enumerators =>
            new TheoryData<Type, Type, Type, Type, Type, Type>
            {
                {
                    typeof(Enumerator<>).MakeGenericType(typeof(int)),
                    typeof(Enumerator<>).MakeGenericType(typeof(int)),
                    typeof(Enumerator<>).MakeGenericType(typeof(int)),
                    null,
                    null,
                    typeof(int)
                },
                {
                    typeof(ExplicitEnumerator),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    null,
                    typeof(object)
                },
                {
                    typeof(ExplicitEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    typeof(IDisposable),
                    typeof(int)
                },
            };

        public static TheoryData<Type, Type, Type, Type, Type, Type> InvalidEnumerators =>
            new TheoryData<Type, Type, Type, Type, Type, Type>
            {
                {
                    typeof(MissingCurrentAndMoveNextEnumerator),
                    null,
                    null,
                    null,
                    null,
                    null
                },
                {
                    typeof(MissingCurrentEnumerator),
                    null,
                    typeof(MissingCurrentEnumerator),
                    null,
                    null,
                    null
                },
                {
                    typeof(MissingMoveNextEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(MissingMoveNextEnumerator<>).MakeGenericType(typeof(int)),
                    null,
                    null,
                    null,
                    typeof(int)
                },
            };

        public static TheoryData<Type, Type, Type, Type, Type> AsyncEnumerators =>
            new TheoryData<Type, Type, Type, Type, Type>
            {
                {
                    typeof(AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    null,
                    typeof(int)
                },
                {
                    typeof(ExplicitAsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncDisposable),
                    typeof(int)
                },
            };

        public static TheoryData<Type, Type, Type, Type, Type> InvalidAsyncEnumerators =>
            new TheoryData<Type, Type, Type, Type, Type>
            {
                {
                    typeof(MissingCurrentAndMoveNextEnumerator),
                    null,
                    null,
                    null,
                    null
                },
                {
                    typeof(MissingCurrentEnumerator),
                    null,
                    typeof(MissingCurrentEnumerator),
                    null,
                    null
                },
                {
                    typeof(MissingMoveNextEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(MissingMoveNextEnumerator<>).MakeGenericType(typeof(int)),
                    null,
                    null,
                    typeof(int)
                },
            };
    }
}
