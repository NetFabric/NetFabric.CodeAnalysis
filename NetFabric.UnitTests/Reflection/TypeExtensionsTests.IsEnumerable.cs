using NetFabric.TestData;
using System;
using Xunit;

namespace NetFabric.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {

        [Theory]
        [MemberData(nameof(DataSets.Enumerables), MemberType = typeof(DataSets))]
        public void IsEnumerable_Should_ReturnTrue(Type type, Type getEnumeratorDeclaringType, Type currentDeclaringType, Type moveNextDeclaringType, Type resetDeclaringType, Type disposeDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = type.IsEnumerable(out var enumerableInfo);

            // Assert   
            Assert.True(result);

            Assert.NotNull(enumerableInfo.GetEnumerator);
            Assert.Equal("GetEnumerator", enumerableInfo.GetEnumerator.Name);
            Assert.Equal(getEnumeratorDeclaringType, enumerableInfo.GetEnumerator.DeclaringType);
            Assert.Empty(enumerableInfo.GetEnumerator.GetParameters());

            var enumeratorInfo = enumerableInfo.EnumeratorInfo;

            Assert.NotNull(enumeratorInfo.Current);
            Assert.Equal("Current", enumeratorInfo.Current.Name);
            Assert.Equal(currentDeclaringType, enumeratorInfo.Current.DeclaringType);
            Assert.Equal(itemType, enumeratorInfo.Current.PropertyType);

            Assert.NotNull(enumeratorInfo.MoveNext);
            Assert.Equal("MoveNext", enumeratorInfo.MoveNext.Name);
            Assert.Equal(moveNextDeclaringType, enumeratorInfo.MoveNext.DeclaringType);
            Assert.Empty(enumeratorInfo.MoveNext.GetParameters());

            if (resetDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.Reset);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.Reset);
                Assert.Equal("Reset", enumeratorInfo.Reset.Name);
                Assert.Equal(resetDeclaringType, enumeratorInfo.Reset.DeclaringType);
                Assert.Empty(enumeratorInfo.Reset.GetParameters());
            }

            if (disposeDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.Dispose);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.Dispose);
                Assert.Equal("Dispose", enumeratorInfo.Dispose.Name);
                Assert.Equal(disposeDeclaringType, enumeratorInfo.Dispose.DeclaringType);
                Assert.Empty(enumeratorInfo.Dispose.GetParameters());
            }
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidEnumerables), MemberType = typeof(DataSets))]
        public void IsEnumerable_With_MissingFeatures_Should_ReturnFalse(Type type, Type getEnumeratorDeclaringType, Type currentDeclaringType, Type moveNextDeclaringType, Type resetDeclaringType, Type disposeDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = type.IsEnumerable(out var enumerableInfo);

            // Assert   
            Assert.False(result);

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

            var enumeratorInfo = enumerableInfo.EnumeratorInfo;

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

            if (moveNextDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.MoveNext);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.MoveNext);
                Assert.Equal("MoveNext", enumeratorInfo.MoveNext.Name);
                Assert.Equal(moveNextDeclaringType, enumeratorInfo.MoveNext.DeclaringType);
                Assert.Empty(enumeratorInfo.MoveNext.GetParameters());
            }

            if (resetDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.Reset);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.Reset);
                Assert.Equal("Dispose", enumeratorInfo.Reset.Name);
                Assert.Equal(resetDeclaringType, enumeratorInfo.Reset.DeclaringType);
                Assert.Empty(enumeratorInfo.Reset.GetParameters());
            }

            if (disposeDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.Dispose);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.Dispose);
                Assert.Equal("Dispose", enumeratorInfo.Dispose.Name);
                Assert.Equal(disposeDeclaringType, enumeratorInfo.Dispose.DeclaringType);
                Assert.Empty(enumeratorInfo.Dispose.GetParameters());
            }
        }
    }
}
