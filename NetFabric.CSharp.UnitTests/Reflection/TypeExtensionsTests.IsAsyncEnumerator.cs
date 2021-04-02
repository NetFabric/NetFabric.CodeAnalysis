using NetFabric.CSharp.TestData;
using System;
using Xunit;

namespace NetFabric.Reflection.CSharp.UnitTests
{
    public partial class TypeExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.AsyncEnumerators), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerator_Should_ReturnTrue(Type type, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type? disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = type.IsAsyncEnumerator(out var enumeratorInfo);

            // Assert   
            Assert.True(result);

            Assert.NotNull(enumeratorInfo!.Current);
            Assert.Equal("Current", enumeratorInfo!.Current!.Name);
            Assert.Equal(currentDeclaringType, enumeratorInfo!.Current!.DeclaringType);
            Assert.Equal(itemType, enumeratorInfo!.Current!.PropertyType);

            Assert.NotNull(enumeratorInfo!.MoveNextAsync);
            Assert.Equal("MoveNextAsync", enumeratorInfo!.MoveNextAsync!.Name);
            Assert.Equal(moveNextAsyncDeclaringType, enumeratorInfo!.MoveNextAsync!.DeclaringType);
            Assert.Empty(enumeratorInfo!.MoveNextAsync!.GetParameters());

            if (disposeAsyncDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.DisposeAsync);
            }
            else
            {
                Assert.NotNull(enumeratorInfo!.DisposeAsync);
                Assert.Equal("DisposeAsync", enumeratorInfo!.DisposeAsync!.Name);
                Assert.Equal(disposeAsyncDeclaringType, enumeratorInfo!.DisposeAsync!.DeclaringType);
                Assert.Empty(enumeratorInfo!.DisposeAsync!.GetParameters());
            }
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidAsyncEnumerators), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerator_With_MissingFeatures_Should_ReturnFalse(Type type, Type? currentDeclaringType, Type? moveNextAsyncDeclaringType, Type? _)
        {
            // Arrange

            // Act
            var result = type.IsAsyncEnumerator(out var _, out var errors);

            // Assert   
            Assert.False(result);

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
