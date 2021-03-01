using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ComparisonGenerator.IntegrationTests
{
    internal class TestAnalyzerConfigOptionsProvider :
        AnalyzerConfigOptionsProvider
    {
        public TestAnalyzerConfigOptionsProvider()
        {
            this.GlobalOptions = TestAnalyzerConfigOptions.Empty;
        }

        public TestAnalyzerConfigOptionsProvider(
            AnalyzerConfigOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.GlobalOptions = options;
        }

        public TestAnalyzerConfigOptionsProvider(
            IDictionary<string, string> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.GlobalOptions = new TestAnalyzerConfigOptions(options);
        }

        public override AnalyzerConfigOptions GetOptions(
            SyntaxTree tree)
        {
            return TestAnalyzerConfigOptions.Empty;
        }

        public override AnalyzerConfigOptions GetOptions(
            AdditionalText textFile)
        {
            return TestAnalyzerConfigOptions.Empty;
        }

        public override AnalyzerConfigOptions GlobalOptions { get; }
    }
}
