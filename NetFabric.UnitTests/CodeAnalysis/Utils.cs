using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NetFabric.Hyperlinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetFabric.CodeAnalysis.UnitTests
{
    public static class Utils
    {
        public static CSharpCompilation Compile(params string[] paths)
            => CSharpCompilation.Create(
                Guid.NewGuid().ToString(),
                paths
                    .Select(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path)))
                    .ToArray(),
                new[] {
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IEnumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IAsyncEnumerable<>).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(IValueEnumerable<,>).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ValueTask).Assembly.Location),
                },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)); 

        public static ITypeSymbol GetTypeSymbol(this CSharpCompilation compilation, Type type)
        {
            if (type == typeof(int))
                return compilation.GetSpecialType(SpecialType.System_Int32);

            var symbol = compilation.GetTypeByMetadataName($"{type.Namespace}.{type.Name}");

            if (type.IsGenericType)
            {
                var typeArguments = type.GenericTypeArguments
                    .Select(argumentType => compilation.GetTypeSymbol(argumentType))
                    .ToArray();
                return symbol.Construct(typeArguments);
            }

            return symbol;
        }
    }
}
