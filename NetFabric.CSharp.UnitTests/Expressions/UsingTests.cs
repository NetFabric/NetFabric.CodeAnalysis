using System;
using System.Linq;
using NetFabric.CSharp.TestData;
using NetFabric.Reflection;
using Xunit;
using static NetFabric.Expressions.ExpressionEx;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions.CSharp.UnitTests
{
    public class UsingTests
    {
        public static TheoryData<int[]> Data =>
            new()
            {
                Array.Empty<int>(),
                new[] { 1 },
                new[] { 1, 2, 3, 4, 5 },
                new[] { 1, 2, 3, 4, 5 },
            };

        [Theory]
        [MemberData(nameof(Data))]
        public void Using_With_DisposableValueType_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new EnumerableWithDisposableValueTypeEnumerator<int>(source);
            var expected = source.Sum();
            
            // Act
            var result = Sum(enumerable);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Using_With_NotDisposableValueType_Must_Throw(int[] source)
        {
            // Arrange
            var enumerable = new EnumerableWithValueTypeEnumerator<int>(source);
            
            // Act
            Action action = () => Sum(enumerable);

            // Assert
            var exception = Assert.Throws<Exception>(action);
            Assert.Equal("'ValueTypeEnumerator`1': type used in a using statement must be implicitly convertible to 'System.IDisposable'", exception.Message);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Using_With_DisposableByRefLike_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new EnumerableWithDisposableByRefLikeEnumerator<int>(source);
            var expected = source.Sum();
            
            // Act
            var result = Sum(enumerable);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Using_With_NotDisposableByRefLike_Must_Throw(int[] source)
        {
            // Arrange
            var enumerable = new EnumerableWithByRefLikeEnumerator<int>(source);
            
            // Act
            Action action = () => Sum(enumerable);

            // Assert
            var exception = Assert.Throws<Exception>(action);
            Assert.Equal("'ByRefLikeEnumerator`1': type used in a using statement must be implicitly convertible to 'System.IDisposable'", exception.Message);
        }

        int Sum<TEnumerable>(TEnumerable enumerable)
        {
            if (!typeof(TEnumerable).IsEnumerable(out var enumerableInfo))
                throw new Exception("Not an enumerable!");
            
            var enumerableParameter = Parameter(typeof(TEnumerable), "enumerable");
            var enumeratorVariable = Variable(enumerableInfo.GetEnumerator.ReturnType, "enumerator");
            var sumVariable = Variable(typeof(int), "sum");
            var expression = Block(
                new[] {enumeratorVariable, sumVariable},
                Assign(enumeratorVariable, Call(enumerableParameter, enumerableInfo.GetEnumerator)),
                Assign(sumVariable, Constant(0)),
                Using(
                    enumeratorVariable,
                    While(
                        Call(enumeratorVariable, enumerableInfo.EnumeratorInfo.MoveNext),
                        AddAssign(sumVariable, Property(enumeratorVariable, enumerableInfo.EnumeratorInfo.Current))
                    )
                ),
                sumVariable);
            var sum = Lambda<Func<TEnumerable, int>>(expression, enumerableParameter).Compile();

            return sum(enumerable);
        }
    }
}
