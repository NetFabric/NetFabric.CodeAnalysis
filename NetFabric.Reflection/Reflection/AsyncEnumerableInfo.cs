using System;
using System.Reflection;
using System.Threading;

namespace NetFabric.Reflection
{
    /// <summary>
    /// Contains information about methods and properties that ´await foreach´ will use to enumerate a given type.
    /// </summary>
    public class AsyncEnumerableInfo
    {
        /// <summary>
        /// Information on the method that 'await foreach' will use to get a new instance of the enumerator.
        /// </summary>
        public MethodInfo GetAsyncEnumerator { get; }
        
        /// <summary>
        /// Information on the enumerator methods that 'await foreach' will use.
        /// </summary>
        public AsyncEnumeratorInfo EnumeratorInfo { get; }

        internal AsyncEnumerableInfo(MethodInfo getAsyncEnumerator, AsyncEnumeratorInfo enumeratorInfo)
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
                    0 => GetAsyncEnumerator.Invoke(instance, null)!,
                    1 => GetAsyncEnumerator.Invoke(instance, new object[] { token })!,
                    _ => throw new Exception("Unexpected number of parameters for GetAsyncEnumerator()."),
                };
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {GetAsyncEnumerator.DeclaringType!.Name}.GetAsyncEnumerator().", exception.InnerException!);
            }
        }
    }
}