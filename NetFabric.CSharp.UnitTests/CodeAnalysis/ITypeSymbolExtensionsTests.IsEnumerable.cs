using NetFabric.CSharp.TestData;
using System;
using Xunit;

namespace NetFabric.CodeAnalysis.CSharp.UnitTests;

public partial class ITypeSymbolExtensionsTests
{
    [Theory]
    [MemberData(nameof(EnumerableDataSets.Arrays), MemberType = typeof(EnumerableDataSets))]
    [MemberData(nameof(EnumerableDataSets.Enumerables), MemberType = typeof(EnumerableDataSets))]
    [MemberData(nameof(EnumerableDataSets.CodeAnalysisEnumerables), MemberType = typeof(EnumerableDataSets))]
    public void IsEnumerable_Should_ReturnTrue(Type enumerableType, EnumerableDataSets.EnumerableTestData enumerableTestData)
    {
        // Arrange
        var compilation = Utils.Compile(
            @"TestData/Enumerables.cs",
            @"TestData/Enumerators.cs",
            @"TestData/RangeEnumerable.cs",
            @"../../../../NetFabric.Reflection/Reflection/EnumerableWrapper.cs");
        var typeSymbol = compilation.GetTypeSymbol(enumerableType);

        // Act
        var result = typeSymbol.IsEnumerable(compilation, out var enumerableSymbols, out var error);

        // Assert   
        Assert.True(result, error.ToString());
        Assert.NotNull(enumerableSymbols);
        Assert.Equal(IsEnumerableError.None, error);

        var getEnumerator = enumerableSymbols!.GetEnumerator;
        Assert.NotNull(getEnumerator);
        Assert.Equal(NameOf.GetEnumerator, getEnumerator.Name);
        Assert.Equal(enumerableTestData.GetEnumeratorDeclaringType.Name, getEnumerator.ContainingType.MetadataName);

        var enumeratorSymbols = enumerableSymbols.EnumeratorSymbols;
        Assert.NotNull(enumeratorSymbols);

        var enumeratorTestData = enumerableTestData.EnumeratorTestData;

        var current = enumeratorSymbols.Current;
        Assert.NotNull(current);
        Assert.Equal(NameOf.Current, current.Name);
        Assert.Equal(enumeratorTestData.CurrentDeclaringType.Name, current.ContainingType.MetadataName);
        if (current is {ReturnsByRef:true} or {ReturnsByRefReadonly:true})
            Assert.Equal(enumeratorTestData.ItemType.Name, current.Type.MetadataName + '&');
        else
            Assert.Equal(enumeratorTestData.ItemType.Name, current.Type.MetadataName);

        var moveNext = enumeratorSymbols.MoveNext;
        Assert.NotNull(moveNext);
        Assert.Equal(NameOf.MoveNext, moveNext.Name);
        Assert.Equal(enumeratorTestData.MoveNextDeclaringType.Name, moveNext.ContainingType.MetadataName);
        Assert.Empty(enumeratorSymbols.MoveNext.Parameters);

        var reset = enumeratorSymbols.Reset;
        if (enumeratorTestData.ResetDeclaringType is null)
        {
            Assert.Null(reset);
        }
        else
        {
            Assert.NotNull(reset);
            Assert.Equal(NameOf.Reset, reset!.Name);
            Assert.Equal(enumeratorTestData.ResetDeclaringType.Name, reset.ContainingType.MetadataName);
            Assert.Empty(reset.Parameters);
        }

        var dispose = enumeratorSymbols.Dispose;
        if (enumeratorTestData.DisposeDeclaringType is null)
        {
            Assert.Null(dispose);
        }
        else
        {
            Assert.NotNull(dispose);
            Assert.Equal(NameOf.Dispose, dispose!.Name);
            Assert.Equal(enumeratorTestData.DisposeDeclaringType.Name, dispose.ContainingType.MetadataName);
            Assert.Empty(dispose.Parameters);
        }
        
        Assert.Equal(enumeratorTestData.IsValueType, enumeratorSymbols.IsValueType);
        Assert.Equal(enumeratorTestData.IsByRefLikeType, enumeratorSymbols.IsRefLikeType);
    }

    [Theory]
    [MemberData(nameof(EnumerableDataSets.InvalidEnumerables), MemberType = typeof(EnumerableDataSets))]
    public void IsEnumerable_With_MissingFeatures_Should_ReturnFalse(Type enumerableType, IsEnumerableError expectedError)
    {
        // Arrange
        var compilation = Utils.Compile(
            @"TestData/Enumerables.cs",
            @"TestData/Enumerators.cs");
        var typeSymbol = compilation.GetTypeSymbol(enumerableType);

        // Act
        var result = typeSymbol.IsEnumerable(compilation, out _, out var error);

        // Assert   
        Assert.False(result);
        Assert.Equal(expectedError, error);
    }
}
