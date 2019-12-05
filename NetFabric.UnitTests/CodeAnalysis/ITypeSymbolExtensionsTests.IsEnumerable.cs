using NetFabric.TestData;
using System;
using Xunit;

namespace NetFabric.CodeAnalysis.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.Enumerables), MemberType = typeof(DataSets))]
        public void IsEnumerable_Should_ReturnTrue(Type enumerableType, Type getEnumeratorDeclaringType, Type currentDeclaringType, Type moveNextDeclaringType, Type disposeDeclaringType, Type itemType)
        {
            // Arrange
            var compilation = Utils.Compile(
                @"TestData/Enumerables.cs",
                @"TestData/Enumerators.cs",
                @"TestData/RangeEnumerable.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumerableType);

            // Act
            var result = typeSymbol.IsEnumerable(compilation, out var enumerableSymbols);

            // Assert   
            Assert.True(result);

            Assert.Equal(enumerableSymbols.GetEnumerator?.ContainingType.MetadataName, enumerableSymbols.EnumerableType?.MetadataName);
            Assert.Equal(enumerableSymbols.GetEnumerator?.ReturnType.MetadataName, enumerableSymbols.EnumeratorType?.MetadataName);
            Assert.Equal(itemType?.Name, enumerableSymbols.ItemType?.MetadataName);

            Assert.NotNull(enumerableSymbols.GetEnumerator);
            Assert.Equal("GetEnumerator", enumerableSymbols.GetEnumerator.Name);
            Assert.Equal(getEnumeratorDeclaringType.Name, enumerableSymbols.GetEnumerator.ContainingType.MetadataName);
            Assert.Empty(enumerableSymbols.GetEnumerator.Parameters);

            Assert.NotNull(enumerableSymbols.Current);
            Assert.Equal("Current", enumerableSymbols.Current.Name);
            Assert.Equal(currentDeclaringType.Name, enumerableSymbols.Current.ContainingType.MetadataName);
            Assert.Equal(itemType.Name, enumerableSymbols.Current.Type.MetadataName);

            Assert.NotNull(enumerableSymbols.MoveNext);
            Assert.Equal("MoveNext", enumerableSymbols.MoveNext.Name);
            Assert.Equal(moveNextDeclaringType.Name, enumerableSymbols.MoveNext.ContainingType.MetadataName);
            Assert.Empty(enumerableSymbols.MoveNext.Parameters);

            if (disposeDeclaringType is null)
            {
                Assert.Null(enumerableSymbols.Dispose);
            }
            else
            {
                Assert.NotNull(enumerableSymbols.Dispose);
                Assert.Equal("Dispose", enumerableSymbols.Dispose.Name);
                Assert.Equal(disposeDeclaringType.Name, enumerableSymbols.Dispose.ContainingType.MetadataName);
                Assert.Empty(enumerableSymbols.Dispose.Parameters);
            }
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidEnumerables), MemberType = typeof(DataSets))]
        public void IsEnumerable_With_MissingFeatures_Should_ReturnFalse(Type enumerableType, Type getEnumeratorDeclaringType, Type currentDeclaringType, Type moveNextDeclaringType, Type disposeDeclaringType, Type itemType)
        {
            // Arrange
            var compilation = Utils.Compile(
                @"TestData/Enumerables.cs",
                @"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumerableType);

            // Act
            var result = typeSymbol.IsEnumerable(compilation, out var enumerableSymbols);

            // Assert   
            Assert.False(result);

            Assert.Equal(enumerableSymbols.GetEnumerator?.ContainingType.MetadataName, enumerableSymbols.EnumerableType?.MetadataName);
            Assert.Equal(enumerableSymbols.GetEnumerator?.ReturnType.MetadataName, enumerableSymbols.EnumeratorType?.MetadataName);
            Assert.Equal(itemType?.Name, enumerableSymbols.ItemType?.MetadataName);

            if (getEnumeratorDeclaringType is null)
            {
                Assert.Null(enumerableSymbols.GetEnumerator);
            }
            else
            {
                Assert.NotNull(enumerableSymbols.GetEnumerator);
                Assert.Equal("GetEnumerator", enumerableSymbols.GetEnumerator.Name);
                Assert.Equal(getEnumeratorDeclaringType.Name, enumerableSymbols.GetEnumerator.ContainingType.MetadataName);
                Assert.Empty(enumerableSymbols.GetEnumerator.Parameters);
            }

            if (currentDeclaringType is null)
            {
                Assert.Null(enumerableSymbols.Current);
            }
            else
            {
                Assert.NotNull(enumerableSymbols.Current);
                Assert.Equal("Current", enumerableSymbols.Current.Name);
                Assert.Equal(currentDeclaringType.Name, enumerableSymbols.Current.ContainingType.MetadataName);
                Assert.Equal(itemType.Name, enumerableSymbols.Current.Type.MetadataName);
            }

            if (moveNextDeclaringType is null)
            {
                Assert.Null(enumerableSymbols.MoveNext);
            }
            else
            {
                Assert.NotNull(enumerableSymbols.MoveNext);
                Assert.Equal("MoveNext", enumerableSymbols.MoveNext.Name);
                Assert.Equal(moveNextDeclaringType.Name, enumerableSymbols.MoveNext.ContainingType.MetadataName);
                Assert.Empty(enumerableSymbols.MoveNext.Parameters);
            }

            if (disposeDeclaringType is null)
            {
                Assert.Null(enumerableSymbols.Dispose);
            }
            else
            {
                Assert.NotNull(enumerableSymbols.Dispose);
                Assert.Equal("Dispose", enumerableSymbols.Dispose.Name);
                Assert.Equal(disposeDeclaringType.Name, enumerableSymbols.Dispose.ContainingType.MetadataName);
                Assert.Empty(enumerableSymbols.Dispose.Parameters);
            }
        }
    }
}
