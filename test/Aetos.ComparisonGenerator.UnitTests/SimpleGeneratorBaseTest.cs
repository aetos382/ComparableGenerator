using System;

using NUnit.Framework;

namespace Aetos.ComparisonGenerator.UnitTests
{
    public class LightweightGeneratorBaseTest
    {
        private static readonly string newLine = Environment.NewLine;

        [Test]
        public void 初期状態では空()
        {
            var generator = new SimpleGeneratorBase();

            string result = generator.TransformText();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void 出力した文字列が格納される()
        {
            var generator = new SimpleGeneratorBase();

            generator.Write($"aaa{newLine}bbb{newLine}");

            string result = generator.TransformText();

            Assert.That(result, Is.EqualTo($"aaa{newLine}bbb{newLine}"));
        }

        [Test]
        public void インデントを設定している場合は各行の前に出力される()
        {
            var generator = new SimpleGeneratorBase();
            generator.PushIndent("    ");

            generator.Write($"aaa{newLine}bbb{newLine}");

            string result = generator.TransformText();

            Assert.That(result, Is.EqualTo($"    aaa{newLine}    bbb{newLine}"));
        }

        [Test]
        public void 複数回出力する()
        {
            var generator = new SimpleGeneratorBase();
            generator.PushIndent("    ");

            generator.Write($"aaa{newLine}");
            generator.Write($"bbb{newLine}");

            string result = generator.TransformText();

            Assert.That(result, Is.EqualTo($"    aaa{newLine}    bbb{newLine}"));
        }
    }
}
