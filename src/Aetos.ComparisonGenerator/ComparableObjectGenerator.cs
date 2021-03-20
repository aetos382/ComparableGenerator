using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
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
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

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

            if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            {
                return;
            }

            var knownTypes = new KnownTypes(context.Compilation);
            var candidateTypes = new List<SourceTypeInfo>();

            foreach (var candidateSyntax in receiver.CandidateSyntaxes)
            {
                var sourceTypeInfo = SourceTypeInfo.TryCreate(
                    context,
                    candidateSyntax,
                    knownTypes,
                    this._options,
                    out var diagnostics);

                context.ReportDiagnostics(diagnostics);

                if (sourceTypeInfo is null)
                {
                    continue;
                }

                candidateTypes.Add(sourceTypeInfo);
            }

            var typeMap = candidateTypes.ToDictionary(
                x => x.TypeSymbol,
                (IEqualityComparer<ITypeSymbol>)SymbolEqualityComparer.Default);

            foreach (var candidateType in candidateTypes.ToArray())
            {
                var options = candidateType.GenerateOptions;

                foreach (var member in candidateType.Members)
                {
                    if (options.GenerateGenericComparable ||
                        options.GenerateNonGenericComparable ||
                        options.GenerateComparisonOperators ||
                        options.GenerateStructuralComparable)
                    {
                        var memberType = member.Type;

                        if (knownTypes.IsGenericComparable(memberType))
                        {
                            continue;
                        }

                        if (knownTypes.IsNonGenericComparable(memberType))
                        {
                            continue;
                        }

                        if (knownTypes.IsStructuralComparable(memberType))
                        {
                            continue;
                        }

                        if (typeMap.TryGetValue(memberType, out var targetType) &&
                            targetType.HasComparableAttribute)
                        {
                            continue;
                        }

                        if (!knownTypes.TryGetNullableUnderlyingType(memberType, out var underlyingType))
                        {
                            continue;
                        }

                        if (typeMap.TryGetValue(underlyingType, out var targetType2) &&
                            targetType2.HasComparableAttribute)
                        {
                            continue;
                        }

                        var memberLocation = member.Symbol.DeclaringSyntaxReferences
                            .Select(x => Location.Create(x.SyntaxTree, x.Span))
                            .ToArray();

                        context.ReportDiagnostic(
                            DiagnosticFactory.MemberIsNotComparable(
                                candidateType.FullName,
                                member.Name,
                                member.TypeName,
                                memberLocation[0],
                                memberLocation.Skip(1)));
                    }
                }
            }

            foreach (var sourceType in candidateTypes)
            {
                var options = sourceType.GenerateOptions;
                string fullName = sourceType.FullName;

                GenerateCode(
                    context,
                    new CommonGenerator(sourceType),
                    $"{fullName}_Common.cs");

                if (options.GenerateEquatable &&
                    !sourceType.IsEquatable)
                {
                    GenerateCode(
                        context,
                        new EquatableGenerator(sourceType),
                        $"{fullName}_Equatable.cs");
                }

                if (options.GenerateGenericComparable &&
                    !sourceType.IsGenericComparable)
                {
                    GenerateCode(
                        context,
                        new GenericComparableGenerator(sourceType),
                        $"{fullName}_GenericComparable.cs");
                }

                if (options.GenerateNonGenericComparable &&
                    !sourceType.IsNonGenericComparable)
                {
                    GenerateCode(
                        context,
                        new NonGenericComparableGenerator(sourceType),
                        $"{fullName}_NonGenericComparable.cs");
                }

                if (options.GenerateObjectEquals &&
                    !sourceType.OverridesObjectEquals)
                {
                    GenerateCode(
                        context,
                        new ObjectEqualsGenerator(sourceType),
                        $"{fullName}_ObjectEquals.cs");
                }

                if (options.GenerateEqualityOperators &&
                    !sourceType.DefinedEqualityOperators)
                {
                    GenerateCode(
                        context,
                        new EqualityOperatorsGenerator(sourceType),
                        $"{fullName}_EqualityOperators.cs");
                }

                if (options.GenerateComparisonOperators &&
                    !sourceType.DefinedComparisonOperators)
                {
                    GenerateCode(
                        context,
                        new ComparisonOperatorsGenerator(sourceType),
                        $"{fullName}_ComparisonOperators.cs");
                }
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
            GeneratorBase generator,
            string hintName)
        {
            string code = generator.TransformText();
            context.AddSource(hintName, code);
        }

        private class SyntaxReceiver :
            ISyntaxReceiver
        {
            private readonly List<TypeDeclarationSyntax> _syntaxes = new();

            public IReadOnlyList<TypeDeclarationSyntax> CandidateSyntaxes
            {
                get
                {
                    return this._syntaxes;
                }
            }

            public void OnVisitSyntaxNode(
                SyntaxNode syntaxNode)
            {
                if (syntaxNode is null)
                {
                    throw new ArgumentNullException(nameof(syntaxNode));
                }

                if (syntaxNode is not TypeDeclarationSyntax tds)
                {
                    return;
                }

                var attrs = tds.AttributeLists.SelectMany(x => x.Attributes);
                if (!attrs.Any())
                {
                    return;
                }

                this._syntaxes.Add(tds);
            }
        }
    }
}
