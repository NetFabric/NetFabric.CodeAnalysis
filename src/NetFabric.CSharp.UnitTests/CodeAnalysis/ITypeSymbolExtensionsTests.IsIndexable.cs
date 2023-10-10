using NetFabric.CSharp.TestData;
using System;
using Xunit;

namespace NetFabric.CodeAnalysis.CSharp.UnitTests;

public partial class ITypeSymbolExtensionsTests
{
    [Theory]
    [MemberData(nameof(IndexableDataSets.Arrays), MemberType = typeof(IndexableDataSets))]
    [MemberData(nameof(IndexableDataSets.Indexables), MemberType = typeof(IndexableDataSets))]
    public void IsIndexable_Should_ReturnTrue(Type enumerableType, IndexableDataSets.IndexableTestData testData)
    {
        // Arrange
        var compilation = Utils.Compile(
            @"TestData/Indexables.cs"
        );
        var typeSymbol = compilation.GetTypeSymbol(enumerableType);

        // Act
        var result = typeSymbol.IsIndexable(out var indexableSymbols, out var error);

        // Assert   
        Assert.True(result, error.ToString());
        Assert.NotNull(indexableSymbols);
        Assert.Equal(IsIndexableError.None, error);

        //var indexer = indexableSymbols!.Indexer;
        //Assert.NotNull(indexer);
        //Assert.Equal(NameOf.Indexer, indexer.Name);
        //Assert.Equal(testData.IndexerDeclaringType.Name, indexer.ContainingType.MetadataName);

        //var countOrLength = indexableSymbols!.CountOrLength;
        //Assert.NotNull(countOrLength);
        //Assert.True(countOrLength.Name == NameOf.Count || countOrLength.Name == NameOf.Length);
        //Assert.Equal(testData.CountOrLengthDeclaringType.Name, countOrLength.ContainingType.MetadataName);

        var countOrLength = indexableSymbols!.CountOrLength;
        Assert.NotNull(countOrLength);
        Assert.True(countOrLength.Name == NameOf.Count || countOrLength.Name == NameOf.Length);

        var indexer = indexableSymbols!.Indexer;

        if (enumerableType.IsArray)
        {
            var arrayType = compilation.GetTypeSymbol(typeof(Array));
            Assert.Equal(arrayType, countOrLength.ContainingType);

            Assert.Null(indexer);
        }
        else
        {
            Assert.Equal(testData.CountOrLengthDeclaringType.Name, countOrLength.ContainingType.MetadataName);

            Assert.NotNull(indexer);
            Assert.Equal(NameOf.Indexer, indexer.Name);
            Assert.Equal(testData.IndexerDeclaringType.Name, indexer.ContainingType.MetadataName);
        }
    }

    [Theory]
    [MemberData(nameof(IndexableDataSets.InvalidIndexables), MemberType = typeof(IndexableDataSets))]
    public void IsIndexable_With_MissingFeatures_Should_ReturnFalse(Type enumerableType, IsIndexableError expectedError)
    {
        // Arrange
        var compilation = Utils.Compile(
            @"TestData/Indexables.cs"
        );
        var typeSymbol = compilation.GetTypeSymbol(enumerableType);

        // Act
        var result = typeSymbol.IsIndexable(out _, out var error);

        // Assert   
        Assert.False(result);
        Assert.Equal(expectedError, error);
    }
}
