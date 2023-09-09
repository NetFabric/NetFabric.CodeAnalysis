using System;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions;

public static partial class ExpressionEx
{
    /// <summary>
    /// Creates a <see cref="System.Linq.Expressions.Expression"/> that represents a type conversion where <c>true</c> is supplied and the conversion result is assigned to <paramref name="result"/> if the conversion succeeds.
    /// </summary>
    /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> that will be converted.</param>
    /// <param name="type">The <see cref="System.Type"/> to try to convert to.</param>
    /// <param name="result">A <see cref="System.Linq.Expressions.ParameterExpression"/> to which the conversion is assigned to if it succeeded.</param>
    /// <returns>An <see cref="System.Linq.Expressions.Expression"/> that evaluates to <c>true</c> if conversion succeeds, otherwise evaluates to <c>false</c>.</returns>
    public static Expression TypeIs(Expression expression, Type type, ParameterExpression result)
        => Block(
            Assign(result, TypeAs(expression, type)),
            NotEqual(result, Constant(null))
        );
}