using NUnit.Framework;

namespace Aetos.ComparisonGenerator.IntegrationTests
{
    public class DeepHierarchyTest :
        GeneratorTestBase
    {
        [Test]
        public void 深い階層の生成テスト()
        {
            const string source = @"
using Aetos.ComparisonGenerator;

namespace Aetos.ComparisonGenerator
{
    namespace IntegrationTest
    {
        public partial class Outer
        {
            [Comparable]
            public partial class Inner
            {
                [CompareBy]
                public int Value { get; set; }
            }
        }
    }
}";

            var generator = new ComparableObjectGenerator();

            var assembly = RunGeneratorAndGenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Aetos.ComparisonGenerator.IntegrationTest.Outer+Inner");

            Assert.That(type, Is.Not.Null);
        }
    }
}
