using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NetFabric.Reflection
{
    public struct AsyncEnumeratorInfo
    {
        public readonly PropertyInfo Current;
        public readonly MethodInfo MoveNextAsync;
        public readonly MethodInfo DisposeAsync;

        public AsyncEnumeratorInfo(PropertyInfo current, MethodInfo moveNextAsync, MethodInfo disposeAsync)
        {
            Current = current;
            MoveNextAsync = moveNextAsync;
            DisposeAsync = disposeAsync;
        }

        public object GetValueCurrent(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            if (Current is null)
                throw new Exception("Current is not defined.");

            try
            {
                return Current.GetValue(instance);
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {Current.DeclaringType}.Current", exception.InnerException);
            }
        }

        public ValueTask<bool> InvokeMoveNextAsync(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            if (MoveNextAsync is null)
                throw new Exception("MoveNextAsync() is not defined.");

            try
            {
                return (ValueTask<bool>)MoveNextAsync.Invoke(instance, Array.Empty<object>());
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {MoveNextAsync.DeclaringType}.MoveNextAsync()", exception.InnerException);
            }
        }

        public ValueTask InvokeDisposeAsync(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            if (DisposeAsync is null)
                throw new Exception("DisposeAsync() is not defined.");

            try
            {
                return (ValueTask)DisposeAsync.Invoke(instance, Array.Empty<object>());
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {DisposeAsync.DeclaringType}.DisposeAsync()", exception.InnerException);
            }
        }
    }
}