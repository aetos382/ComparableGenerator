using System;

using ComparableGenerator;

namespace Hoge
{
    [Comparable]
    partial class Quux :
        IEquatable<Quux>
    {
        bool IEquatable<Quux>.Equals(
            Quux other)
        {
            return true;
        }

        public bool Equals(
            Quux other)
        {
            return true;
        }

        [CompareBy(0)]
        public int x;
    }
}
