using System;
using NetFabric.CSharp.TestData;
using Xunit;

namespace NetFabric.Reflection.CSharp.UnitTests
{
    public partial class TypeExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.AsyncEnumerators), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerator_Should_ReturnTrue(Type type)
        {
            // Arrange

            // Act
            var result = type.IsAsyncEnumerator();

            // Assert   
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidAsyncEnumerators), MemberType = typeof(DataSets))]
        public void IsAsyncEnumerator_With_MissingFeatures_Should_ReturnFalse(Type type, bool missingCurrent, bool missingMoveNext)
        {
            // Arrange

            // Act
            var result = type.IsAsyncEnumerator(out var errors);

            // Assert   
            Assert.False(result);
            Assert.Equal(missingCurrent, errors.HasFlag(Errors.MissingCurrent));
            Assert.Equal(missingMoveNext, errors.HasFlag(Errors.MissingMoveNext));
        }
    }
}