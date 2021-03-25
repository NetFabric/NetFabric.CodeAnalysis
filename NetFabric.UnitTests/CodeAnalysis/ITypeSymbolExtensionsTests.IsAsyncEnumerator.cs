using NetFabric.TestData;
using System;
using Xunit;

namespace NetFabric.CodeAnalysis.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.AsyncEnumerators), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerator_Should_ReturnTrue(Type enumeratorType, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type? disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumeratorType);

            // Act
            var result = typeSymbol.IsAsyncEnumerator(compilation, out var enumeratorSymbols);

            // Assert   
            Assert.True(result);

            Assert.NotNull(enumeratorSymbols!.Current);
            Assert.Equal("Current", enumeratorSymbols!.Current!.Name);
            Assert.Equal(currentDeclaringType.Name, enumeratorSymbols!.Current!.ContainingType.MetadataName);
            Assert.Equal(itemType.Name, enumeratorSymbols!.Current!.Type.MetadataName);

            Assert.NotNull(enumeratorSymbols!.MoveNextAsync);
            Assert.Equal("MoveNextAsync", enumeratorSymbols!.MoveNextAsync!.Name);
            Assert.Equal(moveNextAsyncDeclaringType.Name, enumeratorSymbols!.MoveNextAsync!.ContainingType.MetadataName);
            Assert.Empty(enumeratorSymbols!.MoveNextAsync!.Parameters);

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols!.DisposeAsync);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols!.DisposeAsync);
                Assert.Equal("DisposeAsync", enumeratorSymbols!.DisposeAsync!.Name);
                Assert.Equal(disposeAsyncDeclaringType.Name, enumeratorSymbols!.DisposeAsync!.ContainingType.MetadataName);
                Assert.Empty(enumeratorSymbols!.DisposeAsync!.Parameters);
            }
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidAsyncEnumerators), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerator_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType, Type? currentDeclaringType, Type? moveNextAsyncDeclaringType, Type? itemType)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumeratorType);

            // Act
            var result = typeSymbol.IsAsyncEnumerator(compilation, out var _, out var errors);

            // Assert   
            Assert.False(result);

            if (currentDeclaringType is null)
                Assert.True(errors.HasFlag(Errors.MissingCurrent));
            else
                Assert.False(errors.HasFlag(Errors.MissingCurrent));

            if (moveNextAsyncDeclaringType is null)
                Assert.True(errors.HasFlag(Errors.MissingMoveNext));
            else
                Assert.False(errors.HasFlag(Errors.MissingMoveNext));
        }
    }
}
