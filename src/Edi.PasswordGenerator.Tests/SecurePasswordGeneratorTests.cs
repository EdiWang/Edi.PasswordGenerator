using System.Text.RegularExpressions;
using Xunit;

namespace Edi.PasswordGenerator.Tests;

public class SecurePasswordGeneratorTests : IDisposable
{
    private readonly SecurePasswordGenerator _generator;

    public SecurePasswordGeneratorTests()
    {
        _generator = new SecurePasswordGenerator();
    }

    public void Dispose()
    {
        // Cleanup if needed
    }

    #region GeneratePassword Method Tests

    [Fact]
    public void GeneratePassword_WithNullRule_ShouldUseDefaultRule()
    {
        // Act
        var password = _generator.GeneratePassword(null);

        // Assert
        Assert.NotNull(password);
        Assert.Equal(12, password.Length);
        AssertPasswordMeetsRequirements(password, 12, 1);
    }

    [Fact]
    public void GeneratePassword_WithCustomRule_ShouldUseProvidedRule()
    {
        // Arrange
        var rule = new PasswordRule(16, 3);

        // Act
        var password = _generator.GeneratePassword(rule);

        // Assert
        Assert.NotNull(password);
        Assert.Equal(16, password.Length);
        AssertPasswordMeetsRequirements(password, 16, 3);
    }

    [Fact]
    public void GeneratePassword_WithMinimumValidRule_ShouldGenerateValidPassword()
    {
        // Arrange
        var rule = new PasswordRule(8, 0);

        // Act
        var password = _generator.GeneratePassword(rule);

        // Assert
        Assert.NotNull(password);
        Assert.Equal(8, password.Length);
        AssertPasswordMeetsRequirements(password, 8, 0);
    }

    #endregion

    #region GenerateSecurePassword Static Method Tests

    [Fact]
    public void GenerateSecurePassword_WithDefaultParameters_ShouldGenerateValidPassword()
    {
        // Act
        var password = SecurePasswordGenerator.GenerateSecurePassword();

        // Assert
        Assert.NotNull(password);
        Assert.Equal(12, password.Length);
        AssertPasswordMeetsRequirements(password, 12, 1);
    }

    [Fact]
    public void GenerateSecurePassword_WithCustomLength_ShouldGeneratePasswordWithCorrectLength()
    {
        // Arrange
        const int expectedLength = 20;

        // Act
        var password = SecurePasswordGenerator.GenerateSecurePassword(expectedLength);

        // Assert
        Assert.NotNull(password);
        Assert.Equal(expectedLength, password.Length);
        AssertPasswordMeetsRequirements(password, expectedLength, 1);
    }

    [Fact]
    public void GenerateSecurePassword_WithMultipleSpecialChars_ShouldIncludeRequiredSpecialChars()
    {
        // Arrange
        const int length = 15;
        const int minSpecialChars = 5;

        // Act
        var password = SecurePasswordGenerator.GenerateSecurePassword(length, minSpecialChars);

        // Assert
        Assert.NotNull(password);
        Assert.Equal(length, password.Length);
        AssertPasswordMeetsRequirements(password, length, minSpecialChars);
    }

    [Fact]
    public void GenerateSecurePassword_WithZeroSpecialChars_ShouldGenerateValidPassword()
    {
        // Arrange
        const int length = 10;
        const int minSpecialChars = 0;

        // Act
        var password = SecurePasswordGenerator.GenerateSecurePassword(length, minSpecialChars);

        // Assert
        Assert.NotNull(password);
        Assert.Equal(length, password.Length);
        AssertPasswordMeetsRequirements(password, length, minSpecialChars);
    }

    [Fact]
    public void GenerateSecurePassword_WithMinimumLength_ShouldGenerateValidPassword()
    {
        // Arrange
        const int minLength = 8;

        // Act
        var password = SecurePasswordGenerator.GenerateSecurePassword(minLength, 1);

        // Assert
        Assert.NotNull(password);
        Assert.Equal(minLength, password.Length);
        AssertPasswordMeetsRequirements(password, minLength, 1);
    }

    [Fact]
    public void GenerateSecurePassword_MultipleGenerations_ShouldProduceDifferentPasswords()
    {
        // Arrange
        var passwords = new HashSet<string>();
        const int iterations = 100;

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var password = SecurePasswordGenerator.GenerateSecurePassword(12, 1);
            passwords.Add(password);
        }

        // Assert - Should have close to 100 unique passwords (allowing for minimal collision chance)
        Assert.True(passwords.Count > 95, $"Expected > 95 unique passwords, got {passwords.Count}");
    }

    #endregion

    #region Exception Tests

    [Fact]
    public void GenerateSecurePassword_WithLengthLessThanMinimum_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => SecurePasswordGenerator.GenerateSecurePassword(7, 1));
    }

    [Fact]
    public void GenerateSecurePassword_WithNegativeSpecialChars_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => SecurePasswordGenerator.GenerateSecurePassword(12, -1));
    }

    [Fact]
    public void GenerateSecurePassword_WithTooManySpecialChars_ShouldThrowException()
    {
        // Act & Assert - length 8 with minSpecialChars 6 leaves only 2 positions for required uppercase, lowercase, digit
        Assert.Throws<ArgumentOutOfRangeException>(() => SecurePasswordGenerator.GenerateSecurePassword(8, 6));
    }

    [Fact]
    public void GenerateSecurePassword_WithMaxAllowedSpecialChars_ShouldGenerateValidPassword()
    {
        // Arrange - length 8 allows maximum 5 special chars (8 - 3 required categories)
        const int length = 8;
        const int maxSpecialChars = 5;

        // Act
        var password = SecurePasswordGenerator.GenerateSecurePassword(length, maxSpecialChars);

        // Assert
        Assert.NotNull(password);
        Assert.Equal(length, password.Length);
        AssertPasswordMeetsRequirements(password, length, maxSpecialChars);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void GeneratePassword_WithLargeLength_ShouldGenerateValidPassword()
    {
        // Arrange
        var rule = new PasswordRule(100, 10);

        // Act
        var password = _generator.GeneratePassword(rule);

        // Assert
        Assert.NotNull(password);
        Assert.Equal(100, password.Length);
        AssertPasswordMeetsRequirements(password, 100, 10);
    }

    [Fact]
    public void GenerateSecurePassword_ConsistentBehavior_ShouldAlwaysMeetRequirements()
    {
        // Arrange & Act - Test multiple combinations to ensure consistency
        var testCases = new[]
        {
            new { Length = 8, SpecialChars = 0 },
            new { Length = 8, SpecialChars = 1 },
            new { Length = 12, SpecialChars = 2 },
            new { Length = 16, SpecialChars = 4 },
            new { Length = 20, SpecialChars = 8 },
            new { Length = 50, SpecialChars = 15 }
        };

        foreach (var testCase in testCases)
        {
            // Act
            var password = SecurePasswordGenerator.GenerateSecurePassword(testCase.Length, testCase.SpecialChars);

            // Assert
            Assert.NotNull(password);
            Assert.Equal(testCase.Length, password.Length);
            AssertPasswordMeetsRequirements(password, testCase.Length, testCase.SpecialChars);
        }
    }

    #endregion

    #region Helper Methods

    private static void AssertPasswordMeetsRequirements(string password, int expectedLength, int minSpecialChars)
    {
        Assert.NotNull(password);
        Assert.Equal(expectedLength, password.Length);

        // Check for required character categories
        var hasUppercase = password.Any(c => char.IsUpper(c) && char.IsLetter(c));
        var hasLowercase = password.Any(c => char.IsLower(c) && char.IsLetter(c));
        var hasDigit = password.Any(c => char.IsDigit(c));

        Assert.True(hasUppercase, "Password must contain at least one uppercase letter");
        Assert.True(hasLowercase, "Password must contain at least one lowercase letter");
        Assert.True(hasDigit, "Password must contain at least one digit");

        // Check special characters count
        var specialCharsPattern = @"[!@#$%^&*()\-_=+\[\]{}|;:,.<>?]";
        var specialCharCount = Regex.Matches(password, specialCharsPattern).Count;
        Assert.True(specialCharCount >= minSpecialChars,
            $"Password must contain at least {minSpecialChars} special characters, found {specialCharCount}");

        // Verify all characters are from allowed sets
        var allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:,.<>?";
        Assert.True(password.All(c => allowedChars.Contains(c)),
            "Password contains invalid characters");
    }

    #endregion
}