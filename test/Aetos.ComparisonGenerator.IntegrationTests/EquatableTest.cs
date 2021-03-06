﻿using System;

using NUnit.Framework;

using NSubstitute;

using Aetos.ComparisonGenerator.IntegrationTests.Injection;

namespace Aetos.ComparisonGenerator.IntegrationTests
{
    public class EquatableTest :
        GeneratorTestBase
    {
        [Test]
        public void IEquatableT_Equalsが生成される()
        {
            const string source = @"
using Aetos.ComparisonGenerator;

[Comparable]
public partial class Person
{
    [CompareBy(Order = 0)]
    public string FirstName { get; set; }

    [CompareBy(Order = 1)]
    public string LastName { get; set; }
}";

            var options = new GenerateOptions(
                generateEquatable: true);

            var generator = new ComparableObjectGenerator(options);

            var assembly = RunGeneratorAndGenerateAssembly(
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
using Aetos.ComparisonGenerator;

[Comparable]
public partial class Person
{
    [CompareBy(Order = 0)]
    public string FirstName { get; set; }

    [CompareBy(Order = 1)]
    public string LastName { get; set; }
}";

            var options = new GenerateOptions(
                generateEquatable: false);

            var generator = new ComparableObjectGenerator(options);

            var assembly = RunGeneratorAndGenerateAssembly(
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
using Aetos.ComparisonGenerator;

[Comparable]
public partial class Person
{
    [CompareBy(Order = 0)]
    public string FirstName { get; set; }

    [CompareBy(Order = 1)]
    public string LastName { get; set; }
}";

            var options = new GenerateOptions(
                generateEquatable: true,
                generateMethodsAsVirtual: true);

            var generator = new ComparableObjectGenerator(options);

            var assembly = RunGeneratorAndGenerateAssembly(
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
using Aetos.ComparisonGenerator;

[Comparable]
public partial class Person
{
    [CompareBy(Order = 0)]
    public string FirstName { get; set; }

    [CompareBy(Order = 1)]
    public string LastName { get; set; }
}";

            var options = new GenerateOptions(
                generateEquatable: true,
                generateMethodsAsVirtual: false);

            var generator = new ComparableObjectGenerator(options);

            var assembly = RunGeneratorAndGenerateAssembly(
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
        [Ignore("移譲は未実装")]
        public void IEquitableT_EqualsがIComparableT_CompareToに移譲される()
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

    [CompareBy(Order = 0)]
    public string FirstName { get; set; }

    [CompareBy(Order = 1)]
    public string LastName { get; set; }

    int IComparable<Person>.CompareTo(
        Person? other)
    {
        return this._hooks.CompareHook(this, other);
    }
}";

            var options = new GenerateOptions(
                generateEquatable: true);

            var generator = new ComparableObjectGenerator(options);

            var assembly = RunGeneratorAndGenerateAssembly(
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
        [Ignore("移譲は未実装")]
        public void IEquitableT_EqualsがIComparable_CompareToに移譲される()
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

    [CompareBy(Order = 0)]
    public string FirstName { get; set; }

    [CompareBy(Order = 1)]
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

            var assembly = RunGeneratorAndGenerateAssembly(
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
