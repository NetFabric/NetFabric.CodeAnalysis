using NetFabric.TestData;
using System;
using System.Linq;
using Xunit;

namespace NetFabric.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.InstanceMethods), MemberType = typeof(DataSets))]
        public void GetPublicMethod_Should_ReturnMethod(string methodName, Type[] parameters)
        {
            // Arrange
            var type = typeof(PropertiesAndMethods);

            // Act
            var result = type.GetPublicMethod(methodName, parameters);

            // Assert   
            Assert.NotNull(result);
            Assert.Equal(methodName, result!.Name);
            Assert.True(result.GetParameters()
                .Select(parameter => parameter.ParameterType)
                .SequenceEqual(parameters));
        }

        [Theory]
        [MemberData(nameof(DataSets.ExplicitInstanceMethods), MemberType = typeof(DataSets))]
        public void GetPublicMethod_With_ExplicitOrStaticMethods_Should_ReturnNull(string methodName, Type[] parameters)
        {
            // Arrange
            var type = typeof(PropertiesAndMethods);

            // Act
            var result = type.GetPublicMethod(methodName, parameters);

            // Assert 
            Assert.Null(result);
        }
    }
}
