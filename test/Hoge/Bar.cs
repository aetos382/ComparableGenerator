using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComparableGenerator;

namespace Hoge
{
    [ComparableGenerator.Comparable]
    partial class Bar
    {
        [CompareBy(0)]
        public object Foo { get; set; }
    }
}
