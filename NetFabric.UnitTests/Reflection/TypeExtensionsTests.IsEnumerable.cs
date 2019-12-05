using NetFabric.TestData;
using System;
using Xunit;

namespace NetFabric.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {

        [Theory]
        [MemberData(nameof(DataSets.Enumerables), MemberType = typeof(DataSets))]
        public void IsEnumerable_Should_ReturnTrue(Type type, Type getEnumeratorDeclaringType, Type currentDeclaringType, Type moveNextDeclaringType, Type disposeDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = type.IsEnumerable(out var enumerableInfo);

            // Assert   
            Assert.True(result);

            Assert.Equal(getEnumeratorDeclaringType, enumerableInfo.EnumerableType);
            Assert.Equal(enumerableInfo.GetEnumerator?.ReturnType, enumerableInfo.EnumeratorType);
            Assert.Equal(itemType, enumerableInfo.ItemType);

            Assert.NotNull(enumerableInfo.GetEnumerator);
            Assert.Equal("GetEnumerator", enumerableInfo.GetEnumerator.Name);
            Assert.Equal(getEnumeratorDeclaringType, enumerableInfo.GetEnumerator.DeclaringType);
            Assert.Empty(enumerableInfo.GetEnumerator.GetParameters());

            Assert.NotNull(enumerableInfo.Current);
            Assert.Equal("Current", enumerableInfo.Current.Name);
            Assert.Equal(currentDeclaringType, enumerableInfo.Current.DeclaringType);
            Assert.Equal(itemType, enumerableInfo.Current.PropertyType);

            Assert.NotNull(enumerableInfo.MoveNext);
            Assert.Equal("MoveNext", enumerableInfo.MoveNext.Name);
            Assert.Equal(moveNextDeclaringType, enumerableInfo.MoveNext.DeclaringType);
            Assert.Empty(enumerableInfo.MoveNext.GetParameters());

            if (disposeDeclaringType is null)
            {
                Assert.Null(enumerableInfo.Dispose);
            }
            else
            {
                Assert.NotNull(enumerableInfo.Dispose);
                Assert.Equal("Dispose", enumerableInfo.Dispose.Name);
                Assert.Equal(disposeDeclaringType, enumerableInfo.Dispose.DeclaringType);
                Assert.Empty(enumerableInfo.Dispose.GetParameters());
            }
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidEnumerables), MemberType = typeof(DataSets))]
        public void IsEnumerable_With_MissingFeatures_Should_ReturnFalse(Type type, Type getEnumeratorDeclaringType, Type currentDeclaringType, Type moveNextDeclaringType, Type disposeDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = type.IsEnumerable(out var enumerableInfo);

            // Assert   
            Assert.False(result);

            Assert.Equal(getEnumeratorDeclaringType, enumerableInfo.EnumerableType);
            Assert.Equal(enumerableInfo.GetEnumerator?.ReturnType, enumerableInfo.EnumeratorType);
            Assert.Equal(itemType, enumerableInfo.ItemType);

            if (getEnumeratorDeclaringType is null)
            {
                Assert.Null(enumerableInfo.GetEnumerator);
            }
            else
            {
                Assert.NotNull(enumerableInfo.GetEnumerator);
                Assert.Equal("GetEnumerator", enumerableInfo.GetEnumerator.Name);
                Assert.Equal(getEnumeratorDeclaringType, enumerableInfo.GetEnumerator.DeclaringType);
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

            if (moveNextDeclaringType is null)
            {
                Assert.Null(enumerableInfo.MoveNext);
            }
            else
            {
                Assert.NotNull(enumerableInfo.MoveNext);
                Assert.Equal("MoveNext", enumerableInfo.MoveNext.Name);
                Assert.Equal(moveNextDeclaringType, enumerableInfo.MoveNext.DeclaringType);
                Assert.Empty(enumerableInfo.MoveNext.GetParameters());
            }

            if (disposeDeclaringType is null)
            {
                Assert.Null(enumerableInfo.Dispose);
            }
            else
            {
                Assert.NotNull(enumerableInfo.Dispose);
                Assert.Equal("Dispose", enumerableInfo.Dispose.Name);
                Assert.Equal(disposeDeclaringType, enumerableInfo.Dispose.DeclaringType);
                Assert.Empty(enumerableInfo.Dispose.GetParameters());
            }
        }
    }
}
