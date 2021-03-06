using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace Aetos.ComparisonGenerator
{
    internal static class GeneratorExecutionContextExtensions
    {
        public static void ReportDiagnostics(
            this GeneratorExecutionContext context,
            IEnumerable<Diagnostic> diagnostics)
        {
            if (diagnostics is null)
            {
                throw new ArgumentNullException(nameof(diagnostics));
            }

            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
