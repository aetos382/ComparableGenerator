using System;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace Aetos.ComparisonGenerator
{
    internal class SourceMemberInfo
    {
        public ISymbol Symbol { get; }

        public string Name { get; }

        public ITypeSymbol Type { get; }

        public string TypeName { get; }

        public string FullTypeNameWithGlobalPrefix { get; }

        public int ComparisonOrder { get; }

        public bool? PreferStructuralComparison { get; }

        public SourceMemberInfo(
            IFieldSymbol member,
            AttributeData compareByAttribute)
            : this(
                (ISymbol)member,
                compareByAttribute)
        {
        }

        public SourceMemberInfo(
            IPropertySymbol member,
            AttributeData compareByAttribute)
            : this(
                (ISymbol)member,
                compareByAttribute)
        {
        }

        private SourceMemberInfo(
            ISymbol memberSymbol,
            AttributeData compareByAttribute)
        {
            if (memberSymbol is null)
            {
                throw new ArgumentNullException(nameof(memberSymbol));
            }

            if (compareByAttribute is null)
            {
                throw new ArgumentNullException(nameof(compareByAttribute));
            }

            this.Symbol = memberSymbol;
            this.Name = memberSymbol.Name;

            var type = memberSymbol switch {
                IPropertySymbol ps => ps.Type,
                IFieldSymbol fs => fs.Type,
                _ => throw new ArgumentException()
            };

            this.Type = type;
            this.TypeName = type.GetFullName();
            this.FullTypeNameWithGlobalPrefix = type.GetFullName(true);

            var arguments = compareByAttribute.NamedArguments.ToDictionary(
                x => x.Key,
                x => x.Value,
                StringComparer.Ordinal);

            if (arguments.TryGetValue("Order", out var order))
            {
                this.ComparisonOrder = (int)order.Value!;
            }

            if (arguments.TryGetValue("PreferStructuralComparison", out var preferStructuralComparison))
            {
                this.PreferStructuralComparison = (bool)preferStructuralComparison.Value!;
            }
        }
    }
}
