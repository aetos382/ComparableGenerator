using ComparableGenerator;

namespace Hoge
{
    [ComparableGenerator.Comparable]
    partial class Bar
    {
        [CompareBy(0)]
        public int Foo { get; set; }
    }
}
