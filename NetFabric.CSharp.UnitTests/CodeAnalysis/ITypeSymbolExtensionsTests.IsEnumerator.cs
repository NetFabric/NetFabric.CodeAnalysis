using NetFabric.CSharp.TestData;
using System;
using System.Collections;
using Xunit;

namespace NetFabric.CodeAnalysis.CSharp.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
    {
        [Theory]
        [MemberData(nameof(DataSets.Enumerators), MemberType = typeof(DataSets))]
        public void IsEnumerator_Should_ReturnTrue(Type enumeratorType, Type currentDeclaringType, Type moveNextDeclaringType, Type? resetDeclaringType, Type? disposeDeclaringType, Type itemType, bool isRefLikeType)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumeratorType);

            // Act
            var result = typeSymbol.IsEnumerator(compilation, out var enumeratorSymbols);

            // Assert   
            Assert.True(result);

            Assert.NotNull(enumeratorSymbols!.Current);
            Assert.Equal(nameof(IEnumerator.Current), enumeratorSymbols.Current.Name);
            Assert.Equal(currentDeclaringType.Name, enumeratorSymbols.Current.ContainingType.MetadataName);
            Assert.Equal(itemType.Name, enumeratorSymbols.Current.Type.MetadataName);

            Assert.NotNull(enumeratorSymbols.MoveNext);
            Assert.Equal(nameof(IEnumerator.MoveNext), enumeratorSymbols.MoveNext.Name);
            Assert.Equal(moveNextDeclaringType.Name, enumeratorSymbols.MoveNext.ContainingType.MetadataName);
            Assert.Empty(enumeratorSymbols.MoveNext.Parameters);

            if (resetDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols.Reset);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.Reset);
                Assert.Equal(nameof(IEnumerator.Reset), enumeratorSymbols!.Reset!.Name);
                Assert.Equal(resetDeclaringType.Name, enumeratorSymbols!.Reset.ContainingType.MetadataName);
                Assert.Empty(enumeratorSymbols.Reset.Parameters);
            }
            
            if (disposeDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols.Dispose);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.Dispose);
                Assert.Equal(nameof(IDisposable.Dispose), enumeratorSymbols!.Dispose!.Name);
                Assert.Equal(disposeDeclaringType.Name, enumeratorSymbols.Dispose.ContainingType.MetadataName);
                Assert.Empty(enumeratorSymbols.Dispose.Parameters);
            }
             
            Assert.Equal(isRefLikeType, enumeratorSymbols!.IsRefLikeType);
        }

        [Theory]
        [MemberData(nameof(DataSets.InvalidEnumerators), MemberType = typeof(DataSets))]
        public void IsEnumerator_With_MissingFeatures_Should_ReturnFalse(Type enumeratorType, Type? currentDeclaringType, Type? moveNextDeclaringType, Type? itemType)
        {
            // Arrange
            var compilation = Utils.Compile(@"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumeratorType);

            // Act
            var result = typeSymbol.IsEnumerator(compilation, out var _, out var errors);

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
