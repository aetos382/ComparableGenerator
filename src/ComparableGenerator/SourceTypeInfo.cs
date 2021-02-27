using System;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace ComparableGenerator
{
    internal class SourceTypeInfo
    {
        public INamedTypeSymbol Symbol { get; }

        public bool IsValueType
        {
            get
            {
                return this.Symbol.IsValueType;
            }
        }

        public bool IsEquatable { get; }

        public bool IsGenericComparable { get; }

        public bool IsNonGenericComparable { get; }

        public bool OverridesObjectEquals { get; }

        public bool DefinedNullableEqualityOperators { get; }

        public bool DefinedNullableComparisonOperators { get; }

        public bool DefinedNonNullableEqualityOperators { get; }

        public bool DefinedNonNullableComparisonOperators { get; }

        public bool HasEqualityContract { get; }

        public SourceTypeInfo(
            INamedTypeSymbol symbol,
            CommonTypes commonTypes,
            NullableContext nullableContext)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (commonTypes is null)
            {
                throw new ArgumentNullException(nameof(commonTypes));
            }

            this.Symbol = symbol;

            var comparer = SymbolEqualityComparer.Default;

            this.IsEquatable = commonTypes.IsEquatable(symbol);
            this.IsGenericComparable = commonTypes.IsGenericComparable(symbol);
            this.IsNonGenericComparable = commonTypes.IsNonGenericComparable(symbol);

            var objectEquals = commonTypes.Object.GetMembers(nameof(object.Equals))
                .OfType<IMethodSymbol>()
                .Single(x =>
                    x.Parameters.Length == 1 &&
                    comparer.Equals(x.Parameters[0].Type, commonTypes.Object));

            var objectEqualsOverride =
                symbol.GetOverrideSymbol(objectEquals, comparer);

            this.OverridesObjectEquals = objectEqualsOverride is not null;

            ITypeSymbol nullableType;

            if (symbol.IsValueType)
            {
                nullableType = commonTypes.MakeNullable(symbol);
            }
            else if (nullableContext.AnnotationsEnabled())
            {
                nullableType = symbol.WithNullableAnnotation(NullableAnnotation.Annotated);
            }
            else
            {
                nullableType = symbol;
            }

            var operators =
                symbol.GetMembers()
                    .OfType<IMethodSymbol>()
                    .Where(x =>
                        x.MethodKind == MethodKind.UserDefinedOperator &&
                        x.Parameters.Length == 2);

            var nullableComparer = SymbolEqualityComparer.IncludeNullability;

            foreach (var op in operators)
            {
                var type1 = op.Parameters[0].Type;
                var type2 = op.Parameters[1].Type;

                bool matchNonNullableTypes =
                    nullableComparer.Equals(type1, symbol) &&
                    nullableComparer.Equals(type2, symbol);

                bool matchNullableTypes =
                    nullableComparer.Equals(type1, nullableType) &&
                    nullableComparer.Equals(type2, nullableType);

                switch (op.Name)
                {
                    case "op_Equality":
                        if (matchNonNullableTypes)
                        {
                            this.DefinedNonNullableEqualityOperators = true;
                        }
                        else if (matchNullableTypes)
                        {
                            this.DefinedNullableEqualityOperators = true;
                        }

                        break;

                    case "op_LessThan":
                        if (matchNonNullableTypes)
                        {
                            this.DefinedNonNullableComparisonOperators = true;
                        }
                        else if (matchNullableTypes)
                        {
                            this.DefinedNullableComparisonOperators = true;
                        }

                        break;
                }
            }

            bool hasEqualityContract = symbol.GetMembers("EqualityContract")
                .OfType<IPropertySymbol>()
                .Any(x =>
                    x.IsVirtual &&
                    comparer.Equals(x.Type, commonTypes.Type));

            this.HasEqualityContract = hasEqualityContract;
        }
    }
}
