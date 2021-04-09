using NetFabric.CSharp.TestData;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace NetFabric.Reflection.CSharp.UnitTests
{
    public partial class TypeExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.AsyncEnumerables), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerable_Should_ReturnTrue(Type type, Type getAsyncEnumeratorDeclaringType, int getAsyncEnumeratorParametersCount, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type? disposeAsyncDeclaringType, Type itemType, bool isValueType)
        {
            // Arrange

            // Act
            var result = type.IsAsyncEnumerable(out var enumerableInfo);

            // Assert   
            Assert.True(result);
            Assert.NotNull(enumerableInfo);

            var getAsyncEnumerator = enumerableInfo!.GetAsyncEnumerator;
            Assert.NotNull(getAsyncEnumerator);
            Assert.Equal(nameof(IAsyncEnumerable<int>.GetAsyncEnumerator), getAsyncEnumerator.Name);
            Assert.Equal(getAsyncEnumeratorDeclaringType, getAsyncEnumerator.DeclaringType);
            Assert.Equal(getAsyncEnumeratorParametersCount, getAsyncEnumerator.GetParameters().Length);

            var enumeratorInfo = enumerableInfo!.EnumeratorInfo;
            Assert.NotNull(enumeratorInfo);

            var current = enumeratorInfo!.Current;
            Assert.NotNull(current);
            Assert.Equal(nameof(IAsyncEnumerator<int>.Current), current!.Name);
            Assert.Equal(currentDeclaringType, current.DeclaringType);
            Assert.Equal(itemType, current!.PropertyType);

            var moveNextAsync = enumeratorInfo!.MoveNextAsync;
            Assert.NotNull(moveNextAsync);
            Assert.Equal(nameof(IAsyncEnumerator<int>.MoveNextAsync), moveNextAsync.Name);
            Assert.Equal(moveNextAsyncDeclaringType, moveNextAsync.DeclaringType);
            Assert.Empty(moveNextAsync.GetParameters());

            var disposeAsync = enumeratorInfo!.DisposeAsync;
            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(disposeAsync);
            }
            else
            {
                Assert.NotNull(disposeAsync);
                Assert.Equal(nameof(IAsyncDisposable.DisposeAsync), disposeAsync!.Name);
                Assert.Equal(disposeAsyncDeclaringType, disposeAsync!.DeclaringType);
                Assert.Empty(disposeAsync!.GetParameters());
            }
            
            Assert.Equal(isValueType, enumeratorInfo.IsValueType);
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
}
