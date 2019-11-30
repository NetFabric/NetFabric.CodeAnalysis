using NetFabric.Assertive;
using System;
using System.Linq;
using Xunit;

namespace NetFabric.CodeAnalysis.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        public static TheoryData<Type, Type> Enumerators =>
            new TheoryData<Type, Type>
            {
                { typeof(TestData.Enumerator<>).MakeGenericType(typeof(int)), typeof(int) },
                { typeof(TestData.ExplicitEnumerator<>).MakeGenericType(typeof(int)), typeof(int) },
            };

        [Theory]
        [MemberData(nameof(Enumerators))]
        public void IsEnumerator_Should_ReturnTrue(Type enumeratorType, Type itemType)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsEnumerator(out var current, out var moveNext);

            // Assert   
            result.Must()
                .BeTrue();

            current.Must()
                .BeNotNull()
                .EvaluatesTrue(property =>
                    property.Name == "Current" &&
                    property.PropertyType == itemType);

            moveNext.Must()
                .BeNotNull()
                .EvaluatesTrue(method =>
                    method.Name == "MoveNext" &&
                    method.GetParameters().Length == 0);
        }

        public static TheoryData<Type, Type, bool, bool> InvalidEnumerators =>
            new TheoryData<Type, Type, bool, bool>
            {
                { typeof(TestData.MissingCurrentAndMoveNextEnumerator), null, true, true },
                { typeof(TestData.MissingCurrentEnumerator), null, true, false },
                { typeof(TestData.MissingMoveNextEnumerator<>).MakeGenericType(typeof(int)), typeof(int), false, true },
            };

        [Theory]
        [MemberData(nameof(InvalidEnumerators))]
        public void IsEnumerator_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType, Type itemType, bool currentMustBeNull, bool moveNextMustBeNull)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsEnumerator(out var current, out var moveNext);

            // Assert   
            result.Must()
                .BeFalse();

            if (currentMustBeNull)
                current.Must()
                    .BeNull();
            else
                current.Must()
                    .BeNotNull()
                    .EvaluatesTrue(property => 
                        property.Name == "Current" &&
                        property.PropertyType == itemType);

            if (moveNextMustBeNull)
                moveNext.Must()
                    .BeNull();
            else
                moveNext.Must()
                    .BeNotNull()
                    .EvaluatesTrue(method =>
                        method.Name == "MoveNext" &&
                        method.GetParameters().Length == 0);
        }
    }
}
