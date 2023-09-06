using System;
using NetFabric.CSharp.TestData;
using Xunit;

namespace NetFabric.Reflection.CSharp.UnitTests
{
    public partial class TypeExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.Enumerators), MemberType = typeof(DataSets))]
        public void IsEnumerator_Should_ReturnTrue(Type type)
        {
            // Arrange

            // Act
            var result = type.IsEnumerator();

            // Assert   
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidEnumerators), MemberType = typeof(DataSets))]
        public void IsEnumerator_With_MissingFeatures_Should_ReturnFalse(Type type, bool missingCurrent, bool missingMoveNext)
        {
            // Arrange

            // Act
            var result = type.IsEnumerator(out var errors);

            // Assert   
            Assert.False(result);
            Assert.Equal(missingCurrent, errors.HasFlag(Errors.MissingCurrent));
            Assert.Equal(missingMoveNext, errors.HasFlag(Errors.MissingMoveNext));
        }
    }
}