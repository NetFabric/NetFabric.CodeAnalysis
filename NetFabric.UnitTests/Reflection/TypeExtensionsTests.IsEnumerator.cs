using NetFabric.TestData;
using System;
using System.Collections;
using Xunit;

namespace NetFabric.Reflection.UnitTests
{
    public partial class TypeExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.Enumerators), MemberType = typeof(DataSets))]
        public void IsEnumerator_Should_ReturnTrue(Type type, Type currentDeclaringType, Type moveNextDeclaringType, Type? resetDeclaringType, Type? disposeDeclaringType, Type itemType, bool isByRefLike)
        {
            // Arrange

            // Act
            var result = type.IsEnumerator(out var enumeratorInfo);

            // Assert   
            Assert.True(result);

            Assert.NotNull(enumeratorInfo!.Current);
            Assert.Equal(nameof(IEnumerator.Current), enumeratorInfo!.Current!.Name);
            Assert.Equal(currentDeclaringType, enumeratorInfo!.Current!.DeclaringType);
            Assert.Equal(itemType, enumeratorInfo!.Current!.PropertyType);

            Assert.NotNull(enumeratorInfo!.MoveNext);
            Assert.Equal(nameof(IEnumerator.MoveNext), enumeratorInfo!.MoveNext!.Name);
            Assert.Equal(moveNextDeclaringType, enumeratorInfo!.MoveNext!.DeclaringType);
            Assert.Empty(enumeratorInfo!.MoveNext!.GetParameters());

            if (resetDeclaringType is null)
            {
                Assert.Null(enumeratorInfo!.Reset);
            }
            else
            {
                Assert.NotNull(enumeratorInfo!.Reset);
                Assert.Equal(nameof(IEnumerator.Reset), enumeratorInfo!.Reset!.Name);
                Assert.Equal(resetDeclaringType, enumeratorInfo!.Reset!.DeclaringType);
                Assert.Empty(enumeratorInfo!.Reset!.GetParameters());
            }

            if (disposeDeclaringType is null)
            {
                Assert.Null(enumeratorInfo!.Dispose);
            }
            else
            {
                Assert.NotNull(enumeratorInfo!.Dispose);
                Assert.Equal(nameof(IDisposable.Dispose), enumeratorInfo!.Dispose!.Name);
                var declaringType = enumeratorInfo!.Dispose!.DeclaringType!;
                Assert.Equal(disposeDeclaringType, declaringType.IsGenericType ? declaringType.GetGenericTypeDefinition() : declaringType);
                Assert.Empty(enumeratorInfo!.Dispose!.GetParameters());
            }
            
            Assert.Equal(isByRefLike, enumeratorInfo!.IsByRefLike);
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidEnumerators), MemberType = typeof(DataSets))]
        public void IsEnumerator_With_MissingFeatures_Should_ReturnFalse(Type type, Type? currentDeclaringType, Type? moveNextDeclaringType, Type? itemType)
        {
            // Arrange

            // Act
            var result = type.IsEnumerator(out var _, out var errors);

            // Assert   
            Assert.False(result);

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
