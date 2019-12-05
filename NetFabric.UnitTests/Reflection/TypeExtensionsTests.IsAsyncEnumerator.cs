using NetFabric.TestData;
using System;
using Xunit;

namespace NetFabric.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.AsyncEnumerators), MemberType = typeof(DataSets))]
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

        [Theory]
        [MemberData(nameof(DataSets.InvalidAsyncEnumerators), MemberType = typeof(DataSets))]
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
