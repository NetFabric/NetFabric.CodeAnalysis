using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis;

public static partial class CompilationExtensions
{
    /// <summary>
    /// Retrieves a collection of extension methods within the specified <paramref name="compilation"/>
    /// that are applicable to the given <paramref name="typeSymbol"/> and have the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="compilation">The <see cref="Compilation"/> containing the types and methods to search in.</param>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> representing the type to which extension methods should be applicable.</param>
    /// <param name="name">The name of the extension method to search for.</param>
    /// <returns>
    /// A collection of <see cref="IMethodSymbol"/> representing extension methods that match the criteria.
    /// An empty collection is returned if no matching extension methods are found.
    /// </returns>
    /// <remarks>
    /// This method searches for extension methods based on their name and applicability to the specified <paramref name="typeSymbol"/>.
    /// Extension methods are methods that are marked as static and defined in non-nested, non-generic, static classes.
    /// </remarks>
    public static IEnumerable<IMethodSymbol> GetExtensionMethodsWithName(this Compilation compilation, ITypeSymbol typeSymbol, string name)
    {
        var typeArguments = default(ITypeSymbol[]);
        foreach (var symbol in compilation.GetSymbolsWithName(name, SymbolFilter.Member))
        {
            if (symbol is IMethodSymbol methodSymbol && 
                methodSymbol.IsStatic && 
                methodSymbol.IsExtensionMethod && 
                SymbolEqualityComparer.Default.Equals(methodSymbol.Parameters[0].Type.OriginalDefinition, typeSymbol.OriginalDefinition))
            {
                typeArguments ??= typeSymbol is INamedTypeSymbol namedTypeSymbol 
                    ? namedTypeSymbol.TypeArguments.ToArray()
                    : Array.Empty<ITypeSymbol>();

                yield return typeArguments.Length != 0
                    ? methodSymbol.Construct(typeArguments) 
                    : methodSymbol;
            }
        }
    }
}