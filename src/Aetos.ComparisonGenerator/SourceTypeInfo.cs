using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Aetos.ComparisonGenerator
{
    internal class SourceTypeInfo :
        BaseTypeInfo
    {
        public TypeDeclarationSyntax SyntaxNode { get; }

        public Location SyntaxLocation { get; }

        public string NamespaceName { get; }

        public IReadOnlyList<INamedTypeSymbol> EnclosingTypes { get; }

        public IReadOnlyList<SourceMemberInfo> Members { get; }

        public NullableContext NullableContext { get; }

        public bool NullableAnnotationsEnabled
        {
            get
            {
                return this.NullableContext.AnnotationsEnabled();
            }
        }

        public GenerateOptions GenerateOptions { get; }

        public BaseTypeInfo? BaseType { get; }

        public bool HasComparableAttribute { get; }

        public bool HasEquatableAttribute { get; }

        public bool IsValueType
        {
            get
            {
                return this.TypeSymbol.IsValueType;
            }
        }

        public bool IsNullableValueType { get; }

        public static SourceTypeInfo? TryCreate(
            GeneratorExecutionContext context,
            TypeDeclarationSyntax syntax,
            KnownTypes knownTypes,
            GenerateOptions? options,
            out ImmutableArray<Diagnostic> diagnostics)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            if (knownTypes is null)
            {
                throw new ArgumentNullException(nameof(knownTypes));
            }

            var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

            try
            {
                var semanticModel = context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = ModelExtensions.GetDeclaredSymbol(semanticModel, syntax, context.CancellationToken);

                if (symbol is not INamedTypeSymbol ts)
                {
                    // TODO: Diagnostics ?
                    return null;
                }

                string fullTypeName = ts.GetFullName();
                var location = syntax.GetLocation();

                var equatableAttribute = knownTypes.GetEquatableAttribute(ts);
                var comparableAttribute = knownTypes.GetComparableAttribute(ts);

                var attribute = comparableAttribute;

                if (attribute is null)
                {
                    attribute = equatableAttribute;
                }
                else if (equatableAttribute is not null)
                {
                    // TODO: Diagnostic
                    // TODO: CodeFix
                }

                if (attribute is null)
                {
                    return null;
                }

                if (!syntax.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    diagnosticsBuilder.Add(
                        DiagnosticFactory.TypeIsNotPartial(
                            fullTypeName,
                            location));

                    return null;
                }

                if (syntax.Modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    diagnosticsBuilder.Add(
                        DiagnosticFactory.TypeIsStatic(
                            fullTypeName,
                            location));

                    return null;
                }

                var enclosingTypes = new List<INamedTypeSymbol>();
                var enclosingType = ts.ContainingType;

                while (enclosingType is not null)
                {
                    var invalidSyntaxLocations = enclosingType.DeclaringSyntaxReferences
                        .Select(x => x.GetSyntax(context.CancellationToken))
                        .OfType<TypeDeclarationSyntax>()
                        .Where(x => !x.Modifiers.Any(SyntaxKind.PartialKeyword))
                        .Select(x => x.GetLocation())
                        .ToArray();

                    if (invalidSyntaxLocations.Length > 0)
                    {
                        diagnosticsBuilder.Add(
                            DiagnosticFactory.TypeIsNotPartial(
                                enclosingType.GetFullName(),
                                invalidSyntaxLocations[0],
                                invalidSyntaxLocations.Skip(1)));

                        return null;
                    }

                    enclosingTypes.Add(enclosingType);
                    enclosingType = enclosingType.ContainingType;
                }

                enclosingTypes.Reverse();

                var members = new List<SourceMemberInfo>();

                foreach (var member in ts.GetMembers())
                {
                    var compareByAttribute = knownTypes.GetCompareByAttribute(member);
                    if (compareByAttribute is null)
                    {
                        continue;
                    }

                    if (member is IPropertySymbol ps)
                    {
                        members.Add(new SourceMemberInfo(ps, compareByAttribute));
                    }
                    else if (member is IFieldSymbol fs)
                    {
                        members.Add(new SourceMemberInfo(fs, compareByAttribute));
                    }
                    else
                    {
                        // TODO: Diagnostic?
                        return null;
                    }
                }

                if (members.Count == 0)
                {
                    diagnosticsBuilder.Add(
                        DiagnosticFactory.NoMembers(
                            fullTypeName,
                            location));

                    return null;
                }

                options ??= new GenerateOptions(context, syntax, attribute);

                var nullableContext = semanticModel.GetNullableContext(syntax.SpanStart);

                var sortedMembers = members
                    .OrderBy(x => x.ComparisonOrder)
                    .ThenBy(x => x.Name)
                    .ToArray();

                var sourceTypeInfo = new SourceTypeInfo(
                    syntax,
                    location,
                    ts,
                    (equatableAttribute is not null),
                    (comparableAttribute is not null),
                    enclosingTypes,
                    sortedMembers,
                    nullableContext,
                    options,
                    knownTypes);

                return sourceTypeInfo;
            }
            finally
            {
                diagnostics = diagnosticsBuilder.ToImmutable();
            }
        }

        private SourceTypeInfo(
            TypeDeclarationSyntax syntaxNode,
            Location syntaxLocation,
            INamedTypeSymbol typeSymbol,
            bool hasEquatableAttribute,
            bool hasComparableAttribute,
            IReadOnlyList<INamedTypeSymbol> enclosingTypes,
            IReadOnlyList<SourceMemberInfo> members,
            NullableContext nullableContext,
            GenerateOptions options,
            KnownTypes knownTypes)
            : base(
                  typeSymbol,
                  knownTypes)
        {
            this.SyntaxNode = syntaxNode;
            this.SyntaxLocation = syntaxLocation;
            this.HasEquatableAttribute = hasEquatableAttribute;
            this.HasComparableAttribute = hasComparableAttribute;
            this.EnclosingTypes = enclosingTypes;
            this.Members = members;
            this.NullableContext = nullableContext;
            this.GenerateOptions = options;
            this.IsNullableValueType = knownTypes.IsNullableValueType(typeSymbol);
            this.NamespaceName = typeSymbol.ContainingNamespace?.GetFullName() ?? string.Empty;

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
