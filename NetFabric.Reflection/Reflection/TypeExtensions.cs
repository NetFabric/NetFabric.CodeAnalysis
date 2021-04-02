using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace NetFabric.Reflection
{
    public static class TypeExtensions
    {        
        public static bool IsEnumerable(this Type type, [NotNullWhen(true)] out EnumerableInfo? enumerableInfo)
            => IsEnumerable(type, out enumerableInfo, out _);

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
                    
                    var current = enumeratorType.GetPublicInstanceReadProperty(nameof(IEnumerator.Current));
                    if (current is null)
                    {
                        enumerableInfo = default;
                        errors = Errors.MissingCurrent;
                        return false;
                    }
                    
                    var moveNext = enumeratorType.GetPublicInstanceMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes);
                    if (moveNext is null)
                    {
                        enumerableInfo = default;
                        errors = Errors.MissingMoveNext;
                        return false;
                    }
                    
                    var reset = enumeratorType.GetPublicInstanceMethod(nameof(IEnumerator.Reset), Type.EmptyTypes);
                    _ = enumeratorType.IsDisposable(out var dispose, out var isByRefLike);

                    enumerableInfo = new EnumerableInfo(
                        getEnumerator,
                        new EnumeratorInfo(
                            current,
                            moveNext,
                            reset,
                            dispose,
                            isByRefLike
                        )
                    );
                    errors = Errors.None; 
                    return true;
                }
            }

            if (type.ImplementsInterface(typeof(IEnumerable<>), out var genericArguments))
            {
                var genericType = typeof(IEnumerable<>).MakeGenericType(genericArguments[0]);
                enumerableInfo = new EnumerableInfo(
                    genericType.GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerable<int>.GetEnumerator), Type.EmptyTypes)!,
                    new EnumeratorInfo(
                        typeof(IEnumerator<>).MakeGenericType(genericArguments[0]).GetPublicInstanceDeclaredOnlyReadProperty(nameof(IEnumerator<int>.Current))!,
                        typeof(IEnumerator).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes)!,
                        typeof(IEnumerator).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerator.Reset), Type.EmptyTypes),
                        typeof(IDisposable).GetPublicInstanceDeclaredOnlyMethod(nameof(IDisposable.Dispose), Type.EmptyTypes),
                        false
                    )
                );
                errors = Errors.None; 
                return true;
            }

            if (type.ImplementsInterface(typeof(IEnumerable), out _))
            {
                enumerableInfo = new EnumerableInfo(
                    typeof(IEnumerable).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerable.GetEnumerator), Type.EmptyTypes)!,
                    new EnumeratorInfo(
                        typeof(IEnumerator).GetPublicInstanceDeclaredOnlyReadProperty(nameof(IEnumerator.Current))!,
                        typeof(IEnumerator).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerator.MoveNext), Type.EmptyTypes)!,
                        typeof(IEnumerator).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerator.Reset), Type.EmptyTypes),
                        null,
                        false
                    )
                );
                errors = Errors.None; 
                return true;
            }

            enumerableInfo = default;
            errors = Errors.MissingGetEnumerator;
            return false;
        }

        public static bool IsAsyncEnumerable(this Type type, [NotNullWhen(true)] out AsyncEnumerableInfo? enumerableInfo)
            => IsAsyncEnumerable(type, out enumerableInfo, out _);

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
                        getAsyncEnumerator,
                        new AsyncEnumeratorInfo(
                            current,
                            moveNextAsync,
                            dispose
                        )
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
                    enumerableType.GetPublicInstanceDeclaredOnlyMethod(nameof(IAsyncEnumerable<int>.GetAsyncEnumerator), typeof(CancellationToken))!,
                    new AsyncEnumeratorInfo(
                        enumeratorType.GetPublicInstanceDeclaredOnlyReadProperty(nameof(IAsyncEnumerator<int>.Current))!,
                        enumeratorType.GetPublicInstanceDeclaredOnlyMethod(nameof(IAsyncEnumerator<int>.MoveNextAsync), Type.EmptyTypes)!,
                        typeof(IAsyncDisposable).GetPublicInstanceDeclaredOnlyMethod(nameof(IAsyncDisposable.DisposeAsync), Type.EmptyTypes)
                    )
                );
                errors = Errors.None; 
                return true;
            }

            enumerableInfo = default;
            errors = Errors.MissingGetEnumerator;
            return false;
        }

        public static bool IsDisposable(this Type type, [NotNullWhen(true)] out MethodInfo? dispose, out bool isByRefLike)
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

        public static bool IsAsyncDisposable(this Type type, [NotNullWhen(true)] out MethodInfo? disposeAsync)
        {
            if (type.ImplementsInterface(typeof(IAsyncDisposable), out _))
                disposeAsync = typeof(IAsyncDisposable).GetPublicInstanceDeclaredOnlyMethod(nameof(IAsyncDisposable.DisposeAsync), Type.EmptyTypes)!;
            else
                disposeAsync = default;

            return disposeAsync is not null;
        }

        public static bool ImplementsInterface(this Type type, Type interfaceType, [NotNullWhen(true)] out Type[]? genericArguments)
        {
            if (!interfaceType.IsGenericType)
            {
                genericArguments = Type.EmptyTypes;
                if (type == interfaceType)
                    return true;
                
                return type.GetAllInterfaces().Contains(interfaceType);
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == interfaceType.GetGenericTypeDefinition())
            {
                genericArguments = type.GetGenericArguments();
                return true;
            }

            foreach (var @interface in type.GetAllInterfaces())
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == interfaceType.GetGenericTypeDefinition())
                {
                    genericArguments = @interface.GetGenericArguments();
                    return true;
                }
            }

            genericArguments = default;
            return false;
        }

        public static IEnumerable<Type> GetAllInterfaces(this Type type)
        {
            foreach (var @interface in type.GetInterfaces())
            {
                yield return @interface;

                foreach (var baseInterface in @interface.GetAllInterfaces())
                    yield return baseInterface;
            }
        }

        const BindingFlags PublicInstance =
            BindingFlags.Public | BindingFlags.Instance;

        internal static PropertyInfo? GetPublicInstanceReadProperty(this Type type, string name)
            => type.GetProperties(PublicInstance)
                .FirstOrDefault(property => property.Name == name && property.GetGetMethod() is not null);
        
        internal static MethodInfo? GetPublicInstanceMethod(this Type type, string name, params Type[] types)
            => type.GetMethod(name, PublicInstance, null, types, null);

        const BindingFlags PublicInstanceDeclaredOnly =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        
        internal static PropertyInfo? GetPublicInstanceDeclaredOnlyReadProperty(this Type type, string name)
            => type.GetProperties(PublicInstanceDeclaredOnly)
                .FirstOrDefault(property => property.Name == name && property.GetGetMethod() is not null);
        
        internal static MethodInfo? GetPublicInstanceDeclaredOnlyMethod(this Type type, string name, params Type[] types)
            => type.GetMethod(name, PublicInstanceDeclaredOnly, null, types, null);

        internal static bool IsByRefLike(this Type type) // this implementation works on any target framework
            => type.GetCustomAttributes()
                .FirstOrDefault(attribute => attribute.GetType().Name == "IsByRefLikeAttribute") is not null;
    }
}