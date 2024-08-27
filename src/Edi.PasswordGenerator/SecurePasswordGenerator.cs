using System.Security.Cryptography;

namespace Edi.PasswordGenerator;

public class SecurePasswordGenerator
{
    private static readonly char[] UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private static readonly char[] LowercaseLetters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    private static readonly char[] Digits = "0123456789".ToCharArray();
    private static readonly char[] SpecialCharacters = "!@#$%^&*()-_=+[]{}|;:,.<>?".ToCharArray();

    public static string GenerateSecurePassword(int length = 12)
    {
        if (length < 8)
        {
            throw new ArgumentException("Password length must be at least 8 characters.");
        }

        var charCategories = new[]
        {
            UppercaseLetters,
            LowercaseLetters,
            Digits,
            SpecialCharacters
        };

        var passwordChars = new char[length];

        // Ensure the password contains at least one character from each category
        for (int i = 0; i < charCategories.Length; i++)
        {
            passwordChars[i] = GetRandomCharFromCategory(charCategories[i]);
        }

        // Fill the remaining characters with random characters from all categories
        for (int i = charCategories.Length; i < length; i++)
        {
            var randomCategory = charCategories[GetRandomNumber(0, charCategories.Length)];
            passwordChars[i] = GetRandomCharFromCategory(randomCategory);
        }

        // Shuffle the characters to ensure randomness
        passwordChars = passwordChars.OrderBy(c => GetRandomNumber(0, length)).ToArray();

        return new string(passwordChars);
    }

    private static char GetRandomCharFromCategory(char[] category)
    {
        return category[GetRandomNumber(0, category.Length)];
    }

    private static int GetRandomNumber(int minValue, int maxValue)
    {
        byte[] randomNumber = new byte[4];
        RandomNumberGenerator.Fill(randomNumber);
        int value = BitConverter.ToInt32(randomNumber, 0);
        return Math.Abs(value % (maxValue - minValue)) + minValue;
    }
}