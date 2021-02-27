using NUnit.Framework;

namespace ComparableGenerator.UnitTests
{
    public class LightweightGeneratorBaseTest
    {
        [Test]
        public void 初期状態では空()
        {
            var generator = new LightweightGeneratorBase();

            string result = generator.TransformText();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void 出力した文字列が格納される()
        {
            var generator = new LightweightGeneratorBase();

            generator.Write("aaa\r\nbbb\r\n");

            string result = generator.TransformText();

            Assert.That(result, Is.EqualTo("aaa\r\nbbb\r\n"));
        }

        [Test]
        public void インデントを設定している場合は各行の前に出力される()
        {
            var generator = new LightweightGeneratorBase();
            generator.PushIndent("    ");

            generator.Write("aaa\r\nbbb\r\n");

            string result = generator.TransformText();

            Assert.That(result, Is.EqualTo("    aaa\r\n    bbb\r\n"));
        }

        [Test]
        public void 複数回出力する()
        {
            var generator = new LightweightGeneratorBase();
            generator.PushIndent("    ");

            generator.Write("aaa\r\n");
            generator.Write("bbb\r\n");

            string result = generator.TransformText();

            Assert.That(result, Is.EqualTo("    aaa\r\n    bbb\r\n"));
        }
    }
}
