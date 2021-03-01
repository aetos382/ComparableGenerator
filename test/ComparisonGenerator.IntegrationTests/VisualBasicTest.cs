using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;

using NUnit.Framework;

namespace ComparisonGenerator.IntegrationTests
{
    public class VisualBasicTest
    {
        [Test]
        public void VisualBasicはサポートしません()
        {
            const string source = @"";

            var generator = new OrdinalComparableObjectGenerator();
            var driver = CSharpGeneratorDriver.Create(generator);

            var syntaxTree = VisualBasicSyntaxTree.ParseText(source);

            string assemblyName = Guid.NewGuid().ToString("D");
            
            var references = new[] {
                MetadataReference.CreateFromFile(typeof(Type).Assembly.Location)
            };

            var compilation = VisualBasicCompilation.Create(
                assemblyName,
                new[] {
                    syntaxTree
                },
                references);

            driver.RunGeneratorsAndUpdateCompilation(
                compilation,
                out var outputCompilation,
                out var diagnostics);

            foreach (var diag in diagnostics)
            {
                TestContext.WriteLine(diag.ToString());
            }

            Assert.True(diagnostics.Any(x =>
                x.Descriptor == DiagnosticDescriptors.LanguageNotSupported));
        }
    }
}
