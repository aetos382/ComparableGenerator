using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;

using NUnit.Framework;

namespace ComparableGenerator.UnitTests
{
    [TestFixture]
    public class Hoge
    {
        private static Compilation CreateCompilation(
            string source)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            string assemblyName = Guid.NewGuid().ToString("D");
            
            var references = new[] {
                MetadataReference.CreateFromFile(typeof(Type).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] {
                    syntaxTree
                },
                references);

            return compilation;
        }

        [Test]
        public void VisualBasicはサポートしません()
        {
            const string source = @"";

            var generator = new ComparableGenerator();
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

        [Test]
        public void HogeTest()
        {
            const string source = @"";

            var generator = new ComparableGenerator();
            var driver = CSharpGeneratorDriver.Create(generator);
            var compilation = CreateCompilation(source);

            driver.RunGeneratorsAndUpdateCompilation(
                compilation,
                out var outputCompilation,
                out var diagnostics);

            foreach (var diag in diagnostics)
            {
                TestContext.WriteLine(diag.ToString());
            }
        }

        [Test]
        public void HogeTest2()
        {
            const string source = @"
using ComparableGenerator;

[Comparable]
partial class Hoge
{
    [CompareBy(0)]
    public string FirstName { get; set; }

    [CompareBy(1)]
    public string LastName { get; set; }
}";

            var generator = new ComparableGenerator();
            var driver = CSharpGeneratorDriver.Create(generator);
            var compilation = CreateCompilation(source);

            driver.RunGeneratorsAndUpdateCompilation(
                compilation,
                out var outputCompilation,
                out var diagnostics);

            foreach (var diag in diagnostics)
            {
                TestContext.WriteLine(diag.ToString());
            }

            Assert.IsEmpty(diagnostics);
        }
    }
}
