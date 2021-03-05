using System;

using NSubstitute;

using NUnit.Framework;

using Aetos.ComparisonGenerator.IntegrationTests.Injection;

namespace Aetos.ComparisonGenerator.IntegrationTests
{
    public class ObjectEqualsTest :
        GeneratorTestBase
    {
        [Test]
        public void Object_Equalsが生成される()
        {
            const string source = @"
using Aetos.ComparisonGenerator;

[Comparable]
public partial class Person
{
    [CompareBy(0)]
    public string FirstName { get; set; }

    [CompareBy(1)]
    public string LastName { get; set; }
}";

            var options = new GenerateOptions(
                generateObjectEquals: true);

            var generator = new ComparableObjectGenerator(options);

            var assembly = GenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Person");

            Assert.That(type, Is.Not.Null);

            var equals = GetMethod(
                type, "Equals", typeof(object));

            Assert.That(equals, Is.Not.Null);
        }

        [Test]
        public void Object_Equalsが生成さない()
        {
            const string source = @"
using Aetos.ComparisonGenerator;

[Comparable]
public partial class Person
{
    [CompareBy(0)]
    public string FirstName { get; set; }

    [CompareBy(1)]
    public string LastName { get; set; }
}";

            var options = new GenerateOptions(
                generateObjectEquals: false);

            var generator = new ComparableObjectGenerator(options);

            var assembly = GenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Person");

            Assert.That(type, Is.Not.Null);

            var equals = GetMethod(
                type, "Equals", typeof(object));

            Assert.That(equals, Is.Null);
        }

        [Test]
        public void Object_EqualsがIEquitableT_Equalsに移譲される()
        {
            const string source = @"
using System;

using Aetos.ComparisonGenerator;
using Aetos.ComparisonGenerator.IntegrationTests.Injection;

[Comparable]
public partial class Person :
    IEquatable<Person>
{
    private readonly ITestHooks _hooks;

    public Person(
        ITestHooks hooks)
    {
        this._hooks = hooks;
    }

    [CompareBy(0)]
    public string FirstName { get; set; }

    [CompareBy(1)]
    public string LastName { get; set; }

    bool IEquatable<Person>.Equals(
        Person? other)
    {
        return this._hooks.EqualsHook(this, other);
    }
}";

            var options = new GenerateOptions(
                generateObjectEquals: true,
                generateEquatable: false);

            var generator = new ComparableObjectGenerator(options);

            var assembly = GenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Person");

            Assert.That(type, Is.Not.Null);

            var equals = GetMethod(type, "Equals", typeof(object));
            Assert.That(equals, Is.Not.Null);

            var hook = Substitute.For<ITestHooks>();
            var hook2 = new TestHooks(hook);

            var instance = Activator.CreateInstance(type, hook2);

            hook
                .EqualsHook(Arg.Any<object>(), Arg.Any<object>())
                .Returns(true);

            var result = equals.Invoke(instance, new[] { instance });
            Assert.That(result, Is.True);

            hook.Received().EqualsHook(Arg.Any<object>(), Arg.Any<object>());
        }

        [Test]
        public void Object_EqualsがIComparableT_CompareToに移譲される()
        {
            const string source = @"
using System;

using Aetos.ComparisonGenerator;
using Aetos.ComparisonGenerator.IntegrationTests.Injection;

[Comparable]
public partial class Person :
    IComparable<Person>
{
    private readonly ITestHooks _hooks;

    public Person(
        ITestHooks hooks)
    {
        this._hooks = hooks;
    }

    [CompareBy(0)]
    public string FirstName { get; set; }

    [CompareBy(1)]
    public string LastName { get; set; }

    int IComparable<Person>.CompareTo(
        Person? other)
    {
        return this._hooks.CompareHook(this, other);
    }
}";

            var options = new GenerateOptions(
                generateObjectEquals: true);

            var generator = new ComparableObjectGenerator(options);

            var assembly = GenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Person");

            Assert.That(type, Is.Not.Null);

            var equals = GetMethod(type, "Equals", typeof(object));
            Assert.That(equals, Is.Not.Null);

            var hook = Substitute.For<ITestHooks>();
            var hook2 = new TestHooks(hook);

            var instance = Activator.CreateInstance(type, hook2);

            hook
                .CompareHook(Arg.Any<object>(), Arg.Any<object>())
                .Returns(0);

            var result = equals.Invoke(instance, new[] { instance });
            Assert.That(result, Is.True);

            hook.Received()
                .CompareHook(Arg.Any<object>(), Arg.Any<object>());
        }


        [Test]
        public void Object_EqualsがIComparable_CompareToに移譲される()
        {
            const string source = @"
using System;

using Aetos.ComparisonGenerator;
using Aetos.ComparisonGenerator.IntegrationTests.Injection;

[Comparable]
public partial class Person :
    IComparable
{
    private readonly ITestHooks _hooks;

    public Person(
        ITestHooks hooks)
    {
        this._hooks = hooks;
    }

    [CompareBy(0)]
    public string FirstName { get; set; }

    [CompareBy(1)]
    public string LastName { get; set; }

    int IComparable.CompareTo(
        object? other)
    {
        return this._hooks.CompareHook(this, other);
    }
}";

            var options = new GenerateOptions(
                generateEquatable: true,
                generateGenericComparable: false);

            var generator = new ComparableObjectGenerator(options);

            var assembly = GenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Person");

            Assert.That(type, Is.Not.Null);

            var equals = GetMethod(type, "Equals", typeof(object));
            Assert.That(equals, Is.Not.Null);

            var hook = Substitute.For<ITestHooks>();
            var hook2 = new TestHooks(hook);

            var instance = Activator.CreateInstance(type, hook2);

            hook
                .CompareHook(Arg.Any<object>(), Arg.Any<object>())
                .Returns(0);

            var result = equals.Invoke(instance, new[] { instance });
            Assert.That(result, Is.True);

            hook.Received()
                .CompareHook(Arg.Any<object>(), Arg.Any<object>());
        }
    }
}
