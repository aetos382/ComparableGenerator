using Microsoft.CodeAnalysis;

using Aetos.ComparisonGenerator.Properties;

namespace Aetos.ComparisonGenerator
{
    internal static class DiagnosticDescriptors
    {
        private static LocalizableResourceString GetResourceString(
            string name)
        {
            var result = new LocalizableResourceString(
                name,
                Resources.ResourceManager,
                typeof(Resources));

            return result;
        }

        private static DiagnosticDescriptor CreateDiagnosticDescriptor(
            string id,
            string titleResourceName,
            string messageResourceName,
            string category,
            DiagnosticSeverity severity,
            bool isEnabledByDefault = true,
            string? descriptionResourceName = null,
            string? helpLinkUri = null,
            params string[] customTags)
        {
            var description = descriptionResourceName switch {
                null => null,
                var name => GetResourceString(name)
            };

            var descriptor = new DiagnosticDescriptor(
                id,
                GetResourceString(titleResourceName),
                GetResourceString(messageResourceName),
                category,
                severity,
                isEnabledByDefault,
                description,
                helpLinkUri,
                customTags);

            return descriptor;
        }

        private static class DiagnosticCategories
        {
            public const string Design = nameof(Design);
        }

        public static readonly DiagnosticDescriptor TypeIsNotPartial =
            CreateDiagnosticDescriptor(
                "CG0001",
                nameof(Resources.TypeIsNotPartialTitle),
                nameof(Resources.TypeIsNotPartialMessage),
                DiagnosticCategories.Design,
                DiagnosticSeverity.Error);

        public static readonly DiagnosticDescriptor TypeIsStatic =
            CreateDiagnosticDescriptor(
                "CG0002",
                nameof(Resources.TypeIsStaticTitle),
                nameof(Resources.TypeIsNotPartialMessage),
                DiagnosticCategories.Design,
                DiagnosticSeverity.Error);

        public static readonly DiagnosticDescriptor NoMembers =
            CreateDiagnosticDescriptor(
                "CG0003",
                nameof(Resources.NoMembersTitle),
                nameof(Resources.NoMembersMessage),
                DiagnosticCategories.Design,
                DiagnosticSeverity.Warning);

        public static readonly DiagnosticDescriptor MemberTypeIsNotComparable =
            CreateDiagnosticDescriptor(
                "CG0004",
                nameof(Resources.MemberTypeIsNotComparableTitle),
                nameof(Resources.MemberTypeIsNotComparableMessage),
                DiagnosticCategories.Design,
                DiagnosticSeverity.Warning);
    }
}
