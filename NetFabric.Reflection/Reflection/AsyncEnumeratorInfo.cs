using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NetFabric.Reflection
{
    /// <summary>
    /// Contains information about methods and properties of an enumerator that ´await foreach´ will use to enumerate a given type.
    /// </summary>
    public class AsyncEnumeratorInfo
    {
        /// <summary>
        /// Information on the property that 'await foreach' will use to get the value of current item.
        /// </summary>
        public PropertyInfo Current { get; }
        
        /// <summary>
        /// Information on the method that 'await foreach' will use to iterate to the next item.
        /// </summary>
        public MethodInfo MoveNextAsync { get; }
        
        /// <summary>
        /// Information on the method that 'await foreach' will use to dispose the enumerator, if it's disposable.
        /// </summary>
        /// <remarks>
        /// To be considered disposable, the enumerator has to implement <see cref="System.IAsyncDisposable"/>,
        /// except if it's a by reference like value type (ref struct) which only needs to have a public
        /// method named 'DisposeAsync'.
        /// </remarks>
        public MethodInfo? DisposeAsync { get; init; }
        
        /// <summary>
        /// Indicates if 'await foreach' considers the enumerator to be a value type.
        /// </summary>
        public bool IsValueType { get; init; }    
        
        /// <summary>
        /// Indicates if 'await foreach' considers the enumerator to be of type <see cref="System.Collections.Generic.IAsyncEnumerator&lt;&gt;"/>.
        /// </summary>
        public bool IsAsyncEnumeratorInterface { get; init; }    

        internal AsyncEnumeratorInfo(PropertyInfo current, MethodInfo moveNextAsync)
        {
            Current = current;
            MoveNextAsync = moveNextAsync;
        }

        public object? GetValueCurrent(object instance)
        {
            try
            {
                return Current.GetValue(instance);
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {Current.DeclaringType!.Name}.Current.", exception.InnerException!);
            }
        }

        public ValueTask<bool> InvokeMoveNextAsync(object instance)
        {
            try
            {
                return (ValueTask<bool>)MoveNextAsync.Invoke(instance, null)!;
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {MoveNextAsync.DeclaringType!.Name}.MoveNextAsync().", exception.InnerException!);
            }
        }

        public ValueTask InvokeDisposeAsync(object instance)
        {
            if (DisposeAsync is null)
                throw new NotSupportedException("DisposeAsync() is not defined.");

            try
            {
                return (ValueTask)DisposeAsync.Invoke(instance, null)!;
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {DisposeAsync.DeclaringType!.Name}.DisposeAsync().", exception.InnerException!);
            }
        }
    }
}