using System;

using Microsoft.CodeAnalysis;

namespace Aetos.ComparisonGenerator
{
    internal class KnownTypes
    {
        public KnownTypes(
            Compilation compilation)
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            this.EquatableAttribute = GetType(compilation, Attributes.EquatableAttributeName);
            this.ComparableAttribute = GetType(compilation, Attributes.ComparableAttributeName);
            this.CompareByAttribute = GetType(compilation, Attributes.CompareByAttributeName);

            this.Object = compilation.GetSpecialType(SpecialType.System_Object);
            this.Type = GetType(compilation, typeof(Type));
            this.Equatable = GetType(compilation, typeof(IEquatable<>));
            this.Nullable = GetType(compilation, typeof(Nullable<>));
            this.GenericComparable = GetType(compilation, typeof(IComparable<>));
            this.NonGenericComparable = GetType(compilation, typeof(IComparable));
        }

        public INamedTypeSymbol EquatableAttribute { get; }

        public INamedTypeSymbol ComparableAttribute { get; }

        public INamedTypeSymbol CompareByAttribute { get; }

        public INamedTypeSymbol Object { get; }

        public INamedTypeSymbol Type { get; }

        public INamedTypeSymbol Equatable { get; }

        public INamedTypeSymbol Nullable { get; }

        public INamedTypeSymbol GenericComparable { get; }

        public INamedTypeSymbol NonGenericComparable { get; }

        public bool IsEquatable(
            ITypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var equatable = this.MakeConstructedEquatable(type);
            bool result = type.HasInterface(equatable);

            return result;
        }

        public bool IsNullable(
            INamedTypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!type.IsValueType || !type.IsGenericType)
            {
                return false;
            }

            return
                SymbolEqualityComparer.Default.Equals(type.ConstructedFrom, this.Nullable);
        }

        public bool IsGenericComparable(
            ITypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var comparable = this.MakeConstructedComparable(type);
            bool result = type.HasInterface(comparable);

            return result;
        }
                
        public bool IsNonGenericComparable(
            ITypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            bool result = type.HasInterface(this.NonGenericComparable);

            return result;
        }

        public INamedTypeSymbol MakeNullable(
            ITypeSymbol typeArgument)
        {
            if (typeArgument is null)
            {
                throw new ArgumentNullException(nameof(typeArgument));
            }

            if (!typeArgument.IsValueType)
            {
                throw new ArgumentException(
                    $"The typeArgument must be value type.",
                    nameof(typeArgument));
            }

            return this.Nullable.Construct(typeArgument);
        }

        public INamedTypeSymbol MakeConstructedEquatable(
            ITypeSymbol typeArgument)
        {
            if (typeArgument is null)
            {
                throw new ArgumentNullException(nameof(typeArgument));
            }

            return this.Equatable.Construct(typeArgument);
        }
        
        public INamedTypeSymbol MakeConstructedComparable(
            ITypeSymbol typeArgument)
        {
            if (typeArgument is null)
            {
                throw new ArgumentNullException(nameof(typeArgument));
            }

            return this.GenericComparable.Construct(typeArgument);
        }

        public AttributeData? GetComparableAttribute(
            ISymbol symbol)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            var data = symbol.GetAttribute(this.ComparableAttribute);
            return data;
        }

        public AttributeData? GetComparisonOrder(
            ISymbol symbol)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (symbol is not IPropertySymbol &&
                symbol is not IFieldSymbol)
            {
                return null;
            }

            var compareByAttribute = symbol.GetAttribute(this.CompareByAttribute);
            return compareByAttribute;
        }

        private static INamedTypeSymbol GetType(
            Compilation compilation,
            Type type)
        {
            return GetType(compilation, type.FullName);
        }

        private static INamedTypeSymbol GetType(
            Compilation compilation,
            string typeName)
        {
            var type = compilation.GetTypeByMetadataName(typeName);

            if (type is null)
            {
                throw new ArgumentException($"{typeName} not found.");
            }

            return type;
        }
    }
}
