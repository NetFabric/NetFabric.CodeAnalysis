using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace NetFabric.RoslynHelpers.UnitTests
{
    public static class Utils
    {
        public static CSharpCompilation Compile(string text)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(text);
            return CSharpCompilation.Create(
                "assemblyName",
                new[] { syntaxTree },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }
    }
}
