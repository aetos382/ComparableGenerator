using System;

using Microsoft.CodeAnalysis;

namespace ComparisonGenerator
{
    internal class SourceMemberInfo
    {
        public ISymbol Symbol { get; }

        public ITypeSymbol Type { get; }

        public string TypeName { get; }

        public string Name { get; }

        public SourceMemberInfo(
            ISymbol memberSymbol)
        {
            if (memberSymbol is null)
            {
                throw new ArgumentNullException(nameof(memberSymbol));
            }

            this.Symbol = memberSymbol;

            var type = memberSymbol switch {
                IPropertySymbol ps => ps.Type,
                IFieldSymbol fs => fs.Type,
                _ => throw new ArgumentException()
            };

            this.Type = type;
            this.TypeName = type.GetFullName();
            this.Name = memberSymbol.Name;
        }
    }
}
