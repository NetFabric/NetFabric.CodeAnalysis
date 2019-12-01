using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace NetFabric.CodeAnalysis.Reflection
{
    public static class TypeExtensions
    {
        public static bool IsEnumerable(this Type type, out EnumerableInfo enumerableInfo)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsEnumerableType(out var getEnumerator))
            {
                enumerableInfo = default;
                return false;
            }

            var isEnumerator = getEnumerator.ReturnType.IsEnumerator(out var current, out var moveNext, out var dispose);
            enumerableInfo = new EnumerableInfo(getEnumerator, current, moveNext, dispose);
            return isEnumerator;
        }

        public static bool IsAsyncEnumerable(this Type type, out EnumerableInfo enumerableInfo)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsAsyncEnumerableType(out var getAsyncEnumerator))
            {
                enumerableInfo = default;
                return false;
            }

            var isEnumerator = getAsyncEnumerator.ReturnType.IsAsyncEnumerator(out var current, out var moveNextAsync, out var disposeAsync);
            enumerableInfo = new EnumerableInfo(getAsyncEnumerator, current, moveNextAsync, disposeAsync);
            return isEnumerator;
        }

        static bool IsEnumerableType(this Type type, out MethodInfo getEnumerator)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            getEnumerator = type.GetPublicMethod("GetEnumerator");
            if (getEnumerator is object)
                return true;

            if (type.ImplementsInterface(typeof(IEnumerable<>), out var genericArguments))
            {
                getEnumerator = typeof(IEnumerable<>).MakeGenericType(genericArguments[0]).GetMethod("GetEnumerator", Array.Empty<Type>());
                return true;
            }

            if (type.ImplementsInterface(typeof(IEnumerable), out _))
            {
                getEnumerator = typeof(IEnumerable).GetMethod("GetEnumerator", Array.Empty<Type>());
                return true;
            }

            return false;
        }

        static bool IsAsyncEnumerableType(this Type type, out MethodInfo getAsyncEnumerator)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

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

        public static bool IsEnumerator(this Type type, out PropertyInfo current, out MethodInfo moveNext, out MethodInfo dispose)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type.ImplementsInterface(typeof(IDisposable), out _))
                dispose = typeof(IDisposable).GetMethod("Dispose", Array.Empty<Type>());
            else
                dispose = null;

            current = type.GetPublicProperty("Current");
            moveNext = type.GetPublicMethod("MoveNext");
            if (current is object && moveNext is object)
                return true;

            if (type.ImplementsInterface(typeof(IEnumerator<>), out var genericArguments))
            {
                current = typeof(IEnumerator<>).MakeGenericType(genericArguments[0]).GetProperty("Current");
                moveNext = typeof(IEnumerator).GetMethod("MoveNext", Array.Empty<Type>());
                return true;
            }

            if (type.ImplementsInterface(typeof(IEnumerator), out _))
            {
                current = typeof(IEnumerator).GetProperty("Current");
                moveNext = typeof(IEnumerator).GetMethod("MoveNext", Array.Empty<Type>());
                return true;
            }

            return false;
        }

        public static bool IsAsyncEnumerator(this Type type, out PropertyInfo current, out MethodInfo moveNextAsync, out MethodInfo disposeAsync)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type.ImplementsInterface(typeof(IAsyncDisposable), out _))
                disposeAsync = typeof(IAsyncDisposable).GetMethod("DisposeAsync", Array.Empty<Type>());
            else
                disposeAsync = null;

            current = type.GetPublicProperty("Current");
            moveNextAsync = type.GetPublicMethod("MoveNextAsync");
            if (current is object && moveNextAsync is object)
                return true;

            if (type.ImplementsInterface(typeof(IAsyncEnumerator<>), out var genericArguments))
            {
                var interfaceType = typeof(IAsyncEnumerator<>).MakeGenericType(genericArguments[0]);
                current = interfaceType.GetProperty("Current");
                moveNextAsync = interfaceType.GetMethod("MoveNextAsync", Array.Empty<Type>());
                return true;
            }

            return false;
        }

        public static PropertyInfo GetPublicProperty(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

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

            return null;
        }

        public static MethodInfo GetPublicMethod(this Type type, string name, params Type[] parameters)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

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

            return null;
        }

        public static bool ImplementsInterface(this Type type, Type interfaceType, out Type[] genericArguments)
        {
            if (!interfaceType.IsGenericType)
            {
                genericArguments = null;
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

            genericArguments = null;
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