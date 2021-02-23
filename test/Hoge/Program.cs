using System;

namespace Hoge
{
    internal static class Program
    {
        internal static void Main()
        {
            var people = new Person[]
            {
                new("Yamada", "Taro"),
                new("Suzuki", "Ichiro"),
                new("Suzuki", "Jiro")
            };

            foreach (var person in people)
            {
                Console.WriteLine($"{person.FirstName} {person.LastName}");
            }
        }
    }
}
