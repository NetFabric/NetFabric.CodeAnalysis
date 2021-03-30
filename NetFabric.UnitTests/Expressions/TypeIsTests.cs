using System;
using Xunit;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions.UnitTests
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
            var resultParameter = Parameter(typeof(bool), "result");
            var outputParameter = Parameter(typeof(string), "output");
            var expression = Block(
                new[] { resultParameter, outputParameter },
                Assign(resultParameter, ExpressionEx.TypeIs(valueParameter, typeof(string), outputParameter)),
                New(typeof(ValueTuple<bool, string>).GetConstructor(new[] {typeof(bool), typeof(string)})!, resultParameter, outputParameter));
            var func = Lambda<Func<object, ValueTuple<bool, string>>>(expression, valueParameter).Compile();

            // Act
            var (result, output) = func(value);

            // Assert
            Assert.Equal(expected, result);
            Assert.Equal(expectedOutput, output);
        }
    }
}
