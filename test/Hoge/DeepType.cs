using Aetos.ComparisonGenerator;

namespace Hoge
{
    public partial class Outer
    {
        [Comparable]
        public partial class Inner
        {
            [CompareBy]
            public int X { get; }
        }
    }
}
