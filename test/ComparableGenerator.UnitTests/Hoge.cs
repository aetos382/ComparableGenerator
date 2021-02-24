using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace ComparableGenerator.UnitTests
{
    [TestFixture]
    public class Hoge
    {
        [Test]
        public void HogeTest()
        {
            const string source = @"";

            var generator = new ComparableGenerator();
            var driver = CSharpGeneratorDriver.Create(generator);

            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            string assemblyName = Guid.NewGuid().ToString("D");

            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] {
                    syntaxTree
                });

            driver.RunGeneratorsAndUpdateCompilation(
                compilation,
                out var outputCompilation,
                out var diagnostics);
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

            driver.RunGeneratorsAndUpdateCompilation(
                compilation,
                out var outputCompilation,
                out var diagnostics);

            Assert.IsEmpty(diagnostics);
        }
    }
}
