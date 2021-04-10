using System;
using System.Reflection;

namespace NetFabric.Reflection
{
    /// <summary>
    /// Contains information about methods and properties of an enumerator that ´foreach´ will use to enumerate a given type.
    /// </summary>
    public class EnumeratorInfo
    {
        /// <summary>
        /// Information on the property that 'foreach' will use to get the value of current item.
        /// </summary>
        public PropertyInfo Current { get; }
        
        /// <summary>
        /// Information on the method that 'foreach' will use to iterate to the next item.
        /// </summary>
        public MethodInfo MoveNext { get; }
        
        /// <summary>
        /// Information on the 'Reset' method, if defined. 
        /// </summary>
        public MethodInfo? Reset { get; init; }
        
        /// <summary>
        /// Information on the method that 'foreach' will use to dispose the enumerator, if it's disposable.
        /// </summary>
        /// <remarks>
        /// To be considered disposable, the enumerator has to implement <see cref="System.IDisposable"/>,
        /// except if it's a by reference like value type (ref struct) which only needs to have a public
        /// method named 'Dispose'.
        /// </remarks>
        public MethodInfo? Dispose { get; init; }
        
        /// <summary>
        /// Indicates if 'foreach' considers the enumerator to be a value type.
        /// </summary>
        public bool IsValueType { get; init; }    
        
        /// <summary>
        /// Indicates if 'foreach' considers the enumerator to be a by reference like value type (ref struct).
        /// </summary>
        public bool IsByRefLike { get; init; }
        
        /// <summary>
        /// Indicates if 'foreach' considers the enumerator to be of type <see cref="System.Collections.Generic.IEnumerator&lt;&gt;"/>.
        /// </summary>
        public bool IsGenericsEnumeratorInterface { get; init; }    
        
        /// <summary>
        /// Indicates if 'foreach' considers the enumerator to be of type <see cref="System.Collections.IEnumerator"/>.
        /// </summary>
        public bool IsEnumeratorInterface { get; init; }    

        internal EnumeratorInfo(PropertyInfo current, MethodInfo moveNext)
        {
            Current = current;
            MoveNext = moveNext;
        }

        public object? InvokeGetCurrent(object instance)
        {
            try
            {
                return Current.GetValue(instance);
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {Current.DeclaringType!.Name}.get_Current.", exception.InnerException!);
            }
        }

        public bool InvokeMoveNext(object instance)
        {
            try
            {
                return (bool)MoveNext.Invoke(instance, null)!;
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {MoveNext.DeclaringType!.Name}.MoveNext().", exception.InnerException!);
            }
        }

        public void InvokeReset(object instance)
        {
            if (Reset is null)
                throw new NotSupportedException("Reset() is not defined.");

            try
            {
                Reset.Invoke(instance, null);
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {Reset.DeclaringType!.Name}.Reset().", exception.InnerException!);
            }
        }

        public void InvokeDispose(object instance)
        {
            if (Dispose is null)
                throw new NotSupportedException("Dispose() is not defined.");

            try
            {
                Dispose.Invoke(instance, null);
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {Dispose.DeclaringType!.Name}.Dispose().", exception.InnerException!);
            }
        }
    }
}