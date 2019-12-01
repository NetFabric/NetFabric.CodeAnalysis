using NetFabric.Assertive;
using System;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.CodeAnalysis.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        public static TheoryData<Type, Type, Type, Type, Type> AsyncEnumerators =>
            new TheoryData<Type, Type, Type, Type, Type>
            {
                { 
                    typeof(TestData.AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(TestData.AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(TestData.AsyncEnumerator<>).MakeGenericType(typeof(int)),
                    null,
                    typeof(int) 
                },
                { 
                    typeof(TestData.ExplicitAsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncEnumerator<>).MakeGenericType(typeof(int)),
                    typeof(IAsyncDisposable),
                    typeof(int) 
                },
            };

        [Theory]
        [MemberData(nameof(AsyncEnumerators))]
        public void IsAsyncEnumerator_Should_ReturnTrue(Type enumeratorType, Type currentDeclaringType, Type moveNextAsyncDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsAsyncEnumerator(out var current, out var moveNext, out var disposeAsync);

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
                    method.Name == "MoveNextAsync" &&
                    method.DeclaringType == moveNextAsyncDeclaringType &&
                    method.GetParameters().Length == 0);

            if (disposeAsyncDeclaringType is null)
                disposeAsync.Must()
                    .BeNull();
            else
                disposeAsync.Must()
                    .BeNotNull()
                    .EvaluatesTrue(method =>
                        method.Name == "DisposeAsync" &&
                        method.DeclaringType == disposeAsyncDeclaringType &&
                        method.GetParameters().Length == 0);
        }

        public static TheoryData<Type, Type, Type, Type, Type> InvalidAsyncEnumerators =>
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
        [MemberData(nameof(InvalidAsyncEnumerators))]
        public void IsAsyncEnumerator_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType, Type currentDeclaringType, Type moveNextDeclaringType, Type disposeAsyncDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsAsyncEnumerator(out var current, out var moveNext, out var disposeAsync);

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
                        method.Name == "MoveNextAsync" &&
                        method.DeclaringType == moveNextDeclaringType &&
                        method.GetParameters().Length == 0);

            if (disposeAsyncDeclaringType is null)
                disposeAsync.Must()
                    .BeNull();
            else
                disposeAsync.Must()
                    .BeNotNull()
                    .EvaluatesTrue(method =>
                        method.Name == "DisposeAsync" &&
                        method.DeclaringType == disposeAsyncDeclaringType &&
                        method.GetParameters().Length == 0);
        }
    }
}
