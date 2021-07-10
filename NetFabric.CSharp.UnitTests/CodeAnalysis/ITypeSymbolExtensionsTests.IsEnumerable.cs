using NetFabric.CSharp.TestData;
using System;
using System.Collections;
using Xunit;

namespace NetFabric.CodeAnalysis.CSharp.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.Arrays), MemberType = typeof(DataSets))]
        [MemberData(nameof(DataSets.Enumerables), MemberType = typeof(DataSets))]
        public void IsEnumerable_Should_ReturnTrue(Type enumerableType, Type getEnumeratorDeclaringType, Type currentDeclaringType, Type moveNextDeclaringType, Type? resetDeclaringType, Type? disposeDeclaringType, Type itemType, bool isValueType, bool isRefLikeType)
        {
            // Arrange
            var compilation = Utils.Compile(
                @"TestData/Enumerables.cs",
                @"TestData/Enumerators.cs",
                @"TestData/RangeEnumerable.cs",
                @"../../../../NetFabric.Reflection/Reflection/EnumerableWrapper.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumerableType);

            // Act
            var result = typeSymbol.IsEnumerable(compilation, out var enumerableSymbols);

            // Assert   
            Assert.True(result);
            Assert.NotNull(enumerableSymbols);

            var getEnumerator = enumerableSymbols!.GetEnumerator;
            Assert.NotNull(getEnumerator);
            Assert.Equal(nameof(IEnumerable.GetEnumerator), getEnumerator.Name);
            Assert.Equal(getEnumeratorDeclaringType.Name, getEnumerator.ContainingType.MetadataName);
            Assert.Empty(getEnumerator.Parameters);

            var enumeratorSymbols = enumerableSymbols.EnumeratorSymbols;
            Assert.NotNull(enumeratorSymbols);

            var current = enumeratorSymbols.Current;
            Assert.NotNull(current);
            Assert.Equal(nameof(IEnumerator.Current), current.Name);
            Assert.Equal(currentDeclaringType.Name, current.ContainingType.MetadataName);
            if (current is {ReturnsByRef:true} or {ReturnsByRefReadonly:true})
                Assert.Equal(itemType.Name, current.Type.MetadataName + '&');
            else
                Assert.Equal(itemType.Name, current.Type.MetadataName);

            var moveNext = enumeratorSymbols.MoveNext;
            Assert.NotNull(moveNext);
            Assert.Equal(nameof(IEnumerator.MoveNext), moveNext.Name);
            Assert.Equal(moveNextDeclaringType.Name, moveNext.ContainingType.MetadataName);
            Assert.Empty(enumeratorSymbols.MoveNext.Parameters);

            var reset = enumeratorSymbols.Reset;
            if (resetDeclaringType is null)
            {
                Assert.Null(reset);
            }
            else
            {
                Assert.NotNull(reset);
                Assert.Equal(nameof(IEnumerator.Reset), reset!.Name);
                Assert.Equal(resetDeclaringType.Name, reset.ContainingType.MetadataName);
                Assert.Empty(reset.Parameters);
            }

            var dispose = enumeratorSymbols.Dispose;
            if (disposeDeclaringType is null)
            {
                Assert.Null(dispose);
            }
            else
            {
                Assert.NotNull(dispose);
                Assert.Equal(nameof(IDisposable.Dispose), dispose!.Name);
                Assert.Equal(disposeDeclaringType.Name, dispose.ContainingType.MetadataName);
                Assert.Empty(dispose.Parameters);
            }
            
            Assert.Equal(isValueType, enumeratorSymbols.IsValueType);
            Assert.Equal(isRefLikeType, enumeratorSymbols.IsRefLikeType);
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidEnumerables), MemberType = typeof(DataSets))]
        public void IsEnumerable_With_MissingFeatures_Should_ReturnFalse(Type enumerableType, Type? getEnumeratorDeclaringType, Type? currentDeclaringType, Type moveNextDeclaringType, Type itemType)
        {
            // Arrange
            var compilation = Utils.Compile(
                @"TestData/Enumerables.cs",
                @"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumerableType);

            // Act
            var result = typeSymbol.IsEnumerable(compilation, out _, out var errors);

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
