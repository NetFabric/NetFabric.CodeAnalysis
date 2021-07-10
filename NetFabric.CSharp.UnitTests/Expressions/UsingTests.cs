using System;
using AgileObjects.ReadableExpressions;
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

        [Fact]
        public void Using_With_NotDisposableValueType_Must_Throw()
        {
            // Arrange
            
            // Act
            Action action = () =>
            {
                var disposable = Variable(typeof(NotDisposableValueType), "disposable");
                var label = Label(typeof(int));
                _ = Block(
                    new[] {disposable},
                    Using(
                        disposable, 
                        Empty()
                    )
                );
            };

            // Assert
            var exception = Assert.Throws<Exception>(action);
            Assert.Equal("'NotDisposableValueType': type used in a using statement must be implicitly convertible to 'System.IDisposable'", exception.Message);
        }

        [Fact]
        public void Using_With_DisposableValueType_Must_Succeed()
        {
            // Arrange
            const string expectedExpression = @"UsingTests.DisposableValueType disposable;
try
{
    return true;
}
finally
{
    disposable.Dispose();
}

return false;";
            
            // Act
            var disposable = Variable(typeof(DisposableValueType), "disposable");
            var label = Label(typeof(bool));
            var expression = Block(
                new[] {disposable},
                Using(
                    disposable, 
                    Return(label, Constant(true))
                ),
                Label(label, Constant(false))
            );

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
        }
        
        [Fact]
        public void Using_With_NotDisposableReferenceType_Must_Throw()
        {
            // Arrange
            
            // Act
            Action action = () =>
            {
                var disposable = Variable(typeof(NotDisposableReferenceType), "disposable");
                _ = Block(
                    new[] {disposable},
                    Using(
                        disposable, 
                        Empty()
                    )
                );
            };

            // Assert
            var exception = Assert.Throws<Exception>(action);
            Assert.Equal("'NotDisposableReferenceType': type used in a using statement must be implicitly convertible to 'System.IDisposable'", exception.Message);
        }

        [Fact]
        public void Using_With_DisposableReferenceType_Must_Succeed()
        {
            // Arrange
            const string expectedExpression = @"UsingTests.DisposableReferenceType disposable;
try
{
    return true;
}
finally
{
    if (disposable != null)
    {
        ((IDisposable)disposable).Dispose();
    }
}

return false;";
            
            // Act
            var disposable = Variable(typeof(DisposableReferenceType), "disposable");
            var label = Label(typeof(bool));
            var expression = Block(
                new[] {disposable},
                Using(
                    disposable, 
                    Return(label, Constant(true))
                ),
                Label(label, Constant(false))
            );

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
        }

        [Fact]
        public void Using_With_NotDisposableByRefLikeType_Must_Throw()
        {
            // Arrange
            
            // Act
            Action action = () =>
            {
                var disposable = Variable(typeof(NotDisposableByRefLikeType), "disposable");
                _ = Block(
                    new[] {disposable},
                    Using(
                        disposable, 
                        Empty()
                    )
                );
            };

            // Assert
            var exception = Assert.Throws<Exception>(action);
            Assert.Equal("'NotDisposableByRefLikeType': type used in a using statement must be implicitly convertible to 'System.IDisposable'", exception.Message);
        }

        [Fact]
        public void Using_With_DisposableByRefLike_Must_Succeed()
        {
            // Arrange
            const string expectedExpression = @"UsingTests.DisposableByRefLikeType disposable;
try
{
    return true;
}
finally
{
    disposable.Dispose();
}

return false;";
            
            // Act
            var disposable = Variable(typeof(DisposableByRefLikeType), "disposable");
            var label = Label(typeof(bool));
            var expression = Block(
                new[] {disposable},
                Using(
                    disposable, 
                    Return(label, Constant(true))
                ),
                Label(label, Constant(false))
            );

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
        }

        [Fact]
        public void Using_With_EnumerableOfExpression_Must_Succeed()
        {
            // Arrange
            var expectedExpression = @"UsingTests.DisposableValueType disposableValueType;
UsingTests.DisposableReferenceType disposableReferenceType;
try
{
    try
    {
        return true;
    }
    finally
    {
        disposableValueType.Dispose();
    }
}
finally
{
    if (disposableReferenceType != null)
    {
        ((IDisposable)disposableReferenceType).Dispose();
    }
}

return false;";
            
            // Act
            var disposableValueType = Variable(typeof(DisposableValueType), "disposableValueType");
            var disposableReferenceType = Variable(typeof(DisposableReferenceType), "disposableReferenceType");
            var label = Label(typeof(bool));
            var expression = Block(
                new[] {disposableValueType, disposableReferenceType},
                Using(
                    new[] {disposableValueType, disposableReferenceType} , 
                    Return(label, Constant(true))
                ),
                Label(label, Constant(false))
            );

            // Assert
            var readableExpression = expression.ToReadableString();
            Assert.Equal(expectedExpression, readableExpression);
        }

        struct NotDisposableValueType
        {
            public void Dispose() {}
        }

        ref struct NotDisposableByRefLikeType
        {
        }

        class NotDisposableReferenceType
        {
            public void Dispose() {}
        }

        struct DisposableValueType : IDisposable
        {
            public void Dispose() {}
        }

        ref struct DisposableByRefLikeType
        {
            public void Dispose() {}
        }

        class DisposableReferenceType : IDisposable
        {
            public void Dispose() {}
        }
        
    }
}
