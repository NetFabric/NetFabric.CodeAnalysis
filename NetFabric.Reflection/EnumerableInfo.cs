using System;
using System.Reflection;

namespace NetFabric.Reflection
{
    public class EnumerableInfo
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
            try
            {
                return GetEnumerator.Invoke(instance, Array.Empty<object>());
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {GetEnumerator.DeclaringType.Name}.GetEnumerator().", exception.InnerException);
            }
        }
    }
}