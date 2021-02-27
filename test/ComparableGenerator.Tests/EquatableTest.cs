using System;

using NUnit.Framework;

using NSubstitute;

using ComparableGenerator.Tests.Injection;

namespace ComparableGenerator.Tests
{
    public class EquatableTest :
        GeneratorTestBase
    {
        [Test]
        public void IEquatableT_Equalsが生成される()
        {
            const string source = @"
using ComparableGenerator;

[Comparable]
public partial class Person
{
    [CompareBy(0)]
    public string FirstName { get; set; }

    [CompareBy(1)]
    public string LastName { get; set; }
}";

            var options = new GenerateOptions(
                generateEquatable: true);

            var generator = new ComparableGenerator(options);

            var assembly = GenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Person");

            Assert.That(type, Is.Not.Null);

            var equals = GetMethod(
                type, "Equals", type);

            Assert.That(equals, Is.Not.Null);
        }

        [Test]
        public void IEquatableT_Equalsが生成さない()
        {
            const string source = @"
using ComparableGenerator;

[Comparable]
public partial class Person
{
    [CompareBy(0)]
    public string FirstName { get; set; }

    [CompareBy(1)]
    public string LastName { get; set; }
}";

            var options = new GenerateOptions(
                generateEquatable: false);

            var generator = new ComparableGenerator(options);

            var assembly = GenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Person");

            Assert.That(type, Is.Not.Null);

            var equals = GetMethod(
                type, "Equals", type);

            Assert.That(equals, Is.Null);
        }

        [Test]
        public void IEquatableT_Equalsがvirtualで生成される()
        {
            const string source = @"
using ComparableGenerator;

[Comparable]
public partial class Person
{
    [CompareBy(0)]
    public string FirstName { get; set; }

    [CompareBy(1)]
    public string LastName { get; set; }
}";

            var options = new GenerateOptions(
                generateEquatable: true,
                generateMethodsAsVirtual: true);

            var generator = new ComparableGenerator(options);

            var assembly = GenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Person");

            Assert.That(type, Is.Not.Null);

            var equals = GetMethod(
                type, "Equals", type);

            Assert.That(equals, Is.Not.Null);
            Assert.That(equals.IsFinal, Is.False);
        }

        [Test]
        public void IEquatableT_Equalsが非virtualで生成される()
        {
            const string source = @"
using ComparableGenerator;

[Comparable]
public partial class Person
{
    [CompareBy(0)]
    public string FirstName { get; set; }

    [CompareBy(1)]
    public string LastName { get; set; }
}";

            var options = new GenerateOptions(
                generateEquatable: true,
                generateMethodsAsVirtual: false);

            var generator = new ComparableGenerator(options);

            var assembly = GenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Person");

            Assert.That(type, Is.Not.Null);

            var equals = GetMethod(
                type, "Equals", type);

            Assert.That(equals, Is.Not.Null);

            // インターフェイスの実装であるメソッドは明示的に virtual と書かなくても IsVirtual は True になる。
            // 明示的に virtual と書いていない場合、IsFinal が True になり、オーバーライドできない。
            Assert.That(equals.IsFinal, Is.True);
        }

        [Test]
        public void IEquitableT_EqualsがObject_Equalsに移譲される()
        {
            const string source = @"
using ComparableGenerator;
using ComparableGenerator.Tests.Injection;

[Comparable]
public partial class Person
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

    public override bool Equals(
        object? other)
    {
        return this._hooks.EqualsHook(this, other);
    }
}";

            var options = new GenerateOptions();

            var generator = new ComparableGenerator(options);

            var assembly = GenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Person");

            Assert.That(type, Is.Not.Null);

            var equals = GetMethod(type, "Equals", type);
            Assert.That(equals, Is.Not.Null);

            var hook = Substitute.For<ITestHooks>();
            var hook2 = new TestHooks(hook);

            var instance = Activator.CreateInstance(type, hook2);

            hook
                .EqualsHook(Arg.Any<object>(), Arg.Any<object>())
                .Returns(true);

            var result = equals.Invoke(instance, new[] { instance });
            Assert.That(result, Is.True);

            hook.Received()
                .EqualsHook(Arg.Any<object>(), Arg.Any<object>());
        }

        [Test]
        public void IEquitableT_EqualsがIComparableT_CompareToに移譲される()
        {
            const string source = @"
using System;

using ComparableGenerator;
using ComparableGenerator.Tests.Injection;

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
                generateEquatable: true);

            var generator = new ComparableGenerator(options);

            var assembly = GenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Person");

            Assert.That(type, Is.Not.Null);

            var equals = GetMethod(type, "Equals", type);
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
        public void IEquitableT_EqualsがIComparable_CompareToに移譲される()
        {
            const string source = @"
using System;

using ComparableGenerator;
using ComparableGenerator.Tests.Injection;

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

            var generator = new ComparableGenerator(options);

            var assembly = GenerateAssembly(
                generator,
                source,
                out _);

            Assert.That(assembly, Is.Not.Null);

            var type = assembly.GetType("Person");

            Assert.That(type, Is.Not.Null);

            var equals = GetMethod(type, "Equals", type);
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
