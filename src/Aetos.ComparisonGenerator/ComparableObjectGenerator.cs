using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Aetos.ComparisonGenerator
{
    [Generator]
    public class ComparableObjectGenerator :
        ISourceGenerator
    {
        public ComparableObjectGenerator()
        {
        }

        private readonly GenerateOptions? _options;

        internal ComparableObjectGenerator(
            GenerateOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this._options = options;
        }

        public void Initialize(
            GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(
                () => new SyntaxReceiver(context.CancellationToken));

            context.RegisterForPostInitialization(this.PostInitialize);
        }

        private void PostInitialize(
            GeneratorPostInitializationContext context)
        {
            context.AddSource("EquatableAttribute.cs", Attributes.EquatableAttributeSource);
            context.AddSource("ComparableAttribute.cs", Attributes.ComparableAttributeSource);
            context.AddSource("CompareByAttribute.cs", Attributes.CompareByAttributeSource);
        }

        public void Execute(
            GeneratorExecutionContext context)
        {
            LaunchDebugger(context);

            if (context.SyntaxContextReceiver is not SyntaxReceiver receiver)
            {
                return;
            }

            var knownTypes = new KnownTypes(context.Compilation);

            foreach (var candidateSymbol in receiver.CandidateSymbols)
            {
                GenerateCode(
                    context,
                    candidateSymbol,
                    knownTypes,
                    this._options);
            }
        }

        [Conditional("DEBUG")]
        private static void LaunchDebugger(
            GeneratorExecutionContext context)
        {
            var options = context.AnalyzerConfigOptions.GlobalOptions;

            if (!Debugger.IsAttached &&
                options.TryGetBooleanOption("build_property.DebugGenerator", out var debug) &&
                debug)
            {
                var currentProcessName = Process.GetCurrentProcess().ProcessName;
                if (string.Equals(currentProcessName, "dotnet", StringComparison.OrdinalIgnoreCase))
                {
                    Debugger.Launch();
                }
            }
        }

        private static void GenerateCode(
            GeneratorExecutionContext context,
            CandidateTypeInfo candidateType,
            KnownTypes knownTypes,
            GenerateOptions? options)
        {
            ValidateAttribute(
                candidateType,
                out var attribute,
                out var attributeDiagnostics);

            context.ReportDiagnostics(attributeDiagnostics);

            if (!ValidateModifiers(
                candidateType,
                out var modifierDiagnostics))
            {
                context.ReportDiagnostics(modifierDiagnostics);
                return;
            }

            options ??= new GenerateOptions(
                context,
                candidateType.SyntaxNode,
                attribute);

            var sourceTypeInfo = new SourceTypeInfo(
                candidateType.TypeSymbol,
                knownTypes);

            var symbol = candidateType.TypeSymbol;

            if (!ValidateTypeEnclosingHierarchy(
                symbol,
                out var types,
                out var hierarchyDiagnostics,
                context.CancellationToken))
            {
                context.ReportDiagnostics(hierarchyDiagnostics);
                return;
            }

            ValidateMembers(
                symbol,
                knownTypes,
                options,
                out var members,
                out var memberDiagnostics);

            context.ReportDiagnostics(memberDiagnostics);

            if (!members.Any())
            {
                return;
            }

            var c = new ComparisonGeneratorContext(
                context.Compilation,
                candidateType.NamespaceName,
                types,
                members,
                options,
                knownTypes,
                sourceTypeInfo,
                candidateType.NullableContext);

            string fullName = candidateType.FullName;

            GenerateCode(
                context,
                new CommonGenerator(c),
                $"{fullName}_Common.cs");

            if (options.GenerateEquatable &&
                !sourceTypeInfo.IsEquatable)
            {
                GenerateCode(
                    context,
                    new EquatableGenerator(c),
                    $"{fullName}_Equatable.cs");
            }

            if (options.GenerateGenericComparable &&
                !sourceTypeInfo.IsGenericComparable)
            {
                GenerateCode(
                    context,
                    new GenericComparableGenerator(c),
                    $"{fullName}_GenericComparable.cs");
            }

            if (options.GenerateNonGenericComparable &&
                !sourceTypeInfo.IsNonGenericComparable)
            {
                GenerateCode(
                    context,
                    new NonGenericComparableGenerator(c),
                    $"{fullName}_NonGenericComparable.cs");
            }

            if (options.GenerateObjectEquals &&
                !sourceTypeInfo.OverridesObjectEquals)
            {
                GenerateCode(
                    context,
                    new ObjectEqualsGenerator(c),
                    $"{fullName}_ObjectEquals.cs");
            }

            if (options.GenerateEqualityOperators &&
                !sourceTypeInfo.DefinedEqualityOperators)
            {
                GenerateCode(
                    context,
                    new EqualityOperatorsGenerator(c),
                    $"{fullName}_EqualityOperators.cs");
            }

            if (options.GenerateComparisonOperators &&
                !sourceTypeInfo.DefinedComparisonOperators)
            {
                GenerateCode(
                    context,
                    new ComparisonOperatorsGenerator(c),
                    $"{fullName}_ComparisonOperators.cs");
            }
        }

        private static void ValidateAttribute(
            CandidateTypeInfo candidateSymbol,
            out AttributeData attribute,
            out ImmutableArray<Diagnostic> diagnostics)
        {
            var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>(1);

            AttributeData? candidateAttribute = null;

            if (candidateSymbol.EquatableAttribute is not null)
            {
                candidateAttribute = candidateSymbol.EquatableAttribute;
            }

            if (candidateSymbol.ComparableAttribute is not null)
            {
                if (candidateAttribute is not null)
                {
                    // TODO: 警告を出す
                }

                candidateAttribute = candidateSymbol.ComparableAttribute;
            }

            Debug.Assert(candidateAttribute is not null);

            attribute = candidateAttribute!;
            diagnostics = diagnosticsBuilder.ToImmutable();
        }

        private static bool ValidateModifiers(
            CandidateTypeInfo candidateSymbol,
            out ImmutableArray<Diagnostic> diagnostics)
        {
            var builder = ImmutableArray.CreateBuilder<Diagnostic>(2);

            var syntax = candidateSymbol.SyntaxNode;
            var symbolName = candidateSymbol.TypeSymbol.GetFullName();

            var location = syntax.GetLocation();

            if (!syntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                builder.Add(
                    Diagnostic.Create(
                        DiagnosticDescriptors.TypeIsNotPartial,
                        location,
                        symbolName));
            }

            if (syntax.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                builder.Add(
                    Diagnostic.Create(
                        DiagnosticDescriptors.TypeIsStatic,
                        location,
                        symbolName));
            }

            diagnostics = builder.ToImmutable();
            return !diagnostics.Any();
        }

        private static bool ValidateTypeEnclosingHierarchy(
            INamedTypeSymbol symbol,
            out ImmutableArray<INamedTypeSymbol> enclosingHierarchy,
            out ImmutableArray<Diagnostic> diagnostics,
            CancellationToken cancellationToken)
        {
            var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

            var typesBuilder = ImmutableArray.CreateBuilder<INamedTypeSymbol>();
            typesBuilder.Add(symbol);

            var enclosingType = symbol.ContainingType;

            for (int i = 0; enclosingType is not null; ++i)
            {
                var invalidSyntaxes = enclosingType.DeclaringSyntaxReferences
                    .Select(x => x.GetSyntax(cancellationToken))
                    .OfType<TypeDeclarationSyntax>()
                    .Where(x => !x.Modifiers.Any(SyntaxKind.PartialKeyword));

                foreach (var invalidSyntax in invalidSyntaxes)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.TypeIsNotPartial,
                        invalidSyntax.GetLocation(),
                        invalidSyntax.Identifier.ValueText);

                    diagnosticsBuilder.Add(diagnostic);
                }

                typesBuilder.Add(enclosingType);
                enclosingType = enclosingType.ContainingType;
            }

            typesBuilder.Reverse();

            enclosingHierarchy = typesBuilder.ToImmutable();
            diagnostics = diagnosticsBuilder.ToImmutable();

            return !diagnostics.Any();
        }

        // TODO: IStructuralEquatable / IStructuralComparable を考慮する
        // TODO: 対象のメンバーが複数あって Order が設定されていない場合は Diagnostic を出す
        private static bool ValidateMembers(
            INamedTypeSymbol symbol,
            KnownTypes knownTypes,
            GenerateOptions options,
            out ImmutableArray<SourceMemberInfo> members,
            out ImmutableArray<Diagnostic> diagnostics)
        {
            var membersBuilder = ImmutableArray.CreateBuilder<SourceMemberInfo>();
            var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>(1);

            foreach (var member in symbol.GetMembers())
            {
                var compareByAttribute = knownTypes.GetCompareByAttribute(member);
                if (compareByAttribute is null)
                {
                    continue;
                }

                var memberInfo = member switch {
                    IFieldSymbol fs => new SourceMemberInfo(fs, compareByAttribute),
                    IPropertySymbol ps => new SourceMemberInfo(ps, compareByAttribute),
                    _ => null
                };

                if (memberInfo is null)
                {
                    continue;
                }

                if (options.GenerateGenericComparable ||
                    options.GenerateNonGenericComparable ||
                    options.GenerateComparisonOperators)
                {
                    var memberType = memberInfo.Type;

                    if (!knownTypes.IsGenericComparable(memberType) &&
                        !knownTypes.IsNonGenericComparable(memberType) &&
                        !knownTypes.IsStructuralComparable(memberType))
                    {
                        diagnosticsBuilder.Add(
                            Diagnostic.Create(
                                DiagnosticDescriptors.TypeIsNotComparable,
                                member.Locations[0],
                                member.Locations.Skip(1),
                                symbol.GetFullName(),
                                memberInfo.Name,
                                memberInfo.TypeName));
                    }
                }

                membersBuilder.Add(memberInfo);
            }

            if (!membersBuilder.Any())
            {
                diagnosticsBuilder.Add(
                    Diagnostic.Create(
                        DiagnosticDescriptors.NoMembers,
                        symbol.Locations[0],
                        symbol.Locations.Skip(1),
                        default,
                        default));
            }

            members = membersBuilder
                .OrderBy(x => x.ComparisonOrder)
                .ThenBy(x => x.Name)
                .ToImmutableArray();

            diagnostics = diagnosticsBuilder.ToImmutable();

            return members.Any();
        }

        private static void GenerateCode(
            GeneratorExecutionContext context,
            GeneratorBase generator,
            string hintName)
        {
            string code = generator.TransformText();
            context.AddSource(hintName, code);
        }

        private class SyntaxReceiver :
            ISyntaxContextReceiver
        {
            private readonly CancellationToken _cancellationToken;

            public SyntaxReceiver(
                CancellationToken cancellationToken = default)
            {
                this._cancellationToken = cancellationToken;
            }

            public readonly List<CandidateTypeInfo> CandidateSymbols = new();

            public void OnVisitSyntaxNode(
                GeneratorSyntaxContext context)
            {
                if (context.Node is not TypeDeclarationSyntax tds)
                {
                    return;
                }

                var attrs = tds.AttributeLists.SelectMany(x => x.Attributes);
                if (!attrs.Any())
                {
                    return;
                }

                var semanticModel = context.SemanticModel;

                var symbol = semanticModel.GetDeclaredSymbol(tds, this._cancellationToken);
                var attributes = symbol.GetAttributes();

                AttributeData? equatableAttribute = null;
                AttributeData? comparableAttribute = null;

                foreach (var attribute in attributes)
                {
                    var attributeClassName = attribute.AttributeClass?.GetFullName(true);

                    if (attributeClassName == Attributes.EquatableAttributeNameWithGlobalPrefix)
                    {
                        equatableAttribute = attribute;
                    }
                    else if (attributeClassName == Attributes.ComparableAttributeNameWithGlobalPrefix)
                    {
                        comparableAttribute = attribute;
                    }
                }

                if (equatableAttribute is null &&
                    comparableAttribute is null)
                {
                    return;
                }

                var nullableContext = semanticModel.GetNullableContext(tds.Span.End);

                var candidate = new CandidateTypeInfo(
                    tds, symbol, equatableAttribute, comparableAttribute, nullableContext);

                this.CandidateSymbols.Add(candidate);
            }
        }
    }
}
