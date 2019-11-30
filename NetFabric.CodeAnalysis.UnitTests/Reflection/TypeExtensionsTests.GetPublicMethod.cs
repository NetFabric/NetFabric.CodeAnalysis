using NetFabric.Assertive;
using System;
using System.Linq;
using Xunit;

namespace NetFabric.CodeAnalysis.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        public static TheoryData<string, Type[]> InstanceMethods =>
            new TheoryData<string, Type[]>
            {
                { "Method", new Type[] { } },
                { "Method", new Type[] { typeof(int), typeof(string) } },
                { "InheritedMethod", new Type[] { } },
                { "InheritedMethod", new Type[] { typeof(int), typeof(string) } },
            };

        [Theory]
        [MemberData(nameof(InstanceMethods))]
        public void GetPublicMethod_Should_ReturnMethod(string methodName, Type[] parameters)
        {
            // Arrange
            var type = typeof(TestData.PropertiesAndMethods);

            // Act
            var result = type.GetPublicMethod(methodName, parameters);

            // Assert   
            result.Must()
                .BeNotNull()
                .EvaluatesTrue(method =>
                    method.Name == methodName &&
                    method.GetParameters()
                        .Select(parameter => parameter.ParameterType)
                        .SequenceEqual(parameters));
        }

        public static TheoryData<string, Type[]> ExplicitInstanceMethods =>
            new TheoryData<string, Type[]>
            {
                { "ExplicitMethod", new Type[] { } },
                { "ExplicitMethod", new Type[] { typeof(int), typeof(string) } },
                { "StaticMethod", new Type[] { } },
                { "StaticMethod", new Type[] { typeof(int), typeof(string) } },
            };

        [Theory]
        [MemberData(nameof(ExplicitInstanceMethods))]
        public void GetPublicMethod_With_ExplicitOrStaticMethods_Should_ReturnNull(string methodName, Type[] parameters)
        {
            // Arrange
            var type = typeof(TestData.PropertiesAndMethods);

            // Act
            var result = type.GetPublicMethod(methodName, parameters);

            // Assert   
            result.Must()
                .BeNull();
        }
    }
}
