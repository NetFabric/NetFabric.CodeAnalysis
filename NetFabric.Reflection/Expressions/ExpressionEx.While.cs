using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions
{
    public static partial class ExpressionEx
    {
        public static Expression While(Expression condition, Expression body)
        {
            var breakLabel = Label();
            return Loop(
                IfThenElse(
                    condition,
                    body,
                    Break(breakLabel)
                ),
                breakLabel);
        }
    }
}