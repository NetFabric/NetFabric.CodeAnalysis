using System;
using Xunit;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions.UnitTests
{
    public class IsTests
    {
        public static TheoryData<object, Type, bool> IsData =>
            new()
            {
                { "a string!", typeof(string), true },
                { "a string!", typeof(int[]), false },
                { Array.Empty<int>(), typeof(int[]), true },
            };

        [Theory]
        [MemberData(nameof(IsData))]
        public void Is_Must_Succeed(object value, Type type, bool expected)
        {
            // Arrange
            var valueParameter = Parameter(typeof(object), "value");
            var expression = ExpressionEx.Is(valueParameter, type);
            var func = Lambda<Func<object, bool>>(expression, valueParameter).Compile();

            // Act
            var result = func(value);

            // Assert
            Assert.Equal(expected, result);
        }

        public static TheoryData<object, bool, string?> IsOutputData =>
            new()
            {
                { "a string!", true, "a string!" },
                { Array.Empty<int>(), false, null },
            };
        
        [Theory]
        [MemberData(nameof(IsOutputData))]
        public void Is_With_Output_Must_Succeed(object value, bool expected, string? expectedOutput)
        {
            // Arrange
            var valueParameter = Parameter(typeof(object), "value");
            var resultParameter = Parameter(typeof(bool), "result");
            var outputParameter = Parameter(typeof(string), "output");
            var expression = Block(
                new[] { resultParameter, outputParameter },
                Assign(resultParameter, ExpressionEx.Is(valueParameter, typeof(string), outputParameter)),
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
