using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ComparableGenerator
{
    internal static class Attributes
    {
        private const string _comparableAttributeSource = @"
using System;
using System.Diagnostics;

namespace ComparableGenerator
{
    [Conditional(""COMPILE_TIME_ONLY"")]
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct)]
    internal sealed class ComparableAttribute :
        Attribute
    {
        public bool GenerateEquatable { get; init; } = true;

        public bool GenerateGenericComparable { get; init; } = true;

        public bool GenerateNonGenericComparable { get; init; } = true;

        public bool GenerateObjectEquals { get; init; } = true;

        public bool GenerateEqualityContract { get; init; } = true;

        public bool GenerateEqualityOperators { get; init; } = false;

        public bool GenerateComparisonOperators { get; init; } = false;

        public bool GenerateMethodsAsVirtual { get; init; } = true;
    }
}
";

        private const string _compareByAttributeSource = @"
using System;
using System.Diagnostics;

namespace ComparableGenerator
{
    [Conditional(""COMPILE_TIME_ONLY"")]
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field)]
    internal sealed class CompareByAttribute :
        Attribute
    {
        public CompareByAttribute(
            int order)
        {
            this.Order = order;
        }

        public int Order { get; }
    }
}";

        public const string ComparableAttributeName = "ComparableGenerator.ComparableAttribute";
        public const string CompareByAttributeName = "ComparableGenerator.CompareByAttribute";

        public static void AddToProject(
            GeneratorExecutionContext context)
        {
            context.AddSource("ComparableAttribute.cs", _comparableAttributeSource);
            context.AddSource("CompareByAttribute.cs", _compareByAttributeSource);
        }

        public static Compilation AddToCompilation(
            GeneratorExecutionContext context)
        {
            var sourceCodes = new[] {
                _comparableAttributeSource,
                _compareByAttributeSource
            };

            var syntaxTrees = sourceCodes
                .Select(source =>
                    CSharpSyntaxTree.ParseText(
                        source,
                        (CSharpParseOptions) context.ParseOptions,
                        cancellationToken: context.CancellationToken));

            var compilation = context.Compilation.AddSyntaxTrees(syntaxTrees);
            return compilation;
        }
    }
}
