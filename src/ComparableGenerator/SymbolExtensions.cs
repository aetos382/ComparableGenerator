using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace ComparableGenerator
{
    internal static class SymbolExtensions
    {
        public static bool HasInterface(
            this ITypeSymbol type,
            INamedTypeSymbol interfaceSymbol)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (interfaceSymbol is null)
            {
                throw new ArgumentNullException(nameof(interfaceSymbol));
            }

            if (interfaceSymbol.TypeKind != TypeKind.Interface)
            {
                throw new ArgumentException();
            }

            if (interfaceSymbol.IsGenericType)
            {
                bool hasTypeParameter =
                    interfaceSymbol.TypeArguments.Any(x => x.TypeKind == TypeKind.TypeParameter);

                if (hasTypeParameter)
                {
                    throw new ArgumentException();
                }
            }

            var comparer = SymbolEqualityComparer.Default;

            var result = type.AllInterfaces.Any(x => comparer.Equals(x, interfaceSymbol));
            return result;
        }

        public static AttributeData? GetAttribute(
            this ISymbol symbol,
            INamedTypeSymbol attributeSymbol)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (attributeSymbol is null)
            {
                throw new ArgumentNullException(nameof(attributeSymbol));
            }

            var comparer = SymbolEqualityComparer.Default;

            foreach (var attribute in symbol.GetAttributes())
            {
                if (comparer.Equals(attribute.AttributeClass, attributeSymbol))
                {
                    return attribute;
                }
            }

            return null;
        }

        public static IMethodSymbol? GetOverrideSymbol(
            this ITypeSymbol type,
            IMethodSymbol baseSymbol,
            IEqualityComparer<ISymbol?> comparer)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (baseSymbol is null)
            {
                throw new ArgumentNullException(nameof(baseSymbol));
            }

            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            var methods =
                type.GetMembers(baseSymbol.Name).OfType<IMethodSymbol>();

            foreach (var method in methods)
            {
                if (!method.IsOverride)
                {
                    continue;
                }

                if (comparer.Equals(method.OverriddenMethod, baseSymbol))
                {
                    return method;
                }
            }

            return null;
        }

        public static string GetFullName(
            this ITypeSymbol type)
        {
            return GetFullName(type, out _, out _);
        }

        public static string GetFullName(
            this ITypeSymbol type,
            out string namespaceName,
            out string typeName)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var typeNames = new List<string>();

            ITypeSymbol? currentType = type;
            var lastValidType = type;

            while (currentType is not null)
            {
                typeNames.Add(currentType.Name);

                lastValidType = currentType;
                currentType = currentType.ContainingType;
            }

            var namespaceNames = new List<string>();

            var currentNamespace = lastValidType.ContainingNamespace;

            while (currentNamespace is not null && !currentNamespace.IsGlobalNamespace)
            {
                namespaceNames.Add(currentNamespace.Name);
                currentNamespace = currentNamespace.ContainingNamespace;
            }

            typeNames.Reverse();
            namespaceNames.Reverse();

            typeName = string.Join(".", typeNames);

            if (namespaceNames.Any())
            {
                namespaceName = string.Join(".", namespaceNames);
                return $"{namespaceName}.{typeName}";
            }
            else
            {
                namespaceName = string.Empty;
                return typeName;
            }
        }
    }
}
