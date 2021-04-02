using System;
using Xunit;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions.CSharp.UnitTests
{
    public class IsTests
    {
        public static TheoryData<object, bool, string?> IsOutputData =>
            new()
            {
                { "a string!", true, "a string!" },
                { Array.Empty<int>(), false, null },
            };
        
        [Theory]
        [MemberData(nameof(IsOutputData))]
        public void TypeIs_Must_Succeed(object value, bool expected, string? expectedOutput)
        {
            // Arrange
            var valueParameter = Parameter(typeof(object), "value");
            var resultVariable = Variable(typeof(bool), "result");
            var outputVariable = Variable(typeof(string), "output");
            var expression = Block(
                new[] { resultVariable, outputVariable },
                Assign(resultVariable, ExpressionEx.TypeIs(valueParameter, typeof(string), outputVariable)),
                New(typeof(ValueTuple<bool, string>).GetConstructor(new[] {typeof(bool), typeof(string)})!, resultVariable, outputVariable));
            var func = Lambda<Func<object, ValueTuple<bool, string>>>(expression, valueParameter).Compile();

            // Act
            var (result, output) = func(value);

            // Assert
            Assert.Equal(expected, result);
            Assert.Equal(expectedOutput, output);
        }
    }
}
