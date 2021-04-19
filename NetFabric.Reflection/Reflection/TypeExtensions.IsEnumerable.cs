using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NetFabric.Reflection
{
    public static partial class TypeExtensions
    {        
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
        /// <param name="errors">Gets information on what error caused the method to return <c>false</c>.</param>
        /// <returns><c>true</c> if 'foreach' considers <see cref="System.Type"/> to be enumerable; otherwise, <c>false</c>.</returns>
        public static bool IsEnumerable(this Type type,
            [NotNullWhen(true)] out EnumerableInfo? enumerableInfo,
            out Errors errors)
        {
            if (!type.IsArray && !type.IsInterface)
            {
                var getEnumerator = type.GetPublicInstanceMethod(nameof(IEnumerable.GetEnumerator), Type.EmptyTypes);
                if (getEnumerator is not null)
                {
                    var enumeratorType = getEnumerator.ReturnType;
                    if (enumeratorType.IsInterface)
                    {
                        if (enumeratorType.ImplementsInterface(typeof(IEnumerator<>), out var enumeratorGenericArguments))
                        {
                            enumerableInfo = new EnumerableInfo(
                                getEnumerator: getEnumerator,
                                new EnumeratorInfo(
                                    current: typeof(IEnumerator<>).MakeGenericType(enumeratorGenericArguments[0]).GetPublicInstanceDeclaredOnlyReadProperty(nameof(IEnumerator<int>.Current))!,
                                    moveNext: typeof(IEnumerator).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes)!)
                                {
                                    Reset = typeof(IEnumerator).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerator.Reset), Type.EmptyTypes),
                                    Dispose = typeof(IDisposable).GetPublicInstanceDeclaredOnlyMethod(nameof(IDisposable.Dispose), Type.EmptyTypes),
                                    IsGenericsEnumeratorInterface = true,
                                }
                            );                            
                        }
                        else if(enumeratorType.ImplementsInterface(typeof(IEnumerator), out _))
                        {
                            _ = enumeratorType.IsDisposable(out var dispose, out _);
                            enumerableInfo = new EnumerableInfo(
                                getEnumerator: getEnumerator,
                                new EnumeratorInfo(
                                    current: typeof(IEnumerator).GetPublicInstanceDeclaredOnlyReadProperty(nameof(IEnumerator<int>.Current))!,
                                    moveNext: typeof(IEnumerator).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes)!)
                                {
                                    Reset = typeof(IEnumerator).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerator.Reset), Type.EmptyTypes),
                                    Dispose = dispose,
                                    IsEnumeratorInterface = true,
                                }
                            );                            
                        }
                        else
                        {
                            enumerableInfo = default;
                            errors = Errors.MissingCurrent;
                            return false;
                        }
                    }
                    else
                    {
                        var current = enumeratorType.GetPublicInstanceReadProperty(nameof(IEnumerator.Current));
                        if (current is null)
                        {
                            enumerableInfo = default;
                            errors = Errors.MissingCurrent;
                            return false;
                        }

                        var moveNext =
                            enumeratorType.GetPublicInstanceMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes);
                        if (moveNext is null)
                        {
                            enumerableInfo = default;
                            errors = Errors.MissingMoveNext;
                            return false;
                        }

                        var reset = enumeratorType.GetPublicInstanceMethod(nameof(IEnumerator.Reset), Type.EmptyTypes);
                        _ = enumeratorType.IsDisposable(out var dispose, out var isByRefLike);
                        enumerableInfo = new EnumerableInfo(
                            getEnumerator: getEnumerator,
                            new EnumeratorInfo(current, moveNext)
                            {
                                Reset = reset,
                                Dispose = dispose,
                                IsValueType = getEnumerator.ReturnType.IsValueType,
                                IsByRefLike = isByRefLike,
                            }
                        );
                    }

                    errors = Errors.None;
                    return true;
                }
            }

            if (type.ImplementsInterface(typeof(IEnumerable<>), out var enumerableGenericArguments))
            {
                var genericType = typeof(IEnumerable<>).MakeGenericType(enumerableGenericArguments[0]);
                var getEnumerator = genericType.GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerable<int>.GetEnumerator), Type.EmptyTypes)!;
                enumerableInfo = new EnumerableInfo(
                    getEnumerator,
                    new EnumeratorInfo(
                        current: typeof(IEnumerator<>).MakeGenericType(enumerableGenericArguments[0]).GetPublicInstanceDeclaredOnlyReadProperty(nameof(IEnumerator<int>.Current))!,
                        moveNext: typeof(IEnumerator).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes)!)
                        {
                            Reset = typeof(IEnumerator).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerator.Reset), Type.EmptyTypes),
                            Dispose = typeof(IDisposable).GetPublicInstanceDeclaredOnlyMethod(nameof(IDisposable.Dispose), Type.EmptyTypes),
                            IsGenericsEnumeratorInterface = true,
                        }
                );
                errors = Errors.None; 
                return true;
            }

            if (type.ImplementsInterface(typeof(IEnumerable), out _))
            {
                var getEnumerator = typeof(IEnumerable).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerable.GetEnumerator), Type.EmptyTypes)!;
                _ = getEnumerator.ReturnType.IsDisposable(out var dispose, out _);
                enumerableInfo = new EnumerableInfo(
                    getEnumerator,
                    new EnumeratorInfo(
                        current: typeof(IEnumerator).GetPublicInstanceDeclaredOnlyReadProperty(nameof(IEnumerator.Current))!,
                        moveNext: typeof(IEnumerator).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes)!)
                        {
                            Reset = typeof(IEnumerator).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerator.Reset), Type.EmptyTypes),
                            Dispose = dispose,
                            IsEnumeratorInterface = true,
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
}