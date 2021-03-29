using System;
using Xunit;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions.UnitTests
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
        public void For_Must_Succeed(int[] value, int start, int end, int expected)
        {
            // Arrange
            var valueParameter = Parameter(typeof(int[]), "value");
            var startParameter = Parameter(typeof(int), "start");
            var endParameter = Parameter(typeof(int), "end");
            var indexParameter = Parameter(typeof(int), "index");
            var sumParameter = Parameter(typeof(int), "sum");
            var expression = Block(
                new[] { indexParameter, sumParameter },
                Assign(sumParameter, Constant(0)),
                ExpressionEx.For(
                    Assign(indexParameter, startParameter), 
                    LessThan(indexParameter, endParameter), 
                    PostIncrementAssign(indexParameter),
                    AddAssign(sumParameter, ArrayIndex(valueParameter, indexParameter))),
                sumParameter);
            var sum = Lambda<Func<int[], int, int, int>>(expression, valueParameter, startParameter, endParameter).Compile();

            // Act
            var result = sum(value, start, end);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
