using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;

namespace NetFabric.CodeAnalysis.Roslyn.UnitTests
{
    public static class Utils
    {
        public static CSharpCompilation Compile(string path)
        {
            var text = File.ReadAllText(path);
            var syntaxTree = CSharpSyntaxTree.ParseText(text);
            return CSharpCompilation.Create(
                "assemblyName",
                new[] { syntaxTree },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }
    }
}
