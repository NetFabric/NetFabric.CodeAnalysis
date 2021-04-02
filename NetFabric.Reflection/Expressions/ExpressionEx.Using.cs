using NetFabric.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions
{
    public static partial class ExpressionEx
    {
        public static Expression Using(Expression variable, Expression body)
        {
            if (!variable.Type.IsDisposable(out var disposeMethodInfo, out var isByRefLike))
                ThrowMustBeImplicitlyConvertibleToIDisposable<MethodInfo>(variable);
                    
            return TryFinally(
                body,
                variable.Type switch
                {
                    { IsValueType: true } => DisposeValueType(disposeMethodInfo, isByRefLike, variable),
                    _ => DisposeReferenceType(disposeMethodInfo, variable)
                });

            static Expression DisposeValueType(MethodInfo disposeMethodInfo, bool isByRefLike, Expression variable)
            {
                return isByRefLike switch
                {
                    true => DisposeByRefLike(disposeMethodInfo, variable),
                    _ => Dispose(disposeMethodInfo, variable)
                };

                static Expression DisposeByRefLike(MethodInfo disposeMethodInfo, Expression variable)
                    => Call(variable, disposeMethodInfo);

                static Expression Dispose(MethodInfo disposeMethodInfo, Expression variable)
                    => Call(Convert(variable, typeof(IDisposable)), disposeMethodInfo);
            }

            static Expression DisposeReferenceType(MethodInfo disposeMethodInfo, Expression variable)
                => IfThen(
                    NotEqual(variable, Constant(null)),
                    Call(Convert(variable, typeof(IDisposable)), disposeMethodInfo)
                );

            [DoesNotReturn]
            static T ThrowMustBeImplicitlyConvertibleToIDisposable<T>(Expression variable)
                => throw new Exception($"'{variable.Type.Name}': type used in a using statement must be implicitly convertible to 'System.IDisposable'");
        }
    }
}