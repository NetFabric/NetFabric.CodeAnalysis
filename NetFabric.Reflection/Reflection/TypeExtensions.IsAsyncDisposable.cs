using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetFabric.Reflection
{
    public static partial class TypeExtensions
    {
        /// <summary>
        /// Gets a value indicating whether 'await using' considers <see cref="System.Type"/> to be disposable.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to test.</param>
        /// <param name="disposeAsync">If methods returns <c>true</c>, contains information on the disposed method 'await using' will use.</param>
        /// <returns><c>true</c> if 'await using' considers <see cref="System.Type"/> to be disposable; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The 'await foreach' statement uses a 'await using' internally so the same rules apply.
        /// </remarks>
        public static bool IsAsyncDisposable(this Type type, [NotNullWhen(true)] out MethodInfo? disposeAsync)
        {
            if (type.ImplementsInterface(typeof(IAsyncDisposable), out _))
                disposeAsync = typeof(IAsyncDisposable).GetPublicInstanceDeclaredOnlyMethod(nameof(IAsyncDisposable.DisposeAsync), Type.EmptyTypes)!;
            else
                disposeAsync = default;

            return disposeAsync is not null;
        }
    }
}