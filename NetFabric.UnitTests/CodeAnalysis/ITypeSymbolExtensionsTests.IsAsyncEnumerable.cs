using NetFabric.TestData;
using System;
using Xunit;

namespace NetFabric.CodeAnalysis.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.AsyncEnumerables), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerable_Should_ReturnTrue(Type enumerableType, Type getAsyncEnumeratorDeclaringType, int getAsyncEnumeratorParametersCount, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange
            var compilation = Utils.Compile(
                @"TestData/Enumerables.cs",
                @"TestData/Enumerators.cs",
                @"TestData/RangeAsyncEnumerable.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumerableType);

            // Act
            var result = typeSymbol.IsAsyncEnumerable(compilation, out var enumerableSymbols);

            // Assert   
            Assert.True(result);

            Assert.Equal(enumerableSymbols.GetEnumerator?.ContainingType.MetadataName, enumerableSymbols.EnumerableType?.MetadataName);
            Assert.Equal(enumerableSymbols.GetEnumerator?.ReturnType.MetadataName, enumerableSymbols.EnumeratorType?.MetadataName);
            Assert.Equal(itemType?.Name, enumerableSymbols.ItemType?.MetadataName);

            Assert.NotNull(enumerableSymbols.GetEnumerator);
            Assert.Equal("GetAsyncEnumerator", enumerableSymbols.GetEnumerator.Name);
            Assert.Equal(getAsyncEnumeratorDeclaringType.Name, enumerableSymbols.GetEnumerator.ContainingType.MetadataName);
            Assert.Equal(getAsyncEnumeratorParametersCount, enumerableSymbols.GetEnumerator.Parameters.Length);

            Assert.NotNull(enumerableSymbols.Current);
            Assert.Equal("Current", enumerableSymbols.Current.Name);
            Assert.Equal(currentDeclaringType.Name, enumerableSymbols.Current.ContainingType.MetadataName);
            Assert.Equal(itemType.Name, enumerableSymbols.Current.Type.MetadataName);

            Assert.NotNull(enumerableSymbols.MoveNext);
            Assert.Equal("MoveNextAsync", enumerableSymbols.MoveNext.Name);
            Assert.Equal(moveNextAsyncDeclaringType.Name, enumerableSymbols.MoveNext.ContainingType.MetadataName);
            Assert.Empty(enumerableSymbols.MoveNext.Parameters);

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(enumerableSymbols.Dispose);
            }
            else
            {
                Assert.NotNull(enumerableSymbols.Dispose);
                Assert.Equal("DisposeAsync", enumerableSymbols.Dispose.Name);
                Assert.Equal(disposeAsyncDeclaringType.Name, enumerableSymbols.Dispose.ContainingType.MetadataName);
                Assert.Empty(enumerableSymbols.Dispose.Parameters);
            }
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidAsyncEnumerables), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerable_With_MissingFeatures_Should_ReturnFalse(Type enumerableType, Type getAsyncEnumeratorDeclaringType, int getAsyncEnumeratorParametersCount, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange
            var compilation = Utils.Compile(
                @"TestData/Enumerables.cs",
                @"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumerableType);

            // Act
            var result = typeSymbol.IsAsyncEnumerable(compilation, out var enumerableSymbols);

            // Assert   
            Assert.False(result);

            Assert.Equal(enumerableSymbols.GetEnumerator?.ContainingType.MetadataName, enumerableSymbols.EnumerableType?.MetadataName);
            Assert.Equal(enumerableSymbols.GetEnumerator?.ReturnType.MetadataName, enumerableSymbols.EnumeratorType?.MetadataName);
            Assert.Equal(itemType?.Name, enumerableSymbols.ItemType?.MetadataName);

            if (getAsyncEnumeratorDeclaringType is null)
            {
                Assert.Null(enumerableSymbols.GetEnumerator);
            }
            else
            {
                Assert.NotNull(enumerableSymbols.GetEnumerator);
                Assert.Equal("GetAsyncEnumerator", enumerableSymbols.GetEnumerator.Name);
                Assert.Equal(getAsyncEnumeratorDeclaringType.Name, enumerableSymbols.GetEnumerator.ContainingType.MetadataName);
                Assert.Equal(getAsyncEnumeratorParametersCount, enumerableSymbols.GetEnumerator.Parameters.Length);
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

            if (moveNextAsyncDeclaringType is null)
            {
                Assert.Null(enumerableSymbols.MoveNext);
            }
            else
            {
                Assert.NotNull(enumerableSymbols.MoveNext);
                Assert.Equal("MoveNextAsync", enumerableSymbols.MoveNext.Name);
                Assert.Equal(moveNextAsyncDeclaringType.Name, enumerableSymbols.MoveNext.ContainingType.MetadataName);
                Assert.Empty(enumerableSymbols.MoveNext.Parameters);
            }

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(enumerableSymbols.Dispose);
            }
            else
            {
                Assert.NotNull(enumerableSymbols.Dispose);
                Assert.Equal("DisposeAsync", enumerableSymbols.Dispose.Name);
                Assert.Equal(disposeAsyncDeclaringType.Name, enumerableSymbols.Dispose.ContainingType.MetadataName);
                Assert.Empty(enumerableSymbols.Dispose.Parameters);
            }
        }
    }
}
