using NetFabric.CSharp.TestData;
using System;
using System.Collections;
using Xunit;

namespace NetFabric.Reflection.CSharp.UnitTests
{
    public partial class TypeExtensionsTests
    {

        [Theory]
        [MemberData(nameof(DataSets.Arrays), MemberType = typeof(DataSets))]
        [MemberData(nameof(DataSets.Enumerables), MemberType = typeof(DataSets))]
        public void IsEnumerable_Should_ReturnTrue(Type type, DataSets.EnumerableTestData enumerableTestData)
        {
            // Arrange

            // Act
            var result = type.IsEnumerable(out var enumerableInfo, out var errors);

            // Assert   
            Assert.True(result);
            Assert.NotNull(enumerableInfo);
            Assert.Equal(Errors.None, errors);

            Assert.NotNull(enumerableInfo!.EnumeratorInfo);

            var getEnumerator = enumerableInfo!.GetEnumerator;
            Assert.NotNull(getEnumerator);
            Assert.Equal(nameof(IEnumerable.GetEnumerator), getEnumerator!.Name);
            Assert.Equal(enumerableTestData.GetEnumeratorDeclaringType, getEnumerator!.DeclaringType);
            Assert.Empty(getEnumerator!.GetParameters());
            

            var enumeratorInfo = enumerableInfo!.EnumeratorInfo;
            Assert.NotNull(enumeratorInfo);

            var enumeratorTestData = enumerableTestData.EnumeratorTestData;
            
            var current = enumeratorInfo!.Current;
            Assert.NotNull(current);
            Assert.Equal(nameof(IEnumerator.Current), current!.Name);
            Assert.Equal(enumeratorTestData.CurrentDeclaringType, current!.DeclaringType);
            Assert.Equal(enumeratorTestData.ItemType, current!.PropertyType);

            var moveNext = enumeratorInfo!.MoveNext;
            Assert.NotNull(moveNext);
            Assert.Equal(nameof(IEnumerator.MoveNext), moveNext!.Name);
            Assert.Equal(enumeratorTestData.MoveNextDeclaringType, moveNext!.DeclaringType);
            Assert.Empty(moveNext!.GetParameters());

            var reset = enumeratorInfo!.Reset;
            if (enumeratorTestData.ResetDeclaringType is null)
            {
                Assert.Null(reset);
            }
            else
            {
                Assert.NotNull(reset);
                Assert.Equal(nameof(IEnumerator.Reset), reset!.Name);
                Assert.Equal(enumeratorTestData.ResetDeclaringType, reset!.DeclaringType);
                Assert.Empty(reset!.GetParameters());
            }

            var dispose = enumeratorInfo!.Dispose;
            if (enumeratorTestData.DisposeDeclaringType is null)
            {
                Assert.Null(dispose);
            }
            else
            {
                Assert.NotNull(dispose);
                Assert.Equal(nameof(IDisposable.Dispose), dispose!.Name);
                Assert.Equal(enumeratorTestData.DisposeDeclaringType, dispose!.DeclaringType);
                Assert.Empty(dispose!.GetParameters());
            }
            
            Assert.Equal(enumeratorTestData.IsValueType, enumeratorInfo!.IsValueType);
            Assert.Equal(enumeratorTestData.IsByRefLikeType, enumeratorInfo!.IsByRefLike);
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidEnumerables), MemberType = typeof(DataSets))]
        public void IsEnumerable_With_MissingFeatures_Should_ReturnFalse(Type type, Type? getEnumeratorDeclaringType, Type? currentDeclaringType, Type? moveNextDeclaringType, Type itemType)
        {
            // Arrange

            // Act
            var result = type.IsEnumerable(out _, out var errors);

            // Assert   
            Assert.False(result);

            if (getEnumeratorDeclaringType is null)
            {
                Assert.True(errors.HasFlag(Errors.MissingGetEnumerator));
            }
            else
            {
                Assert.False(errors.HasFlag(Errors.MissingGetEnumerator));

                if (currentDeclaringType is null)
                    Assert.True(errors.HasFlag(Errors.MissingCurrent));
                else
                    Assert.False(errors.HasFlag(Errors.MissingCurrent));

                if (moveNextDeclaringType is null)
                    Assert.True(errors.HasFlag(Errors.MissingMoveNext));
                else
                    Assert.False(errors.HasFlag(Errors.MissingMoveNext));
            }
        }
    }
}
