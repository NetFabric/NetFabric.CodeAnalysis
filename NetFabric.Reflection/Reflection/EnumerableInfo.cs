using System.Reflection;

namespace NetFabric.Reflection
{
    /// <summary>
    /// Contains information about methods and properties that ´foreach´ will use to enumerate a given type.
    /// </summary>
    public class EnumerableInfo
    {
        /// <summary>
        /// Information on the method that 'foreach' will use to get a new instance of the enumerator.
        /// </summary>
        public MethodInfo GetEnumerator { get; }
        
        /// <summary>
        /// Information on the enumerator methods that 'foreach' will use.
        /// </summary>
        public EnumeratorInfo EnumeratorInfo { get; }

        internal EnumerableInfo(MethodInfo getEnumerator, EnumeratorInfo enumeratorInfo)
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