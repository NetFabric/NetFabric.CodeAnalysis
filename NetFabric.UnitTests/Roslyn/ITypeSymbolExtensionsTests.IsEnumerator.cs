using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NetFabric.CodeAnalysis.UnitTests
{
    public partial class ITypeSymbolExtensionsTests
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
            var compilation = Utils.Compile(@"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumeratorType);

            // Act
            var result = typeSymbol.IsEnumerator(compilation, out var enumeratorSymbols);

            // Assert   
            Assert.True(result);

            Assert.Equal(currentDeclaringType?.Name, enumeratorSymbols.EnumeratorType?.MetadataName);
            Assert.Equal(itemType?.Name, enumeratorSymbols.ItemType?.MetadataName);

            Assert.NotNull(enumeratorSymbols.Current);
            Assert.Equal("Current", enumeratorSymbols.Current.Name);
            Assert.Equal(currentDeclaringType.Name, enumeratorSymbols.Current.ContainingType.MetadataName);
            Assert.Equal(itemType.Name, enumeratorSymbols.Current.Type.MetadataName);

            Assert.NotNull(enumeratorSymbols.MoveNext);
            Assert.Equal("MoveNext", enumeratorSymbols.MoveNext.Name);
            Assert.Equal(moveNextDeclaringType.Name, enumeratorSymbols.MoveNext.ContainingType.MetadataName);
            Assert.Empty(enumeratorSymbols.MoveNext.Parameters);

            if (disposeDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols.Dispose);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.Dispose);
                Assert.Equal("Dispose", enumeratorSymbols.Dispose.Name);
                Assert.Equal(disposeDeclaringType.Name, enumeratorSymbols.Dispose.ContainingType.MetadataName);
                Assert.Empty(enumeratorSymbols.Dispose.Parameters);
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
            var compilation = Utils.Compile(@"TestData/Enumerators.cs");
            var typeSymbol = compilation.GetTypeSymbol(enumeratorType);

            // Act
            var result = typeSymbol.IsEnumerator(compilation, out var enumeratorSymbols);

            // Assert   
            Assert.False(result);

            Assert.Equal(currentDeclaringType?.Name, enumeratorSymbols.EnumeratorType?.MetadataName);
            Assert.Equal(itemType?.Name, enumeratorSymbols.ItemType?.MetadataName);

            if (currentDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols.Current);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.Current);
                Assert.Equal("Current", enumeratorSymbols.Current.Name);
                Assert.Equal(currentDeclaringType.Name, enumeratorSymbols.Current.ContainingType.MetadataName);
                Assert.Equal(itemType.Name, enumeratorSymbols.Current.Type.MetadataName);
            }

            if (moveNextDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols.MoveNext);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.MoveNext);
                Assert.Equal("MoveNext", enumeratorSymbols.MoveNext.Name);
                Assert.Equal(moveNextDeclaringType.Name, enumeratorSymbols.MoveNext.ContainingType.MetadataName);
                Assert.Empty(enumeratorSymbols.MoveNext.Parameters);
            }

            if (disposeDeclaringType is null)
            {
                Assert.Null(enumeratorSymbols.Dispose);
            }
            else
            {
                Assert.NotNull(enumeratorSymbols.Dispose);
                Assert.Equal("Dispose", enumeratorSymbols.Dispose.Name);
                Assert.Equal(disposeDeclaringType.Name, enumeratorSymbols.Dispose.ContainingType.MetadataName);
                Assert.Empty(enumeratorSymbols.Dispose.Parameters);
            }
        }
    }
}
