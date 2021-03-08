using System;

using Microsoft.CodeAnalysis;

namespace Aetos.ComparisonGenerator
{
    internal class SourceTypeInfo :
        BaseTypeInfo
    {
        public BaseTypeInfo? BaseType { get; }

        public bool IsValueType
        {
            get
            {
                return this.TypeSymbol.IsValueType;
            }
        }

        public SourceTypeInfo(
            INamedTypeSymbol typeSymbol,
            KnownTypes knownTypes)
            : base(
                  typeSymbol,
                  knownTypes)
        {
            if (typeSymbol is null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }

            if (knownTypes is null)
            {
                throw new ArgumentNullException(nameof(knownTypes));
            }

            var baseTypeSymbol = typeSymbol.BaseType;

            if (!typeSymbol.IsValueType &&
                baseTypeSymbol is not null)
            {
                this.BaseType = new BaseTypeInfo(
                    baseTypeSymbol,
                    knownTypes);
            }
        }
    }
}
