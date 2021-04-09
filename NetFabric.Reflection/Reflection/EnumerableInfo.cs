using System;
using System.Reflection;

namespace NetFabric.Reflection
{
    public class EnumerableInfo
    {
        public MethodInfo GetEnumerator { get; }
        public EnumeratorInfo EnumeratorInfo { get; }

        public EnumerableInfo(MethodInfo getEnumerator, EnumeratorInfo enumeratorInfo)
        {
            GetEnumerator = getEnumerator;
            EnumeratorInfo = enumeratorInfo;
        }

        public object InvokeGetEnumerator(object instance)
        {
            try
            {
                return GetEnumerator.Invoke(instance, null)!;
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {GetEnumerator.DeclaringType!.Name}.GetEnumerator().", exception.InnerException!);
            }
        }
    }
}