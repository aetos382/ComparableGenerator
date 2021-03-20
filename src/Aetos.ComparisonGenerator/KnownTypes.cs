using System;
using System.Collections;

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
            this.StructuralEquatable = GetType(compilation, typeof(IStructuralEquatable));
            this.StructuralComparable = GetType(compilation, typeof(IStructuralComparable));
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

        public INamedTypeSymbol StructuralEquatable { get; }

        public INamedTypeSymbol StructuralComparable { get; }

        public bool IsNullableValueType(
            ITypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type is not INamedTypeSymbol nts)
            {
                return false;
            }

            if (!nts.IsValueType || !nts.IsGenericType)
            {
                return false;
            }

            return
                SymbolEqualityComparer.Default.Equals(nts.ConstructedFrom, this.Nullable);
        }

        public bool TryGetNullableUnderlyingType(
            ITypeSymbol typeSymbol,
            out INamedTypeSymbol? underlyingTypeSymbol)
        {
            if (typeSymbol is null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }

            underlyingTypeSymbol = default;

            if (!this.IsNullableValueType(typeSymbol))
            {
                return false;
            }

            if (typeSymbol is not INamedTypeSymbol nts)
            {
                return false;
            }

            underlyingTypeSymbol = (INamedTypeSymbol)nts.TypeArguments[0];
            return true;
        }

        public bool IsEquatable(
            ITypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var equatable = this.MakeConstructedEquatable(type);

            bool result = type.HasInterface(equatable);
            if (result)
            {
                return result;
            }

            if (type is INamedTypeSymbol nts)
            {
                if (this.IsNullableValueType(nts))
                {
                    result = this.IsEquatable(nts.TypeArguments[0]);
                }
            }

            return result;
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
            if (result)
            {
                return result;
            }

            if (type is INamedTypeSymbol nts)
            {
                if (this.IsNullableValueType(nts))
                {
                    result = this.IsGenericComparable(nts.TypeArguments[0]);
                }
            }

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
            if (result)
            {
                return result;
            }

            if (type is INamedTypeSymbol nts)
            {
                if (this.IsNullableValueType(nts))
                {
                    result = this.IsNonGenericComparable(nts.TypeArguments[0]);
                }
            }

            return result;
        }

        public bool IsStructuralEquatable(
            ITypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            bool result = type.HasInterface(this.StructuralEquatable);
            if (result)
            {
                return result;
            }

            if (type is INamedTypeSymbol nts)
            {
                if (this.IsNullableValueType(nts))
                {
                    result = this.IsStructuralEquatable(nts.TypeArguments[0]);
                }
            }

            return result;
        }

        public bool IsStructuralComparable(
            ITypeSymbol type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            bool result = type.HasInterface(this.StructuralComparable);
            if (result)
            {
                return result;
            }

            if (type is INamedTypeSymbol nts)
            {
                if (this.IsNullableValueType(nts))
                {
                    result = this.IsStructuralComparable(nts.TypeArguments[0]);
                }
            }

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
                    "The typeArgument must be value type.",
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

        public AttributeData? GetEquatableAttribute(
            ISymbol symbol)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            var data = symbol.GetAttribute(this.EquatableAttribute);
            return data;
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

        public AttributeData? GetCompareByAttribute(
            ISymbol symbol)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (symbol is not (IPropertySymbol or IFieldSymbol))
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
