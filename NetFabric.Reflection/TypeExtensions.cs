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
        const BindingFlags PublicInstanceDeclaredOnly = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        static readonly MethodInfo GetEnumeratorInfo = typeof(IEnumerable).GetMethod(nameof(IEnumerable.GetEnumerator), PublicInstanceDeclaredOnly, null, Type.EmptyTypes, null)!;
        static readonly PropertyInfo CurrentInfo = typeof(IEnumerator).GetProperty(nameof(IEnumerator.Current), PublicInstanceDeclaredOnly)!;
        static readonly MethodInfo MoveNextInfo = typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext), PublicInstanceDeclaredOnly, null, Type.EmptyTypes, null)!;
        static readonly MethodInfo ResetInfo = typeof(IEnumerator).GetMethod(nameof(IEnumerator.Reset), PublicInstanceDeclaredOnly, null, Type.EmptyTypes, null)!;
        static readonly MethodInfo DisposeInfo = typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose), PublicInstanceDeclaredOnly, null, Type.EmptyTypes, null)!;
        static readonly MethodInfo DisposeAsyncInfo = typeof(IAsyncDisposable).GetMethod(nameof(IAsyncDisposable.DisposeAsync), PublicInstanceDeclaredOnly, null, Type.EmptyTypes, null)!;

        public static bool IsEnumerable(this Type type, [NotNullWhen(true)] out EnumerableInfo? enumerableInfo)
            => IsEnumerable(type, out enumerableInfo, out var _);

        public static bool IsEnumerable(this Type type,
            [NotNullWhen(true)] out EnumerableInfo? enumerableInfo,
            out Errors errors)
        {
            if (!type.IsEnumerableType(out var getEnumerator))
            {
                enumerableInfo = default;
                errors = Errors.MissingGetEnumerable;
                return false;
            }

            if (!getEnumerator.ReturnType.IsEnumerator(out var enumeratorInfo, out errors))
            {
                enumerableInfo = default;
                return false;
            }

            enumerableInfo = new EnumerableInfo(getEnumerator, enumeratorInfo);
            return true;
        }

        public static bool IsAsyncEnumerable(this Type type, [NotNullWhen(true)] out AsyncEnumerableInfo? enumerableInfo)
            => IsAsyncEnumerable(type, out enumerableInfo, out var _);

        public static bool IsAsyncEnumerable(this Type type,
            [NotNullWhen(true)] out AsyncEnumerableInfo? enumerableInfo,
            out Errors errors)
        {
            if (!type.IsAsyncEnumerableType(out var getAsyncEnumerator))
            {
                enumerableInfo = default;
                errors = Errors.MissingGetEnumerable;
                return false;
            }

            if (!getAsyncEnumerator.ReturnType.IsAsyncEnumerator(out var asyncEnumeratorInfo, out errors))
            {
                enumerableInfo = default;
                return false;
            }

            enumerableInfo = new AsyncEnumerableInfo(getAsyncEnumerator, asyncEnumeratorInfo);
            return true;
        }

        public static bool IsEnumerator(this Type type, [NotNullWhen(true)] out EnumeratorInfo? enumeratorInfo)
            => IsEnumerator(type, out enumeratorInfo, out var _);

        public static bool IsEnumerator(this Type type,
            [NotNullWhen(true)] out EnumeratorInfo? enumeratorInfo,
            out Errors errors)
        {
            if (type.IsEnumeratorType(out var current, out var moveNext, out var reset, out var dispose, out var isByReflike))
            {
                enumeratorInfo = new EnumeratorInfo(current, moveNext, reset, dispose, isByReflike);
                errors = Errors.None;
                return true;
            }

            enumeratorInfo = default;
            errors = Errors.None;
            if (current is null) errors |= Errors.MissingCurrent;
            if (moveNext is null) errors |= Errors.MissingMoveNext;
            return false;
        }

        public static bool IsAsyncEnumerator(this Type type, [NotNullWhen(true)] out AsyncEnumeratorInfo? enumeratorInfo)
            => IsAsyncEnumerator(type, out enumeratorInfo, out var _);

        public static bool IsAsyncEnumerator(this Type type,
            [NotNullWhen(true)] out AsyncEnumeratorInfo? enumeratorInfo, 
            out Errors errors)
        {
            if (type.IsAsyncEnumeratorType(out var current, out var moveNextAsync, out var disposeAsync))
            {
                enumeratorInfo = new AsyncEnumeratorInfo(current, moveNextAsync, disposeAsync);
                errors = Errors.None;
                return true;
            }

            enumeratorInfo = default;
            errors = Errors.None;
            if (current is null) errors |= Errors.MissingCurrent;
            if (moveNextAsync is null) errors |= Errors.MissingMoveNext;
            return false;
        }

        static bool IsEnumerableType(this Type type,
            [NotNullWhen(true)] out MethodInfo? getEnumerator)
        {
            getEnumerator = type.GetPublicMethod("GetEnumerator");
            if (getEnumerator is not null)
                return true;

            if (type.ImplementsInterface(typeof(IEnumerable<>), out var genericArguments))
            {
                getEnumerator = typeof(IEnumerable<>).MakeGenericType(genericArguments[0]).GetMethod("GetEnumerator");
                return true;
            }

            if (type.ImplementsInterface(typeof(IEnumerable), out _))
            {
                getEnumerator = GetEnumeratorInfo;
                return true;
            }

            return false;
        }

        static bool IsAsyncEnumerableType(this Type type,
            [NotNullWhen(true)] out MethodInfo? getAsyncEnumerator)
        {
            getAsyncEnumerator = type.GetPublicMethod("GetAsyncEnumerator", typeof(CancellationToken));
            if (getAsyncEnumerator is not null)
                return true;

            getAsyncEnumerator = type.GetPublicMethod("GetAsyncEnumerator");
            if (getAsyncEnumerator is not null)
                return true;

            if (type.ImplementsInterface(typeof(IAsyncEnumerable<>), out var genericArguments))
            {
                getAsyncEnumerator = typeof(IAsyncEnumerable<>).MakeGenericType(genericArguments[0]).GetMethod("GetAsyncEnumerator", new[] { typeof(CancellationToken) });
                return true;
            }

            return false;
        }

        static bool IsEnumeratorType(this Type type,
            [NotNullWhen(true)] out PropertyInfo? current,
            [NotNullWhen(true)] out MethodInfo? moveNext, 
            out MethodInfo? reset, 
            out MethodInfo? dispose,
            out bool isByRefLike)
        {
            current = type.GetPublicProperty(nameof(IEnumerator.Current));
            moveNext = type.GetPublicMethod(nameof(IEnumerator.MoveNext));
            reset = type.GetPublicMethod(nameof(IEnumerator.Reset));
            isByRefLike = type.IsByRefLike();
            dispose = isByRefLike switch
            {
                true => type.GetPublicMethod(nameof(IDisposable.Dispose)),
                _ => type.ImplementsInterface(typeof(IDisposable), out _) ? DisposeInfo : default
            };
                
            if (current is not null && moveNext is not null)
                return true;

            if (type.ImplementsInterface(typeof(IEnumerator<>), out var genericArguments))
            {
                current = typeof(IEnumerator<>).MakeGenericType(genericArguments[0]).GetProperty("Current");
                moveNext = MoveNextInfo;
                reset = ResetInfo;
                return true;
            }

            if (type.ImplementsInterface(typeof(IEnumerator), out _))
            {
                current = CurrentInfo;
                moveNext = MoveNextInfo;
                reset = ResetInfo;
                return true;
            }

            return false;
        }

        static bool IsAsyncEnumeratorType(this Type type,
            [NotNullWhen(true)] out PropertyInfo? current,
            [NotNullWhen(true)] out MethodInfo? moveNextAsync, 
            out MethodInfo? disposeAsync)
        {
            current = type.GetPublicProperty("Current");
            moveNextAsync = type.GetPublicMethod("MoveNextAsync");
            disposeAsync = type.ImplementsInterface(typeof(IAsyncDisposable), out _) ? DisposeAsyncInfo : default;
            if (current is not null && moveNextAsync is not null)
                return true;

            if (type.ImplementsInterface(typeof(IAsyncEnumerator<>), out var genericArguments))
            {
                var interfaceType = typeof(IAsyncEnumerator<>).MakeGenericType(genericArguments[0]);
                current = interfaceType.GetProperty("Current");
                moveNextAsync = interfaceType.GetMethod("MoveNextAsync");
                return true;
            }

            return false;
        }

        public static PropertyInfo? GetPublicProperty(this Type type, string name)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                if (property.Name == name && property.GetGetMethod() is not null)
                    return property;
            }

            if (type.IsInterface)
            {
                foreach (var @interface in type.GetAllInterfaces())
                {
                    var property = @interface.GetPublicProperty(name);
                    if (property is not null)
                        return property;
                }
            }
            else
            {
                var baseType = type.BaseType;
                if (baseType is not null)
                    return baseType.GetPublicProperty(name);
            }

            return default;
        }

        public static MethodInfo? GetPublicMethod(this Type type, string name, params Type[] parameters)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var method in methods)
            {
                if (method.Name == name && SequenceEqual(method.GetParameters(), parameters))
                    return method;
            }

            if (type.IsInterface)
            {
                foreach (var @interface in type.GetAllInterfaces())
                {
                    var method = @interface.GetPublicMethod(name, parameters);
                    if (method is not null)
                        return method;
                }
            }
            else
            {
                var baseType = type.BaseType;
                if (baseType is not null)
                    return baseType.GetPublicMethod(name);
            }

            return default;
        }

        public static bool ImplementsInterface(this Type type, Type interfaceType, [NotNullWhen(true)] out Type[]? genericArguments)
        {
            if (!interfaceType.IsGenericType)
            {
                genericArguments = Type.EmptyTypes;
                return type.GetAllInterfaces().Contains(interfaceType);
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

        static bool SequenceEqual(ParameterInfo[] parameters, Type[] types)
        {
            if (parameters.Length != types.Length)
                return false;

            for (var index = 0; index < parameters.Length && index < types.Length; index++)
            {
                if (parameters[index].ParameterType != types[index])
                    return false;
            }

            return true;
        }

        static bool IsByRefLike(this Type type)
            => type.GetCustomAttributes()
                .FirstOrDefault(attribute => attribute.GetType().Name == "IsByRefLikeAttribute") is not null;
    }
}