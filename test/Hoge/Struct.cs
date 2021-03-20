using Aetos.ComparisonGenerator;

namespace Hoge
{
    [Comparable]
    public partial struct Struct
    {
        [CompareBy]
        public int X { get; }
    }
}
