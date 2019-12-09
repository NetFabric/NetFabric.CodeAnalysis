using System;
using System.Reflection;
using System.Threading;

namespace NetFabric.Reflection
{
    public struct AsyncEnumerableInfo
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
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            if (GetAsyncEnumerator is null)
                throw new Exception("GetAsyncEnumerator() is not defined.");

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
                throw new Exception("Unhandled exception in GetAsyncEnumerator()", exception.InnerException);
            }
        }
    }
}