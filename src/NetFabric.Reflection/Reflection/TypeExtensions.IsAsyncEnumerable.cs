using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NetFabric.Reflection;

public static partial class TypeExtensions
{        
    /// <summary>
    /// Gets a value indicating whether 'await foreach' considers <see cref="System.Type"/> to be enumerable.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> to test.</param>
    /// <returns><c>true</c> if 'await foreach' considers <see cref="System.Type"/> to be enumerable; otherwise, <c>false</c>.</returns>
    public static bool IsAsyncEnumerable(this Type type)
        => IsAsyncEnumerable(type, out _, out _);

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
    /// <param name="error">Gets information on what error caused the method to return <c>false</c>.</param>
    /// <returns><c>true</c> if 'await foreach' considers <see cref="System.Type"/> to be enumerable; otherwise, <c>false</c>.</returns>
    public static bool IsAsyncEnumerable(this Type type,
        [NotNullWhen(true)] out AsyncEnumerableInfo? enumerableInfo,
        out IsAsyncEnumerableError error)
    {
        var getAsyncEnumerator = type.GetPublicMethod(NameOf.GetAsyncEnumerator, typeof(CancellationToken));
        getAsyncEnumerator ??= type.GetPublicMethod(NameOf.GetAsyncEnumerator);
        if (getAsyncEnumerator is null && !type.IsInterface && type.ImplementsInterface(typeof(IAsyncEnumerable<>), out var genericArguments))
        {       
            var interfaceType = typeof(IAsyncEnumerable<>).MakeGenericType(genericArguments); 
            getAsyncEnumerator ??= interfaceType.GetPublicMethod(NameOf.GetAsyncEnumerator, typeof(CancellationToken));
            getAsyncEnumerator ??= interfaceType.GetPublicMethod(NameOf.GetAsyncEnumerator);
        }        
        if (getAsyncEnumerator is null)
        {
            enumerableInfo = default;
            error = IsAsyncEnumerableError.MissingGetAsyncEnumerator;
            return false;
        }

        var enumeratorType = getAsyncEnumerator.ReturnType;
        
        var current = enumeratorType.GetPublicReadProperty(NameOf.Current);
        var moveNextAsync = enumeratorType.GetPublicMethod(NameOf.MoveNextAsync);

        if ((current is null || moveNextAsync is null) && !type.IsInterface)
        {
            if (type.ImplementsInterface(typeof(IAsyncEnumerator<>), out genericArguments))
            {
                var interfaceType = typeof(IAsyncEnumerator<>).MakeGenericType(genericArguments);
                current ??= interfaceType.GetPublicReadProperty(NameOf.Current);
                moveNextAsync ??= interfaceType.GetPublicMethod(NameOf.MoveNextAsync);
            }
        }

        if (current is null)
        {
            enumerableInfo = default;
            error = IsAsyncEnumerableError.MissingCurrent;
            return false;
        }
        
        if (moveNextAsync is null || moveNextAsync.ReturnType != typeof(ValueTask<bool>))
        {
            enumerableInfo = default;
            error = IsAsyncEnumerableError.MissingMoveNextAsync;
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
        error = IsAsyncEnumerableError.None; 
        return true;
    }
}