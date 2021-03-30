using System;
using System.Collections.Generic;
using System.Linq;
using NetFabric.TestData;
using Xunit;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions.UnitTests
{
    public class ForeachTests
    {
        public static TheoryData<int[]> Data =>
            new()
            {
                Array.Empty<int>(),
                new[] { 1 },
                new[] { 1, 2, 3, 4, 5 },
            };

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_Array_Must_Succeed(int[] source)
        {
            // Arrange
            var expected = source.Sum();
            
            // Act
            var result = Sum(source);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_ValueTypeEnumerator_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new Enumerable<int>(source);
            var expected = source.Sum();
            
            // Act
            var result = Sum(enumerable);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_ReferenceTypeEnumerator_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new HybridEnumerable<int>(source);
            var expected = source.Sum();
            
            // Act
            var result = Sum(enumerable);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_ByRefLikeEnumerator_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new EnumerableRefEnumerator<int>(source);
            var expected = source.Sum();
            
            // Act
            var result = Sum(enumerable);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_DisposableByRefLikeEnumerator_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new EnumerableDisposableRefEnumerator<int>(source);
            var expected = source.Sum();
            
            // Act
            var result = Sum(enumerable);

            // Assert
            Assert.Equal(expected, result);
        }

        int Sum<TEnumerable>(TEnumerable enumerable)
        {
            var enumerableParameter = Parameter(typeof(TEnumerable), "enumerable");
            var sumParameter = Parameter(typeof(int), "sum");
            var expression = Block(
                new[] { sumParameter },
                Assign(sumParameter, Constant(0)),
                ExpressionEx.ForEach(
                    enumerableParameter, 
                    item => AddAssign(sumParameter, item)),
                sumParameter);
            var sum = Lambda<Func<TEnumerable, int>>(expression, enumerableParameter).Compile();

            return sum(enumerable);
        }
    }
}