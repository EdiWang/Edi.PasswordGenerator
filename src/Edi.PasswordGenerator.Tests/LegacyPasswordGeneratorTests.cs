#pragma warning disable CS0618

using System.Text.RegularExpressions;
using Xunit;

namespace Edi.PasswordGenerator.Tests;

public class LegacyPasswordGeneratorTests
{
    [Fact]
    public void GeneratePassword_WithNullRule_UsesLegacyDefaultRule()
    {
        var password = new LegacyPasswordGenerator().GeneratePassword();

        AssertPasswordMeetsRequirements(password, 12, 1);
    }

    [Fact]
    public void GeneratePassword_WithCustomRule_UsesProvidedLegacyRule()
    {
        var password = new LegacyPasswordGenerator().GeneratePassword(new PasswordRule(16, 3));

        AssertPasswordMeetsRequirements(password, 16, 3);
    }

    [Fact]
    public void GenerateLegacyPassword_WithDefaultParameters_GeneratesValidPassword()
    {
        var password = LegacyPasswordGenerator.GenerateLegacyPassword();

        AssertPasswordMeetsRequirements(password, 12, 1);
    }

    [Fact]
    public void GenerateLegacyPassword_WithMaximumSpecialCharacterCount_GeneratesValidPassword()
    {
        const int length = 8;
        const int specialCharacters = 5;

        var password = LegacyPasswordGenerator.GenerateLegacyPassword(length, specialCharacters);

        AssertPasswordMeetsRequirements(password, length, specialCharacters);
    }

    [Fact]
    public void GenerateLegacyPassword_WithInvalidLength_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            LegacyPasswordGenerator.GenerateLegacyPassword(7, 1));
    }

    [Fact]
    public void GenerateLegacyPassword_WithNegativeSpecialCharacterCount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            LegacyPasswordGenerator.GenerateLegacyPassword(12, -1));
    }

    [Fact]
    public void GenerateLegacyPassword_WithTooManySpecialCharacters_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            LegacyPasswordGenerator.GenerateLegacyPassword(8, 6));
    }

    private static void AssertPasswordMeetsRequirements(
        string password,
        int expectedLength,
        int minimumSpecialCharacters)
    {
        Assert.Equal(expectedLength, password.Length);
        Assert.Contains(password, char.IsUpper);
        Assert.Contains(password, char.IsLower);
        Assert.Contains(password, char.IsDigit);

        var specialCharacterCount = Regex.Matches(
            password,
            @"[!@#$%^&*()\-_=+\[\]{}|;:,.<>?]").Count;
        Assert.True(specialCharacterCount >= minimumSpecialCharacters);

        const string allowedCharacters =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:,.<>?";
        Assert.All(password, character => Assert.Contains(character, allowedCharacters));
    }
}
