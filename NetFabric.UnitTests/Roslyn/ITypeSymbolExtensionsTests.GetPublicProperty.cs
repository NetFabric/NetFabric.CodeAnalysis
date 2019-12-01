using System;
using Xunit;

namespace NetFabric.CodeAnalysis.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        public static TheoryData<string, Type> Properties =>
            new TheoryData<string, Type>
            {
                { "Property", typeof(int) },
                { "InheritedProperty", typeof(int) },
            };

        [Theory]
        [MemberData(nameof(Properties))]
        public void GetProperty_Should_ReturnProperty(string propertyName, Type propertyType)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/PropertiesAndMethods.cs");
            var typeSymbol = compilation.GetTypeSymbol(typeof(TestData.PropertiesAndMethods));

            // Act
            var result = typeSymbol.GetPublicProperty(propertyName);

            // Assert   
            Assert.NotNull(result);
            Assert.Equal(propertyName, result.Name);
            Assert.Equal(propertyType.Name, result.Type.Name);
        }

        public static TheoryData<string> ExplicitProperties =>
            new TheoryData<string>
            {
                "ExplicitProperty",
                "StaticProperty",
            };

        [Theory]
        [MemberData(nameof(ExplicitProperties))]
        public void GetProperty_With_ExplicitOrStaticProperties_Should_ReturnNull(string propertyName)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/PropertiesAndMethods.cs");
            var typeSymbol = compilation.GetTypeSymbol(typeof(TestData.PropertiesAndMethods));

            // Act
            var result = typeSymbol.GetPublicProperty(propertyName);

            // Assert   
            Assert.Null(result);
        }
    }
}
