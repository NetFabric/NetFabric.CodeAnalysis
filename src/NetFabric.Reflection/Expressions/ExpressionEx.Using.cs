using NetFabric.Reflection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions;

public static partial class ExpressionEx
{
    /// <summary>
    /// Creates a <see cref="System.Linq.Expressions.TryExpression"/> that that ensures the correct use of <see cref="System.IDisposable"/> objects.
    /// </summary>
    /// <param name="instance">An <see cref="System.Linq.Expressions.ParameterExpression"/> that defines the object instance to be disposed.</param>
    /// <param name="body">The body of the <see cref="System.Linq.Expressions.TryExpression"/>.</param>
    /// <returns>The created <see cref="System.Linq.Expressions.TryExpression"/>.</returns>
    /// <exception cref="Exception">Object type does not disposable.</exception>
    /// <remarks>
    /// <p>
    /// <see cref="NetFabric.Expressions.ExpressionEx.Using(Expression,Expression)"/> ensures that <see cref="System.IDisposable.Dispose"/>
    /// is called even if an exception occurs within the body.
    /// </p>
    /// <p>
    /// The <see cref="System.Linq.Expressions.Expression"/> in property <see cref="System.Linq.Expressions.TryExpression.Finally"/> depends on if the object to
    /// be disposed is an <c>interface</c>, <c>class</c>, <c>struct</c> or <c>ref struct</c>.
    /// </p>
    /// <p>
    /// An object to be considered disposable has to implement <see cref="System.IDisposable"/>, except for <c>ref struct</c> for which is enough
    /// to a have a public parameterless method named Dispose.
    /// </p>
    /// </remarks>
    public static TryExpression Using(ParameterExpression instance, Expression body)
    {
        if (!instance.Type.IsDisposable(out var disposeMethodInfo))
            throw new Exception($"'{instance.Type.Name}': type used in a using statement must be implicitly convertible to 'System.IDisposable'");

        return TryFinally(
            body,
            Dispose(disposeMethodInfo, instance)
        );

        static Expression Dispose(MethodInfo disposeMethodInfo, ParameterExpression instance)
        {
            return instance.Type switch
            {
                {IsByRefLike: true} => DisposeByRefLikeType(disposeMethodInfo, instance),
                {IsValueType: true} => DisposeValueType(disposeMethodInfo, instance),
                _ => DisposeReferenceType(disposeMethodInfo, instance)
            };

            static Expression DisposeByRefLikeType(MethodInfo disposeMethodInfo, ParameterExpression instance)
                => Call(instance, disposeMethodInfo);

            static Expression DisposeValueType(MethodInfo disposeMethodInfo, Expression instance)
                => Call(instance, disposeMethodInfo);

            static Expression DisposeReferenceType(MethodInfo disposeMethodInfo, ParameterExpression instance)
                => IfThen(
                    NotEqual(instance, Constant(null)),
                    Call(Convert(instance, typeof(IDisposable)), disposeMethodInfo)
                );
        }
    }

    /// <summary>
    /// Creates a <see cref="System.Linq.Expressions.TryExpression"/> that that ensures the correct use of <see cref="System.IDisposable"/> objects.
    /// </summary>
    /// <param name="instances">A collection of <see cref="System.Linq.Expressions.Expression"/>, each defining an object instance to be disposed.</param>
    /// <param name="body">The body of the <see cref="System.Linq.Expressions.TryExpression"/>.</param>
    /// <returns>The created <see cref="System.Linq.Expressions.TryExpression"/>.</returns>
    /// <exception cref="Exception">Object type does not implement <see cref="System.IDisposable"/>.</exception>
    /// <remarks>
    /// <p>
    /// <see cref="NetFabric.Expressions.ExpressionEx.Using(IEnumerable&lt;ParameterExpression&gt;,Expression)"/> ensures that <see cref="System.IDisposable.Dispose"/>
    /// is called even if an exception occurs within the body.
    /// </p>
    /// <p>
    /// The <see cref="System.Linq.Expressions.Expression"/> in property <see cref="System.Linq.Expressions.TryExpression.Finally"/> depends on if the object to
    /// be disposed is an <c>interface</c>, <c>class</c>, <c>struct</c> or <c>ref struct</c>.
    /// </p>
    /// </remarks>
    public static TryExpression Using(IEnumerable<ParameterExpression> instances, Expression body)
    {
        TryExpression? result = default;
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach(var instance in instances)
        {
            result = Using(instance, result ?? body);
        }
        if (result is null) 
            throw new ArgumentException($"Must be not empty.", nameof(instances));
        return result;
    }
}