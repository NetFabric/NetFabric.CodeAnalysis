using NetFabric.CSharp.TestData;
using System;
using Xunit;

namespace NetFabric.Reflection.CSharp.UnitTests;

public partial class TypeExtensionsTests
{
    [Theory]
    [MemberData(nameof(AsyncEnumerableDataSets.AsyncEnumerables), MemberType = typeof(AsyncEnumerableDataSets))]
    public void IsAsyncEnumerable_Should_ReturnTrue(Type type, AsyncEnumerableDataSets.AsyncEnumerableTestData enumerableTestData)
    {
        // Arrange

        // Act
        var result = type.IsAsyncEnumerable(out var enumerableInfo, out var error);

        // Assert   
        Assert.True(result, error.ToString());
        Assert.NotNull(enumerableInfo);
        Assert.True(error == IsAsyncEnumerableError.None);

        var getAsyncEnumerator = enumerableInfo!.GetAsyncEnumerator;
        Assert.NotNull(getAsyncEnumerator);
        Assert.EndsWith(NameOf.GetAsyncEnumerator, getAsyncEnumerator.Name);
        Assert.Equal(enumerableTestData.GetAsyncEnumeratorDeclaringType, getAsyncEnumerator.DeclaringType);
        Assert.Equal(enumerableTestData.GetAsyncEnumeratorParametersCount, getAsyncEnumerator.GetParameters().Length);

        var enumeratorInfo = enumerableInfo!.EnumeratorInfo;
        Assert.NotNull(enumeratorInfo);

        var enumeratorTestData = enumerableTestData.AsyncEnumeratorTestData;
        
        var current = enumeratorInfo!.Current;
        Assert.NotNull(current);
        Assert.EndsWith(NameOf.Current, current!.Name);
        Assert.Equal(enumeratorTestData.CurrentDeclaringType, current.DeclaringType);
        Assert.Equal(enumeratorTestData.ItemType, current!.PropertyType);

        var moveNextAsync = enumeratorInfo!.MoveNextAsync;
        Assert.NotNull(moveNextAsync);
        Assert.EndsWith(NameOf.MoveNextAsync, moveNextAsync.Name);
        Assert.Equal(enumeratorTestData.MoveNextAsyncDeclaringType, moveNextAsync.DeclaringType);
        Assert.Empty(moveNextAsync.GetParameters());

        var disposeAsync = enumeratorInfo!.DisposeAsync;
        if (enumeratorTestData.DisposeAsyncDeclaringType is null)
        {
            Assert.Null(disposeAsync);
        }
        else
        {
            Assert.NotNull(disposeAsync);
            Assert.EndsWith(NameOf.DisposeAsync, disposeAsync!.Name);
            Assert.Equal(enumeratorTestData.DisposeAsyncDeclaringType, disposeAsync!.DeclaringType);
            Assert.Empty(disposeAsync!.GetParameters());
        }
        
        Assert.Equal(enumeratorTestData.IsValueType, enumeratorInfo.IsValueType);
    }

    [Theory]
    [MemberData(nameof(AsyncEnumerableDataSets.InvalidAsyncEnumerables), MemberType = typeof(AsyncEnumerableDataSets))]
    public void IsAsyncEnumerable_With_MissingFeatures_Should_ReturnFalse(Type type, IsAsyncEnumerableError expectedError)
    {
        // Arrange

        // Act
        var result = type.IsAsyncEnumerable(out _, out var error);

        // Assert   
        Assert.False(result);
        Assert.Equal(expectedError, error);
    }
}
