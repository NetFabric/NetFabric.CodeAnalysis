using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics.CodeAnalysis;

namespace NetFabric.CodeAnalysis;

public static partial class ITypeSymbolExtensions
{

    /// <summary>
    /// Gets a value indicating whether 'using' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be disposable.
    /// </summary>
    /// <param name="typeSymbol">The <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to test.</param>
    /// <param name="compilation">The <see cref="Microsoft.CodeAnalysis.Compilation"/> context.</param>
    /// <param name="dispose">If methods returns <c>true</c>, contains information on the disposed method 'using' will use.</param>
    /// <returns><c>true</c> if 'using' considers <see cref="Microsoft.CodeAnalysis.ITypeSymbol"/> to be disposable; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// The 'foreach' statement uses a 'using' internally so the same rules apply.
    /// </remarks>
    public static bool IsDisposable(this ITypeSymbol typeSymbol, Compilation compilation,
        [NotNullWhen(true)] out IMethodSymbol? dispose)
        => typeSymbol.IsDisposable(compilation, out dispose, out _);
    
    static bool IsDisposable(this ITypeSymbol typeSymbol, Compilation compilation, 
        [NotNullWhen(true)] out IMethodSymbol? dispose,
        out bool isRefLike)
    {
        isRefLike = typeSymbol.IsRefLikeType;
        if (isRefLike)
            dispose = typeSymbol.GetPublicMethod(NameOf.Dispose);
        else if (typeSymbol.ImplementsInterface(SpecialType.System_IDisposable, out _))
            dispose = compilation.GetSpecialType(SpecialType.System_IDisposable).GetPublicMethod(NameOf.Dispose);
        else
            dispose = default;

        return dispose is not null;
    }

}
