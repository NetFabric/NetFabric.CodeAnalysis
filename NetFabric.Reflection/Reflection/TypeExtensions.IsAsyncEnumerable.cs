using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NetFabric.Reflection;

public static partial class TypeExtensions
{        

    /// <summary>
    /// Gets a value indicating whether 'await foreach' considers <see cref="System.Type"/> to be enumerable.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> to test.</param>
    /// <param name="enumerableInfo">If methods returns <c>true</c>, contains information on the methods 'await foreach' will use to enumerate.</param>
    /// <returns><c>true</c> if 'await foreach' considers <see cref="System.Type"/> to be enumerable; otherwise, <c>false</c>.</returns>
    public static bool IsAsyncEnumerable(this Type type, [NotNullWhen(true)] out AsyncEnumerableInfo? enumerableInfo)
        => IsAsyncEnumerable(type, out enumerableInfo, out _);

    /// <summary>
    /// Gets a value indicating whether 'await foreach' considers <see cref="System.Type"/> to be enumerable.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> to test.</param>
    /// <param name="enumerableInfo">If methods returns <c>true</c>, contains information on the methods 'await foreach' will use to enumerate.</param>
    /// <param name="errors">Gets information on what error caused the method to return <c>false</c>.</param>
    /// <returns><c>true</c> if 'await foreach' considers <see cref="System.Type"/> to be enumerable; otherwise, <c>false</c>.</returns>
    public static bool IsAsyncEnumerable(this Type type,
        [NotNullWhen(true)] out AsyncEnumerableInfo? enumerableInfo,
        out Errors errors)
    {
        if (!type.IsInterface)
        {
            var getAsyncEnumerator = type.GetPublicInstanceMethod(nameof(IAsyncEnumerable<int>.GetAsyncEnumerator), typeof(CancellationToken));
            if (getAsyncEnumerator is null)
                getAsyncEnumerator = type.GetPublicInstanceMethod(nameof(IAsyncEnumerable<int>.GetAsyncEnumerator));
            
            if (getAsyncEnumerator is not null)
            {
                var enumeratorType = getAsyncEnumerator.ReturnType;
                
                var current = enumeratorType.GetPublicInstanceReadProperty(nameof(IEnumerator.Current));
                if (current is null)
                {
                    enumerableInfo = default;
                    errors = Errors.MissingCurrent;
                    return false;
                }
                
                var moveNextAsync = enumeratorType.GetPublicInstanceMethod(nameof(IAsyncEnumerator<int>.MoveNextAsync), Type.EmptyTypes);
                if (moveNextAsync is null)
                {
                    enumerableInfo = default;
                    errors = Errors.MissingMoveNext;
                    return false;
                }
                
                _ = enumeratorType.IsAsyncDisposable(out var dispose);

                enumerableInfo = new AsyncEnumerableInfo(
                    getAsyncEnumerator: getAsyncEnumerator,
                    new AsyncEnumeratorInfo(current, moveNextAsync)
                    {
                        DisposeAsync = dispose,
                        IsValueType = getAsyncEnumerator.ReturnType.IsValueType,
                        IsAsyncEnumeratorInterface = enumeratorType.IsInterface,
                    }
                );
                errors = Errors.None; 
                return true;
            }
        }

        if (type.ImplementsInterface(typeof(IAsyncEnumerable<>), out var genericArguments))
        {
            var enumerableType = typeof(IAsyncEnumerable<>).MakeGenericType(genericArguments[0]);
            var enumeratorType = typeof(IAsyncEnumerator<>).MakeGenericType(genericArguments[0]);
            enumerableInfo = new AsyncEnumerableInfo(
                getAsyncEnumerator: enumerableType.GetPublicInstanceDeclaredOnlyMethod(nameof(IAsyncEnumerable<int>.GetAsyncEnumerator), typeof(CancellationToken))!,
                new AsyncEnumeratorInfo(
                    current: enumeratorType.GetPublicInstanceDeclaredOnlyReadProperty(nameof(IAsyncEnumerator<int>.Current))!,
                    moveNextAsync: enumeratorType.GetPublicInstanceDeclaredOnlyMethod(nameof(IAsyncEnumerator<int>.MoveNextAsync), Type.EmptyTypes)!)
                {
                    DisposeAsync = typeof(IAsyncDisposable).GetPublicInstanceDeclaredOnlyMethod(nameof(IAsyncDisposable.DisposeAsync), Type.EmptyTypes),
                    IsAsyncEnumeratorInterface = true,
                }
            );
            errors = Errors.None; 
            return true;
        }

        enumerableInfo = default;
        errors = Errors.MissingGetEnumerator;
        return false;
    }
}