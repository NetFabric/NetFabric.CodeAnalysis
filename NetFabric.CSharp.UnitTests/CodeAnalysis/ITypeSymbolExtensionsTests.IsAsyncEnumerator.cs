using System;
using NetFabric.CSharp.TestData;
using Xunit;

namespace NetFabric.CodeAnalysis.CSharp.UnitTests;

public partial class ITypeSymbolExtensionsTests
{
    [Theory]
    [MemberData(nameof(DataSets.AsyncEnumerators), MemberType = typeof(DataSets))]
    public void IsAsyncEnumerator_Should_ReturnTrue(Type enumeratorType)
    {
        // Arrange
        var compilation = Utils.Compile(@"TestData/AsyncEnumerators.cs");
        var typeSymbol = compilation.GetTypeSymbol(enumeratorType);

        // Act
        var result = typeSymbol.IsAsyncEnumerator(compilation);

        // Assert   
        Assert.True(result);
    }

    [Theory]
    [MemberData(nameof(DataSets.InvalidAsyncEnumerators), MemberType = typeof(DataSets))]
    public void IsAsyncEnumerator_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType, bool missingCurrent, bool missingMoveNext)
    {
        // Arrange
        var compilation = Utils.Compile(@"TestData/AsyncEnumerators.cs");
        var typeSymbol = compilation.GetTypeSymbol(enumeratorType);

        // Act
        var result = typeSymbol.IsAsyncEnumerator(compilation, out var errors);

        // Assert   
        Assert.False(result);
        Assert.Equal(missingCurrent, errors.HasFlag(Errors.MissingCurrent));
        Assert.Equal(missingMoveNext, errors.HasFlag(Errors.MissingMoveNext));
    }
}