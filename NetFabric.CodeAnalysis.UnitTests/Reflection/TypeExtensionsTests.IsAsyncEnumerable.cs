using NetFabric.Assertive;
using System;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.CodeAnalysis.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        public static TheoryData<Type, Type, int> AsyncEnumerables =>
            new TheoryData<Type, Type, int>
            {
                { 
                    typeof(TestData.AsyncEnumerable<>).MakeGenericType(typeof(int)), 
                    typeof(TestData.AsyncEnumerable<>).MakeGenericType(typeof(int)), 
                    0
                },
                { 
                    typeof(TestData.ExplicitAsyncEnumerable<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncEnumerable<>).MakeGenericType(typeof(int)),
                    1
                },
                { 
                    typeof(TestData.RangeAsyncEnumerable),
                    typeof(TestData.RangeAsyncEnumerable),
                    0
                },
            };

        [Theory]
        [MemberData(nameof(AsyncEnumerables))]
        public void IsAsyncEnumerable_Should_ReturnTrue(Type enumeratorType, Type methodDeclaringType, int methodParametersCount)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsAsyncEnumerable(out var getEnumerator);

            // Assert   
            result.Must()
                .BeTrue();

            getEnumerator.Must()
                .BeNotNull()
                .EvaluatesTrue(method =>
                    method.Name == "GetAsyncEnumerator" &&
                    method.DeclaringType == methodDeclaringType &&
                    method.GetParameters().Length == methodParametersCount);
        }

        public static TheoryData<Type> InvalidAsyncEnumerables =>
            new TheoryData<Type>
            {
                typeof(TestData.MissingGetEnumeratorEnumerable),
            };

        [Theory]
        [MemberData(nameof(InvalidAsyncEnumerables))]
        public void IsAsyncEnumerable_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsEnumerable(out _);

            // Assert   
            result.Must()
                .BeFalse();
        }
    }
}
