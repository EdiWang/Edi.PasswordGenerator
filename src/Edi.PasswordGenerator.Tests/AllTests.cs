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

        Assert.IsNotNull(password);
        Assert.IsNotEmpty(password);

        // Assert if password has both upper and lower case letters
        Assert.IsTrue(password.Any(char.IsUpper));
        Assert.IsTrue(password.Any(char.IsLower));

        // Assert if password has at least one digit
        Assert.IsTrue(password.Any(char.IsDigit));

        // Assert if password has at least one special character
        Assert.IsTrue(password.Any(char.IsPunctuation));

        Assert.Pass();
    }

    [TestCase(5)]
    [TestCase(10)]
    [TestCase(15)]
    public void TestGeneratePasswordRequiredLength(int length)
    {
        var generator = new DefaultPasswordGenerator();
        var password = generator.GeneratePassword(new(length, 0));

        Assert.IsNotNull(password);
        Assert.IsNotEmpty(password);

        Assert.That(password.Length, Is.EqualTo(length));

        Assert.Pass();
    }

    [TestCase(2)]
    [TestCase(3)]
    public void TestGeneratePasswordLeastNum(int num)
    {
        var generator = new DefaultPasswordGenerator();
        var password = generator.GeneratePassword(new(10, num));

        Assert.IsNotNull(password);
        Assert.IsNotEmpty(password);
        Assert.That(password.Length, Is.EqualTo(10));

        char[] extendedPunctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();

        // Assert if password has at least num of character in extendedPunctuations
        Assert.IsTrue(password.Count(c => extendedPunctuations.Contains(c)) >= num);

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