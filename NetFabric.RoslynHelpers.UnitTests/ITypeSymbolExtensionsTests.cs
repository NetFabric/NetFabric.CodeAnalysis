using Microsoft.CodeAnalysis;
using NetFabric.Assertive;
using System.Linq;
using Xunit;

namespace NetFabric.RoslynHelpers.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        [Fact]
        public void GetMethod_With_NoParameters_Should_Return()
        {
            // Arrange
            var code = @"
            using System;
            public class ExampleClass 
            {
                readonly string message;

                public ExampleClass()
                {
                    message = ""Hello World"";
                }

                public string GetMessage() 
                    => message;
            }";
            var compilation = Utils.Compile(code);
            var exampleClass = compilation.GetSymbolsWithName("ExampleClass").First() as ITypeSymbol;

            // Act
            var result = exampleClass.GetPublicMethod("GetMessage");

            // Assert   
            result.Must()
                .BeNotNull()
                .EvaluatesTrue(method => 
                    method.Name == "GetMessage" &&
                    method.Parameters.Length == 0);
        }

        [Fact]
        public void GetMethod_With_Parameters_Should_Return()
        {
            // Arrange
            var code = @"
            using System;
            public class ExampleClass 
            {
                readonly string message;

                public ExampleClass()
                {
                    message = ""Hello World"";
                }

                public string GetMessage() 
                    => message;

                public string GetMessage(int prefix) 
                    => $""{prefix} message"";

                public string GetMessage(string prefix) 
                    => $""{prefix} message"";
            }";
            var compilation = Utils.Compile(code);
            var exampleClass = compilation.GetSymbolsWithName("ExampleClass").First() as ITypeSymbol;

            // Act
            var result = exampleClass.GetPublicMethod("GetMessage", typeof(string));

            // Assert   
            result.Must()
                .BeNotNull()
                .EvaluatesTrue(method =>
                    method.Name == "GetMessage" &&
                    method.Parameters.Length == 1 &&
                    method.Parameters[0].Type.Name == "String");
        }
    }
}
