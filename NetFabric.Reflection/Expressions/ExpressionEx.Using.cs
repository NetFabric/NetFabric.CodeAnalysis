using NetFabric.Reflection;
using System;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions
{
    public static partial class ExpressionEx
    {
        public static Expression Using(Expression variable, Expression body)
        {
            return TryFinally(
                body,
                variable.Type switch
                {
                    { IsValueType: true } => DisposeValueType(variable),
                    _ => DisposeReferenceType(variable)
                });

            static Expression DisposeValueType(Expression variable)
            {
                return variable.Type.IsByRefLike() switch
                {
                    true => DisposeByRefLike(variable),
                    _ => Dispose(variable)
                };

                static Expression DisposeByRefLike(Expression variable)
                {
                    var disposeMethodInfo = variable.Type.GetPublicInstanceDeclaredOnlyMethod(nameof(IDisposable.Dispose), Type.EmptyTypes)
                                            ?? ThrowMustBeImplicitlyConvertibleToIDisposable<MethodInfo>(variable);
                    return Call(variable, disposeMethodInfo);
                }

                static Expression Dispose(Expression variable)
                    => typeof(IDisposable).IsAssignableFrom(variable.Type) switch
                    {
                        false => ThrowMustBeImplicitlyConvertibleToIDisposable<Expression>(variable),

                        _ => Call(Convert(variable, typeof(IDisposable)), Reflection.TypeExtensions.DisposeInfo)
                    };
            }

            static Expression DisposeReferenceType(Expression variable)
                => typeof(IDisposable).IsAssignableFrom(variable.Type) switch
                {
                    false => ThrowMustBeImplicitlyConvertibleToIDisposable<Expression>(variable),

                    _ => IfThen(
                            NotEqual(variable, Constant(null)),
                            Call(Convert(variable, typeof(IDisposable)), Reflection.TypeExtensions.DisposeInfo)
                        )
                };

            static T ThrowMustBeImplicitlyConvertibleToIDisposable<T>(Expression variable)
                => throw new Exception($"'{variable.Type.FullName}': type used in a using statement must be implicitly convertible to 'System.IDisposable'");
        }
    }
}