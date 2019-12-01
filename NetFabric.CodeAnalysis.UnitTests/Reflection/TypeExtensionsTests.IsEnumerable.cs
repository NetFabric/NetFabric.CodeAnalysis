using NetFabric.Assertive;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.CodeAnalysis.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        public static TheoryData<Type, Type> Enumerables =>
            new TheoryData<Type, Type>
            {
                { 
                    typeof(TestData.Enumerable<>).MakeGenericType(typeof(int)),
                    typeof(TestData.Enumerable<>).MakeGenericType(typeof(int))
                },
                { 
                    typeof(TestData.ExplicitEnumerable),
                    typeof(IEnumerable)
                },
                { 
                    typeof(TestData.ExplicitEnumerable<>).MakeGenericType(typeof(int)),
                    typeof(IEnumerable<>).MakeGenericType(typeof(int))
                },
                { 
                    typeof(TestData.RangeEnumerable),
                    typeof(TestData.RangeEnumerable)
                },
            };

        [Theory]
        [MemberData(nameof(Enumerables))]
        public void IsEnumerable_Should_ReturnTrue(Type enumeratorType, Type methodDeclaringType)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsEnumerable(out var enumerableInfo);

            // Assert   
            result.Must()
                .BeTrue();

            enumerableInfo.GetEnumerator.Must()
                .BeNotNull()
                .EvaluatesTrue(method =>
                    method.Name == "GetEnumerator" &&
                    method.DeclaringType == methodDeclaringType &&
                    method.GetParameters().Length == 0);
        }

        public static TheoryData<Type> InvalidEnumerables =>
            new TheoryData<Type>
            {
                typeof(TestData.MissingGetEnumeratorEnumerable),
            };

        [Theory]
        [MemberData(nameof(InvalidEnumerables))]
        public void IsEnumerable_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType)
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
