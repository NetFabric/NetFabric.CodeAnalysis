using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NetFabric.Hyperlinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetFabric.CodeAnalysis.CSharp.UnitTests;

public static class Utils
{
    public static CSharpCompilation Compile(params string[] paths)
        => CSharpCompilation.Create(
            Guid.NewGuid().ToString(),
            paths.Select(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path))),
            GetMetadataReferences(
                typeof(Enumerable),
                typeof(IEnumerable),
                typeof(IAsyncEnumerable<>),
                typeof(ReadOnlySpan<>),
                typeof(ValueTask),
                typeof(IValueEnumerable<,>)),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)); 

    public static ITypeSymbol GetTypeSymbol(this CSharpCompilation compilation, Type type)
    {
        if (type == typeof(int))
            return compilation.GetSpecialType(SpecialType.System_Int32);
        if (type == typeof(string))
            return compilation.GetSpecialType(SpecialType.System_String);

        if (type.IsArray)
            return compilation.CreateArrayTypeSymbol(compilation.GetTypeSymbol(type.GetElementType()!));

        var qualifiedName = $"{type.Namespace}.{type.Name}";
        var symbol = compilation.GetTypeByMetadataName(qualifiedName);
        if (symbol is null)
            throw new Exception($"'{qualifiedName}' not found!");

        if (type.IsGenericType)
        {
            var typeArguments = type.GenericTypeArguments
                .Select(compilation.GetTypeSymbol)
                .ToArray();
            return symbol.Construct(typeArguments);
        }

        return symbol;
    }

    static IEnumerable<MetadataReference> GetMetadataReferences(params Type[] types)
        => types.Select(type => MetadataReference.CreateFromFile(type.Assembly.Location));
}
