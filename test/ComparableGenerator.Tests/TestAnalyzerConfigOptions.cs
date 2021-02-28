using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

namespace ComparableGenerator.Tests
{
    internal class TestAnalyzerConfigOptions :
        AnalyzerConfigOptions
    {
        private readonly Dictionary<string, string> _options;

        private TestAnalyzerConfigOptions()
            : this(new Dictionary<string, string>())
        {
        }

        public TestAnalyzerConfigOptions(
            IDictionary<string, string> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this._options = new(options, KeyComparer);
        }

        public override bool TryGetValue(
            string key,
            [MaybeNullWhen(false)]
            out string value)
        {
            return this._options.TryGetValue(key, out value);
        }

        public static readonly TestAnalyzerConfigOptions Empty = new();
    }
}
