using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public static partial class CompilationExtensions
    {
        /// <summary>
        /// Retrieves an extension method with the specified name that is applicable to the given <paramref name="typeSymbol"/>
        /// within the provided <paramref name="compilation"/>.
        /// </summary>
        /// <param name="compilation">The compilation to search for extension methods in.</param>
        /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> representing the type for which to find extension methods.</param>
        /// <param name="name">The name of the extension method to retrieve.</param>
        /// <returns>
        /// An <see cref="IMethodSymbol"/> representing the extension method with the specified <paramref name="name"/> that is
        /// applicable to the provided <paramref name="typeSymbol"/>, or <c>null</c> if no such extension method is found.
        /// </returns>
        /// <remarks>
        /// This method searches for extension methods within the given <paramref name="compilation"/> that can be applied to the
        /// specified <paramref name="typeSymbol"/> based on their names and accessibility.
        /// </remarks>
        public static IMethodSymbol? GetExtensionMethodWithName(this Compilation compilation, ITypeSymbol typeSymbol, string name)
        {
            foreach (var symbol in compilation.GetSymbolsWithName(name, SymbolFilter.Member))
            {
                if (symbol is IMethodSymbol methodSymbol && 
                    methodSymbol.IsStatic && 
                    methodSymbol.IsExtensionMethod && 
                    methodSymbol.Parameters.Length > 0 && 
                    SymbolEqualityComparer.Default.Equals(methodSymbol.Parameters[0].Type.OriginalDefinition, typeSymbol.OriginalDefinition))
                {
                    return typeSymbol is INamedTypeSymbol namedTypeSymbol 
                        ? methodSymbol.Construct(namedTypeSymbol.TypeArguments.ToArray()) 
                        : methodSymbol;
                }
            }

            return null;
        }
    }
}