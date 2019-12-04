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
        public void IsEnumerator_Should_ReturnTrue(Type type, Type currentDeclaringType, Type moveNextDeclaringType, Type disposeDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = type.IsEnumerator(out var enumeratorInfo);

            // Assert   
            Assert.True(result);

            Assert.Equal(currentDeclaringType, enumeratorInfo.EnumeratorType);
            Assert.Equal(itemType, enumeratorInfo.ItemType);

            Assert.NotNull(enumeratorInfo.Current);
            Assert.Equal("Current", enumeratorInfo.Current.Name);
            Assert.Equal(currentDeclaringType, enumeratorInfo.Current.DeclaringType);
            Assert.Equal(itemType, enumeratorInfo.Current.PropertyType);

            Assert.NotNull(enumeratorInfo.MoveNext);
            Assert.Equal("MoveNext", enumeratorInfo.MoveNext.Name);
            Assert.Equal(moveNextDeclaringType, enumeratorInfo.MoveNext.DeclaringType);
            Assert.Empty(enumeratorInfo.MoveNext.GetParameters());

            if (disposeDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.Dispose);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.Dispose);
                Assert.Equal("Dispose", enumeratorInfo.Dispose.Name);
                Assert.Equal(disposeDeclaringType, enumeratorInfo.Dispose.DeclaringType);
                Assert.Empty(enumeratorInfo.Dispose.GetParameters());
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
        public void IsEnumerator_With_MissingFeatures_Should_ReturnFalse(Type type, Type currentDeclaringType, Type moveNextDeclaringType, Type disposeDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = type.IsEnumerator(out var enumeratorInfo);

            // Assert   
            Assert.False(result);

            Assert.Equal(currentDeclaringType, enumeratorInfo.EnumeratorType);
            Assert.Equal(itemType, enumeratorInfo.ItemType);

            if (currentDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.Current);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.Current);
                Assert.Equal("Current", enumeratorInfo.Current.Name);
                Assert.Equal(currentDeclaringType, enumeratorInfo.Current.DeclaringType);
                Assert.Equal(itemType, enumeratorInfo.Current.PropertyType);
            }

            if (moveNextDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.MoveNext);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.MoveNext);
                Assert.Equal("MoveNext", enumeratorInfo.MoveNext.Name);
                Assert.Equal(moveNextDeclaringType, enumeratorInfo.MoveNext.DeclaringType);
                Assert.Empty(enumeratorInfo.MoveNext.GetParameters());
            }

            if (disposeDeclaringType is null)
            {
                Assert.Null(enumeratorInfo.Dispose);
            }
            else
            {
                Assert.NotNull(enumeratorInfo.Dispose);
                Assert.Equal("Dispose", enumeratorInfo.Dispose.Name);
                Assert.Equal(disposeDeclaringType, enumeratorInfo.Dispose.DeclaringType);
                Assert.Empty(enumeratorInfo.Dispose.GetParameters());
            }
        }
    }
}
