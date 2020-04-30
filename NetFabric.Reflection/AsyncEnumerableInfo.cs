using System;
using System.Reflection;
using System.Threading;

namespace NetFabric.Reflection
{
    public class AsyncEnumerableInfo
    {
        public readonly MethodInfo GetAsyncEnumerator;
        public readonly AsyncEnumeratorInfo EnumeratorInfo;

        public AsyncEnumerableInfo(MethodInfo getAsyncEnumerator, AsyncEnumeratorInfo enumeratorInfo)
        {
            GetAsyncEnumerator = getAsyncEnumerator;
            EnumeratorInfo = enumeratorInfo;
        }

        public object InvokeGetAsyncEnumerator(object instance, CancellationToken token = default)
        {
            try
            {
                return GetAsyncEnumerator.GetParameters().Length switch
                {
                    0 => GetAsyncEnumerator.Invoke(instance, Array.Empty<object>()),
                    1 => GetAsyncEnumerator.Invoke(instance, new object[] { token }),
                    _ => throw new Exception("Unexpected number of parameters for GetAsyncEnumerator()."),
                };
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {GetAsyncEnumerator.DeclaringType.Name}.GetAsyncEnumerator().", exception.InnerException);
            }
        }
    }
}