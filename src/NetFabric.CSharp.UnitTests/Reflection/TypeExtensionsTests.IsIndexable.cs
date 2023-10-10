using NetFabric.CSharp.TestData;
using System;
using Xunit;

namespace NetFabric.Reflection.CSharp.UnitTests;

public partial class TypeExtensionsTests
{

    [Theory]
    [MemberData(nameof(IndexableDataSets.Arrays), MemberType = typeof(IndexableDataSets))]
    [MemberData(nameof(IndexableDataSets.Indexables), MemberType = typeof(IndexableDataSets))]
    public void IsIndexable_Should_ReturnTrue(Type type, IndexableDataSets.IndexableTestData testData)
    {
        // Arrange

        // Act
        var result = type.IsIndexable(out var indexableInfo, out var error);

        // Assert   
        Assert.True(result, error.ToString());
        Assert.NotNull(indexableInfo);
        Assert.Equal(IsIndexableError.None, error);

        var countOrLength = indexableInfo!.CountOrLength;
        Assert.NotNull(countOrLength);
        Assert.True(countOrLength.Name == NameOf.Count || countOrLength.Name == NameOf.Length);

        var indexer = indexableInfo!.Indexer;

        if (type.IsArray)
        {
            Assert.Equal(typeof(Array), countOrLength.DeclaringType);

            Assert.Null(indexer);
        }
        else
        {
            Assert.Equal(testData.CountOrLengthDeclaringType, countOrLength.DeclaringType);

            Assert.NotNull(indexer);
            Assert.Equal(NameOf.Indexer, indexer.Name);
            Assert.Equal(testData.IndexerDeclaringType, indexer.DeclaringType);
        }
    }

    [Theory]
    [MemberData(nameof(IndexableDataSets.InvalidIndexables), MemberType = typeof(IndexableDataSets))]
    public void IsIndexable_With_MissingFeatures_Should_ReturnFalse(Type type, IsIndexableError expectedError)
    {
        // Arrange

        // Act
        var result = type.IsIndexable(out _, out var error);

        // Assert   
        Assert.False(result);
        Assert.Equal(expectedError, error);
    }
}
