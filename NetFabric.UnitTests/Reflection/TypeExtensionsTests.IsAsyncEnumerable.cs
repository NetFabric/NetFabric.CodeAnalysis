using NetFabric.TestData;
using System;
using Xunit;

namespace NetFabric.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.AsyncEnumerables), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerable_Should_ReturnTrue(Type type, Type getAsyncEnumeratorDeclaringType, int getAsyncEnumeratorParametersCount, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
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
            Assert.Equal(getAsyncEnumeratorParametersCount, enumerableInfo.GetEnumerator.GetParameters().Length);

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

        [Theory]
        [MemberData(nameof(DataSets.InvalidAsyncEnumerables), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerable_With_MissingFeatures_Should_ReturnFalse(Type type, Type getAsyncEnumeratorDeclaringType, int getAsyncEnumeratorParametersCount, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
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
                Assert.Equal(getAsyncEnumeratorParametersCount, enumerableInfo.GetEnumerator.GetParameters().Length);
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
