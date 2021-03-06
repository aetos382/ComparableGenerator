using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Aetos.ComparisonGenerator
{
    internal class CandidateTypeInfo
    {
        public TypeDeclarationSyntax SyntaxNode { get; }

        public INamedTypeSymbol TypeSymbol { get; }

        public AttributeData? EquatableAttribute { get; }

        public AttributeData? ComparableAttribute { get; }

        public NullableContext NullableContext { get; }

        public string? NamespaceName { get; }

        public string FullName { get; }

        public CandidateTypeInfo(
            TypeDeclarationSyntax syntaxNode,
            INamedTypeSymbol typeSymbol,
            AttributeData? equatableAttribute,
            AttributeData? comparableAttribute,
            NullableContext nullableContext)
        {
            if (syntaxNode is null)
            {
                throw new ArgumentNullException(nameof(syntaxNode));
            }

            if (typeSymbol is null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }

            this.SyntaxNode = syntaxNode;
            this.TypeSymbol = typeSymbol;
            this.EquatableAttribute = equatableAttribute;
            this.ComparableAttribute = comparableAttribute;
            this.NullableContext = nullableContext;
            this.FullName = typeSymbol.GetFullName();

            var ns = typeSymbol.ContainingNamespace;

            if (ns is not null && !ns.IsGlobalNamespace)
            {
                this.NamespaceName = ns.GetFullName();
            }
        }
    }
}
