namespace Edi.PasswordGenerator;

public interface IPasswordGenerator
{
    string GeneratePassword(PasswordRule rule = null);
}

public class PasswordRule(int length, int leastNumberOfNonAlphanumericCharacters)
{
    public int Length { get; set; } = length;

    public int LeastNumberOfNonAlphanumericCharacters { get; set; } = leastNumberOfNonAlphanumericCharacters;
}