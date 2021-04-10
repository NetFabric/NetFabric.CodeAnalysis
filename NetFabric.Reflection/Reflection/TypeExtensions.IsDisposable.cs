using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetFabric.Reflection
{
    public static partial class TypeExtensions
    {
        /// <summary>
        /// Gets a value indicating whether 'using' considers <see cref="System.Type"/> to be disposable.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to test.</param>
        /// <param name="dispose">If methods returns <c>true</c>, contains information on the disposed method 'using' will use.</param>
        /// <returns><c>true</c> if 'using' considers <see cref="System.Type"/> to be disposable; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The 'foreach' statement uses a 'using' internally so the same rules apply.
        /// </remarks>
        public static bool IsDisposable(this Type type, [NotNullWhen(true)] out MethodInfo? dispose)
            => type.IsDisposable(out dispose, out _);
        
        internal static bool IsDisposable(this Type type, [NotNullWhen(true)] out MethodInfo? dispose, out bool isByRefLike)
        {
            isByRefLike = type.IsByRefLike();
            if (isByRefLike)
                dispose = type.GetPublicInstanceDeclaredOnlyMethod(nameof(IDisposable.Dispose), Type.EmptyTypes);
            else if (type.ImplementsInterface(typeof(IDisposable), out _))
                dispose = typeof(IDisposable).GetPublicInstanceDeclaredOnlyMethod(nameof(IDisposable.Dispose), Type.EmptyTypes)!;
            else
                dispose = default;
            
            return dispose is not null;
        }
    }
}