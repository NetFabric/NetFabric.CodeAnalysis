using NetFabric.Assertive;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        public static TheoryData<Type, Type, Type, Type, Type> Enumerators =>
            new TheoryData<Type, Type, Type, Type, Type>
            {
                { 
                    typeof(TestData.Enumerator<>).MakeGenericType(typeof(int)),
                    typeof(TestData.Enumerator<>).MakeGenericType(typeof(int)),
                    typeof(TestData.Enumerator<>).MakeGenericType(typeof(int)),
                    null,
                    typeof(int)
                },
                { 
                    typeof(TestData.ExplicitEnumerator),
                    typeof(IEnumerator),
                    typeof(IEnumerator),
                    null,
                    typeof(object) 
                },
                { 
                    typeof(TestData.ExplicitEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IEnumerator),
                    typeof(IDisposable),
                    typeof(int) 
                },
            };

        [Theory]
        [MemberData(nameof(Enumerators))]
        public void IsEnumerator_Should_ReturnTrue(Type enumeratorType, Type currentDeclaringType, Type moveNextDeclaringType, Type disposeDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsEnumerator(out var current, out var moveNext, out var dispose);

            // Assert   
            result.Must()
                .BeTrue();

            current.Must()
                .BeNotNull()
                .EvaluatesTrue(property =>
                    property.Name == "Current" &&
                    property.DeclaringType == currentDeclaringType &&
                    property.PropertyType == itemType);

            moveNext.Must()
                .BeNotNull()
                .EvaluatesTrue(method =>
                    method.Name == "MoveNext" &&
                    method.DeclaringType == moveNextDeclaringType &&
                    method.GetParameters().Length == 0);

            if (disposeDeclaringType is null)
                dispose.Must()
                    .BeNull();
            else
                dispose.Must()
                    .BeNotNull()
                    .EvaluatesTrue(method =>
                        method.Name == "Dispose" &&
                        method.DeclaringType == disposeDeclaringType &&
                        method.GetParameters().Length == 0);
        }

        public static TheoryData<Type, Type, Type, Type, Type> InvalidEnumerators =>
            new TheoryData<Type, Type, Type, Type, Type>
            {
                { 
                    typeof(TestData.MissingCurrentAndMoveNextEnumerator), 
                    null, 
                    null,
                    null,
                    null
                },
                { 
                    typeof(TestData.MissingCurrentEnumerator),
                    null,
                    typeof(TestData.MissingCurrentEnumerator),
                    null,
                    null
                },
                { 
                    typeof(TestData.MissingMoveNextEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(TestData.MissingMoveNextEnumerator<>).MakeGenericType(typeof(int)),
                    null,
                    null,
                    typeof(int)
                },
            };

        [Theory]
        [MemberData(nameof(InvalidEnumerators))]
        public void IsEnumerator_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType, Type currentDeclaringType, Type moveNextDeclaringType, Type disposeDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsEnumerator(out var current, out var moveNext, out var dispose);

            // Assert   
            result.Must()
                .BeFalse();

            if (currentDeclaringType is null)
                current.Must()
                    .BeNull();
            else
                current.Must()
                    .BeNotNull()
                    .EvaluatesTrue(property => 
                        property.Name == "Current" &&
                        property.DeclaringType == currentDeclaringType &&
                        property.PropertyType == itemType);

            if (moveNextDeclaringType is null)
                moveNext.Must()
                    .BeNull();
            else
                moveNext.Must()
                    .BeNotNull()
                    .EvaluatesTrue(method =>
                        method.Name == "MoveNext" &&
                        method.DeclaringType == moveNextDeclaringType &&
                        method.GetParameters().Length == 0);

            if (disposeDeclaringType is null)
                dispose.Must()
                    .BeNull();
            else
                dispose.Must()
                    .BeNotNull()
                    .EvaluatesTrue(method =>
                        method.Name == "Dispose" &&
                        method.DeclaringType == moveNextDeclaringType &&
                        method.GetParameters().Length == 0);
        }
    }
}
