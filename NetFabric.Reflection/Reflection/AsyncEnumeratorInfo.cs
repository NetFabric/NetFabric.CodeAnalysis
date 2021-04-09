using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NetFabric.Reflection
{
    public class AsyncEnumeratorInfo
    {
        public PropertyInfo Current { get; }
        public MethodInfo MoveNextAsync { get; }
        public MethodInfo? DisposeAsync { get; init; }
        public bool IsValueType { get; init; }    
        public bool IsAsyncEnumeratorInterface { get; init; }    

        public AsyncEnumeratorInfo(PropertyInfo current, MethodInfo moveNextAsync)
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