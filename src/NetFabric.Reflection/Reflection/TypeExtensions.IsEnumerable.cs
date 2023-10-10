using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NetFabric.Reflection;

public static partial class TypeExtensions
{        
    /// <summary>
    /// Gets a value indicating whether 'foreach' considers <see cref="System.Type"/> to be enumerable.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> to test.</param>
    /// <returns><c>true</c> if 'foreach' considers <see cref="System.Type"/> to be enumerable; otherwise, <c>false</c>.</returns>
    public static bool IsEnumerable(this Type type)
        => IsEnumerable(type, out _, out _);

    /// <summary>
    /// Gets a value indicating whether 'foreach' considers <see cref="System.Type"/> to be enumerable.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> to test.</param>
    /// <param name="enumerableInfo">If methods returns <c>true</c>, contains information on the methods 'foreach' will use to enumerate.</param>
    /// <returns><c>true</c> if 'foreach' considers <see cref="System.Type"/> to be enumerable; otherwise, <c>false</c>.</returns>
    public static bool IsEnumerable(this Type type, [NotNullWhen(true)] out EnumerableInfo? enumerableInfo)
        => IsEnumerable(type, out enumerableInfo, out _);

    /// <summary>
    /// Gets a value indicating whether 'foreach' considers <see cref="System.Type"/> to be enumerable.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> to test.</param>
    /// <param name="enumerableInfo">If methods returns <c>true</c>, contains information on the methods 'foreach' will use to enumerate.</param>
    /// <param name="error">Gets information on what error caused the method to return <c>false</c>.</param>
    /// <returns><c>true</c> if 'foreach' considers <see cref="System.Type"/> to be enumerable; otherwise, <c>false</c>.</returns>
    public static bool IsEnumerable(this Type type,
        [NotNullWhen(true)] out EnumerableInfo? enumerableInfo,
        out IsEnumerableError error)
    {
        var forEachUsesIndexer = type.IsArray || type.IsSpanOrReadOnlySpan();
        var isGenericsEnumeratorInterface = false;
        var isEnumeratorInterface = false;

        var getEnumerator = type.GetPublicMethod(NameOf.GetEnumerator);
        if (getEnumerator is null && !type.IsInterface)
        {
            if (type.ImplementsInterface(typeof(IEnumerable<>), out var genericArguments))
            {
                getEnumerator ??= typeof(IEnumerable<>).MakeGenericType(genericArguments).GetPublicMethod(NameOf.GetEnumerator);
                isGenericsEnumeratorInterface = true;
            }
            else if (type.ImplementsInterface(typeof(IEnumerable), out _))
            {
                getEnumerator ??= typeof(IEnumerable).GetPublicMethod(NameOf.GetEnumerator);
                isEnumeratorInterface = true;
            }
        }
        if (getEnumerator is null)
        {
            enumerableInfo = default;
            error = IsEnumerableError.MissingGetEnumerator;
            return false;
        }

        var enumeratorType = getEnumerator.ReturnType;

        var current = enumeratorType.GetPublicReadProperty(NameOf.Current);
        var moveNext = enumeratorType.GetPublicMethod(NameOf.MoveNext);
        var reset = enumeratorType.GetPublicMethod(NameOf.Reset);

        if ((current is null || moveNext is null) && !type.IsInterface)
        {
            if (type.ImplementsInterface(typeof(IEnumerator<>), out var genericArguments))
            {
                var interfaceType = typeof(IEnumerator<>).MakeGenericType(genericArguments);
                current ??= interfaceType.GetPublicReadProperty(NameOf.Current);
                moveNext ??= interfaceType.GetPublicMethod(NameOf.MoveNext);
            }
            else if (type.ImplementsInterface(typeof(IEnumerator), out _))
            {
                var interfaceType = typeof(IEnumerator);
                current ??= interfaceType.GetPublicReadProperty(NameOf.Current);
                moveNext ??= interfaceType.GetPublicMethod(NameOf.MoveNext);
                reset ??= interfaceType.GetPublicMethod(NameOf.Reset);
            }
        }

        if (current is null)
        {
            enumerableInfo = default;
            error = IsEnumerableError.MissingCurrent;
            return false;
        }

        if (moveNext is null || moveNext.ReturnType != typeof(bool))
        {
            enumerableInfo = default;
            error = IsEnumerableError.MissingMoveNext;
            return false;
        }

        _ = enumeratorType.IsDisposable(out var dispose);

        enumerableInfo = new EnumerableInfo(
            forEachUsesIndexer,
            getEnumerator,
            new EnumeratorInfo(current, moveNext)
            {
                Reset = reset,
                Dispose = dispose,
                IsValueType = enumeratorType.IsValueType,
                IsByRefLike = enumeratorType.IsByRefLike,
                IsGenericsEnumeratorInterface = isGenericsEnumeratorInterface,
                IsEnumeratorInterface = isEnumeratorInterface,
            }
        );

        error = IsEnumerableError.None;
        return true;
    }
}