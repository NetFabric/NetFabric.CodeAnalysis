using Microsoft.CodeAnalysis;
using System.Collections;

namespace NetFabric.CodeAnalysis
{
    public static partial class ITypeSymbolExtensions
    {
        /// <summary>
        /// Gets a value indicating whether 'foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be an enumerator.
        /// </summary>
        /// <param name="typeSymbol">The <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to test.</param>
        /// <param name="compilation">The <see cref="Microsoft.CodeAnalysis.Compilation"/> context.</param>
        /// <returns><c>true</c> if 'foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be an enumerator; otherwise, <c>false</c>.</returns>
        public static bool IsEnumerator(this ITypeSymbol typeSymbol, Compilation compilation)
            => IsEnumerator(typeSymbol, compilation, out _);

        /// <summary>
        /// Gets a value indicating whether 'foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be an enumerator.
        /// </summary>
        /// <param name="typeSymbol">The <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to test.</param>
        /// <param name="compilation">The <see cref="Microsoft.CodeAnalysis.Compilation"/> context.</param>
        /// <param name="errors">Gets information on what error caused the method to return <c>false</c>.</param>
        /// <returns><c>true</c> if 'foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be an enumerator; otherwise, <c>false</c>.</returns>
        public static bool IsEnumerator(this ITypeSymbol typeSymbol, Compilation compilation, out Errors errors)
        {
            errors = Errors.None;
            if (typeSymbol.TypeKind != TypeKind.Interface)
            {
                var current = typeSymbol.GetPublicReadProperty(NameOf.Current);
                if (current is null)
                {
                    errors = Errors.MissingCurrent;
                }
                else
                {
                    var moveNext = typeSymbol.GetPublicMethod(NameOf.MoveNext);
                    if (moveNext is not null)
                        return true;

                    errors = Errors.MissingMoveNext;
                }
            }

            return typeSymbol.ImplementsInterface(SpecialType.System_Collections_IEnumerator, out _);
        }
    }
}
