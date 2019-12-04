using System;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        public static TheoryData<Type, Type, int, Type, Type, Type, Type> AsyncEnumerables =>
            new TheoryData<Type, Type, int, Type, Type, Type, Type>
            {
                { 
                    typeof(TestData.AsyncEnumerable<>).MakeGenericType(typeof(int)), 
                    typeof(TestData.AsyncEnumerable<>).MakeGenericType(typeof(int)), 
                    0,
                    typeof(TestData.AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(TestData.AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    null,
                    typeof(int)
                },
                { 
                    typeof(TestData.ExplicitAsyncEnumerable<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncEnumerable<>).MakeGenericType(typeof(int)),
                    1,
                    typeof(IAsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncDisposable),
                    typeof(int)
                },
                { 
                    typeof(TestData.RangeAsyncEnumerable),
                    typeof(TestData.RangeAsyncEnumerable),
                    0,
                    typeof(TestData.RangeAsyncEnumerable.AsyncEnumerator),
                    typeof(TestData.RangeAsyncEnumerable.AsyncEnumerator),
                    null,
                    typeof(int)
                },
            };

        [Theory]
        [MemberData(nameof(AsyncEnumerables))]
        public void IsAsyncEnumerable_Should_ReturnTrue(Type type, Type getAsyncEnumeratorDeclaringType, int getEnumeratorParametersCount, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = type.IsAsyncEnumerable(out var enumerableInfo);

            // Assert   
            Assert.True(result);

            Assert.Equal(getAsyncEnumeratorDeclaringType, enumerableInfo.EnumerableType);
            Assert.Equal(enumerableInfo.GetEnumerator?.ReturnType, enumerableInfo.EnumeratorType);
            Assert.Equal(itemType, enumerableInfo.ItemType);

            Assert.NotNull(enumerableInfo.GetEnumerator);
            Assert.Equal("GetAsyncEnumerator", enumerableInfo.GetEnumerator.Name);
            Assert.Equal(getAsyncEnumeratorDeclaringType, enumerableInfo.GetEnumerator.DeclaringType);
            Assert.Equal(getEnumeratorParametersCount, enumerableInfo.GetEnumerator.GetParameters().Length);

            Assert.NotNull(enumerableInfo.Current);
            Assert.Equal("Current", enumerableInfo.Current.Name);
            Assert.Equal(currentDeclaringType, enumerableInfo.Current.DeclaringType);
            Assert.Equal(itemType, enumerableInfo.Current.PropertyType);

            Assert.NotNull(enumerableInfo.MoveNext);
            Assert.Equal("MoveNextAsync", enumerableInfo.MoveNext.Name);
            Assert.Equal(moveNextAsyncDeclaringType, enumerableInfo.MoveNext.DeclaringType);
            Assert.Empty(enumerableInfo.MoveNext.GetParameters());

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(enumerableInfo.Dispose);
            }
            else
            {
                Assert.NotNull(enumerableInfo.Dispose);
                Assert.Equal("DisposeAsync", enumerableInfo.Dispose.Name);
                Assert.Equal(disposeAsyncDeclaringType, enumerableInfo.Dispose.DeclaringType);
                Assert.Empty(enumerableInfo.Dispose.GetParameters());
            }
        }

        public static TheoryData<Type, Type, int, Type, Type, Type, Type> InvalidAsyncEnumerables =>
            new TheoryData<Type, Type, int, Type, Type, Type, Type>
            {
                {
                    typeof(TestData.MissingGetEnumeratorEnumerable),
                    null,
                    0,
                    null,
                    null,
                    null,
                    null
                },
                {
                    typeof(TestData.MissingCurrentEnumerable),
                    typeof(TestData.MissingCurrentEnumerable),
                    0,
                    null,
                    typeof(TestData.MissingCurrentEnumerator),
                    null,
                    null
                },
                {
                    typeof(TestData.MissingMoveNextEnumerable<int>),
                    typeof(TestData.MissingMoveNextEnumerable<int>),
                    0,
                    typeof(TestData.MissingMoveNextEnumerator<int>),
                    null,
                    null,
                    typeof(int)
                },
            };

        [Theory]
        [MemberData(nameof(InvalidAsyncEnumerables))]
        public void IsAsyncEnumerable_With_MissingFeatures_Should_ReturnFalse(Type type, Type getAsyncEnumeratorDeclaringType, int getEnumeratorParametersCount, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = type.IsAsyncEnumerable(out var enumerableInfo);

            // Assert   
            Assert.False(result);

            Assert.Equal(getAsyncEnumeratorDeclaringType, enumerableInfo.EnumerableType);
            Assert.Equal(enumerableInfo.GetEnumerator?.ReturnType, enumerableInfo.EnumeratorType);
            Assert.Equal(itemType, enumerableInfo.ItemType);

            if (getAsyncEnumeratorDeclaringType is null)
            {
                Assert.Null(enumerableInfo.GetEnumerator);
            }
            else
            {
                Assert.NotNull(enumerableInfo.GetEnumerator);
                Assert.Equal("GetAsyncEnumerator", enumerableInfo.GetEnumerator.Name);
                Assert.Equal(getAsyncEnumeratorDeclaringType, enumerableInfo.GetEnumerator.DeclaringType);
                Assert.Equal(getEnumeratorParametersCount, enumerableInfo.GetEnumerator.GetParameters().Length);
            }

            if (currentDeclaringType is null)
            {
                Assert.Null(enumerableInfo.Current);
            }
            else
            {
                Assert.NotNull(enumerableInfo.Current);
                Assert.Equal("Current", enumerableInfo.Current.Name);
                Assert.Equal(currentDeclaringType, enumerableInfo.Current.DeclaringType);
                Assert.Equal(itemType, enumerableInfo.Current.PropertyType);
            }

            if (moveNextAsyncDeclaringType is null)
            {
                Assert.Null(enumerableInfo.MoveNext);
            }
            else
            {
                Assert.NotNull(enumerableInfo.MoveNext);
                Assert.Equal("MoveNextAsync", enumerableInfo.MoveNext.Name);
                Assert.Equal(moveNextAsyncDeclaringType, enumerableInfo.MoveNext.DeclaringType);
                Assert.Empty(enumerableInfo.MoveNext.GetParameters());
            }

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(enumerableInfo.Dispose);
            }
            else
            {
                Assert.NotNull(enumerableInfo.Dispose);
                Assert.Equal("DisposeAsync", enumerableInfo.Dispose.Name);
                Assert.Equal(disposeAsyncDeclaringType, enumerableInfo.Dispose.DeclaringType);
                Assert.Empty(enumerableInfo.Dispose.GetParameters());
            }
        }
    }
}
