using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ComparableGenerator
{
    internal class GenerateOptions
    {
        public GenerateOptions(
            GeneratorExecutionContext context,
            TypeDeclarationSyntax syntax,
            AttributeData comparableAttribute)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            if (comparableAttribute is null)
            {
                throw new ArgumentNullException(nameof(comparableAttribute));
            }

            var attributeDictionary = comparableAttribute.NamedArguments
                .ToDictionary(
                    x => x.Key,
                    x => x.Value,
                    StringComparer.Ordinal);

            this.GenerateEquatable = LocalGetOption(nameof(this.GenerateEquatable)) ?? true;
            this.GenerateGenericComparable = LocalGetOption(nameof(this.GenerateGenericComparable)) ?? true;
            this.GenerateNonGenericComparable = LocalGetOption(nameof(this.GenerateNonGenericComparable)) ?? true;
            this.GenerateObjectEquals = LocalGetOption(nameof(this.GenerateObjectEquals)) ?? true;
            this.GenerateEqualityContract = LocalGetOption(nameof(this.GenerateEqualityContract)) ?? true;
            this.GenerateEqualityOperators = LocalGetOption(nameof(this.GenerateEqualityOperators)) ?? false;
            this.GenerateComparisonOperators = LocalGetOption(nameof(this.GenerateComparisonOperators)) ?? false;

            bool? LocalGetOption(string optionName)
            {
                return GetOption(context, syntax, attributeDictionary, optionName);
            }
        }

        public bool GenerateEquatable { get; }

        public bool GenerateGenericComparable { get; }

        public bool GenerateNonGenericComparable { get; }

        public bool GenerateObjectEquals { get; }

        public bool GenerateEqualityOperators { get; }

        public bool GenerateComparisonOperators { get; }

        public bool GenerateEqualityContract { get; }

        private static bool? GetOption(
            GeneratorExecutionContext context,
            TypeDeclarationSyntax syntax,
            IDictionary<string, TypedConstant> attributeArguments,
            string optionName)
        {
            if (attributeArguments.TryGetValue(optionName, out var objValue) &&
                !objValue.IsNull &&
                objValue.Value is bool boolValue1)
            {
                return boolValue1;
            }

            var options = context.AnalyzerConfigOptions;
            string buildPropertyName = $"build_property.{optionName}";

            if (options.GetOptions(syntax.SyntaxTree).TryGetBooleanOption(buildPropertyName, out var boolValue2))
            {
                return boolValue2;
            }

            if (options.GlobalOptions.TryGetBooleanOption(buildPropertyName, out var boolValue3))
            {
                return boolValue3;
            }

            return null;
        }
    }
}
