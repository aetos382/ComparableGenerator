using System;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace Aetos.ComparisonGenerator
{
    internal class BaseTypeInfo
    {
        public BaseTypeInfo(
            INamedTypeSymbol typeSymbol,
            KnownTypes knownTypes)
        {
            if (typeSymbol is null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }

            if (knownTypes is null)
            {
                throw new ArgumentNullException(nameof(knownTypes));
            }

            var comparer = SymbolEqualityComparer.Default;

            this.TypeSymbol = typeSymbol;
            this.FullName = typeSymbol.GetFullName();

            this.IsEquatable = knownTypes.IsEquatable(typeSymbol);
            this.IsGenericComparable = knownTypes.IsGenericComparable(typeSymbol);
            this.IsNonGenericComparable = knownTypes.IsNonGenericComparable(typeSymbol);
            this.IsStructuralEquatable = knownTypes.IsStructuralEquatable(typeSymbol);
            this.IsStructuralComparable = knownTypes.IsStructuralComparable(typeSymbol);

            var objectEquals = knownTypes.Object.GetMembers(nameof(object.Equals))
                .OfType<IMethodSymbol>()
                .Single(x =>
                    x.Parameters.Length == 1 &&
                    comparer.Equals(x.Parameters[0].Type, knownTypes.Object));

            var objectEqualsOverride =
                typeSymbol.GetOverrideSymbol(objectEquals!, comparer);

            this.OverridesObjectEquals = objectEqualsOverride is not null;
            
            var objectGetHashCode = knownTypes.Object.GetMembers(nameof(object.GetHashCode))
                .OfType<IMethodSymbol>()
                .Single(x => x.Parameters.Length == 0);

            var objectGetHashCodeOverride =
                typeSymbol.GetOverrideSymbol(objectGetHashCode!, comparer);

            this.OverridesObjectGetHashCode = objectGetHashCodeOverride is not null;

            var operators =
                typeSymbol.GetMembers()
                    .OfType<IMethodSymbol>()
                    .Where(x =>
                        x.MethodKind == MethodKind.UserDefinedOperator &&
                        x.Parameters.Length == 2);

            var operatorParameterType = typeSymbol;

            if (typeSymbol.IsValueType)
            {
                operatorParameterType = knownTypes.MakeNullable(typeSymbol);
            }

            foreach (var op in operators)
            {
                var type1 = op.Parameters[0].Type;
                var type2 = op.Parameters[1].Type;

                bool matchTypes =
                    comparer.Equals(type1, operatorParameterType) &&
                    comparer.Equals(type2, operatorParameterType);

                switch (op.Name)
                {
                    case "op_Equality":
                        if (matchTypes)
                        {
                            this.DefinedEqualityOperators = true;
                        }

                        break;

                    case "op_LessThan":
                        if (matchTypes)
                        {
                            this.DefinedComparisonOperators = true;
                        }

                        break;
                }
            }

            bool hasEqualityContract = typeSymbol.GetMembers("EqualityContract")
                .OfType<IPropertySymbol>()
                .Any(x =>
                    x.IsVirtual &&
                    comparer.Equals(x.Type, knownTypes.Type));

            this.HasEqualityContract = hasEqualityContract;
        }

        public INamedTypeSymbol TypeSymbol { get; }

        public string FullName { get; }

        public bool IsEquatable { get; }

        public bool IsGenericComparable { get; }

        public bool IsNonGenericComparable { get; }

        public bool IsStructuralEquatable { get; }

        public bool IsStructuralComparable { get; }

        public bool OverridesObjectEquals { get; }

        public bool OverridesObjectGetHashCode { get; }

        public bool DefinedEqualityOperators { get; }

        public bool DefinedComparisonOperators { get; }

        public bool HasEqualityContract { get; }
    }
}
