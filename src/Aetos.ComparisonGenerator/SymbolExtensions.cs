using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;

namespace Aetos.ComparisonGenerator
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

                var overriddenMethod = method.OverriddenMethod;

                while (overriddenMethod is not null)
                {
                    if (comparer.Equals(overriddenMethod, baseSymbol))
                    {
                        return method;
                    }

                    overriddenMethod = overriddenMethod.OverriddenMethod;
                }
            }

            return null;
        }

        private static readonly ConditionalWeakTable<INamespaceOrTypeSymbol, string> _names = new();

        private static readonly ConditionalWeakTable<INamespaceOrTypeSymbol, string> _namesWithGlobalPrefix = new();

        private static readonly SymbolDisplayFormat _withoutGlobalNamespace =
            SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(
                SymbolDisplayGlobalNamespaceStyle.Omitted);

        public static string GetFullName(
            this INamespaceOrTypeSymbol type,
            bool includeGlobalNamespacePrefix = false)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (includeGlobalNamespacePrefix)
            {
                return GetName(_namesWithGlobalPrefix, type, SymbolDisplayFormat.FullyQualifiedFormat);
            }
            else
            {
                return GetName(_names, type, _withoutGlobalNamespace);
            }

            static string GetName(
                ConditionalWeakTable<INamespaceOrTypeSymbol, string> table,
                INamespaceOrTypeSymbol symbol,
                SymbolDisplayFormat format)
            {
                return table.GetValue(symbol, x => x.ToDisplayString(format));
            }
        }
    }
}
