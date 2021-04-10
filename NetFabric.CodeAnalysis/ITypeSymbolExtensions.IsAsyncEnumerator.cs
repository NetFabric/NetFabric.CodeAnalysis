using Microsoft.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public static partial class ITypeSymbolExtensions
    {

        /// <summary>
        /// Gets a value indicating whether 'await foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be an asynchronous enumerator.
        /// </summary>
        /// <param name="typeSymbol">The <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to test.</param>
        /// <param name="compilation">The <see cref="Microsoft.CodeAnalysis.Compilation"/> context.</param>
        /// <returns><c>true</c> if 'await foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be an asynchronous enumerator; otherwise, <c>false</c>.</returns>
        public static bool IsAsyncEnumerator(this ITypeSymbol typeSymbol, Compilation compilation)
            => IsAsyncEnumerator(typeSymbol, compilation, out _);

        /// <summary>
        /// Gets a value indicating whether 'await foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be an asynchronous enumerator.
        /// </summary>
        /// <param name="typeSymbol">The <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to test.</param>
        /// <param name="compilation">The <see cref="Microsoft.CodeAnalysis.Compilation"/> context.</param>
        /// <param name="errors">Gets information on what error caused the method to return <c>false</c>.</param>
        /// <returns><c>true</c> if 'await foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be an asynchronous enumerator; otherwise, <c>false</c>.</returns>
        public static bool IsAsyncEnumerator(this ITypeSymbol typeSymbol, Compilation compilation, out Errors errors)
        {
            errors = Errors.None; 
            if (typeSymbol.TypeKind != TypeKind.Interface)
            {
                var current = typeSymbol.GetPublicReadProperty("Current");
                if (current is null)
                {
                    errors = Errors.MissingCurrent;
                }
                else
                {
                    var moveNext = typeSymbol.GetPublicMethod("MoveNextAsync");
                    if (moveNext is not null)
                        return true;

                    errors = Errors.MissingMoveNext;
                }
            }

            var asyncEnumeratorType = compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerator`1")!;
            return typeSymbol.ImplementsInterface(asyncEnumeratorType, out _);
        }
    }
}
