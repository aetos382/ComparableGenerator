using System;

using ComparableGenerator;

namespace Hoge
{
    [Comparable(
        GenerateEquatable = true)]
    public partial class Person
    {
        public Person(
            string firstName,
            string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        [CompareBy(0)]
        public string FirstName { get; init; }

        [CompareBy(1)]
        public string LastName { get; init; }
    }
}
