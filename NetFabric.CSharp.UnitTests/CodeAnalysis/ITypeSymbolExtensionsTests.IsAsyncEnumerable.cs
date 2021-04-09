using NetFabric.CSharp.TestData;
using System;
using Xunit;

namespace NetFabric.CodeAnalysis.CSharp.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.AsyncEnumerables), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerable_Should_ReturnTrue(Type enumerableType, Type getAsyncEnumeratorDeclaringType, int getAsyncEnumeratorParametersCount, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type? disposeAsyncDeclaringType, Type itemType, bool isValueType)
        {
            // Arrange
            var compilation = Utils.Compile(
                @"TestData/AsyncEnumerables.cs",
                @"TestData/AsyncEnumerators.cs",
                @"TestData/RangeAsyncEnumerable.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumerableType);

            // Act
            var result = typeSymbol.IsAsyncEnumerable(compilation, out var enumerableSymbols);

            // Assert   
            Assert.True(result);
            Assert.NotNull(enumerableSymbols);

            var getAsyncEnumerator = enumerableSymbols!.GetAsyncEnumerator;
            Assert.NotNull(getAsyncEnumerator);
            Assert.Equal("GetAsyncEnumerator", getAsyncEnumerator.Name);
            Assert.Equal(getAsyncEnumeratorDeclaringType.Name, getAsyncEnumerator.ContainingType.MetadataName);
            Assert.Equal(getAsyncEnumeratorParametersCount, getAsyncEnumerator.Parameters.Length);

            var enumeratorSymbols = enumerableSymbols.EnumeratorSymbols;
            Assert.NotNull(enumeratorSymbols);

            var current = enumeratorSymbols.Current;
            Assert.NotNull(current);
            Assert.Equal("Current", current!.Name);
            Assert.Equal(currentDeclaringType.Name, current!.ContainingType.MetadataName);
            Assert.Equal(itemType.Name, current!.Type.MetadataName);

            var moveNextAsync = enumeratorSymbols.MoveNextAsync;
            Assert.NotNull(moveNextAsync);
            Assert.Equal("MoveNextAsync", moveNextAsync!.Name);
            Assert.Equal(moveNextAsyncDeclaringType.Name, moveNextAsync!.ContainingType.MetadataName);
            Assert.Empty(moveNextAsync!.Parameters);

            var disposeAsync = enumeratorSymbols.DisposeAsync;
            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(disposeAsync);
            }
            else
            {
                Assert.NotNull(disposeAsync);
                Assert.Equal("DisposeAsync", disposeAsync!.Name);
                Assert.Equal(disposeAsyncDeclaringType.Name, disposeAsync!.ContainingType.MetadataName);
                Assert.Empty(disposeAsync!.Parameters);
            }
            
            Assert.Equal(isValueType, enumeratorSymbols.IsValueType);
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidAsyncEnumerables), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerable_With_MissingFeatures_Should_ReturnFalse(Type enumerableType, Type? getAsyncEnumeratorDeclaringType, int getAsyncEnumeratorParametersCount, Type? currentDeclaringType, Type? moveNextAsyncDeclaringType, Type? itemType)
        {
            // Arrange
            var compilation = Utils.Compile(
                @"TestData/AsyncEnumerables.cs",
                @"TestData/AsyncEnumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumerableType);

            // Act
            var result = typeSymbol.IsAsyncEnumerable(compilation, out _, out var errors);

            // Assert   
            Assert.False(result);

            if (getAsyncEnumeratorDeclaringType is null)
            {
                Assert.True(errors.HasFlag(Errors.MissingGetEnumerator));
            }
            else
            {
                Assert.False(errors.HasFlag(Errors.MissingGetEnumerator));

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
}
