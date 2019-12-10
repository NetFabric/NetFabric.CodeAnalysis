using System;
using System.Reflection;

namespace NetFabric.Reflection
{
    public struct EnumeratorInfo
    {
        public readonly PropertyInfo Current;
        public readonly MethodInfo MoveNext;
        public readonly MethodInfo Reset;
        public readonly MethodInfo Dispose;

        public EnumeratorInfo(PropertyInfo current, MethodInfo moveNext, MethodInfo reset, MethodInfo dispose)
        {
            Current = current;
            MoveNext = moveNext;
            Reset = reset;
            Dispose = dispose;
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

        public bool InvokeMoveNext(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            if (MoveNext is null)
                throw new Exception("MoveNext() is not defined.");

            try
            {
                return (bool)MoveNext.Invoke(instance, Array.Empty<object>());
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {MoveNext.DeclaringType}.MoveNext()", exception.InnerException);
            }
        }

        public void InvokeReset(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            if (Dispose is null)
                throw new Exception("Reset() is not defined.");

            try
            {
                Reset.Invoke(instance, Array.Empty<object>());
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {Reset.DeclaringType}.Reset()", exception.InnerException);
            }
        }

        public void InvokeDispose(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            if (Dispose is null)
                throw new Exception("Dispose() is not defined.");

            try
            {
                Dispose.Invoke(instance, Array.Empty<object>());
            }
            catch (TargetInvocationException exception)
            {
                throw new EnumerationException($"Unhandled exception in {Dispose.DeclaringType}.Dispose()", exception.InnerException);
            }
        }
    }
}