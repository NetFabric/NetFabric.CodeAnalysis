using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace NetFabric.CodeAnalysis
{
    public static class TypeExtensions
    {
        public static bool IsEnumerable(this Type type, out MethodInfo getEnumerator, out PropertyInfo current, out MethodInfo moveNext)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsEnumerable(out getEnumerator))
            {
                current = null;
                moveNext = null;
                return false;
            }

            return getEnumerator.ReturnType.IsEnumerator(out current, out moveNext);
        }

        public static bool IsAsyncEnumerable(this Type type, out MethodInfo getAsyncEnumerator, out PropertyInfo current, out MethodInfo moveNextAsync)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsAsyncEnumerable(out getAsyncEnumerator))
            {
                current = null;
                moveNextAsync = null;
                return false;
            }

            return getAsyncEnumerator.ReturnType.IsAsyncEnumerator(out current, out moveNextAsync);
        }

        public static bool IsEnumerable(this Type type, out MethodInfo getEnumerator)
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

        public static bool IsAsyncEnumerable(this Type type, out MethodInfo getAsyncEnumerator)
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

        public static bool IsEnumerator(this Type type, out PropertyInfo current, out MethodInfo moveNext)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

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

        public static bool IsAsyncEnumerator(this Type type, out PropertyInfo current, out MethodInfo moveNextAsync)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

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
            if (type.IsInterface)
                throw new ArgumentException("Type must not be an interface.", nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            for (var index = 0; index < properties.Length; index++)
            {
                var property = properties[index];
                if (property.Name == name && property.GetGetMethod() is object)
                    return property;
            }

            var baseType = type.BaseType;
            if (baseType is null)
                return null;

            return baseType.GetPublicProperty(name);
        }

        public static MethodInfo GetPublicMethod(this Type type, string name, params Type[] parameters)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsInterface)
                throw new ArgumentException("Type must not be an interface.", nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            for (var index = 0; index < methods.Length; index++)
            {
                var method = methods[index];
                if (method.Name == name && SequenceEqual(method.GetParameters(), parameters))
                    return method;
            }

            var baseType = type.BaseType;
            if (baseType is null)
                return null;

            return baseType.GetPublicMethod(name, parameters);
        }

        static bool ImplementsInterface(this Type type, Type interfaceType, out Type[] genericArguments)
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

        static IEnumerable<Type> GetAllInterfaces(this Type type)
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