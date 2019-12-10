using System;
using System.Reflection;

namespace NetFabric.Reflection
{
    public struct EnumerableInfo
    {
        public readonly MethodInfo GetEnumerator;
        public readonly EnumeratorInfo EnumeratorInfo;

        public EnumerableInfo(MethodInfo getEnumerator, EnumeratorInfo enumeratorInfo)
        {
            GetEnumerator = getEnumerator;
            EnumeratorInfo = enumeratorInfo;
        }

        public object InvokeGetEnumerator(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            if (GetEnumerator is null)
                throw new Exception("GetEnumerator() is not defined.");

            try
            {
                return GetEnumerator.Invoke(instance, Array.Empty<object>());
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {GetEnumerator.DeclaringType}.GetEnumerator()", exception.InnerException);
            }
        }
    }
}