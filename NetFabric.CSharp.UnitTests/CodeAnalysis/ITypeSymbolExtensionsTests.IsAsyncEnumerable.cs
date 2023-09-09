using NetFabric.CSharp.TestData;
using System;
using Xunit;

namespace NetFabric.CodeAnalysis.CSharp.UnitTests;

public partial class ITypeSymbolExtensionsTests
{
    [Theory]
    [MemberData(nameof(DataSets.AsyncEnumerables), MemberType = typeof(DataSets))]
    [MemberData(nameof(DataSets.CodeAnalysisAsyncEnumerables), MemberType = typeof(DataSets))]
    public void IsAsyncEnumerable_Should_ReturnTrue(Type enumerableType, DataSets.AsyncEnumerableTestData enumerableTestData)
    {
        // Arrange
        var compilation = Utils.Compile(
            @"TestData/AsyncEnumerables.cs",
            @"TestData/AsyncEnumerators.cs",
            @"TestData/RangeAsyncEnumerable.cs",
            @"../../../../NetFabric.Reflection/Reflection/AsyncEnumerableWrapper.cs");
        var typeSymbol = compilation.GetTypeSymbol(enumerableType);

        // Act
        var result = typeSymbol.IsAsyncEnumerable(compilation, out var enumerableSymbols, out var errors);

        // Assert   
        Assert.True(result, errors.ToString());
        Assert.NotNull(enumerableSymbols);
        Assert.Equal(Errors.None, errors);

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
