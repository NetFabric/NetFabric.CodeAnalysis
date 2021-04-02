using System;
using System.Linq.Expressions;
using AgileObjects.ReadableExpressions;
using Xunit;
using static NetFabric.Expressions.ExpressionEx;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions.CSharp.UnitTests
{
    public class ForTests
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
        public void For_Must_Succeed(int[] value, int start, int end, int expectedSum)
        {
            // Arrange
            const string expectedExpression = @"var sum = 0;
var index = start;
while (true)
{
    if (index < end)
    {
        sum += value[index];
        index++;
    }
    else
    {
        break;
    }
}

return sum;";

            // Act
            var (expression, _, _, _) = CreateSumExpression();
            var sum = Sum(value, start, end);

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
            Assert.Equal(expectedSum, sum);
        }
        
        static (Expression, ParameterExpression, ParameterExpression, ParameterExpression) CreateSumExpression()
        {
            var valueParameter = Parameter(typeof(int[]), "value");
            var startParameter = Parameter(typeof(int), "start");
            var endParameter = Parameter(typeof(int), "end");
            var indexVariable = Variable(typeof(int), "index");
            var sumVariable = Variable(typeof(int), "sum");
            var expression = Block(
                new[] { indexVariable, sumVariable },
                Assign(sumVariable, Constant(0)),
                For(
                    Assign(indexVariable, startParameter), 
                    LessThan(indexVariable, endParameter), 
                    PostIncrementAssign(indexVariable),
                    AddAssign(sumVariable, ArrayIndex(valueParameter, indexVariable))),
                sumVariable);
            return (expression, valueParameter, startParameter, endParameter);
        }

        static int Sum(int[] value, int start, int end)
        {
            var (expression, valueParameter, startParameter, endParameter) = CreateSumExpression();
            var sum = Lambda<Func<int[], int, int, int>>(expression, valueParameter, startParameter, endParameter).Compile();
            return sum(value, start, end);
        }

    }
}
