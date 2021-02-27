using Microsoft.CodeAnalysis;

namespace ComparableGenerator
{
    internal static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor LanguageNotSupported =
            new DiagnosticDescriptor(
                nameof(LanguageNotSupported),
                "LanguageNotSupported",
                "{0} is not supported.",
                "Hoge",
                DiagnosticSeverity.Error,
                true);

        public static readonly DiagnosticDescriptor TypeIsNotPartial =
            new DiagnosticDescriptor(
                nameof(TypeIsNotPartial),
                "TypeIsNotPartial",
                "{0} is not partial type.",
                "Hoge",
                DiagnosticSeverity.Error,
                true);

        public static readonly DiagnosticDescriptor TypeIsStatic =
            new DiagnosticDescriptor(
                nameof(TypeIsStatic),
                "TypeIsStatic",
                "TypeIsStatic",
                "Hoge",
                DiagnosticSeverity.Warning,
                true);

        public static readonly DiagnosticDescriptor NoMembers =
            new DiagnosticDescriptor(
                nameof(NoMembers),
                "NoMembers",
                "NoMembers",
                "Hoge",
                DiagnosticSeverity.Warning,
                true);

        public static readonly DiagnosticDescriptor UnsupportedSyntax =
            new DiagnosticDescriptor(
                nameof(UnsupportedSyntax),
                "UnsupportedSyntax",
                "UnsupportedSyntax",
                "Hoge",
                DiagnosticSeverity.Warning,
                true);

        public static readonly DiagnosticDescriptor TypeIsNotComparable =
            new DiagnosticDescriptor(
                nameof(TypeIsNotComparable),
                "TypeIsNotComparable",
                "{0}.{1}: {2} is not comparable.",
                "Hoge",
                DiagnosticSeverity.Error,
                true);
    }
}
