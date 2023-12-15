using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edi.PasswordGenerator.Tests;

[TestFixture]
public class Tests
{
    // Create unit test for DefaultPasswordGenerator
    [Test]
    public void TestGeneratePasswordNoRule()
    {
        var generator = new DefaultPasswordGenerator();
        var password = generator.GeneratePassword();

        Assert.That(password, Is.Not.Null);
        Assert.That(password, Is.Not.Empty);

        // Assert if password has both upper and lower case letters
        Assert.That(password.Any(char.IsUpper), Is.True);
        Assert.That(password.Any(char.IsLower), Is.True);

        // Assert if password has at least one digit
        Assert.That(password.Any(char.IsDigit), Is.True);

        // Assert if password has at least one special character
        Assert.That(password.Any(char.IsPunctuation), Is.True);

        Assert.Pass();
    }

    [TestCase(5)]
    [TestCase(10)]
    [TestCase(15)]
    public void TestGeneratePasswordRequiredLength(int length)
    {
        var generator = new DefaultPasswordGenerator();
        var password = generator.GeneratePassword(new(length, 0));

        Assert.That(password, Is.Not.Null);
        Assert.That(password, Is.Not.Empty);

        Assert.That(password.Length, Is.EqualTo(length));

        Assert.Pass();
    }

    [TestCase(2)]
    [TestCase(3)]
    public void TestGeneratePasswordLeastNum(int num)
    {
        var generator = new DefaultPasswordGenerator();
        var password = generator.GeneratePassword(new(10, num));

        Assert.That(password, Is.Not.Null);
        Assert.That(password, Is.Not.Empty);
        Assert.That(password.Length, Is.EqualTo(10));

        char[] extendedPunctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();

        // Assert if password has at least num of character in extendedPunctuations
        Assert.That(password.Count(c => extendedPunctuations.Contains(c)) >= num, Is.True);

        Assert.Pass();
    }

    [TestCase(0)]
    [TestCase(129)]
    public void TestGeneratePasswordInvalidLength(int length)
    {
        var generator = new DefaultPasswordGenerator();

        Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
        {
            var password = generator.GeneratePassword(new(length, 0));
        });
    }

    [TestCase(-1)]
    [TestCase(-10)]
    [TestCase(11)]
    public void TestGeneratePasswordInvalidNumberOfNonAlphanumericCharactersLength(int length)
    {
        var generator = new DefaultPasswordGenerator();

        Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
        {
            var password = generator.GeneratePassword(new(10, length));
        });
    }
}