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
            Assert.True(result);

            Assert.NotNull(current);
            Assert.Equal("Current", current.Name);
            Assert.Equal(currentDeclaringType, current.DeclaringType);
            Assert.Equal(itemType, current.PropertyType);

            Assert.NotNull(moveNext);
            Assert.Equal("MoveNext", moveNext.Name);
            Assert.Equal(moveNextDeclaringType, moveNext.DeclaringType);
            Assert.Empty(moveNext.GetParameters());

            if (disposeDeclaringType is null)
            {
                Assert.Null(dispose);
            }
            else
            {
                Assert.NotNull(dispose);
                Assert.Equal("Dispose", dispose.Name);
                Assert.Equal(disposeDeclaringType, dispose.DeclaringType);
                Assert.Empty(dispose.GetParameters());
            }
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
            Assert.False(result);

            if (currentDeclaringType is null)
            {
                Assert.Null(current);
            }
            else
            {
                Assert.NotNull(current);
                Assert.Equal("Current", current.Name);
                Assert.Equal(currentDeclaringType, current.DeclaringType);
                Assert.Equal(itemType, current.PropertyType);
            }

            if (moveNextDeclaringType is null)
            {
                Assert.Null(moveNext);
            }
            else
            {
                Assert.NotNull(moveNext);
                Assert.Equal("MoveNext", moveNext.Name);
                Assert.Equal(moveNextDeclaringType, moveNext.DeclaringType);
                Assert.Empty(moveNext.GetParameters());
            }

            if (disposeDeclaringType is null)
            {
                Assert.Null(dispose);
            }
            else
            {
                Assert.NotNull(dispose);
                Assert.Equal("Dispose", dispose.Name);
                Assert.Equal(disposeDeclaringType, dispose.DeclaringType);
                Assert.Empty(dispose.GetParameters());
            }
        }
    }
}
