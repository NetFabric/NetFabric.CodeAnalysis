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

            Assert.NotNull(enumerableSymbols.GetAsyncEnumerator);
            Assert.Equal("GetAsyncEnumerator", enumerableSymbols.GetAsyncEnumerator.Name);
            Assert.Equal(getAsyncEnumeratorDeclaringType.Name, enumerableSymbols.GetAsyncEnumerator.ContainingType.MetadataName);
            Assert.Equal(getAsyncEnumeratorParametersCount, enumerableSymbols.GetAsyncEnumerator.Parameters.Length);

            var enumeratorSymbols = enumerableSymbols.EnumeratorSymbols;

            Assert.NotNull(enumeratorSymbols.Current);
            Assert.Equal("Current", enumeratorSymbols.Current.Name);
            Assert.Equal(currentDeclaringType.Name, enumeratorSymbols.Current.ContainingType.MetadataName);
            Assert.Equal(itemType.Name, enumeratorSymbols.Current.Type.MetadataName);

            Assert.NotNull(enumeratorSymbols.MoveNextAsync);
            Assert.Equal("MoveNextAsync", enumeratorSymbols.MoveNextAsync.Name);
            Assert.Equal(moveNextAsyncDeclaringType.Name, enumeratorSymbols.MoveNextAsync.ContainingType.MetadataName);
            Assert.Empty(enumeratorSymbols.MoveNextAsync.Parameters);

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols.DisposeAsync);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.DisposeAsync);
                Assert.Equal("DisposeAsync", enumeratorSymbols.DisposeAsync.Name);
                Assert.Equal(disposeAsyncDeclaringType.Name, enumeratorSymbols.DisposeAsync.ContainingType.MetadataName);
                Assert.Empty(enumeratorSymbols.DisposeAsync.Parameters);
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

            if (getAsyncEnumeratorDeclaringType is null)
            {
                Assert.Null(enumerableSymbols.GetAsyncEnumerator);
            }
            else
            {
                Assert.NotNull(enumerableSymbols.GetAsyncEnumerator);
                Assert.Equal("GetAsyncEnumerator", enumerableSymbols.GetAsyncEnumerator.Name);
                Assert.Equal(getAsyncEnumeratorDeclaringType.Name, enumerableSymbols.GetAsyncEnumerator.ContainingType.MetadataName);
                Assert.Equal(getAsyncEnumeratorParametersCount, enumerableSymbols.GetAsyncEnumerator.Parameters.Length);
            }

            var enumeratorSymbols = enumerableSymbols.EnumeratorSymbols;

            if (currentDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols.Current);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.Current);
                Assert.Equal("Current", enumeratorSymbols.Current.Name);
                Assert.Equal(currentDeclaringType.Name, enumeratorSymbols.Current.ContainingType.MetadataName);
                Assert.Equal(itemType.Name, enumeratorSymbols.Current.Type.MetadataName);
            }

            if (moveNextAsyncDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols.MoveNextAsync);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.MoveNextAsync);
                Assert.Equal("MoveNextAsync", enumeratorSymbols.MoveNextAsync.Name);
                Assert.Equal(moveNextAsyncDeclaringType.Name, enumeratorSymbols.MoveNextAsync.ContainingType.MetadataName);
                Assert.Empty(enumeratorSymbols.MoveNextAsync.Parameters);
            }

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols.DisposeAsync);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.DisposeAsync);
                Assert.Equal("DisposeAsync", enumeratorSymbols.DisposeAsync.Name);
                Assert.Equal(disposeAsyncDeclaringType.Name, enumeratorSymbols.DisposeAsync.ContainingType.MetadataName);
                Assert.Empty(enumeratorSymbols.DisposeAsync.Parameters);
            }
        }
    }
}
