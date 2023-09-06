using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace NetFabric.CodeAnalysis
{
    public static partial class ITypeSymbolExtensions
    {
        
        /// <summary>
        /// Gets a value indicating whether 'await using' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be disposable.
        /// </summary>
        /// <param name="typeSymbol">The <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to test.</param>
        /// <param name="compilation">The <see cref="Microsoft.CodeAnalysis.Compilation"/> context.</param>
        /// <param name="disposeAsync">If methods returns <c>true</c>, contains information on the disposed method 'await using' will use.</param>
        /// <returns><c>true</c> if 'await using' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be disposable; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The 'await foreach' statement uses a 'await using' internally so the same rules apply.
        /// </remarks>
        public static bool IsAsyncDisposable(this ITypeSymbol typeSymbol, Compilation compilation,
            [NotNullWhen(true)] out IMethodSymbol? disposeAsync)
        {
            var asyncDisposableType = compilation.GetTypeByMetadataName("System.IAsyncDisposable")!;
            if (typeSymbol.ImplementsInterface(asyncDisposableType, out _))
                disposeAsync = asyncDisposableType.GetPublicMethod(NameOf.DisposeAsync);
            else
                disposeAsync = default;

            return disposeAsync is not null;
        }
    }
}
