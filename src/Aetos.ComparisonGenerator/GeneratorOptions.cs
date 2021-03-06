using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Aetos.ComparisonGenerator
{
    internal class GeneratorOptions
    {
        internal GeneratorOptions(
            bool generateEquatable = true,
            bool generateGenericComparable = true,
            bool generateNonGenericComparable = true,
            bool generateObjectEquals = true,
            bool generateEqualityContract = true,
            bool generateEqualityOperators = false,
            bool generateComparisonOperators = false,
            bool generateMethodsAsVirtual = true)
        {
            this.GenerateEquatable = generateEquatable;
            this.GenerateGenericComparable = generateGenericComparable;
            this.GenerateNonGenericComparable = generateNonGenericComparable;
            this.GenerateObjectEquals = generateObjectEquals;
            this.GenerateEqualityContract = generateEqualityContract;
            this.GenerateEqualityOperators = generateEqualityOperators;
            this.GenerateComparisonOperatos = generateComparisonOperators;
            this.GenerateMethodsAsVirtual = generateMethodsAsVirtual;
        }

        public GeneratorOptions(
            GeneratorExecutionContext context,
            TypeDeclarationSyntax syntax,
            AttributeData attribute)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            if (attribute is null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            var attributeDictionary = attribute.NamedArguments
                .ToDictionary(
                    x => x.Key,
                    x => x.Value,
                    StringComparer.OrdinalIgnoreCase);

            this.GenerateEquatable = LocalGetOption(nameof(this.GenerateEquatable)) ?? true;
            this.GenerateGenericComparable = LocalGetOption(nameof(this.GenerateGenericComparable)) ?? true;
            this.GenerateNonGenericComparable = LocalGetOption(nameof(this.GenerateNonGenericComparable)) ?? true;
            this.GenerateObjectEquals = LocalGetOption(nameof(this.GenerateObjectEquals)) ?? true;
            this.GenerateEqualityContract = LocalGetOption(nameof(this.GenerateEqualityContract)) ?? true;
            this.GenerateEqualityOperators = LocalGetOption(nameof(this.GenerateEqualityOperators)) ?? false;
            this.GenerateComparisonOperators = LocalGetOption(nameof(this.GenerateComparisonOperators)) ?? false;
            this.GenerateMethodsAsVirtual = LocalGetOption(nameof(this.GenerateMethodsAsVirtual)) ?? true;

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

        public bool GenerateComparisonOperatos { get; }

        public bool GenerateComparisonOperators { get; }

        public bool GenerateEqualityContract { get; }
        
        public bool GenerateMethodsAsVirtual { get; }

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

            var options = context.AnalyzerConfigOptions.GetOptions(syntax.SyntaxTree);
            bool value;

            // from ItemMetadata
            string buildMetadataName = $"build_metadata.Compile.{optionName}";
            if (options.TryGetBooleanOption(buildMetadataName, out value))
            {
                return value;
            }

            // from .editorconfig
            if (options.TryGetBooleanOption(optionName, out value))
            {
                return value;
            }

            // from PropertyGroup or CommandLine
            // build property に指定されたものも SyntaxTree ごとの options を通じて取得可能
            string buildPropertyName = $"build_property.{optionName}";
            if (options.TryGetBooleanOption(buildPropertyName, out value))
            {
                return value;
            }

            return null;
        }
    }
}
