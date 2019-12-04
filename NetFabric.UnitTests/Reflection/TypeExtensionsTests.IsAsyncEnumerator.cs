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
        public void IsAsyncEnumerator_Should_ReturnTrue(Type type, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = type.IsAsyncEnumerator(out var enumeratorInfo);

            // Assert   
            Assert.True(result);

            Assert.Equal(currentDeclaringType, enumeratorInfo.EnumeratorType);
            Assert.Equal(itemType, enumeratorInfo.ItemType);

            Assert.NotNull(enumeratorInfo.Current);
            Assert.Equal("Current", enumeratorInfo.Current.Name);
            Assert.Equal(currentDeclaringType, enumeratorInfo.Current.DeclaringType);
            Assert.Equal(itemType, enumeratorInfo.Current.PropertyType);

            Assert.NotNull(enumeratorInfo.MoveNext);
            Assert.Equal("MoveNextAsync", enumeratorInfo.MoveNext.Name);
            Assert.Equal(moveNextAsyncDeclaringType, enumeratorInfo.MoveNext.DeclaringType);
            Assert.Empty(enumeratorInfo.MoveNext.GetParameters());

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.Dispose);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.Dispose);
                Assert.Equal("DisposeAsync", enumeratorInfo.Dispose.Name);
                Assert.Equal(disposeAsyncDeclaringType, enumeratorInfo.Dispose.DeclaringType);
                Assert.Empty(enumeratorInfo.Dispose.GetParameters());
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
        public void IsAsyncEnumerator_With_MissingFeatures_Should_ReturnFalse(Type type, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = type.IsAsyncEnumerator(out var enumeratorInfo);

            // Assert   
            Assert.False(result);

            Assert.Equal(currentDeclaringType, enumeratorInfo.EnumeratorType);
            Assert.Equal(itemType, enumeratorInfo.ItemType);

            if (currentDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.Current);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.Current);
                Assert.Equal("Current", enumeratorInfo.Current.Name);
                Assert.Equal(currentDeclaringType, enumeratorInfo.Current.DeclaringType);
                Assert.Equal(itemType, enumeratorInfo.Current.PropertyType);
            }

            if (moveNextAsyncDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.MoveNext);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.MoveNext);
                Assert.Equal("MoveNextAsync", enumeratorInfo.MoveNext.Name);
                Assert.Equal(moveNextAsyncDeclaringType, enumeratorInfo.MoveNext.DeclaringType);
                Assert.Empty(enumeratorInfo.MoveNext.GetParameters());
            }

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.Dispose);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.Dispose);
                Assert.Equal("DisposeAsync", enumeratorInfo.Dispose.Name);
                Assert.Equal(disposeAsyncDeclaringType, enumeratorInfo.Dispose.DeclaringType);
                Assert.Empty(enumeratorInfo.Dispose.GetParameters());
            }
        }
    }
}
