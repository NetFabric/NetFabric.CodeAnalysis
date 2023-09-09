using NetFabric.CSharp.TestData;
using Microsoft.CodeAnalysis;
using System;
using Xunit;

namespace NetFabric.CodeAnalysis.CSharp.UnitTests;

public partial class ITypeSymbolExtensionsTests
{
    public static TheoryData<Type, SpecialType, bool, Type[]?> ImplementsInterfaceData
        => new()
        {
            {
                typeof(EnumerableWithValueTypeEnumerator<TestType>),
                SpecialType.System_Collections_IEnumerable,
                false,
                null
            },
            {
                typeof(ExplicitEnumerable<TestType>),
                SpecialType.System_Collections_IEnumerable,
                true,
                null
            },
            {
                typeof(ExplicitGenericEnumerable<TestType>),
                SpecialType.System_Collections_Generic_IEnumerable_T,
                true,
                new[] { typeof(TestType) }
            },
            {
                typeof(HybridEnumerable<TestType>),
                SpecialType.System_Collections_Generic_IEnumerable_T,
                true,
                new[] { typeof(TestType) }
            },
            {
                typeof(HybridEnumerable<TestType>),
                SpecialType.System_Collections_Generic_IReadOnlyList_T,
                false,
                new[] { typeof(TestType) }
            },
        };


    [Theory]
    [MemberData(nameof(ImplementsInterfaceData))]
    public void ImplementsInterface_Should_Succeed(Type type, SpecialType @interface, bool expected, Type[]? expectedGenericArguments)
    {
        // Arrange
        var compilation = Utils.Compile(@"TestData/Enumerables.cs", @"TestData/TestType.cs");
        var typeSymbol = compilation.GetTypeSymbol(type);

        // Act
        var result = typeSymbol.ImplementsInterface(@interface, out var genericArguments);

        // Assert   
        Assert.Equal(expected, result);
        if (result)
        {
            for (var index = 0; index < genericArguments.Length; index++)
            {
                Assert.Equal(expectedGenericArguments![index].Name, genericArguments[index].Name);
            }
        }
    }
}
