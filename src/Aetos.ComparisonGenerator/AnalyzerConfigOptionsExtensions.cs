using System;

using Microsoft.CodeAnalysis.Diagnostics;

namespace Aetos.ComparisonGenerator
{
    internal static class AnalyzerConfigOptionsExtensions
    {
        public static bool TryGetBooleanOption(
            this AnalyzerConfigOptions options,
            string optionName,
            out bool optionValue)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (optionName is null)
            {
                throw new ArgumentNullException(nameof(optionName));
            }

            optionValue = default;

            if (options.TryGetValue(optionName, out var strValue) &&
                bool.TryParse(strValue, out var boolValue))
            {
                optionValue = boolValue;
                return true;
            }

            return false;
        }
    }
}
