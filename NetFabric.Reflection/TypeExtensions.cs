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
        static readonly MethodInfo GetEnumeratorInfo = typeof(IEnumerable).GetMethod("GetEnumerator");
        static readonly PropertyInfo CurrentInfo = typeof(IEnumerator).GetProperty("Current");
        static readonly MethodInfo MoveNextInfo = typeof(IEnumerator).GetMethod("MoveNext");
        static readonly MethodInfo ResetInfo = typeof(IEnumerator).GetMethod("Reset");
        static readonly MethodInfo DisposeInfo = typeof(IDisposable).GetMethod("Dispose");
        static readonly MethodInfo DisposeAsyncInfo = typeof(IAsyncDisposable).GetMethod("DisposeAsync");

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
            if (type.IsEnumeratorType(out var current, out var moveNext, out var reset, out var dispose))
            {
                enumeratorInfo = new EnumeratorInfo(current, moveNext, reset, dispose);
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
            if (getEnumerator is object)
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
            if (getAsyncEnumerator is object)
                return true;

            getAsyncEnumerator = type.GetPublicMethod("GetAsyncEnumerator");
            if (getAsyncEnumerator is object)
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
            out MethodInfo? dispose)
        {
            current = type.GetPublicProperty("Current");
            moveNext = type.GetPublicMethod("MoveNext");
            reset = type.GetPublicMethod("Reset");
            dispose = type.ImplementsInterface(typeof(IDisposable), out _) ? DisposeInfo : default;
            if (current is object && moveNext is object)
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
            if (current is object && moveNextAsync is object)
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
            for (var index = 0; index < properties.Length; index++)
            {
                var property = properties[index];
                if (property.Name == name && property.GetGetMethod() is object)
                    return property;
            }

            if (type.IsInterface)
            {
                foreach (var @interface in type.GetAllInterfaces())
                {
                    var property = @interface.GetPublicProperty(name);
                    if (property is object)
                        return property;
                }
            }
            else
            {
                var baseType = type.BaseType;
                if (baseType is object)
                    return baseType.GetPublicProperty(name);
            }

            return default;
        }

        public static MethodInfo? GetPublicMethod(this Type type, string name, params Type[] parameters)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            for (var index = 0; index < methods.Length; index++)
            {
                var method = methods[index];
                if (method.Name == name && SequenceEqual(method.GetParameters(), parameters))
                    return method;
            }

            if (type.IsInterface)
            {
                foreach (var @interface in type.GetAllInterfaces())
                {
                    var method = @interface.GetPublicMethod(name, parameters);
                    if (method is object)
                        return method;
                }
            }
            else
            {
                var baseType = type.BaseType;
                if (baseType is object)
                    return baseType.GetPublicMethod(name);
            }

            return default;
        }

        public static bool ImplementsInterface(this Type type, Type interfaceType, [NotNullWhen(true)] out Type[]? genericArguments)
        {
            if (!interfaceType.IsGenericType)
            {
                genericArguments = Array.Empty<Type>();
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

            for (var index = 0; index < parameters.Length; index++)
            {
                if (parameters[index].ParameterType != types[index])
                    return false;
            }

            return true;
        }
    }
}