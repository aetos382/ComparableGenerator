using ComparableGenerator;

namespace Hoge
{
    [ComparableGenerator.Comparable(
        GenerateObjectEquals = false)]
    partial class Bar
    {
        [CompareBy(0)]
        public int Foo { get; set; }
    }
}
