using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions
{
    public static partial class ExpressionEx
    {
        public static Expression For(Expression initialization, Expression condition, Expression iterator, Expression body) 
            => Block(initialization,
                While(condition, Block(
                    body,
                    iterator
                ))
            );
    }
}