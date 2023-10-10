using NetFabric.Reflection;
using System;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions;

public static partial class ExpressionEx
{
    /// <summary>
    /// Creates a 'for' loop expression in C# with the specified initialization, condition,
    /// iterator, and loop body expressions.
    /// </summary>
    /// <param name="initialization">
    /// An <see cref="Expression"/> representing the loop initialization.
    /// This expression is executed once before the loop begins.
    /// </param>
    /// <param name="condition">
    /// An <see cref="Expression"/> representing the loop condition.
    /// The loop continues executing as long as this condition evaluates to true.
    /// </param>
    /// <param name="iterator">
    /// An <see cref="Expression"/> representing the loop iterator.
    /// This expression is executed after each iteration of the loop.
    /// </param>
    /// <param name="body">
    /// An <see cref="Expression"/> representing the body of the loop.
    /// This expression contains the statements to be executed in each iteration.
    /// </param>
    /// <returns>
    /// An <see cref="Expression"/> representing the 'for' loop with the specified components.
    /// </returns>
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

    /// <summary>
    /// Creates a 'for' loop expression in C# for iterating over elements of an array.
    /// </summary>
    /// <param name="array">
    /// An <see cref="Expression"/> representing the array to iterate over.
    /// </param>
    /// <param name="indexableInfo">
    /// An <see cref="IndexableInfo"/> object providing information about the indexable element,
    /// including the index variable and any additional indexing information. This object is obtained by
    /// calling the <see cref="IsIndexable(Type, out IndexableInfo?)"/> method on the array type.
    /// </param>
    /// <param name="body">
    /// A delegate that defines the loop body, taking an <see cref="Expression"/> representing the current
    /// element and returning an <see cref="Expression"/> representing the body of the loop for that element.
    /// </param>
    /// <returns>
    /// An <see cref="Expression"/> representing the 'for' loop with the specified components.
    /// </returns>
    /// <remarks>
    /// This method is specifically designed for creating 'for' loops to iterate over elements of arrays.
    /// The <paramref name="array"/> should represent an array, and the <paramref name="indexableInfo"/>
    /// should provide the necessary information to access and iterate over its elements. The <paramref name="body"/>
    /// delegate defines the operations to be performed on each element during iteration.
    /// </remarks>
    public static Expression For(Expression array, IndexableInfo indexableInfo, Func<Expression, Expression> body)
    {
        var arrayType = array.Type;
        if (!arrayType.IsArray)
            throw new ArgumentException($"'{arrayType}' is not an array", nameof(array));

        var indexVariable = Variable(indexableInfo.CountOrLength.GetGetMethod()!.ReturnType, "__index__");
        var arrayVariable = Variable(arrayType, "__array__");
        return Block(
            new[] { indexVariable, arrayVariable },
            Assign(arrayVariable, array), // local array for JIT compiler to remove bounds check
            For(
                Assign(indexVariable, Constant(0)),
                LessThan(indexVariable, Property(arrayVariable, indexableInfo.CountOrLength)),
                PostIncrementAssign(indexVariable),
                body(indexableInfo.Indexer is null
                    ? ArrayIndex(arrayVariable, indexVariable)
                    : Property(arrayVariable, indexableInfo.Indexer, indexVariable))
            )
        );
    }

}