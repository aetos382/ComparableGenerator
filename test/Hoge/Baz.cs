using ComparableGenerator;

namespace Hoge
{
    internal partial class Outer
    {
    	[ComparableGenerator.Comparable]
    	public partial class Inner
    	{
        	[CompareBy(0)]
        	public object Foo { get; set; }
        }
    }

    internal partial class Outer
    {
        public partial class Inner
        {
            [CompareBy(1)]
            public object Bar { get; set; }
        }
    }
}
