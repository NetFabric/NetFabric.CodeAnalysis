using System;
using NetFabric.CodeAnalysis.CSharp.UnitTests;
using NetFabric.CSharp.TestData;
using Xunit;

namespace NetFabric.CodeAnalysis.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.Enumerators), MemberType = typeof(DataSets))]
        public void IsEnumerator_Should_ReturnTrue(Type enumeratorType)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumeratorType);

            // Act
            var result = typeSymbol.IsEnumerator(compilation);

            // Assert   
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidEnumerators), MemberType = typeof(DataSets))]
        public void IsEnumerator_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType, bool missingCurrent, bool missingMoveNext)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumeratorType);

            // Act
            var result = typeSymbol.IsEnumerator(compilation, out var errors);

            // Assert   
            Assert.False(result);
            Assert.Equal(missingCurrent, errors.HasFlag(Errors.MissingCurrent));
            Assert.Equal(missingMoveNext, errors.HasFlag(Errors.MissingMoveNext));
        }
    }
}