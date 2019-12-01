using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;

namespace NetFabric.CodeAnalysis.UnitTests
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

        public static ITypeSymbol GetTypeSymbol(this CSharpCompilation compilation, Type type)
        {
            if (type == typeof(int))
                return compilation.GetSpecialType(SpecialType.System_Int32);

            if (type.IsGenericType)
            {
                var name = type.Name.Substring(0, type.Name.IndexOf('`'));
                var symbols = compilation.GetSymbolsWithName(name);
                var symbol = symbols
                    .OfType<INamedTypeSymbol>()
                    .FirstOrDefault(s => 
                        s.MetadataName == type.Name &&
                        s.TypeParameters.Length == type.GenericTypeArguments.Length);
                var typeArguments = type.GenericTypeArguments
                    .Select(argumentType => compilation.GetTypeSymbol(argumentType))
                    .ToArray();
                return symbol.Construct(typeArguments);
            }

            return compilation.GetSymbolsWithName(type.Name)
                .OfType<INamedTypeSymbol>()
                .FirstOrDefault(s => s.MetadataName == type.Name);
        }
    }
}
