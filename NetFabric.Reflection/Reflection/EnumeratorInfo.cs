using System;
using System.Reflection;

namespace NetFabric.Reflection
{
    public class EnumeratorInfo
    {
        public readonly PropertyInfo Current;
        public readonly MethodInfo MoveNext;
        public readonly MethodInfo? Reset;
        public readonly MethodInfo? Dispose;
        public readonly bool IsByRefLike;

        public EnumeratorInfo(PropertyInfo current, MethodInfo moveNext, MethodInfo? reset, MethodInfo? dispose, bool isByRefLike)
        {
            Current = current;
            MoveNext = moveNext;
            Reset = reset;
            Dispose = dispose;
            IsByRefLike = isByRefLike;
        }

        public object GetValueCurrent(object instance)
        {
            try
            {
                return Current.GetValue(instance);
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {Current.DeclaringType.Name}.Current.", exception.InnerException);
            }
        }

        public bool InvokeMoveNext(object instance)
        {
            try
            {
                return (bool)MoveNext.Invoke(instance, null);
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {MoveNext.DeclaringType.Name}.MoveNext().", exception.InnerException);
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
                throw new EnumerationException($"Unhandled exception in {Reset.DeclaringType.Name}.Reset().", exception.InnerException);
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
                throw new EnumerationException($"Unhandled exception in {Dispose.DeclaringType.Name}.Dispose().", exception.InnerException);
            }
        }
    }
}