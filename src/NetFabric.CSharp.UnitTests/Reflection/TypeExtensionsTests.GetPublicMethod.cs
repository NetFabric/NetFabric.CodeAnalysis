using NetFabric.CSharp.TestData;
using System;
using System.Linq;
using Xunit;

namespace NetFabric.Reflection.CSharp.UnitTests;

public partial class TypeExtensionsTests
{
    [Theory]
    [MemberData(nameof(EnumerableDataSets.InstanceMethods), MemberType = typeof(EnumerableDataSets))]
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
    [MemberData(nameof(EnumerableDataSets.StaticInstanceMethods), MemberType = typeof(EnumerableDataSets))]
    [MemberData(nameof(EnumerableDataSets.ExplicitInstanceMethods), MemberType = typeof(EnumerableDataSets))]
    public void GetPublicMethod_With_StaticMethods_Should_ReturnNull(string methodName, Type[] parameters)
    {
        // Arrange
        var type = typeof(PropertiesAndMethods);

        // Act
        var result = type.GetPublicMethod(methodName, parameters);

        // Assert 
        Assert.Null(result);
    }
}
