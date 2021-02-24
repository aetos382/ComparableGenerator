using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

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
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            };

            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] {
                    syntaxTree
                },
                references,
                options);

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

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void HogeTest3()
        {
            const string source = @"
using ComparableGenerator;

[Comparable]
public partial class Hoge
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

            using var assemblyStream = new MemoryStream();
            using var debugStream = new MemoryStream();

            var emitResult =
                outputCompilation.Emit(assemblyStream, debugStream);

            foreach (var d in emitResult.Diagnostics)
            {
                TestContext.WriteLine(d);
            }

            if (!emitResult.Success)
            {
                Assert.Fail();
                return;
            }

            assemblyStream.Seek(0, SeekOrigin.Begin);
            debugStream.Seek(0, SeekOrigin.Begin);

            var assemblyLoadContext = new TestAssemblyLoadContext();
            var assembly = assemblyLoadContext.LoadFromStream(
                assemblyStream, debugStream);

            var type = assembly.GetType("Hoge");
            var compareTo = type.GetMethod("CompareTo", new[] { type });

            var instance = Activator.CreateInstance(type);
            var result = (int)compareTo.Invoke(instance, new object[] { instance });

            Assert.AreEqual(0, result);

            assemblyLoadContext.Unload();
        }

        private class TestAssemblyLoadContext :
            AssemblyLoadContext
        {
            public TestAssemblyLoadContext()
                : base(true)
            {
            }
        }
    }
}
