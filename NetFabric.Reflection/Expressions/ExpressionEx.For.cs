using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions;

public static partial class ExpressionEx
{
    /// <summary>
    /// Creates a <see cref="System.Linq.Expressions.Expression"/> with behaviour similar to the <c>for</c> statement in C#.
    /// </summary>
    /// <param name="initialization">An <see cref="System.Linq.Expressions.Expression"/> containing the initialization before the loop execution.</param>
    /// <param name="condition">An <see cref="System.Linq.Expressions.Expression"/> that breaks the loop when evaluates to false.</param>
    /// <param name="iterator">An <see cref="System.Linq.Expressions.Expression"/> containing the operation executed after the body on every loop iteration.</param>
    /// <param name="body">An <see cref="System.Linq.Expressions.Expression"/> containing the body executed on every loop iteration.</param>
    /// <returns>The created <see cref="System.Linq.Expressions.Expression"/>.</returns>
    public static Expression For(Expression initialization, Expression condition, Expression iterator, Expression body) 
        => Block(
            initialization,
            While(
                condition, 
                Block(
                    body,
                    iterator
                )
            )
        );
}