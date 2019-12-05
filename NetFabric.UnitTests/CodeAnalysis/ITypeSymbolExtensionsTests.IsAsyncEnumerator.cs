using NetFabric.TestData;
using System;
using Xunit;

namespace NetFabric.CodeAnalysis.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.AsyncEnumerators), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerator_Should_ReturnTrue(Type enumeratorType, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumeratorType);

            // Act
            var result = typeSymbol.IsAsyncEnumerator(compilation, out var enumeratorSymbols);

            // Assert   
            Assert.True(result);

            Assert.Equal(currentDeclaringType?.Name, enumeratorSymbols.EnumeratorType?.MetadataName);
            Assert.Equal(itemType?.Name, enumeratorSymbols.ItemType?.MetadataName);

            Assert.NotNull(enumeratorSymbols.Current);
            Assert.Equal("Current", enumeratorSymbols.Current.Name);
            Assert.Equal(currentDeclaringType.Name, enumeratorSymbols.Current.ContainingType.MetadataName);
            Assert.Equal(itemType.Name, enumeratorSymbols.Current.Type.MetadataName);

            Assert.NotNull(enumeratorSymbols.MoveNext);
            Assert.Equal("MoveNextAsync", enumeratorSymbols.MoveNext.Name);
            Assert.Equal(moveNextAsyncDeclaringType.Name, enumeratorSymbols.MoveNext.ContainingType.MetadataName);
            Assert.Empty(enumeratorSymbols.MoveNext.Parameters);

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols.Dispose);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.Dispose);
                Assert.Equal("DisposeAsync", enumeratorSymbols.Dispose.Name);
                Assert.Equal(disposeAsyncDeclaringType.Name, enumeratorSymbols.Dispose.ContainingType.MetadataName);
                Assert.Empty(enumeratorSymbols.Dispose.Parameters);
            }
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidAsyncEnumerators), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerator_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumeratorType);

            // Act
            var result = typeSymbol.IsAsyncEnumerator(compilation, out var enumeratorSymbols);

            // Assert   
            Assert.False(result);

            Assert.Equal(currentDeclaringType?.Name, enumeratorSymbols.EnumeratorType?.MetadataName);
            Assert.Equal(itemType?.Name, enumeratorSymbols.ItemType?.MetadataName);

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
                Assert.Null(enumeratorSymbols.MoveNext);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.MoveNext);
                Assert.Equal("MoveNextAsync", enumeratorSymbols.MoveNext.Name);
                Assert.Equal(moveNextAsyncDeclaringType.Name, enumeratorSymbols.MoveNext.ContainingType.MetadataName);
                Assert.Empty(enumeratorSymbols.MoveNext.Parameters);
            }

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols.Dispose);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.Dispose);
                Assert.Equal("DisposeAsync", enumeratorSymbols.Dispose.Name);
                Assert.Equal(disposeAsyncDeclaringType.Name, enumeratorSymbols.Dispose.ContainingType.MetadataName);
                Assert.Empty(enumeratorSymbols.Dispose.Parameters);
            }
        }
    }
}
