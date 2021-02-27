using ComparableGenerator;

[Comparable]
public partial class Person
{
    [CompareBy(0)]
    public string FirstName { get; set; }

    [CompareBy(1)]
    public string LastName { get; set; }
}