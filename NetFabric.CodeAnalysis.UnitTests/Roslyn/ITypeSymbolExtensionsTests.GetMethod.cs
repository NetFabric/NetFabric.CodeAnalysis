using Microsoft.CodeAnalysis;
using NetFabric.Assertive;
using System;
using System.Linq;
using Xunit;

namespace NetFabric.CodeAnalysis.Roslyn.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        public static TheoryData<string, Type[]> Methods =>
            new TheoryData<string, Type[]>
            {
                { "Method", new Type[] { } },
                { "Method", new Type[] { typeof(int), typeof(string) } },
                { "InheritedMethod", new Type[] { } },
                { "InheritedMethod", new Type[] { typeof(int), typeof(string) } },
            };

        [Theory]
        [MemberData(nameof(Methods))]
        public void GetMethod_Should_ReturnMethod(string methodName, Type[] parameters)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/PropertiesAndMethods.cs");
            var typeSymbol = compilation.GetSymbolsWithName("PropertiesAndMethods").First() as ITypeSymbol;

            // Act
            var result = typeSymbol.GetPublicMethod(methodName, parameters);

            // Assert   
            result.Must()
                .BeNotNull()
                .EvaluatesTrue(method =>
                    method.Name == methodName &&
                    method.Parameters
                        .Select(parameter => parameter.Type.Name)
                        .SequenceEqual(parameters
                            .Select(type => type.Name)));
        }

        public static TheoryData<string, Type[]> ExplicitMethods =>
            new TheoryData<string, Type[]>
            {
                { "ExplicitMethod", new Type[] { } },
                { "ExplicitMethod", new Type[] { typeof(int), typeof(string) } },
                { "StaticMethod", new Type[] { } },
                { "StaticMethod", new Type[] { typeof(int), typeof(string) } },
            };

        [Theory]
        [MemberData(nameof(ExplicitMethods))]
        public void GetMethod_With_ExplicitMethods_Should_ReturnMethod(string methodName, Type[] parameters)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/PropertiesAndMethods.cs");
            var typeSymbol = compilation.GetSymbolsWithName("PropertiesAndMethods").First() as ITypeSymbol;

            // Act
            var result = typeSymbol.GetPublicMethod(methodName, parameters);

            // Assert   
            result.Must()
                .BeNull();
        }
    }
}
