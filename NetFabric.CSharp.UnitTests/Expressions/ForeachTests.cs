using System;
using System.Linq;
using System.Linq.Expressions;
using AgileObjects.ReadableExpressions;
using NetFabric.CSharp.TestData;
using Xunit;
using static NetFabric.Expressions.ExpressionEx;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions.CSharp.UnitTests
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
            var expectedSum = source.Sum();
            const string expectedExpression = @"var sum = 0;
var index = 0;
while (true)
{
    if (index < enumerable.Length)
    {
        sum += enumerable[index];
        index++;
    }
    else
    {
        break;
    }
}

return sum;";
            
            // Act
            var (expression, _) = CreateSumExpression<int[]>();
            var sum = Sum(source);

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
            Assert.Equal(expectedSum, sum);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_ValueTypeEnumerator_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new EnumerableWithValueTypeEnumerator<int>(source);
            var expectedSum = source.Sum();
            const string expectedExpression = @"var sum = 0;
var enumerator = enumerable.GetEnumerator();
while (true)
{
    if (enumerator.MoveNext())
    {
        sum += enumerator.Current;
    }
    else
    {
        break;
    }
}

return sum;";
            
            // Act
            var (expression, _) = CreateSumExpression<EnumerableWithValueTypeEnumerator<int>>();
            var sum = Sum(enumerable);

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
            Assert.Equal(expectedSum, sum);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_DisposableValueTypeEnumerator_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new EnumerableWithDisposableValueTypeEnumerator<int>(source);
            var expectedSum = source.Sum();
            const string expectedExpression = @"var sum = 0;
var enumerator = enumerable.GetEnumerator();
try
{
    while (true)
    {
        if (enumerator.MoveNext())
        {
            sum += enumerator.Current;
        }
        else
        {
            break;
        }
    }
}
finally
{
    ((IDisposable)enumerator).Dispose();
}

return sum;";
            
            // Act
            var (expression, _) = CreateSumExpression<EnumerableWithDisposableValueTypeEnumerator<int>>();
            var sum = Sum(enumerable);

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
            Assert.Equal(expectedSum, sum);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_ReferenceTypeEnumerator_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new EnumerableWithReferenceTypeEnumerator<int>(source);
            var expectedSum = source.Sum();
            const string expectedExpression = @"var sum = 0;
var enumerator = enumerable.GetEnumerator();
try
{
    while (true)
    {
        if (enumerator.MoveNext())
        {
            sum += enumerator.Current;
        }
        else
        {
            break;
        }
    }
}
finally
{
    var disposable = enumerator as IDisposable;

    if (disposable != null)
    {
        disposable.Dispose();
    }
}

return sum;";
            
            // Act
            var (expression, _) = CreateSumExpression<EnumerableWithReferenceTypeEnumerator<int>>();
            var sum = Sum(enumerable);

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
            Assert.Equal(expectedSum, sum);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_DisposableReferenceTypeEnumerator_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new EnumerableWithDisposableReferenceTypeEnumerator<int>(source);
            var expectedSum = source.Sum();
            const string expectedExpression = @"var sum = 0;
var enumerator = enumerable.GetEnumerator();
try
{
    while (true)
    {
        if (enumerator.MoveNext())
        {
            sum += enumerator.Current;
        }
        else
        {
            break;
        }
    }
}
finally
{
    if (enumerator != null)
    {
        ((IDisposable)enumerator).Dispose();
    }
}

return sum;";
            
            // Act
            var (expression, _) = CreateSumExpression<EnumerableWithDisposableReferenceTypeEnumerator<int>>();
            var sum = Sum(enumerable);

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
            Assert.Equal(expectedSum, sum);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_ByRefLikeEnumerator_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new EnumerableWithByRefLikeEnumerator<int>(source);
            var expectedSum = source.Sum();
            const string expectedExpression = @"var sum = 0;
var enumerator = enumerable.GetEnumerator();
while (true)
{
    if (enumerator.MoveNext())
    {
        sum += enumerator.Current;
    }
    else
    {
        break;
    }
}

return sum;";
            
            // Act
            var (expression, _) = CreateSumExpression<EnumerableWithByRefLikeEnumerator<int>>();
            var sum = Sum(enumerable);

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
            Assert.Equal(expectedSum, sum);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_DisposableByRefLikeEnumerator_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new EnumerableWithDisposableByRefLikeEnumerator<int>(source);
            var expectedSum = source.Sum();
            const string expectedExpression = @"var sum = 0;
var enumerator = enumerable.GetEnumerator();
try
{
    while (true)
    {
        if (enumerator.MoveNext())
        {
            sum += enumerator.Current;
        }
        else
        {
            break;
        }
    }
}
finally
{
    enumerator.Dispose();
}

return sum;";
            
            // Act
            var (expression, _) = CreateSumExpression<EnumerableWithDisposableByRefLikeEnumerator<int>>();
            var sum = Sum(enumerable);

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
            Assert.Equal(expectedSum, sum);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_ExplicitEnumerable_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new ExplicitEnumerable<int>(source);
            var expectedSum = source.Sum();
            const string expectedExpression = @"var sum = 0;
var enumerator = ((IEnumerable)enumerable).GetEnumerator();
try
{
    while (true)
    {
        if (enumerator.MoveNext())
        {
            sum += (int)enumerator.Current;
        }
        else
        {
            break;
        }
    }
}
finally
{
    var disposable = enumerator as IDisposable;

    if (disposable != null)
    {
        disposable.Dispose();
    }
}

return sum;";
            
            // Act
            var (expression, _) = CreateSumWithCastExpression<ExplicitEnumerable<int>>();
            var sum = SumWithCast(enumerable);

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
            Assert.Equal(expectedSum, sum);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ForEach_With_ExplicitGenericEnumerable_Must_Succeed(int[] source)
        {
            // Arrange
            var enumerable = new ExplicitGenericEnumerable<int>(source);
            var expectedSum = source.Sum();
            const string expectedExpression = @"var sum = 0;
var enumerator = ((IEnumerable<int>)enumerable).GetEnumerator();
try
{
    while (true)
    {
        if (enumerator.MoveNext())
        {
            sum += enumerator.Current;
        }
        else
        {
            break;
        }
    }
}
finally
{
    if (enumerator != null)
    {
        ((IDisposable)enumerator).Dispose();
    }
}

return sum;";
            
            // Act
            var (expression, _) = CreateSumExpression<ExplicitGenericEnumerable<int>>();
            var sum = Sum(enumerable);

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
            Assert.Equal(expectedSum, sum);
        }

        static (Expression, ParameterExpression) CreateSumExpression<TEnumerable>()
        {
            var enumerableParameter = Parameter(typeof(TEnumerable), "enumerable");
            var sumVariable = Variable(typeof(int), "sum");
            var expression = Block(
                new[] {sumVariable},
                Assign(sumVariable, Constant(0)),
                ForEach(
                    enumerableParameter,
                    item => AddAssign(sumVariable, item)),
                sumVariable);
            return (expression, enumerableParameter);
        }

        static int Sum<TEnumerable>(TEnumerable enumerable)
        {
            var (expression, enumerableParameter) = CreateSumExpression<TEnumerable>();
            var sum = Lambda<Func<TEnumerable, int>>(expression, enumerableParameter).Compile();
            return sum(enumerable);
        }

        static (Expression, ParameterExpression) CreateSumWithCastExpression<TEnumerable>()
        {
            var enumerableParameter = Parameter(typeof(TEnumerable), "enumerable");
            var sumVariable = Variable(typeof(int), "sum");
            var expression = Block(
                new[] {sumVariable},
                Assign(sumVariable, Constant(0)),
                ForEach(
                    enumerableParameter,
                    item => AddAssign(sumVariable, Convert(item, typeof(int)))),
                sumVariable);
            return (expression, enumerableParameter);
        }

        static int SumWithCast<TEnumerable>(TEnumerable enumerable)
        {
            var (expression, enumerableParameter) = CreateSumWithCastExpression<TEnumerable>();
            var sum = Lambda<Func<TEnumerable, int>>(expression, enumerableParameter).Compile();
            return sum(enumerable);
        }
    }
}