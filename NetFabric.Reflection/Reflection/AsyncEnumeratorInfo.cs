using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NetFabric.Reflection
{
    public class AsyncEnumeratorInfo
    {
        public readonly PropertyInfo Current;
        public readonly MethodInfo MoveNextAsync;
        public readonly MethodInfo? DisposeAsync;

        public AsyncEnumeratorInfo(PropertyInfo current, MethodInfo moveNextAsync, MethodInfo? disposeAsync)
        {
            Current = current;
            MoveNextAsync = moveNextAsync;
            DisposeAsync = disposeAsync;
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