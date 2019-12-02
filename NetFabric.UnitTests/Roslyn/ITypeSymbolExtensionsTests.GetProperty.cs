using Microsoft.CodeAnalysis;
using NetFabric.Assertive;
using System;
using System.Linq;
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
            var typeSymbol = compilation.GetSymbolsWithName("PropertiesAndMethods").First() as ITypeSymbol;

            // Act
            var result = typeSymbol.GetPublicProperty(propertyName);

            // Assert   
            result.Must()
                .BeNotNull()
                .EvaluatesTrue(property =>
                    property.Name == propertyName &&
                    property.Type.Name == propertyType.Name);
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
            var typeSymbol = compilation.GetSymbolsWithName("PropertiesAndMethods").First() as ITypeSymbol;

            // Act
            var result = typeSymbol.GetPublicProperty(propertyName);

            // Assert   
            result.Must()
                .BeNull();
        }
    }
}
