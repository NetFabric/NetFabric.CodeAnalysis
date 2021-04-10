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
        //[MemberData(nameof(DataSets.VisualBasicEnumerables), MemberType = typeof(DataSets))]
        public void IsEnumerable_Should_ReturnTrue(Type type, Type getEnumeratorDeclaringType, Type currentDeclaringType, Type moveNextDeclaringType, Type? resetDeclaringType, Type? disposeDeclaringType, Type itemType, bool isValueType, bool isByRefLike)
        {
            // Arrange

            // Act
            var result = type.IsEnumerable(out var enumerableInfo);

            // Assert   
            Assert.True(result);
            Assert.NotNull(enumerableInfo);
            Assert.NotNull(enumerableInfo!.EnumeratorInfo);

            var getEnumerator = enumerableInfo!.GetEnumerator;
            Assert.NotNull(getEnumerator);
            Assert.Equal(nameof(IEnumerable.GetEnumerator), getEnumerator!.Name);
            Assert.Equal(getEnumeratorDeclaringType, getEnumerator!.DeclaringType);
            Assert.Empty(getEnumerator!.GetParameters());
            

            var enumeratorInfo = enumerableInfo!.EnumeratorInfo;

            var current = enumeratorInfo!.Current;
            Assert.NotNull(current);
            Assert.Equal(nameof(IEnumerator.Current), current!.Name);
            Assert.Equal(currentDeclaringType, current!.DeclaringType);
            Assert.Equal(itemType, current!.PropertyType);

            var moveNext = enumeratorInfo!.MoveNext;
            Assert.NotNull(moveNext);
            Assert.Equal(nameof(IEnumerator.MoveNext), moveNext!.Name);
            Assert.Equal(moveNextDeclaringType, moveNext!.DeclaringType);
            Assert.Empty(moveNext!.GetParameters());

            var reset = enumeratorInfo!.Reset;
            if (resetDeclaringType is null)
            {
                Assert.Null(reset);
            }
            else
            {
                Assert.NotNull(reset);
                Assert.Equal(nameof(IEnumerator.Reset), reset!.Name);
                Assert.Equal(resetDeclaringType, reset!.DeclaringType);
                Assert.Empty(reset!.GetParameters());
            }

            var dispose = enumeratorInfo!.Dispose;
            if (disposeDeclaringType is null)
            {
                Assert.Null(dispose);
            }
            else
            {
                Assert.NotNull(dispose);
                Assert.Equal(nameof(IDisposable.Dispose), dispose!.Name);
                Assert.Equal(disposeDeclaringType, dispose!.DeclaringType);
                Assert.Empty(dispose!.GetParameters());
            }
            
            Assert.Equal(isValueType, enumeratorInfo!.IsValueType);
            Assert.Equal(isByRefLike, enumeratorInfo!.IsByRefLike);
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
