using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions;

public static partial class ExpressionEx
{
    /// <summary>
    /// Creates a <see cref="System.Linq.Expressions.LoopExpression"/> with the given continuation condition and body.
    /// </summary>
    /// <param name="condition">An <see cref="System.Linq.Expressions.Expression"/> that breaks the loop when evaluates to false.</param>
    /// <param name="body">The body of the loop.</param>
    /// <returns>A <see cref="System.Linq.Expressions.LoopExpression"/> containing the condition evaluation and a break label. If the condition evaluates to false, it breaks to the label.</returns>
    public static LoopExpression While(Expression condition, Expression body)
    {
        var label = Label();
        return Loop(
            IfThenElse(
                condition,
                body,
                Break(label)
            ),
            label
        );
    }
}