using System;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        public static TheoryData<Type, Type, Type, Type, Type> AsyncEnumerators =>
            new TheoryData<Type, Type, Type, Type, Type>
            {
                { 
                    typeof(TestData.AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(TestData.AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(TestData.AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    null,
                    typeof(int) 
                },
                { 
                    typeof(TestData.ExplicitAsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncDisposable),
                    typeof(int) 
                },
            };

        [Theory]
        [MemberData(nameof(AsyncEnumerators))]
        public void IsAsyncEnumerator_Should_ReturnTrue(Type enumeratorType, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsAsyncEnumerator(out var current, out var moveNextAsync, out var disposeAsync);

            // Assert   
            Assert.True(result);

            Assert.NotNull(current);
            Assert.Equal("Current", current.Name);
            Assert.Equal(currentDeclaringType, current.DeclaringType);
            Assert.Equal(itemType, current.PropertyType);

            Assert.NotNull(moveNextAsync);
            Assert.Equal("MoveNextAsync", moveNextAsync.Name);
            Assert.Equal(moveNextAsyncDeclaringType, moveNextAsync.DeclaringType);
            Assert.Empty(moveNextAsync.GetParameters());

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(disposeAsync);
            }
            else
            {
                Assert.NotNull(disposeAsync);
                Assert.Equal("DisposeAsync", disposeAsync.Name);
                Assert.Equal(disposeAsyncDeclaringType, disposeAsync.DeclaringType);
                Assert.Empty(disposeAsync.GetParameters());
            }
        }

        public static TheoryData<Type, Type, Type, Type, Type> InvalidAsyncEnumerators =>
            new TheoryData<Type, Type, Type, Type, Type>
            {
                { 
                    typeof(TestData.MissingCurrentAndMoveNextEnumerator), 
                    null,
                    null,
                    null,
                    null
                },
                { 
                    typeof(TestData.MissingCurrentEnumerator),
                    null,
                    typeof(TestData.MissingCurrentEnumerator),
                    null,
                    null
                },
                { 
                    typeof(TestData.MissingMoveNextEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(TestData.MissingMoveNextEnumerator<>).MakeGenericType(typeof(int)),
                    null,
                    null,
                    typeof(int) 
                },
            };

        [Theory]
        [MemberData(nameof(InvalidAsyncEnumerators))]
        public void IsAsyncEnumerator_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsAsyncEnumerator(out var current, out var moveNextAsync, out var disposeAsync);

            // Assert   
            Assert.False(result);

            if (currentDeclaringType is null)
            {
                Assert.Null(current);
            }
            else
            {
                Assert.NotNull(current);
                Assert.Equal("Current", current.Name);
                Assert.Equal(currentDeclaringType, current.DeclaringType);
                Assert.Equal(itemType, current.PropertyType);
            }

            if (moveNextAsyncDeclaringType is null)
            {
                Assert.Null(moveNextAsync);
            }
            else
            {
                Assert.NotNull(moveNextAsync);
                Assert.Equal("MoveNextAsync", moveNextAsync.Name);
                Assert.Equal(moveNextAsyncDeclaringType, moveNextAsync.DeclaringType);
                Assert.Empty(moveNextAsync.GetParameters());
            }

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(disposeAsync);
            }
            else
            {
                Assert.NotNull(disposeAsync);
                Assert.Equal("DisposeAsync", disposeAsync.Name);
                Assert.Equal(disposeAsyncDeclaringType, disposeAsync.DeclaringType);
                Assert.Empty(disposeAsync.GetParameters());
            }
        }
    }
}
