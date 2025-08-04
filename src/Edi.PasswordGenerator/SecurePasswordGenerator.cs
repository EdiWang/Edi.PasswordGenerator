using System.Security.Cryptography;

namespace Edi.PasswordGenerator;

public class SecurePasswordGenerator : IPasswordGenerator
{
    private static readonly char[] UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private static readonly char[] LowercaseLetters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    private static readonly char[] Digits = "0123456789".ToCharArray();
    private static readonly char[] SpecialCharacters = "!@#$%^&*()-_=+[]{}|;:,.<>?".ToCharArray();
    private static readonly char[] AllCharacters = UppercaseLetters
        .Concat(LowercaseLetters)
        .Concat(Digits)
        .Concat(SpecialCharacters)
        .ToArray();

    public string GeneratePassword(PasswordRule? rule = null)
    {
        rule ??= new PasswordRule(12, 1);
        return GenerateSecurePassword(rule.Length, rule.LeastNumberOfNonAlphanumericCharacters);
    }

    public static string GenerateSecurePassword(int length = 12, int minSpecialChars = 1)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(length, 8, nameof(length));
        ArgumentOutOfRangeException.ThrowIfNegative(minSpecialChars, nameof(minSpecialChars));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minSpecialChars, length - 3, nameof(minSpecialChars));

        using var rng = RandomNumberGenerator.Create();
        var password = new char[length];
        var position = 0;

        // Ensure at least one character from each required category
        password[position++] = GetSecureRandomChar(rng, UppercaseLetters);
        password[position++] = GetSecureRandomChar(rng, LowercaseLetters);
        password[position++] = GetSecureRandomChar(rng, Digits);

        // Add required special characters
        for (int i = 0; i < minSpecialChars; i++)
        {
            password[position++] = GetSecureRandomChar(rng, SpecialCharacters);
        }

        // Fill remaining positions with random characters from all categories
        for (int i = position; i < length; i++)
        {
            password[i] = GetSecureRandomChar(rng, AllCharacters);
        }

        // Secure shuffle using Fisher-Yates algorithm
        SecureShuffle(rng, password);

        return new string(password);
    }

    private static char GetSecureRandomChar(RandomNumberGenerator rng, ReadOnlySpan<char> chars)
    {
        return chars[GetSecureRandomInt(rng, chars.Length)];
    }

    private static int GetSecureRandomInt(RandomNumberGenerator rng, int maxValue)
    {
        if (maxValue <= 1)
            return 0;

        // Calculate the largest multiple of maxValue that fits in uint range
        uint maxValidValue = uint.MaxValue - (uint.MaxValue % (uint)maxValue);

        Span<byte> bytes = stackalloc byte[4];
        uint randomValue;

        // Rejection sampling to avoid bias
        do
        {
            rng.GetBytes(bytes);
            randomValue = BitConverter.ToUInt32(bytes);
        } while (randomValue >= maxValidValue);

        return (int)(randomValue % (uint)maxValue);
    }

    private static void SecureShuffle(RandomNumberGenerator rng, Span<char> array)
    {
        // Fisher-Yates shuffle with cryptographically secure randomness
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = GetSecureRandomInt(rng, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}