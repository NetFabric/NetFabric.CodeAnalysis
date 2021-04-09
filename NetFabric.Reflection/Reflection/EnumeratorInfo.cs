using System;
using System.Reflection;

namespace NetFabric.Reflection
{
    public class EnumeratorInfo
    {
        public PropertyInfo Current { get; }
        public MethodInfo MoveNext { get; }
        public MethodInfo? Reset { get; init; }
        public MethodInfo? Dispose { get; init; }
        public bool IsValueType { get; init; }    
        public bool IsByRefLike { get; init; }
        public bool IsGenericsEnumeratorInterface { get; init; }    
        public bool IsEnumeratorInterface { get; init; }    

        public EnumeratorInfo(PropertyInfo current, MethodInfo moveNext)
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