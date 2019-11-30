using System;
using System.Reflection;

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

            getEnumerator = type.GetInstancePublicMethod("GetEnumerator");
            if (getEnumerator is object)
                return true;

            foreach (var @interface in type.GetInterfaces())
            {
                getEnumerator = @interface.GetInstancePublicMethod("GetEnumerator");
                if (getEnumerator is object)
                    return true;
            }

            return false;
        }

        public static bool IsAsyncEnumerable(this Type type, out MethodInfo getAsyncEnumerator)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            getAsyncEnumerator = type.GetInstancePublicMethod("GetAsyncEnumerator");
            if (getAsyncEnumerator is object)
                return true;

            foreach (var @interface in type.GetInterfaces())
            {
                getAsyncEnumerator = @interface.GetInterfacePublicMethod("GetAsyncEnumerator");
                if (getAsyncEnumerator is object)
                    return true;
            }

            return false;
        }

        public static bool IsEnumerator(this Type type, out PropertyInfo current, out MethodInfo moveNext)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            current = type.GetInstancePublicProperty("Current");
            moveNext = type.GetInstancePublicMethod("MoveNext");
            if (current is object && moveNext is object)
                return true;

            foreach (var @interface in type.GetInterfaces())
            {
                current = @interface.GetInterfacePublicProperty("Current");
                moveNext = @interface.GetInterfacePublicMethod("MoveNext");
                if (current is object && moveNext is object)
                    return true;
            }

            return false;
        }

        public static bool IsAsyncEnumerator(this Type type, out PropertyInfo current, out MethodInfo moveNextAsync)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            current = type.GetInstancePublicProperty("Current");
            moveNextAsync = type.GetInstancePublicMethod("MoveNextAsync");
            if (current is object && moveNextAsync is object)
                return true;

            foreach (var @interface in type.GetInterfaces())
            {
                current = @interface.GetInterfacePublicProperty("Current");
                moveNextAsync = @interface.GetInterfacePublicMethod("MoveNextAsync");
                if (current is object && moveNextAsync is object)
                    return true;
            }

            return false;
        }

        public static PropertyInfo GetInstancePublicProperty(this Type type, string name)
        {
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

            return baseType.GetInstancePublicProperty(name);
        }

        public static PropertyInfo GetInterfacePublicProperty(this Type type, string name)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (!type.IsInterface)
                throw new ArgumentException("Type must be an interface.", nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var properties = type.GetProperties();
            for (var index = 0; index < properties.Length; index++)
            {
                var property = properties[index];
                if (property.Name == name && property.GetGetMethod() is object)
                    return property;
            }

            foreach (var @interface in type.GetInterfaces())
            {
                var property = @interface.GetInterfacePublicProperty(name);
                if (property is object)
                    return property;
            }

            return null;
        }

        public static MethodInfo GetInstancePublicMethod(this Type type, string name, params Type[] parameters)
        {
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

            return baseType.GetInstancePublicMethod(name, parameters);
        }

        public static MethodInfo GetInterfacePublicMethod(this Type type, string name, params Type[] parameters)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (!type.IsInterface)
                throw new ArgumentException("Type must be an interface.", nameof(type));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var methods = type.GetMethods();
            for (var index = 0; index < methods.Length; index++)
            {
                var method = methods[index];
                if (method.Name == name && SequenceEqual(method.GetParameters(), parameters))
                    return method;
            }

            foreach (var @interface in type.GetInterfaces())
            {
                var method = @interface.GetInterfacePublicMethod(name);
                if (method is object)
                    return method;
            }

            return null;
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