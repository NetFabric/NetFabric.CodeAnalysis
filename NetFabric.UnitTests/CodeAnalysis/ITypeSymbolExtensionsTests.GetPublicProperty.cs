using NetFabric.TestData;
using System;
using Xunit;

namespace NetFabric.CodeAnalysis.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.InstanceProperties), MemberType = typeof(DataSets))]
        public void GetProperty_Should_ReturnProperty(string propertyName, Type propertyType)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/PropertiesAndMethods.cs");
            var typeSymbol = compilation.GetTypeSymbol(typeof(PropertiesAndMethods));

            // Act
            var result = typeSymbol.GetPublicProperty(propertyName);

            // Assert   
            Assert.NotNull(result);
            Assert.Equal(propertyName, result!.Name);
            Assert.Equal(propertyType.Name, result!.Type.Name);
        }

        [Theory]
        [MemberData(nameof(DataSets.ExplicitInstanceProperties), MemberType = typeof(DataSets))]
        public void GetProperty_With_ExplicitOrStaticProperties_Should_ReturnNull(string propertyName)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/PropertiesAndMethods.cs");
            var typeSymbol = compilation.GetTypeSymbol(typeof(PropertiesAndMethods));

            // Act
            var result = typeSymbol.GetPublicProperty(propertyName);

            // Assert   
            Assert.Null(result);
        }
    }
}
