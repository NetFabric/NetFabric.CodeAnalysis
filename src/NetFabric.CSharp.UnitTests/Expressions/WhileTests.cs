using System;
using Xunit;
using static NetFabric.Expressions.ExpressionEx;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions.CSharp.UnitTests;

public class WhileTests
{
    public static TheoryData<int[], int, int, int> Data =>
        new()
        {
            { Array.Empty<int>(), 0, 0, 0 },
            { new[] { 1 }, 0, 1, 1 },
            { new[] { 1, 2, 3, 4, 5 }, 0, 5, 15 },
            { new[] { 1, 2, 3, 4, 5 }, 1, 4, 9 },
        };

    [Theory]
    [MemberData(nameof(Data))]
    public void While_Must_Succeed(int[] value, int start, int end, int expected)
    {
        // Arrange
        var valueParameter = Parameter(typeof(int[]), "value");
        var startParameter = Parameter(typeof(int), "start");
        var endParameter = Parameter(typeof(int), "end");
        var sumVariable = Variable(typeof(int), "sum");
        var indexVariable = Variable(typeof(int), "index");
        var expression = Block(
            new[] { indexVariable, sumVariable },
            Assign(sumVariable, Constant(0)),
            Assign(indexVariable, startParameter),
            While(
                LessThan(indexVariable, endParameter), 
                Block(
                    AddAssign(sumVariable, ArrayIndex(valueParameter, indexVariable)),
                    PostIncrementAssign(indexVariable)
                )
            ),
            sumVariable);
        var sum = Lambda<Func<int[], int, int, int>>(expression, valueParameter, startParameter, endParameter).Compile();

        // Act
        var result = sum(value, start, end);

        // Assert
        Assert.Equal(expected, result);
    }
}
