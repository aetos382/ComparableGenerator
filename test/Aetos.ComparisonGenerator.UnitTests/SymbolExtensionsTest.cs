using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace Aetos.ComparisonGenerator.UnitTests
{
    public class SymbolExtensionsTest
    {
#pragma warning disable CA1034, CA1051, CA1815

        public struct TestType
        {
            public TestType[] Hoge;
        }

#pragma warning restore CA1034, CA1051, CA1815

        private static INamedTypeSymbol GetTestType()
        {
            var testType = typeof(TestType);

            var references = new[] {
                MetadataReference.CreateFromFile(testType.Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(null, references: references);

            var typeSymbol = compilation.GetTypeByMetadataName(testType.FullName!);

            return typeSymbol!;
        }

        [Test]
        public void 型の完全修飾名が取れる()
        {
            var typeSymbol = GetTestType();

            var typeName = typeSymbol.GetFullName();

            Assert.That(typeName, Is.EqualTo("Aetos.ComparisonGenerator.UnitTests.SymbolExtensionsTest.TestType"));
        }

        [Test]
        public void 型の完全修飾名_global名前空間付き_が取れる()
        {
            var typeSymbol = GetTestType();

            var typeName = typeSymbol.GetFullName(true);

            Assert.That(typeName, Is.EqualTo("global::Aetos.ComparisonGenerator.UnitTests.SymbolExtensionsTest.TestType"));
        }
    }
}
