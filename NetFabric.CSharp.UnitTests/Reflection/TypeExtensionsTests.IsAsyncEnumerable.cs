using NetFabric.CSharp.TestData;
using System;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.Reflection.CSharp.UnitTests;

public partial class TypeExtensionsTests
{
    [Theory]
    [MemberData(nameof(DataSets.AsyncEnumerables), MemberType = typeof(DataSets))]
    public void IsAsyncEnumerable_Should_ReturnTrue(Type type, DataSets.AsyncEnumerableTestData enumerableTestData)
    {
        // Arrange

        // Act
        var result = type.IsAsyncEnumerable(out var enumerableInfo, out var errors);

        // Assert   
        Assert.True(result, errors.ToString());
        Assert.NotNull(enumerableInfo);
        Assert.Equal(Errors.None, errors);

        var getAsyncEnumerator = enumerableInfo!.GetAsyncEnumerator;
        Assert.NotNull(getAsyncEnumerator);
        Assert.Equal(nameof(IAsyncEnumerable<int>.GetAsyncEnumerator), getAsyncEnumerator.Name);
        Assert.Equal(enumerableTestData.GetAsyncEnumeratorDeclaringType, getAsyncEnumerator.DeclaringType);
        Assert.Equal(enumerableTestData.GetAsyncEnumeratorParametersCount, getAsyncEnumerator.GetParameters().Length);

        var enumeratorInfo = enumerableInfo!.EnumeratorInfo;
        Assert.NotNull(enumeratorInfo);

        var enumeratorTestData = enumerableTestData.AsyncEnumeratorTestData;
        
        var current = enumeratorInfo!.Current;
        Assert.NotNull(current);
        Assert.Equal(nameof(IAsyncEnumerator<int>.Current), current!.Name);
        Assert.Equal(enumeratorTestData.CurrentDeclaringType, current.DeclaringType);
        Assert.Equal(enumeratorTestData.ItemType, current!.PropertyType);

        var moveNextAsync = enumeratorInfo!.MoveNextAsync;
        Assert.NotNull(moveNextAsync);
        Assert.Equal(nameof(IAsyncEnumerator<int>.MoveNextAsync), moveNextAsync.Name);
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
            Assert.Equal(nameof(IAsyncDisposable.DisposeAsync), disposeAsync!.Name);
            Assert.Equal(enumeratorTestData.DisposeAsyncDeclaringType, disposeAsync!.DeclaringType);
            Assert.Empty(disposeAsync!.GetParameters());
        }
        
        Assert.Equal(enumeratorTestData.IsValueType, enumeratorInfo.IsValueType);
    }

    [Theory]
    [MemberData(nameof(DataSets.InvalidAsyncEnumerables), MemberType = typeof(DataSets))]
    public void IsAsyncEnumerable_With_MissingFeatures_Should_ReturnFalse(Type type, Type? getAsyncEnumeratorDeclaringType, int getAsyncEnumeratorParametersCount, Type? currentDeclaringType, Type? moveNextAsyncDeclaringType, Type? itemType)
    {
        // Arrange

        // Act
        var result = type.IsAsyncEnumerable(out _, out var errors);

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
