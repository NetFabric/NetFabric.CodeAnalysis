using System;
using System.Collections;
using System.Collections.Generic;

namespace NetFabric.Reflection
{
    public static partial class TypeExtensions
    {        

        /// <summary>
        /// Gets a value indicating whether 'await foreach' considers <see cref="System.Type"/> to be an asynchronous enumerator.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to test.</param>
        /// <returns><c>true</c> if 'await foreach' considers <see cref="System.Type"/> to be an asynchronous enumerator; otherwise, <c>false</c>.</returns>
        public static bool IsAsyncEnumerator(this Type type)
            => IsAsyncEnumerator(type, out _);

        /// <summary>
        /// Gets a value indicating whether 'await foreach' considers <see cref="System.Type"/> to be an asynchronous enumerator.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to test.</param>
        /// <param name="errors">Gets information on what error caused the method to return <c>false</c>.</param>
        /// <returns><c>true</c> if 'await foreach' considers <see cref="System.Type"/> to be an asynchronous enumerator; otherwise, <c>false</c>.</returns>
        public static bool IsAsyncEnumerator(this Type type, out Errors errors)
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
                    var moveNextAsync = type.GetPublicInstanceMethod(nameof(IAsyncEnumerator<int>.MoveNextAsync), Type.EmptyTypes);
                    if (moveNextAsync is not null)
                        return true;

                    errors = Errors.MissingMoveNext;
                }
            }

            return type.ImplementsInterface(typeof(IAsyncEnumerator<>), out _);
        }
    }
}