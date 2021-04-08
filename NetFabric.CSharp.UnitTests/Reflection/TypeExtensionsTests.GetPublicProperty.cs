using NetFabric.CSharp.TestData;
using System;
using Xunit;

namespace NetFabric.Reflection.CSharp.UnitTests
{
    public partial class TypeExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.InstanceProperties), MemberType = typeof(DataSets))]
        public void GetPublicProperty_Should_ReturnProperty(string propertyName, Type propertyType)
        {
            // Arrange
            var type = typeof(PropertiesAndMethods);

            // Act
            var result = type.GetPublicInstanceReadProperty(propertyName);

            // Assert   
            Assert.NotNull(result);
            Assert.Equal(propertyName, result!.Name);
            Assert.Equal(propertyType, result!.PropertyType);
        }

        [Theory]
        [MemberData(nameof(DataSets.ExplicitInstanceProperties), MemberType = typeof(DataSets))]
        public void GetPublicProperty_With_ExplicitOrStaticProperties_Should_ReturnNull(string propertyName)
        {
            // Arrange
            var type = typeof(PropertiesAndMethods);

            // Act
            var result = type.GetPublicInstanceReadProperty(propertyName);

            // Assert   
            Assert.Null(result);
        }
    }
}
