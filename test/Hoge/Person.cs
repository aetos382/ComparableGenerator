using Aetos.ComparisonGenerator;

[Comparable]
public partial class Person
{
    public Person(int x)
    {
        this.FirstName = default;
        this.LastName = default;
    }

    [CompareBy(Order = 0)]
    public string FirstName { get; set; }

    [CompareBy(Order = 1)]
    public string LastName { get; set; }
}