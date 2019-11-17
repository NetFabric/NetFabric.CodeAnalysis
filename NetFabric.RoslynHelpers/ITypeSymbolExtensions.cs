using Microsoft.CodeAnalysis;
using System;

namespace NetFabric.RoslynHelpers
{
    public static class ITypeSymbolExtensions
    {
        public static IPropertySymbol GetPublicProperty(this ITypeSymbol typeSymbol, string name)
        {
            while (typeSymbol is object)
            {
                foreach (var member in typeSymbol.GetMembers(name).OfType<IPropertySymbol>())
                {
                    if (member.DeclaredAccessibility == Accessibility.Public)
                        return member;
                }

                typeSymbol = typeSymbol.BaseType;
            }

            return null;
        }

        public static IMethodSymbol GetPublicMethod(this ITypeSymbol typeSymbol, string name, params Type[] parameters)
        {
            while (typeSymbol is object)
            {
                foreach (var member in typeSymbol.GetMembers(name).OfType<IMethodSymbol>())
                {
                    if (member.DeclaredAccessibility == Accessibility.Public &&
                        member.Parameters.Length == parameters.Length)
                    {
                        var isMember = true;
                        for (var index = 0; index < parameters.Length; index++)
                        {
                            if (member.Parameters[index].Type.Name != parameters[index].Name)
                            {
                                isMember = false;
                                break;
                            }
                        }
                        if (isMember)
                            return member;
                    }
                }

                typeSymbol = typeSymbol.BaseType;
            }

            return null;
        }
    }
}
