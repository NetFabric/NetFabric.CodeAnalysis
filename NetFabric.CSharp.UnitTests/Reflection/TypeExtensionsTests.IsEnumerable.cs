using NetFabric.CSharp.TestData;
using System;
using Xunit;

namespace NetFabric.Reflection.CSharp.UnitTests;

public partial class TypeExtensionsTests
{

    [Theory]
    [MemberData(nameof(EnumerableDataSets.Arrays), MemberType = typeof(EnumerableDataSets))]
    [MemberData(nameof(EnumerableDataSets.Enumerables), MemberType = typeof(EnumerableDataSets))]
    public void IsEnumerable_Should_ReturnTrue(Type type, EnumerableDataSets.EnumerableTestData testData)
    {
        // Arrange

        // Act
        var result = type.IsEnumerable(out var enumerableInfo, out var error);

        // Assert   
        Assert.True(result, error.ToString());
        Assert.NotNull(enumerableInfo);
        Assert.Equal(IsEnumerableError.None, error);

        Assert.NotNull(enumerableInfo!.EnumeratorInfo);

        var getEnumerator = enumerableInfo!.GetEnumerator;
        Assert.NotNull(getEnumerator);
        Assert.EndsWith(NameOf.GetEnumerator, getEnumerator!.Name);
        Assert.Equal(testData.GetEnumeratorDeclaringType, getEnumerator!.DeclaringType);
        Assert.Empty(getEnumerator!.GetParameters());
        

        var enumeratorInfo = enumerableInfo!.EnumeratorInfo;
        Assert.NotNull(enumeratorInfo);

        var enumeratorTestData = testData.EnumeratorTestData;
        
        var current = enumeratorInfo!.Current;
        Assert.NotNull(current);
        Assert.EndsWith(NameOf.Current, current!.Name);
        Assert.Equal(enumeratorTestData.CurrentDeclaringType, current!.DeclaringType);
        Assert.Equal(enumeratorTestData.ItemType, current!.PropertyType);

        var moveNext = enumeratorInfo!.MoveNext;
        Assert.NotNull(moveNext);
        Assert.EndsWith(NameOf.MoveNext, moveNext!.Name);
        Assert.Equal(enumeratorTestData.MoveNextDeclaringType, moveNext!.DeclaringType);
        Assert.Empty(moveNext!.GetParameters());

        var reset = enumeratorInfo!.Reset;
        if (enumeratorTestData.ResetDeclaringType is null)
        {
            Assert.Null(reset);
        }
        else
        {
            Assert.NotNull(reset);
            Assert.EndsWith(NameOf.Reset, reset!.Name);
            Assert.Equal(enumeratorTestData.ResetDeclaringType, reset!.DeclaringType);
            Assert.Empty(reset!.GetParameters());
        }

        var dispose = enumeratorInfo!.Dispose;
        if (enumeratorTestData.DisposeDeclaringType is null)
        {
            Assert.Null(dispose);
        }
        else
        {
            Assert.NotNull(dispose);
            Assert.EndsWith(NameOf.Dispose, dispose!.Name);
            Assert.Equal(enumeratorTestData.DisposeDeclaringType, dispose!.DeclaringType);
            Assert.Empty(dispose!.GetParameters());
        }
        
        Assert.Equal(enumeratorTestData.IsValueType, enumeratorInfo!.IsValueType);
        Assert.Equal(enumeratorTestData.IsByRefLikeType, enumeratorInfo!.IsByRefLike);
    }

    [Theory]
    [MemberData(nameof(EnumerableDataSets.InvalidEnumerables), MemberType = typeof(EnumerableDataSets))]
    public void IsEnumerable_With_MissingFeatures_Should_ReturnFalse(Type type, IsEnumerableError expectedError)
    {
        // Arrange

        // Act
        var result = type.IsEnumerable(out _, out var error);

        // Assert   
        Assert.False(result);
        Assert.Equal(expectedError, error);
    }
}
