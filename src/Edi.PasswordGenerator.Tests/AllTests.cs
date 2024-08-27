using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Edi.PasswordGenerator.Tests;

[TestClass]
public class Tests
{
    [TestMethod]
    public void TestGeneratePasswordNoRule()
    {
        var generator = new DefaultPasswordGenerator();
        var password = generator.GeneratePassword();

        Assert.IsNotNull(password);
        Assert.IsFalse(string.IsNullOrWhiteSpace(password));

        // Assert if password has both upper and lower case letters
        Assert.IsTrue(password.Any(char.IsUpper));
        Assert.IsTrue(password.Any(char.IsLower));

        // Assert if password has at least one digit
        Assert.IsTrue(password.Any(char.IsDigit));

        // Assert if password has at least one special character
        Assert.IsTrue(password.Any(c => "!@#$%^&*()-_=+[]{}|;:,.<>?".ToCharArray().Contains(c)));
    }

    [TestMethod]
    [DataRow(5)]
    [DataRow(10)]
    [DataRow(15)]
    public void TestGeneratePasswordRequiredLength(int length)
    {
        var generator = new DefaultPasswordGenerator();
        var password = generator.GeneratePassword(new(length, 0));

        Assert.IsNotNull(password);
        Assert.IsFalse(string.IsNullOrWhiteSpace(password));

        Assert.AreEqual(password.Length, length);
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(3)]
    public void TestGeneratePasswordLeastNum(int num)
    {
        var generator = new DefaultPasswordGenerator();
        var password = generator.GeneratePassword(new(10, num));

        Assert.IsNotNull(password);
        Assert.IsFalse(string.IsNullOrWhiteSpace(password));
        Assert.AreEqual(10, password.Length);

        char[] extendedPunctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();

        // Assert if password has at least num of character in extendedPunctuations
        Assert.IsTrue(password.Count(c => extendedPunctuations.Contains(c)) >= num);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(129)]
    public void TestGeneratePasswordInvalidLength(int length)
    {
        var generator = new DefaultPasswordGenerator();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            var password = generator.GeneratePassword(new(length, 0));
        });
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(-10)]
    [DataRow(11)]
    public void TestGeneratePasswordInvalidNumberOfNonAlphanumericCharactersLength(int length)
    {
        var generator = new DefaultPasswordGenerator();

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            var password = generator.GeneratePassword(new(10, length));
        });
    }
}