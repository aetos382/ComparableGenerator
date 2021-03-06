namespace Aetos.ComparisonGenerator
{
    internal static class Attributes
    {
        public const string EquatableAttributeSource = @"
using System;
using System.Diagnostics;

namespace Aetos.ComparisonGenerator
{
    [Conditional(""COMPILE_TIME_ONLY"")]
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct)]
    internal class EquatableAttribute :
        Attribute
    {
        public bool GenerateEquatable { get; init; } = true;

        public bool GenerateObjectEquals { get; init; } = true;

        public bool GenerateEqualityContract { get; init; } = true;

        public bool GenerateEqualityOperators { get; init; } = false;

        public bool GenerateMethodsAsVirtual { get; init; } = true;
    }
}";

        public const string ComparableAttributeSource = @"
using System;
using System.Diagnostics;

namespace Aetos.ComparisonGenerator
{
    [Conditional(""COMPILE_TIME_ONLY"")]
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct)]
    internal sealed class ComparableAttribute :
        EquatableAttribute
    {
        public bool GenerateGenericComparable { get; init; } = true;

        public bool GenerateNonGenericComparable { get; init; } = true;

        public bool GenerateComparisonOperators { get; init; } = false;
    }
}";

        public const string CompareByAttributeSource = @"
using System;
using System.Diagnostics;

namespace Aetos.ComparisonGenerator
{
    [Conditional(""COMPILE_TIME_ONLY"")]
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field)]
    internal sealed class CompareByAttribute :
        Attribute
    {
        public int Order { get; init; } = 0;

        public bool PreferStructuralComparison { get; init; } = false;
    }
}";

        public const string EquatableAttributeName = "Aetos.ComparisonGenerator.EquatableAttribute";
        public const string ComparableAttributeName = "Aetos.ComparisonGenerator.ComparableAttribute";
        public const string CompareByAttributeName = "Aetos.ComparisonGenerator.CompareByAttribute";

        public const string EquatableAttributeNameWithGlobalPrefix = "global::Aetos.ComparisonGenerator.EquatableAttribute";
        public const string ComparableAttributeNameWithGlobalPrefix = "global::Aetos.ComparisonGenerator.ComparableAttribute";
    }
}
