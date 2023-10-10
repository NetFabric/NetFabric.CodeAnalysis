using NetFabric.CSharp.TestData;
using System;
using Xunit;

namespace NetFabric.CodeAnalysis.CSharp.UnitTests;

public partial class ITypeSymbolExtensionsTests
{
    [Theory]
    [MemberData(nameof(AsyncEnumerableDataSets.AsyncEnumerables), MemberType = typeof(AsyncEnumerableDataSets))]
    [MemberData(nameof(AsyncEnumerableDataSets.CodeAnalysisAsyncEnumerables), MemberType = typeof(AsyncEnumerableDataSets))]
    public void IsAsyncEnumerable_Should_ReturnTrue(Type enumerableType, AsyncEnumerableDataSets.AsyncEnumerableTestData enumerableTestData)
    {
        // Arrange
        var compilation = Utils.Compile(
            @"TestData/AsyncEnumerables.cs",
            @"TestData/AsyncEnumerators.cs",
            @"TestData/RangeAsyncEnumerable.cs",
            @"../../../../NetFabric.Reflection/Reflection/AsyncEnumerableWrapper.cs");
        var typeSymbol = compilation.GetTypeSymbol(enumerableType);

        // Act
        var result = typeSymbol.IsAsyncEnumerable(compilation, out var enumerableSymbols, out var error);

        // Assert   
        Assert.True(result, error.ToString());
        Assert.NotNull(enumerableSymbols);
        Assert.Equal(IsAsyncEnumerableError.None, error);

        var getAsyncEnumerator = enumerableSymbols!.GetAsyncEnumerator;
        Assert.NotNull(getAsyncEnumerator);
        Assert.Equal("GetAsyncEnumerator", getAsyncEnumerator.Name);
        Assert.Equal(enumerableTestData.GetAsyncEnumeratorDeclaringType.Name, getAsyncEnumerator.ContainingType.MetadataName);

        var enumeratorSymbols = enumerableSymbols.EnumeratorSymbols;
        Assert.NotNull(enumeratorSymbols);

        var enumeratorTestData = enumerableTestData.AsyncEnumeratorTestData;
        
        var current = enumeratorSymbols.Current;
        Assert.NotNull(current);
        Assert.Equal(NameOf.Current, current!.Name);
        Assert.Equal(enumeratorTestData.CurrentDeclaringType.Name, current!.ContainingType.MetadataName);
        Assert.Equal(enumeratorTestData.ItemType.Name, current!.Type.MetadataName);

        var moveNextAsync = enumeratorSymbols.MoveNextAsync;
        Assert.NotNull(moveNextAsync);
        Assert.Equal("MoveNextAsync", moveNextAsync!.Name);
        Assert.Equal(enumeratorTestData.MoveNextAsyncDeclaringType.Name, moveNextAsync!.ContainingType.MetadataName);
        Assert.Empty(moveNextAsync!.Parameters);

        var disposeAsync = enumeratorSymbols.DisposeAsync;
        if (enumeratorTestData.DisposeAsyncDeclaringType is null)
        {
            Assert.Null(disposeAsync);
        }
        else
        {
            Assert.NotNull(disposeAsync);
            Assert.Equal("DisposeAsync", disposeAsync!.Name);
            Assert.Equal(enumeratorTestData.DisposeAsyncDeclaringType.Name, disposeAsync!.ContainingType.MetadataName);
            Assert.Empty(disposeAsync!.Parameters);
        }
        
        Assert.Equal(enumeratorTestData.IsValueType, enumeratorSymbols.IsValueType);
    }

    [Theory]
    [MemberData(nameof(AsyncEnumerableDataSets.InvalidAsyncEnumerables), MemberType = typeof(AsyncEnumerableDataSets))]
    public void IsAsyncEnumerable_With_MissingFeatures_Should_ReturnFalse(Type enumerableType, IsAsyncEnumerableError expectedError)
    {
        // Arrange
        var compilation = Utils.Compile(
            @"TestData/AsyncEnumerables.cs",
            @"TestData/AsyncEnumerators.cs");
        var typeSymbol = compilation.GetTypeSymbol(enumerableType);

        // Act
        var result = typeSymbol.IsAsyncEnumerable(compilation, out _, out var error);

        // Assert   
        Assert.False(result);
        Assert.Equal(expectedError, error);
    }
}
