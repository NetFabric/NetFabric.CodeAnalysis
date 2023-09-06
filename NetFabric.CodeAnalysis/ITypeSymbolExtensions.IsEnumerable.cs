using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public static partial class ITypeSymbolExtensions
    {
        /// <summary>
        /// Gets a value indicating whether 'foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be enumerable.
        /// </summary>
        /// <param name="typeSymbol">The <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to test.</param>
        /// <param name="compilation">The <see cref="Microsoft.CodeAnalysis.Compilation"/> context.</param>
        /// <param name="enumerableSymbols">If methods returns <c>true</c>, contains information on the methods 'foreach' will use to enumerate.</param>
        /// <returns><c>true</c> if 'foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be enumerable; otherwise, <c>false</c>.</returns>
        public static bool IsEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out EnumerableSymbols? enumerableSymbols)
            => IsEnumerable(typeSymbol, compilation, out enumerableSymbols, out _);

        /// <summary>
        /// Gets a value indicating whether 'foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be enumerable.
        /// </summary>
        /// <param name="typeSymbol">The <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to test.</param>
        /// <param name="compilation">The <see cref="Microsoft.CodeAnalysis.Compilation"/> context.</param>
        /// <param name="enumerableSymbols">If methods returns <c>true</c>, contains information on the methods 'foreach' will use to enumerate.</param>
        /// <param name="errors">Gets information on what error caused the method to return <c>false</c>.</param>
        /// <returns><c>true</c> if 'foreach' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be enumerable; otherwise, <c>false</c>.</returns>
        public static bool IsEnumerable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out EnumerableSymbols? enumerableSymbols,
            out Errors errors)
        {
            var forEachUsesIndexer = typeSymbol.TypeKind == TypeKind.Array || typeSymbol.IsSpanOrReadOnlySpanType();

            if (typeSymbol.TypeKind != TypeKind.Interface)
            {
                var getEnumerator = typeSymbol.GetPublicMethod(NameOf.GetEnumerator);
                if (getEnumerator is not null)
                {
                    return HandleGetEnumerator(getEnumerator, compilation, out enumerableSymbols, out errors);
                }
            }

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_Generic_IEnumerable_T, out var genericArguments))
            {
                var genericEnumerableType = compilation
                    .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T)
                    .Construct(genericArguments[0]);
                var genericEnumeratorType = compilation
                    .GetSpecialType(SpecialType.System_Collections_Generic_IEnumerator_T)
                    .Construct(genericArguments[0]);
                var enumeratorType = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerator);
                var disposableType = compilation.GetSpecialType(SpecialType.System_IDisposable);

                enumerableSymbols = new EnumerableSymbols(
                    forEachUsesIndexer,
                    genericEnumerableType.GetPublicMethod(NameOf.GetEnumerator, Type.EmptyTypes)!,
                    new EnumeratorSymbols(
                        genericEnumeratorType.GetPublicReadProperty(NameOf.Current)!,
                        enumeratorType.GetPublicMethod(NameOf.MoveNext, Type.EmptyTypes)!)
                        {
                            Reset = enumeratorType.GetPublicMethod(NameOf.Reset, Type.EmptyTypes),
                            Dispose = disposableType.GetPublicMethod(NameOf.Dispose, Type.EmptyTypes),
                            IsGenericsEnumeratorInterface = true,
                            IsEnumeratorInterface = true,
                        }
                );
                errors = Errors.None; 
                return true;
            }

            if (typeSymbol.ImplementsInterface(SpecialType.System_Collections_IEnumerable, out _))
            {
                var enumerableType = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerable);
                var enumeratorType = compilation.GetSpecialType(SpecialType.System_Collections_IEnumerator);

                enumerableSymbols = new EnumerableSymbols(
                    forEachUsesIndexer,
                    enumerableType.GetPublicMethod(NameOf.GetEnumerator, Type.EmptyTypes)!,
                    new EnumeratorSymbols(
                        enumeratorType.GetPublicReadProperty(NameOf.Current)!,
                        enumeratorType.GetPublicMethod(NameOf.MoveNext, Type.EmptyTypes)!)
                    {
                        Reset = enumeratorType.GetPublicMethod(NameOf.Reset, Type.EmptyTypes),
                        IsEnumeratorInterface = true,
                    }                   
                );
                errors = Errors.None; 
                return true;
            }

            var extensionMethod = compilation.GetExtensionMethodWithName(typeSymbol, NameOf.GetEnumerator);
            if (extensionMethod is not null && extensionMethod.Parameters.Length == 1)
            {
                return HandleGetEnumerator(extensionMethod, compilation, out enumerableSymbols, out errors);
            }

            enumerableSymbols = default;
            errors = Errors.MissingGetEnumerator;
            return false;
        }

        static bool HandleGetEnumerator(IMethodSymbol getEnumerator, 
            Compilation compilation,
            [NotNullWhen(true)] out EnumerableSymbols? enumerableSymbols,
            out Errors errors)
        {
            var enumeratorType = getEnumerator.ReturnType;

            var current = enumeratorType.GetPublicReadProperty(NameOf.Current);
            if (current is null)
            {
                enumerableSymbols = default;
                errors = Errors.MissingCurrent;
                return false;
            }

            var moveNext = enumeratorType.GetPublicMethod(NameOf.MoveNext);
            if (moveNext is null)
            {
                enumerableSymbols = default;
                errors = Errors.MissingMoveNext;
                return false;
            }

            var reset = enumeratorType.GetPublicMethod("Reset");
            _ = enumeratorType.IsDisposable(compilation, out var dispose, out var isRefLike);

            enumerableSymbols = new EnumerableSymbols(
                false,
                getEnumerator,
                new EnumeratorSymbols(current, moveNext)
                {
                    Reset = reset,
                    Dispose = dispose,
                    IsValueType = enumeratorType.IsValueType,
                    IsRefLikeType = isRefLike,
                    IsGenericsEnumeratorInterface =
                        enumeratorType.TypeKind == TypeKind.Interface
                        && enumeratorType.ImplementsInterface(
                            SpecialType.System_Collections_Generic_IEnumerable_T, out _),
                    IsEnumeratorInterface =
                        enumeratorType.TypeKind == TypeKind.Interface,
                }
            );
            errors = Errors.None;
            return true;
        }
    }
}
