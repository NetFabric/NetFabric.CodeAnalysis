using System;
using System.Collections;

namespace NetFabric.Reflection;

public static partial class TypeExtensions
{        
    /// <summary>
    /// Gets a value indicating whether 'foreach' considers <see cref="System.Type"/> to be an enumerator.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> to test.</param>
    /// <returns><c>true</c> if 'foreach' considers <see cref="System.Type"/> to be an enumerator; otherwise, <c>false</c>.</returns>
    public static bool IsEnumerator(this Type type)
        => IsEnumerator(type, out _);

    /// <summary>
    /// Gets a value indicating whether 'foreach' considers <see cref="System.Type"/> to be an enumerator.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> to test.</param>
    /// <param name="errors">Gets information on what error caused the method to return <c>false</c>.</param>
    /// <returns><c>true</c> if 'foreach' considers <see cref="System.Type"/> to be an enumerator; otherwise, <c>false</c>.</returns>
    public static bool IsEnumerator(this Type type, out Errors errors)
    {
        errors = Errors.None;
        if (!type.IsInterface)
        {
            var current = type.GetPublicInstanceReadProperty(nameof(IEnumerator.Current));
            if (current is null)
            {
                errors = Errors.MissingCurrent;
            }
            else
            {
                var moveNext = type.GetPublicInstanceMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes);
                if (moveNext is not null)
                    return true;

                errors = Errors.MissingMoveNext;
            }
        }

        return type.ImplementsInterface(typeof(IEnumerator), out _);
    }
}