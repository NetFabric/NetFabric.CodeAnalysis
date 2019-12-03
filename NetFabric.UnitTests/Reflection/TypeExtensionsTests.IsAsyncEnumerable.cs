using System;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        public static TheoryData<Type, Type, int> AsyncEnumerables =>
            new TheoryData<Type, Type, int>
            {
                { 
                    typeof(TestData.AsyncEnumerable<>).MakeGenericType(typeof(int)), 
                    typeof(TestData.AsyncEnumerable<>).MakeGenericType(typeof(int)), 
                    0
                },
                { 
                    typeof(TestData.ExplicitAsyncEnumerable<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncEnumerable<>).MakeGenericType(typeof(int)),
                    1
                },
                { 
                    typeof(TestData.RangeAsyncEnumerable),
                    typeof(TestData.RangeAsyncEnumerable),
                    0
                },
            };

        [Theory]
        [MemberData(nameof(AsyncEnumerables))]
        public void IsAsyncEnumerable_Should_ReturnTrue(Type enumeratorType, Type methodDeclaringType, int methodParametersCount)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsAsyncEnumerable(out var enumerableInfo);

            // Assert   
            Assert.True(result);

            Assert.NotNull(enumerableInfo.GetEnumerator);
            Assert.Equal("GetAsyncEnumerator", enumerableInfo.GetEnumerator.Name);
            Assert.Equal(methodParametersCount, enumerableInfo.GetEnumerator.GetParameters().Length);
        }

        public static TheoryData<Type, Type, Type, Type, Type, Type> InvalidAsyncEnumerables =>
            new TheoryData<Type, Type, Type, Type, Type, Type>
            {
                {
                    typeof(TestData.MissingGetEnumeratorEnumerable),
                    null,
                    null,
                    null,
                    null,
                    null
                },
                {
                    typeof(TestData.MissingCurrentEnumerable),
                    typeof(TestData.MissingCurrentEnumerable),
                    null,
                    typeof(TestData.MissingCurrentEnumerator),
                    null,
                    null
                },
                {
                    typeof(TestData.MissingMoveNextEnumerable<int>),
                    typeof(TestData.MissingMoveNextEnumerable<int>),
                    typeof(TestData.MissingMoveNextEnumerator<int>),
                    null,
                    null,
                    typeof(int)
                },
            };

        [Theory]
        [MemberData(nameof(InvalidAsyncEnumerables))]
        public void IsAsyncEnumerable_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType, Type getAsyncEnumeratorDeclaringType, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsAsyncEnumerable(out var enumerableInfo);

            // Assert   
            Assert.False(result);

            if (getAsyncEnumeratorDeclaringType is null)
            {
                Assert.Null(enumerableInfo.GetEnumerator);
            }
            else
            {
                Assert.NotNull(enumerableInfo.GetEnumerator);
                Assert.Equal("GetAsyncEnumerator", enumerableInfo.GetEnumerator.Name);
                Assert.Equal(getAsyncEnumeratorDeclaringType, enumerableInfo.GetEnumerator.DeclaringType);
                Assert.Empty(enumerableInfo.GetEnumerator.GetParameters());
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
