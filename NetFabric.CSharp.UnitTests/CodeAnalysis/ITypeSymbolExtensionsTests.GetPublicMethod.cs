using NetFabric.CSharp.TestData;
using System;
using System.Linq;
using Xunit;

namespace NetFabric.CodeAnalysis.CSharp.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.InstanceMethods), MemberType = typeof(DataSets))]
        public void GetMethod_Should_ReturnMethod(string methodName, Type[] parameters)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/PropertiesAndMethods.cs");
            var typeSymbol = compilation.GetTypeSymbol(typeof(PropertiesAndMethods));

            // Act
            var result = typeSymbol.GetPublicMethod(methodName, parameters);

            // Assert   
            Assert.NotNull(result);
            Assert.Equal(methodName, result!.Name);
            Assert.True(result.Parameters
                .Select(parameter => parameter.Type.MetadataName)
                .SequenceEqual(parameters
                    .Select(type => type.Name)));
        }

        [Theory]
        [MemberData(nameof(DataSets.ExplicitInstanceMethods), MemberType = typeof(DataSets))]
        public void GetMethod_With_ExplicitOrStaticMethods_Should_ReturnNull(string methodName, Type[] parameters)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/PropertiesAndMethods.cs");
            var typeSymbol = compilation.GetTypeSymbol(typeof(PropertiesAndMethods));

            // Act
            var result = typeSymbol.GetPublicMethod(methodName, parameters);

            // Assert   
            Assert.Null(result);
        }
    }
}
