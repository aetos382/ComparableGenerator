using Aetos.ComparisonGenerator;

namespace Hoge
{
    [Comparable]
    public partial class Struct
    {
        [CompareBy]
        public int X { get; }
    }
}
