using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.Reflection.UnitTests
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
            Assert.True(result);

            Assert.NotNull(enumerableInfo.GetEnumerator);
            Assert.Equal("GetEnumerator", enumerableInfo.GetEnumerator.Name);
            Assert.Equal(methodDeclaringType, enumerableInfo.GetEnumerator.DeclaringType);
            Assert.Empty(enumerableInfo.GetEnumerator.GetParameters());
        }

        public static TheoryData<Type, Type, Type, Type, Type, Type> InvalidEnumerables =>
            new TheoryData<Type, Type, Type, Type, Type, Type>
            {
                { 
                    typeof(TestData.MissingGetEnumeratorEnumerable),
                    null,
                    null,
                    null,
                    null,
                    null
                },
                { 
                    typeof(TestData.MissingCurrentEnumerable),
                    typeof(TestData.MissingCurrentEnumerable),
                    null,
                    typeof(TestData.MissingCurrentEnumerator),
                    null,
                    null
                },
                { 
                    typeof(TestData.MissingMoveNextEnumerable<int>),
                    typeof(TestData.MissingMoveNextEnumerable<int>),
                    typeof(TestData.MissingMoveNextEnumerator<int>),
                    null,
                    null,
                    typeof(int)
                },
            };

        [Theory]
        [MemberData(nameof(InvalidEnumerables))]
        public void IsEnumerable_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType, Type getEnumeratorDeclaringType, Type currentDeclaringType, Type moveNextDeclaringType, Type disposeDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = enumeratorType.IsEnumerable(out var enumerableInfo);

            // Assert   
            Assert.False(result);

            if (getEnumeratorDeclaringType is null)
            {
                Assert.Null(enumerableInfo.GetEnumerator);
            }
            else
            {
                Assert.NotNull(enumerableInfo.GetEnumerator);
                Assert.Equal("GetEnumerator", enumerableInfo.GetEnumerator.Name);
                Assert.Equal(getEnumeratorDeclaringType, enumerableInfo.GetEnumerator.DeclaringType);
                Assert.Empty(enumerableInfo.GetEnumerator.GetParameters());
            }

            if (currentDeclaringType is null)
            {
                Assert.Null(enumerableInfo.Current);
            }
            else
            {
                Assert.NotNull(enumerableInfo.Current);
                Assert.Equal("Current", enumerableInfo.Current.Name);
                Assert.Equal(currentDeclaringType, enumerableInfo.Current.DeclaringType);
                Assert.Equal(itemType, enumerableInfo.Current.PropertyType);
            }

            if (moveNextDeclaringType is null)
            {
                Assert.Null(enumerableInfo.MoveNext);
            }
            else
            {
                Assert.NotNull(enumerableInfo.MoveNext);
                Assert.Equal("MoveNext", enumerableInfo.MoveNext.Name);
                Assert.Equal(moveNextDeclaringType, enumerableInfo.MoveNext.DeclaringType);
                Assert.Empty(enumerableInfo.MoveNext.GetParameters());
            }

            if (disposeDeclaringType is null)
            {
                Assert.Null(enumerableInfo.Dispose);
            }
            else
            {
                Assert.NotNull(enumerableInfo.Dispose);
                Assert.Equal("Dispose", enumerableInfo.Dispose.Name);
                Assert.Equal(disposeDeclaringType, enumerableInfo.Dispose.DeclaringType);
                Assert.Empty(enumerableInfo.Dispose.GetParameters());
            }
        }
    }
}
