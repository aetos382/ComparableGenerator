using System;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace ComparableGenerator
{
    internal class SourceTypeInfo
    {
        public INamedTypeSymbol Symbol { get; }

        public bool IsEquatable { get; }

        public bool IsGenericComparable { get; }

        public bool IsNonGenericComparable { get; }

        public bool OverridesObjectEquals { get; }

        public bool DefinedEqualityOperators { get; }

        public bool DefinedEqualityComparisonOperators { get; }

        public bool HasEqualityContract { get; }

        public SourceTypeInfo(
            CommonTypes commonTypes,
            INamedTypeSymbol symbol)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
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

            var operators =
                symbol.GetMembers()
                    .OfType<IMethodSymbol>()
                    .Where(x =>
                        x.MethodKind == MethodKind.UserDefinedOperator &&
                        x.Parameters.Length == 2 &&
                        comparer.Equals(x.Parameters[0].Type, symbol) &&
                        comparer.Equals(x.Parameters[1].Type, symbol));

            foreach (var op in operators)
            {
                switch (op.Name)
                {
                    case "op_Equality":
                        this.DefinedEqualityOperators = true;
                        break;

                    case "op_LessThanOrEqual":
                        this.DefinedEqualityComparisonOperators = true;
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
